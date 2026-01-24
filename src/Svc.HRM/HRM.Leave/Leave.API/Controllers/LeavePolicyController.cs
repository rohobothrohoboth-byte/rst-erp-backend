using Asp.Versioning;
using Leave.App.Commands;
using Leave.App.Helpers;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

/// <summary>
/// LEAVE POLICY management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeavePolicy")]
[ApiVersion("1.0")]
public class LeavePolicyController(IMediator med) : ControllerBase
{
    [HttpGet("ActiveLeavePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActiveLeavePolicy()
    {
        var response = await med.Send(new ActiveLeavePolicyQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("AllLeavePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllLeavePolicy()
    {
        var response = await med.Send(new LeavePolicyAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetLeavePolicy/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeavePolicy(Guid id)
    {
        var response = await med.Send(new LeavePolicyByIdQry { Id = id });
        if (response == null) { throw new DomainException($"LEAVE POLICY with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddLeavePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] LeavePolicyAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new LeavePolicyAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New LEAVE POLICY successfully created."));
    }

    [HttpPut("ModLeavePolicy/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] LeavePolicyModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new LeavePolicyModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected LEAVE POLICY successfully updated."));
    }

    [HttpDelete("DelLeavePolicy/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new LeavePolicyDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"LEAVE POLICY with Id {id} successfully deleted."));
    }
}