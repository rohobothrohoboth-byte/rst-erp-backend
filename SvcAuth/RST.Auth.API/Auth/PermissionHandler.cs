using Microsoft.AspNetCore.Authorization;

namespace RST.Auth.API.Auth
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement req)
        {
            if (context.User.HasClaim("permission", req.Permission))
                context.Succeed(req);
            return Task.CompletedTask;
        }
    }
}
