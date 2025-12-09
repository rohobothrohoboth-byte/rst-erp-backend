using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Svc.Auth.Services;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("api:"))
        {
            var permission = policyName.Substring("api:".Length);
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PerApiRequirement(permission))
                .Build();
            return Task.FromResult(policy);
        }
        return BackupPolicyProvider.GetPolicyAsync(policyName);
    }
}