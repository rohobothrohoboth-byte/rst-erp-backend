using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Cor.Module.Services;

namespace Cor.Module.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IExternalSystemService _externalSystemService;
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyService apiKeyService,
        IExternalSystemService externalSystemService)
        : base(options, logger, encoder)
    {
        _apiKeyService = apiKeyService;
        _externalSystemService = externalSystemService;
        _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            // ለSwagger/Health ጥያቄዎች
            var path = Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/swagger") || path.Contains("/health") || path.Contains("/scalar"))
            {
                var anonymousClaims = new[] { new Claim(ClaimTypes.Name, "Anonymous") };
                var anonymousIdentity = new ClaimsIdentity(anonymousClaims, "Anonymous");
                var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
                return AuthenticateResult.Success(new AuthenticationTicket(anonymousPrincipal, "Anonymous"));
            }

            // API Key ን ከHeader ያግኙ
            if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeaderValues))
            {
                _logger.LogWarning("⚠️ API Key missing in request headers");
                return AuthenticateResult.Fail("API Key missing");
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(providedApiKey))
            {
                _logger.LogWarning("⚠️ API Key is empty");
                return AuthenticateResult.Fail("API Key is empty");
            }

            // የAPI Key ትክክለኛነት ያረጋግጡ
            var isValid = await _apiKeyService.ValidateApiKeyAsync(providedApiKey);
            if (!isValid)
            {
                _logger.LogWarning("⚠️ Invalid API Key: {ApiKey}", MaskApiKey(providedApiKey));
                return AuthenticateResult.Fail("Invalid API Key");
            }

            // ክላይንት ስም ያግኙ
            var system = await _externalSystemService.GetSystemByApiKeyAsync(providedApiKey);
            if (system == null || !system.IsActive)
            {
                _logger.LogWarning("⚠️ Invalid or inactive API Key: {ApiKey}", MaskApiKey(providedApiKey));
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var clientName = system.Name;
            var isInternalClient = new[] { "CoreHRMM", "CoreModule", "AuthService", "Gateway" }.Contains(clientName);

            // ፈቃዶችን ያግኙ
            var permissions = await _externalSystemService.GetPermissionsAsync(providedApiKey);

            // Claims ን ይፍጠሩ
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, clientName),
                new Claim(ClaimTypes.AuthenticationMethod, "ApiKey"),
                new Claim("ApiKey", MaskApiKey(providedApiKey)),
                new Claim("ClientName", clientName),
                new Claim("ClientId", system.Id),
                new Claim("ClientType", isInternalClient ? "Internal" : "External"),
                new Claim("IsInternal", isInternalClient ? "true" : "false")
            };

            // ፈቃዶችን ይጨምሩ
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            // Internal ከሆነ ወይም * ፈቃድ ካለ
            if (isInternalClient || permissions.Contains("*"))
            {
                claims.Add(new Claim("Permission", "*"));
            }

            // ✅ LOG ALL CLAIMS BEFORE RETURN
            _logger.LogInformation("✅ API Key authentication successful for client: {ClientName} ({PermissionCount} permissions, Internal: {IsInternal})",
                clientName, permissions.Length, isInternalClient);

            // ✅ Log all claims added
            _logger.LogInformation("📋 Claims added: {Claims}",
                string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during API Key authentication");
            return AuthenticateResult.Fail("Authentication error");
        }
    }

    private string MaskApiKey(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 8)
            return "***";
        return apiKey[..4] + "..." + apiKey[^4..];
    }
}