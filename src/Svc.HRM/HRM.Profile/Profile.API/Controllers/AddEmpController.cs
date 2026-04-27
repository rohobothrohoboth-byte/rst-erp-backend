using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
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
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EmpAddStep1Cmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE successfully created."));
    }

    [HttpPost("Step2")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Step2([FromForm] Step2Dto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EmpAddStep2Cmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE successfully created."));
    }

    [HttpGet("EmpAddPrint/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmpAddPrint(Guid id)
    {
        var response = await med.Send(new EmpAddPrintQry { Id = id });
        if (response == null) { throw new DomainException($"EMPLOYEE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    //[HttpPost("Step2")]
    //[ProducesResponseType(StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Step2([FromBody] Step2Dto addDto)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
    //        throw new ValException(errors);
    //    }

    //    var command = new EmpAddStep2Cmd { AddDto = addDto };
    //    var response = await med.Send(command);
    //    return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE successfully created."));
    //}

    //[HttpPost("Step3")]
    //[ProducesResponseType(StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Step3([FromBody] Step3Dto addDto)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
    //        throw new ValException(errors);
    //    }

    //    var command = new EmpAddStep3Cmd { AddDto = addDto };
    //    var response = await med.Send(command);
    //    return Ok(ApiResponse<object>.Ok(response, "New EMPLOYEE successfully created."));
    //}
}