using System.Security.Claims;

namespace Helpers;

public static class ClaimsPrincipalExtensions
{
    public static bool TryGetEmployeeId(this ClaimsPrincipal user, out Guid employeeId)
    {
        employeeId = Guid.Empty;
        if (user?.Identity?.IsAuthenticated != true) { return false; }

        var claimValue = user.Claims.FirstOrDefault(c => c.Type == "employeeId")?.Value;

        if (string.IsNullOrWhiteSpace(claimValue)) { return false; }

        return Guid.TryParse(claimValue, out employeeId);
    }

    public static Guid GetEmployeeIdOrThrow(this ClaimsPrincipal user)
    {
        if (!user.TryGetEmployeeId(out var id)) { throw new UnauthorizedAccessException("AUTHORIZATION REQUIRED. Please LOGIN!"); }

        return id;
    }
}
