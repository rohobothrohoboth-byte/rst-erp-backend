using Microsoft.AspNetCore.Authorization;
using Svc.Auth.Constants;

namespace Svc.Auth.Services;

public class PerApiRequirement : IAuthorizationRequirement
{
    public string PerApi { get; }
    public PerApiRequirement(string perApi) => PerApi = perApi;
}

public class PerApiHandler : AuthorizationHandler<PerApiRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PerApiRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == AuthCons.PerModule && c.Value == requirement.PerApi))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}