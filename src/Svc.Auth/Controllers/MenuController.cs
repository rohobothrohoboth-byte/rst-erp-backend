using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Queries;  // 👈 ADD THIS - for GetUserMenuStructureQry and GetUserPermissionsQry
using System.Security.Claims;

namespace Svc.Auth.Controllers;

[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Menu")]
[ApiVersion("1.0")]
public class MenuController : ControllerBase
{
    private readonly IMediator _med;

    public MenuController(IMediator med)
    {
        _med = med;
    }

    /// <summary>
    /// Get the user's menu structure (cached)
    /// </summary>
    [HttpGet("structure")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMenuStructure()
    {
        var userId = User.FindFirstValue("userId");
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("User not authenticated");

        var response = await _med.Send(new GetUserMenuStructureQry { UserId = userId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get the user's permission keys (for API authorization)
    /// </summary>
    [HttpGet("permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPermissions()
    {
        var userId = User.FindFirstValue("userId");
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("User not authenticated");

        var response = await _med.Send(new GetUserPermissionsQry { UserId = userId });
        return Ok(ApiResponse<object>.Ok(response));
    }
}