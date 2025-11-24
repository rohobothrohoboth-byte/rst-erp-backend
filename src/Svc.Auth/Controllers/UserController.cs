using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Svc.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All user operations require authentication
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get all users (Admin permission required)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:Users.Read")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetUsersQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a single user by Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:Users.Read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);

        return result != null ? Ok(result) : NotFound();
    }

    /// <summary>
    /// Create a new user (admin-only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Permission:Users.Create")]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var cmd = new RegisterUserCommand(dto);
            var result = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetById), new { id = result.User.Id }, result);
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

    /// <summary>
    /// Assign a role to an existing user
    /// </summary>
    [HttpPost("{id:guid}/assign-role")]
    [Authorize(Policy = "Permission:Users.AssignRole")]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name is required.");

        var cmd = new AssignRoleCommand(id, roleName);

        try
        {
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