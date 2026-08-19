// Leave.API/Controllers/LeaveTypeController.cs

using Asp.Versioning;
using Common;
using Helpers;
using Leave.App.Commands;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave.API.Controllers;

[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeaveType")]
[ApiVersion("1.0")]
public class LeaveTypeController : ControllerBase
{
    private readonly IMediator _med;

    public LeaveTypeController(IMediator med)
    {
        _med = med;
    }

    private IEnumerable<string> GetModelStateErrors()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
    }

    [HttpGet("AllLeaveType")]
    [PerAuth("leave.types.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllLeaveType()
    {
        var response = await _med.Send(new LeaveTypeAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetLeaveType/{id:guid}")]
    [PerAuth("leave.types.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaveType(Guid id)
    {
        var response = await _med.Send(new LeaveTypeByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"Leave type with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddLeaveType")]
    [PerAuth("leave.types.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddLeaveType([FromBody] LeaveTypeAddDto addDto)
    {
        if (!ModelState.IsValid)
            throw new ValException(GetModelStateErrors());

        var command = new LeaveTypeAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave type created successfully."));
    }

    [HttpPut("ModLeaveType/{id:guid}")]
    [PerAuth("leave.types.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ModLeaveType(Guid id, [FromBody] LeaveTypeModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
            throw new ValException(GetModelStateErrors());

        var command = new LeaveTypeModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave type updated successfully."));
    }

    [HttpDelete("DelLeaveType/{id:guid}")]
    [PerAuth("leave.types.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DelLeaveType(Guid id)
    {
        var command = new LeaveTypeDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Leave type deleted successfully."));
    }

    [PerAuth("leave.types.mod")]
    [HttpPatch("StatLeaveType")]  // Note: Your frontend calls "StatLeaveType"
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> StatLeaveType([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid)
            throw new ValException(GetModelStateErrors());

        var command = new LeaveTypeStatCmd { StatDto = statDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave type status changed."));
    }
}