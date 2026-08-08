using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Common;

public sealed class PerReq : IAuthorizationRequirement
{
    public int BitIndex { get; }
    public PerReq(int bitIndex) { BitIndex = bitIndex; }
}

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
            if (!PermissionMap.IndexMap.TryGetValue(permission, out var index)) { return Task.FromResult<AuthorizationPolicy?>(null); }
            var policy = new AuthorizationPolicyBuilder().AddRequirements(new PerReq(index)).Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}