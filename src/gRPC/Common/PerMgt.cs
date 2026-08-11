using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Common;

public sealed class PerReq : IAuthorizationRequirement
{
    // A requirement is satisfied when the user holds ANY one of these permission
    // bits. Single-permission [PerAuth("x")] yields a one-element array; the
    // pipe form [PerAuth("a|b|c")] yields OR semantics across a, b and c.
    public int[] BitIndexes { get; }
    public PerReq(params int[] bitIndexes) { BitIndexes = bitIndexes; }
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
            var indexes = new List<int>();
            foreach (var key in permission.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (PermissionMap.IndexMap.TryGetValue(key, out var index)) { indexes.Add(index); }
            }
            if (indexes.Count == 0) { return Task.FromResult<AuthorizationPolicy?>(null); }
            var policy = new AuthorizationPolicyBuilder().AddRequirements(new PerReq(indexes.ToArray())).Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return _fallback.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();
}