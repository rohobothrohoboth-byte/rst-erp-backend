using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Common;

public sealed class PerAuthAttribute : AuthorizeAttribute
{
    public PerAuthAttribute(string permission) { Policy = PerPolicy.Name(permission); }
}

public interface IPerValService
{
    Task<bool> Validate(string accessToken, string permission);
}

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

        var allowed = await _validator.Validate(token, requirement.Permission);
        if (allowed) { context.Succeed(requirement); }
    }
}

public sealed class PerValService : IPerValService
{
    private readonly IAuthClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PerValService(IAuthClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> Validate(string token, string permission)
    {
        var response = await _client.ValidateToken(token);
        if (!response) return false;
        var per = await _client.GetUser(token);
        return per.PerApi.Contains(permission);
    }
}