using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Status Update Management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpStatus")]
[ApiVersion("1.0")]
public class EmpStatusController(IMediator med) : ControllerBase
{
    [PerAuth("hr.emp.mod")]
    [HttpPut("TermEmp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> TermEmp(Guid id)
    {
        var response = await med.Send(new EmpTermCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Status successfully updated."));
    }

    [PerAuth("hr.emp.mod")]
    [HttpPut("StByEmp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StByEmp(Guid id)
    {
        var response = await med.Send(new EmpStByCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Status successfully updated."));
    }

    [PerAuth("hr.emp.mod")]
    [HttpPut("SuspEmp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SuspEmp(Guid id)
    {
        var response = await med.Send(new EmpSuspCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Status successfully updated."));
    }

    [PerAuth("hr.emp.mod")]
    [HttpPut("RetiEmp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RetiEmp(Guid id)
    {
        var response = await med.Send(new EmpRetiCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S Status successfully updated."));
    }

    [PerAuth("hr.emp.mod")]
    [HttpPut("ReviewEmp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReviewEmp(Guid id, [FromBody] EmpRevDto dto)
    {
        if (!ModelState.IsValid || dto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }
        var response = await med.Send(new EmpAppCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Selected EMPLOYEE'S successfully REVIEWED."));
    }
}