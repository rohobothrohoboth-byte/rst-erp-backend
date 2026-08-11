// Recruit.API/Controllers/OnboardingTaskController.cs
using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// ONBOARDING TASK management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/OnboardingTask")]
[ApiVersion("1.0")]
public class OnboardingTaskController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.onboard.view")]
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> All()
    {
        var response = await med.Send(new OnboardingTaskAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.onboard.view")]
    [HttpGet("Get/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new OnboardingTaskByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"ONBOARDING TASK with id [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.onboard.manage")]
    [HttpPost("Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] OnboardingTaskAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        var command = new OnboardingTaskAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New ONBOARDING TASK successfully created."));
    }

    [PerAuth("hr.recruit.onboard.manage")]
    [HttpPut("Mod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Mod(Guid id, [FromBody] OnboardingTaskModDto modDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        if (modDto.Id != id)
            throw new ValException("ID mismatch between route and body.");

        var command = new OnboardingTaskModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected ONBOARDING TASK successfully updated."));
    }

    [PerAuth("hr.recruit.onboard.manage")]
    [HttpDelete("Del/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Del(Guid id)
    {
        var command = new OnboardingTaskDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"ONBOARDING TASK with Id {id} successfully deleted."));
    }
}