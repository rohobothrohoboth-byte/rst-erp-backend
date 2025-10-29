using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// New Employee Adding Steps end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/AddEmp")]
[ApiVersion("1.0")]

public class AddEmpController(IMediator med) : ControllerBase
{
    [HttpPost("Step1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step1([FromForm] Step1Dto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpAddStep1Cmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee", Details = ex.Message });
        }
    }

    [HttpPost("Step2")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step2([FromBody] Step2Dto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpAddStep2Cmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee", Details = ex.Message });
        }
    }

    [HttpPost("Step3")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step3([FromBody] Step3Dto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpAddStep3Cmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee", Details = ex.Message });
        }
    }

    [HttpPost("Step4")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step4([FromForm] Step4Dto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpAddStep4Cmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee", Details = ex.Message });
        }
    }
}