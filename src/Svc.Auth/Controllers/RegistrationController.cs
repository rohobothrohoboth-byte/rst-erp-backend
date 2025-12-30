using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Svc.Auth.Commands;
using Svc.Auth.Helpers;
using Svc.Auth.Models.Dtos;
using ValidationException = Svc.Auth.Helpers.ValidationException;

namespace Svc.Auth.Controllers;

/// <summary>
/// End point to create USER ACCOUNT for employees
/// </summary>

//[Authorize]
[ApiController]
[Route("api/auth/v{version:apiVersion}/Register")]
[ApiVersion("1.0")]
public class RegistrationController(IMediator med) : ControllerBase
{
    [HttpPost("Step1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step1([FromBody] RegStep1 dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new RegStep1Cmd { Reg = dto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "USER ACCOUNT : Step 1, successfully created."));
    }

    [HttpPost("Step2")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step2([FromBody] RegStep2 dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new RegStep2Cmd { Reg = dto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "USER ACCOUNT : Step 2, successfully created."));
    }

    [HttpPost("Step3")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step3([FromBody] RegStep3 dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new RegStep3Cmd { Reg = dto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "USER ACCOUNT : Step 3, successfully created."));
    }
}