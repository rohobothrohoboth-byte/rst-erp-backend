using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Queries;
using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Controllers;

/// <summary>
/// End point to create USER ACCOUNT for employees
/// </summary>
[ApiController]
[Route("api/auth/v{version:apiVersion}/Register")]
[ApiVersion("1.0")]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _med;

    public RegistrationController(IMediator med)
    {
        _med = med;
    }

    [HttpPost("Step1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step1([FromBody] RegStep1 dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new RegStep1Cmd { Reg = dto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "USER ACCOUNT : Step 1, successfully created."));
    }

    [HttpPost("Step2")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step2([FromBody] RegStep2 dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new RegStep2Cmd { Reg = dto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "USER ACCOUNT : Step 2, successfully created."));
    }

    [HttpPost("Step3")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step3([FromBody] RegStep3 dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new RegStep3Cmd { Reg = dto };
        var response = await _med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "USER ACCOUNT : Step 3, successfully created."));
    }

    [HttpGet("status/{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegistrationStatus(string employeeId)
    {
        var response = await _med.Send(new GetRegistrationStatusQry { EmployeeId = employeeId });
        return Ok(ApiResponse<object>.Ok(response));
    }
}