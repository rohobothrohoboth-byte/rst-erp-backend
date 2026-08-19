using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Cor.Module.Services;

namespace Cor.Module.Authentication;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IExternalSystemService _externalSystemService;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(
        IExternalSystemService externalSystemService,
        ILogger<PermissionAuthorizationHandler> logger)
    {
        _externalSystemService = externalSystemService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        _logger.LogInformation($"🔍 HandleRequirementAsync called for: {requirement.Permission}");

        // Log all claims
        _logger.LogInformation($"📋 User claims: {string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}"))}");

        // Check if authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogWarning("⚠️ User is not authenticated");
            context.Fail();
            return;
        }

        _logger.LogInformation($"✅ User is authenticated: {context.User.Identity.Name}");

        // Check if Internal
        var isInternal = context.User.HasClaim("IsInternal", "true");
        _logger.LogInformation($"📌 IsInternal: {isInternal}");

        if (isInternal)
        {
            _logger.LogInformation($"✅ Internal client - granting permission: {requirement.Permission}");
            context.Succeed(requirement);
            return;
        }

        // Check exact permission claim
        if (context.User.HasClaim("Permission", requirement.Permission))
        {
            _logger.LogInformation($"✅ Permission granted from claim: {requirement.Permission}");
            context.Succeed(requirement);
            return;
        }

        // Check wildcard
        if (context.User.HasClaim("Permission", "*"))
        {
            _logger.LogInformation($"✅ Permission granted from wildcard: {requirement.Permission}");
            context.Succeed(requirement);
            return;
        }

        // Check DB
        var apiKey = context.User.FindFirst("ApiKey")?.Value;
        _logger.LogInformation($"🔑 ApiKey found: {!string.IsNullOrEmpty(apiKey)}");

        if (!string.IsNullOrEmpty(apiKey))
        {
            try
            {
                var system = await _externalSystemService.GetSystemByApiKeyAsync(apiKey);
                if (system != null)
                {
                    _logger.LogInformation($"📌 System found: {system.Name}, Active: {system.IsActive}");
                    var permissions = system.Permissions ?? Array.Empty<string>();
                    _logger.LogInformation($"📌 System permissions: {string.Join(", ", permissions)}");

                    if (permissions.Contains(requirement.Permission) || permissions.Contains("*"))
                    {
                        _logger.LogInformation($"✅ Permission granted from DB: {requirement.Permission}");
                        context.Succeed(requirement);
                        return;
                    }
                }
                else
                {
                    _logger.LogWarning($"⚠️ System not found for API Key");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error checking DB permissions: {ex.Message}");
            }
        }

        _logger.LogWarning($"❌ Permission denied for: {requirement.Permission}");
        context.Fail();
    }
}