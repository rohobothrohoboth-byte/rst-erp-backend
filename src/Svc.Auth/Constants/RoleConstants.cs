using System.Collections.ObjectModel;

namespace Svc.Auth.Constants;

public static class RoleConstants
{
    public const string Admin = nameof(Admin);
    public const string CEO = nameof(CEO);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(
    [
        Admin, CEO
    ]);

    public static bool IsDefaultRole(string roleName) => DefaultRoles.Contains(roleName);
}

