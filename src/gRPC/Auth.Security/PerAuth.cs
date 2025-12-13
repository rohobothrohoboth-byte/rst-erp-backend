using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Auth.Security;

public sealed class PerAuthAttribute : AuthorizeAttribute
{
    public PerAuthAttribute(string permission) { Policy = PerPolicy.Name(permission); }
}

public interface IPerValService { Task<bool> ValidateAsync(string accessToken, string permission); }

public sealed class PerAuthHandler : AuthorizationHandler<PerReq>
{
    private readonly PerValService _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PerAuthHandler(PerValService validator, IHttpContextAccessor httpContextAccessor)
    {
        _validator = validator;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PerReq requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) { return; }

        var token = httpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (string.IsNullOrWhiteSpace(token)) { return; }

        var allowed = await _validator.ValidateAsync(token, requirement.Permission);
        if (allowed) { context.Succeed(requirement); }
    }
}

public sealed class PerValService : IPerValService
{
    private readonly IAuthClient _client;

    public PerValService(IAuthClient client) { _client = client; }

    public async Task<bool> ValidateAsync(string token, string permission)
    {
        var response = await _client.ValidateToken(token);
        if (!response) return false;
        var per = await _client.GetUser(token);
        return per.PerApi.Contains(permission);
    }
}