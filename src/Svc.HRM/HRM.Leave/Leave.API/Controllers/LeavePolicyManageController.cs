// Leave.API/Controllers/LeavePolicyManageController.cs

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

/// <summary>
/// LEAVE POLICY MANAGEMENT
/// </summary>
[Authorize]
[ApiController]
[Route("api/hrm/leave/v{version:apiVersion}/LeavePolicy")]
[ApiVersion("1.0")]
public class LeavePolicyManageController : ControllerBase
{
    private readonly IMediator _med;

    public LeavePolicyManageController(IMediator med)
    {
        _med = med;
    }

    private IEnumerable<string> GetModelStateErrors()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);
    }

    /// <summary>
    /// Get all leave policies
    /// </summary>
    [HttpGet("AllLeavePolicy")]
    [PerAuth("leave.policies.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllLeavePolicy()
    {
        var response = await _med.Send(new LeavePolicyAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get active leave policies
    /// </summary>
    [HttpGet("ActiveLeavePolicy")]
    [PerAuth("leave.policies.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ActiveLeavePolicy()
    {
        var response = await _med.Send(new ActiveLeavePolicyQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get leave policy by ID
    /// </summary>
    [HttpGet("GetLeavePolicy/{id:guid}")]
    [PerAuth("leave.policies.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeavePolicy(Guid id)
    {
        var response = await _med.Send(new LeavePolicyByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"Leave policy with id [{id}] not found.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Create a new leave policy
    /// </summary>
    [HttpPost("AddLeavePolicy")]
    [PerAuth("leave.policies.mod")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddLeavePolicy([FromBody] LeavePolicyAddDto addDto)
    {
        if (!ModelState.IsValid)
            throw new ValException(GetModelStateErrors());

        var command = new LeavePolicyAddCmd { AddDto = addDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave policy created successfully."));
    }

    /// <summary>
    /// Update a leave policy
    /// </summary>
    [HttpPut("ModLeavePolicy/{id:guid}")]
    [PerAuth("leave.policies.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ModLeavePolicy(Guid id, [FromBody] LeavePolicyModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
            throw new ValException(GetModelStateErrors());

        var command = new LeavePolicyModCmd { ModDto = modDto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Leave policy updated successfully."));
    }

    /// <summary>
    /// Delete a leave policy
    /// </summary>
    [HttpDelete("DelLeavePolicy/{id:guid}")]
    [PerAuth("leave.policies.mod")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DelLeavePolicy(Guid id)
    {
        var command = new LeavePolicyDelCmd { Id = id };
        await _med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, "Leave policy deleted successfully."));
    }

    /// <summary>
    /// Assign policies to employees
    /// </summary>
    [HttpPost("AssignLeavePolicy")]
    [PerAuth("leave.policies.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignLeavePolicy()
    {
        var response = await _med.Send(new PolicyAssignCmd());
        return Ok(ApiResponse<object>.Ok(response, "Policy assignment completed."));
    }
}