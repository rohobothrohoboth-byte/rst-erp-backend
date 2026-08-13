using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Profile.API.Middlewares;

/// <summary>
/// Minimal, config-only API-key authentication for trusted internal service-to-service
/// calls (e.g. the Auth service's initial-sync client, which sends the shared
/// X-API-Key header). It validates the key against the "ApiKeys" configuration section
/// and issues an identity whose AuthenticationType is "ApiKey", which <c>PerAuthHandler</c>
/// treats as authorized. No database / ExternalSystems table is required.
/// </summary>
public sealed class InternalApiKeyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiKey";
    private const string HeaderName = "X-API-Key";

    private readonly IConfiguration _configuration;

    public InternalApiKeyAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var providedValues))
            return Task.FromResult(AuthenticateResult.NoResult());

        var apiKey = providedValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
            return Task.FromResult(AuthenticateResult.Fail("Missing API key"));

        // Config-defined keys are trusted internal services. (External partner keys,
        // if ever needed, live in a DB table and are handled by the shared stack in
        // other modules - Profile only needs internal service access here.)
        var configKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>()
                         ?? new Dictionary<string, string>();
        var clientName = configKeys.FirstOrDefault(k => k.Value == apiKey).Key;
        if (string.IsNullOrEmpty(clientName))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, clientName),
            new Claim(ClaimTypes.AuthenticationMethod, "ApiKey"),
            new Claim("ClientType", "Internal")
        };
        // ClaimsIdentity(authenticationType: "ApiKey") -> User.Identity.AuthenticationType == "ApiKey"
        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
