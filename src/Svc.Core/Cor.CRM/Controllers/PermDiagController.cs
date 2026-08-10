// Cor.CRM/Controllers/PermDiagController.cs
// TEMPORARY diagnostic endpoint to debug [PerAuth] enforcement. Read-only.

using Asp.Versioning;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/PermDiag")]
[ApiVersion("1.0")]
public class PermDiagController : ControllerBase
{
    [HttpGet("{permission}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Check(string permission)
    {
        var ph = User.FindFirst("ph")?.Value;

        var inRegistry = PermissionMap.IndexMap.TryGetValue(permission, out var index);
        int? phByteLength = null;
        var granted = false;

        if (!string.IsNullOrEmpty(ph))
        {
            try
            {
                var bytes = System.Convert.FromBase64String(ph);
                phByteLength = bytes.Length;
                if (inRegistry)
                {
                    var byteIndex = index / 8;
                    granted = byteIndex < bytes.Length && (bytes[byteIndex] & (1 << (index % 8))) != 0;
                }
            }
            catch { /* ignore malformed ph */ }
        }

        // Dump all claim types (hide the long ph value) so we can see how the
        // token is being read on the CRM side vs Auth.
        var claims = User.Claims
            .Select(c => new { c.Type, Value = c.Type == "ph" ? $"<{c.Value.Length} chars>" : c.Value })
            .ToList();

        return Ok(ApiResponse<object>.Ok(new
        {
            Permission = permission,
            PhPresent = !string.IsNullOrEmpty(ph),
            PhByteLength = phByteLength,
            InRegistry = inRegistry,
            Index = inRegistry ? index : (int?)null,
            Granted = granted,
            RegistrySize = PermissionMap.IndexMap.Count,
            IsAdminBypass = User.IsInRole("Admin") || User.IsInRole("admin") || User.IsInRole("super_admin"),
            Claims = claims
        }));
    }
}
