using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cor.Finance.Services;

namespace Cor.Finance.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IExternalSystemService _externalSystemService;
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyService apiKeyService,
        IExternalSystemService externalSystemService,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _apiKeyService = apiKeyService;
        _externalSystemService = externalSystemService;
        _logger = logger.CreateLogger<ApiKeyAuthenticationHandler>();
        _configuration = configuration;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrEmpty(providedApiKey))
            {
                _logger.LogWarning("API key header is empty");
                return AuthenticateResult.Fail("Invalid API Key");
            }

            // ✅ STEP 1: Validate the API key
            var isValid = await _apiKeyService.ValidateApiKeyAsync(providedApiKey);
            if (!isValid)
            {
                _logger.LogWarning("Invalid API key provided: {ApiKey}", MaskApiKey(providedApiKey));
                return AuthenticateResult.Fail("Invalid API Key");
            }

            var clientName = await _apiKeyService.GetClientNameAsync(providedApiKey);
            _logger.LogInformation($"📋 Client Name: {clientName}");

            // ✅ STEP 2: Skip endpoint check for internal clients
            var internalClients = new[] { "CoreHRMM", "CoreModule", "AuthService", "Gateway" };
            var isInternalClient = internalClients.Contains(clientName);

            if (!isInternalClient)
            {
                var currentPath = Request.Path.Value ?? "";
                var isAllowed = await _externalSystemService.IsSystemAllowedAsync(providedApiKey, currentPath);
                _logger.LogInformation($"📋 Endpoint: {currentPath}, Allowed: {isAllowed}");

                if (!isAllowed)
                {
                    _logger.LogWarning("❌ Access denied for {ClientName} to endpoint: {Endpoint}",
                        clientName ?? "Unknown", currentPath);

                    Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await Context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = $"Access denied. Endpoint '{currentPath}' is not allowed for this API Key.",
                        statusCode = 403
                    });
                    return AuthenticateResult.Fail("Endpoint not allowed");
                }
            }
            else
            {
                _logger.LogInformation($"✅ Internal client {clientName} - skipping endpoint permission check");
            }

            // ✅ STEP 3: Log usage
            await _apiKeyService.LogApiKeyUsageAsync(
                providedApiKey,
                Request.Path,
                Request.Method,
                true);

            // ✅ STEP 4: Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, clientName ?? "Unknown"),
                new Claim(ClaimTypes.AuthenticationMethod, "ApiKey"),
                new Claim("ApiKey", MaskApiKey(providedApiKey)),
                new Claim("ClientType", isInternalClient ? "Internal" : "External")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            _logger.LogInformation("✅ API Key authentication successful for client: {ClientName}", clientName);

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