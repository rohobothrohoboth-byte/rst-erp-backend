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
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PerReq requirement)
    {
        var hash = context.User.FindFirst("ph")?.Value;

        if (string.IsNullOrEmpty(hash)) { return Task.CompletedTask; }

        var bytes = Convert.FromBase64String(hash);

        var hasPermission = (bytes[requirement.BitIndex / 8] & (1 << (requirement.BitIndex % 8))) != 0;

        if (hasPermission)
        {
            context.Succeed(requirement);
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