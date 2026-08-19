using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// ONBOARDING ASSIGNMENT management end points (assign onboarding tasks to employees)
/// </summary>
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/OnboardingAssignment")]
[ApiVersion("1.0")]
public class OnboardingAssignmentController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.onboard.assignments.view")]
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> All()
    {
        var response = await med.Send(new OnboardingAssignAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.onboard.assignments.view")]
    [HttpGet("Get/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new OnboardingAssignByIdQry { Id = id });
        if (response == null) { throw new DomainException($"ONBOARDING ASSIGNMENT with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.onboard.assignments.view")]
    [HttpGet("ByEmployee/{employeeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ByEmployee(Guid employeeId)
    {
        var response = await med.Send(new OnboardingAssignByEmployeeQry { EmployeeId = employeeId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.onboard.assignments.view")]
    [HttpGet("ByTask/{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ByTask(Guid taskId)
    {
        var response = await med.Send(new OnboardingAssignByTaskQry { TaskId = taskId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.onboard.assignments.create")]
    [HttpPost("Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] OnboardingAssignmentAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var response = await med.Send(new OnboardingAssignAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "New ONBOARDING ASSIGNMENT successfully created."));
    }

    [PerAuth("hr.recruit.onboard.assignments.mod")]
    [HttpPut("Mod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Mod(Guid id, [FromBody] OnboardingAssignmentModDto modDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }
        if (modDto.Id != id) { throw new ValException("ID mismatch between route and body."); }

        var response = await med.Send(new OnboardingAssignModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Selected ONBOARDING ASSIGNMENT successfully updated."));
    }

    [PerAuth("hr.recruit.onboard.assignments.complete")]
    [HttpPatch("Status/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Status(Guid id, [FromBody] OnboardingAssignmentStatusDto statusDto)
    {
        if (string.IsNullOrWhiteSpace(statusDto?.Status)) { throw new ValException("Status is required."); }
        var response = await med.Send(new OnboardingAssignStatusCmd { Id = id, Status = statusDto.Status });
        return Ok(ApiResponse<object>.Ok(response, "ONBOARDING ASSIGNMENT status successfully updated."));
    }

    [PerAuth("hr.recruit.onboard.assignments.del")]
    [HttpDelete("Del/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new OnboardingAssignDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"ONBOARDING ASSIGNMENT with Id {id} successfully deleted."));
    }
}
