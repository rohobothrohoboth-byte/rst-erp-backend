using Microsoft.AspNetCore.Authorization; // ✅ ይህን ይጨምሩ

namespace Cor.Module.Authentication;

public class PermissionRequirement : IAuthorizationRequirement // ✅ አሁን ይሰራል
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}