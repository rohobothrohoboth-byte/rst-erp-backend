using Microsoft.AspNetCore.Authorization;

namespace RST.Auth.API.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }
        public PermissionRequirement(string permissionName) => PermissionName = permissionName;
    }
}
