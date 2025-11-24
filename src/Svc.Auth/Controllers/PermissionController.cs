using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All permission operations require authentication
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all permissions
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:Permissions.Read")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetPermissionsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Assign a permission to a role
    /// </summary>
    [HttpPost("assign")]
    [Authorize(Policy = "Permission:Permissions.Assign")]
    public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var cmd = new AssignPermissionCommand(dto.RoleName, dto.PermissionName);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }
        catch (Domain.DomainException dex)
        {
            return StatusCode(dex.StatusCode, new { error = dex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}