using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Helpers.ExternalAccess;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public string HeaderName { get; set; } = "X-API-Key";
    public bool EnforceEndpointPermissions { get; set; } = true;
}

/// <summary>
/// Single shared API-key authentication handler for all modules. Validates the
/// X-API-Key header, enforces the per-system endpoint allow-list (except internal
/// clients), logs usage, and issues an "ApiKey" identity (AuthenticationMethod=ApiKey)
/// which PerAuthHandler treats as authorized.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly IExternalSystemService _externalSystemService;

    private static readonly string[] InternalClients = { "CoreHRMM", "CoreModule", "AuthService", "Gateway" };

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
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyHeaderValues))
                return AuthenticateResult.NoResult();

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(providedApiKey))
                return AuthenticateResult.Fail("Invalid API Key");

            if (!await _apiKeyService.ValidateApiKeyAsync(providedApiKey))
                return AuthenticateResult.Fail("Invalid API Key");

            var clientName = await _apiKeyService.GetClientNameAsync(providedApiKey);
            var isInternalClient = clientName != null && InternalClients.Contains(clientName);

            if (!isInternalClient && Options.EnforceEndpointPermissions)
            {
                var currentPath = Request.Path.Value ?? "";
                var isAllowed = await _externalSystemService.IsSystemAllowedAsync(providedApiKey, currentPath);
                if (!isAllowed)
                {
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

            await _apiKeyService.LogApiKeyUsageAsync(providedApiKey, Request.Path, Request.Method, true);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, clientName ?? "Unknown"),
                new Claim(ClaimTypes.AuthenticationMethod, "ApiKey"),
                new Claim("ApiKey", MaskApiKey(providedApiKey)),
                new Claim("ClientType", isInternalClient ? "Internal" : "External")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during API Key authentication");
            return AuthenticateResult.Fail("Authentication error");
        }
    }

    private static string MaskApiKey(string apiKey)
        => string.IsNullOrEmpty(apiKey) || apiKey.Length < 8 ? "***" : apiKey[..4] + "..." + apiKey[^4..];
}
