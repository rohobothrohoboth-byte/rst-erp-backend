using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using System.Security.Claims;

namespace Recruit.API.Controllers;

/// <summary>
/// WorkforcePlan Management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/WorkforcePlan")]
[ApiVersion("1.0")]
public class WorkforcePlanController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.workforce.view")]
    [HttpGet("AllWorkforcePlan")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllWorkforcePlan()
    {
        var response = await med.Send(new WorkforcePlanAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.workforce.view")]
    [HttpGet("GetWorkforcePlan/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkforcePlan(Guid id)
    {
        var response = await med.Send(new WorkforcePlanByIdQry { Id = id });
        if (response == null) { throw new DomainException($"WORKFORCE PLAN with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.workforce.manage")]
    [HttpPost("AddWorkforcePlan")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] WorkforcePlanAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        addDto.RequistionById = Guid.Parse(empId!);
        var command = new WorkforcePlanAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New WORKFORCE PLAN successfully created."));
    }

    [PerAuth("hr.recruit.workforce.manage")]
    [HttpPut("ModWorkforcePlan/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] WorkforcePlanModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new WorkforcePlanModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected WORKFORCE PLAN successfully updated."));
    }

    [PerAuth("hr.recruit.workforce.manage")]
    [HttpDelete("DelWorkforcePlan/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new WorkforcePlanDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"WORKFORCE PLAN with Id {id} successfully deleted."));
    }
}