using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.Domain.DTOs;
using System.Security.Claims;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB APPLICATION end points
/// </summary>

[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobApp")]
[ApiVersion("1.0")]
public class JobAppController(IMediator med) : ControllerBase
{
    [HttpPost("AddJobAppInt")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddJobAppInt([FromForm] JobAppIntAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        addDto.EmployeeId = Guid.Parse(empId!);
        var command = new JobAppIntAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New JOB APPLICATION successfully created."));
    }

    [HttpPut("ModJobAppInt/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ModJobAppInt(Guid id, [FromForm] JobAppIntModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobAppIntModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected JOB APPLICATION successfully updated."));
    }






    [HttpDelete("DelJobApp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new JobAppDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"JOB APPLICATION with Id {id} successfully deleted."));
    }
}