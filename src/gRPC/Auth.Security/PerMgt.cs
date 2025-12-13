using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Auth.Security;

public sealed class PerReq : IAuthorizationRequirement
{
    public string Permission { get; }
    public PerReq(string permission) { Permission = permission; }
}


public static class PerPolicy { public static string Name(string permission) => $"api:{permission}"; } //PERMISSION=api}

public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallback;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallback = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("api:"))
        {
            var permission = policyName["api:".Length..];
            var policy = new AuthorizationPolicyBuilder().AddRequirements(new PerReq(permission)).Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}