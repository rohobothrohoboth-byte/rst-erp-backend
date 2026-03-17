using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employees Management by ADMIN end points
/// </summary>

//[Authorize(Roles = "admin")]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/AdminEmp")]
[ApiVersion("1.0")]
public class AdminEmpController(IMediator med) : ControllerBase
{
    [HttpPost("Step1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step1([FromForm] Step1Dto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EmpAddStep1Cmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE successfully created."));
    }
    
    [HttpGet("Step2/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Step2(Guid id)
    {
        var response = await med.Send(new Step2Qry { Id = id });
        if (response == null) { throw new DomainException($"EMPLOYEE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }
}