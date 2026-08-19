using Microsoft.AspNetCore.Authorization;

namespace Common;

public sealed class PerAuthAttribute : AuthorizeAttribute
{
    public PerAuthAttribute(string permission) { Policy = $"api:{permission}"; }
}

public interface IPerValService
{
    Task<bool> Validate(string accessToken, string permission);
}

public sealed class PerAuthHandler : AuthorizationHandler<PerReq>
{
    // Privileged roles bypass fine-grained permission checks so administrators
    // are never locked out while [PerAuth] is rolled out across the system.
    private static readonly string[] PrivilegedRoles =
    {
        "Admin", "admin", "super_admin", "SuperAdmin", "superadmin"
    };

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PerReq requirement)
    {
        if (Array.Exists(PrivilegedRoles, context.User.IsInRole))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // External systems authenticate via the ApiKey scheme (read-only, already
        // gated by the per-system endpoint allow-list at authentication time). They
        // carry no "ph" permission claim, so treat a valid ApiKey identity as
        // authorized here. This lets endpoints shared by internal (Bearer) and
        // external (ApiKey) callers add [PerAuth] for internal enforcement WITHOUT
        // blocking external read access.
        if (string.Equals(context.User.Identity?.AuthenticationType, "ApiKey", StringComparison.OrdinalIgnoreCase)
            || context.User.HasClaim(c =>
                c.Type == System.Security.Claims.ClaimTypes.AuthenticationMethod && c.Value == "ApiKey"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hash = context.User.FindFirst("ph")?.Value;

        if (string.IsNullOrEmpty(hash)) { return Task.CompletedTask; }

        byte[] bytes;
        try { bytes = Convert.FromBase64String(hash); }
        catch { return Task.CompletedTask; }

        foreach (var bitIndex in requirement.BitIndexes)
        {
            var byteIndex = bitIndex / 8;
            if (byteIndex < bytes.Length && (bytes[byteIndex] & (1 << (bitIndex % 8))) != 0)
            {
                context.Succeed(requirement);
                break;
            }
        }

        return Task.CompletedTask;
    }
}

public sealed class PerValService : IPerValService
{
    private readonly IAuthClient _client;
    public PerValService(IAuthClient client) { _client = client; }

    public async Task<bool> Validate(string token, string permission)
    {
        var response = await _client.ValidateToken(token);
        if (!response) return false;
        var per = await _client.GetUser(token);
        return per.PerApi.Contains(permission);
    }
}