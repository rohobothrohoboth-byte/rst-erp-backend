using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Stamp end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpStamp")]
[ApiVersion("1.0")]
public class EmpStampController(IMediator med) : ControllerBase
{

    [HttpGet("EmpStamp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpStamp(Guid id)
    {
        var res = await med.Send(new EmpStampByIdQry { Id = id });
        if (res == null)
        {
            return NotFound(new { Error = $"EMPLOYEE STAMP with Id {id} NOT FOUND" });
        }
        return Ok(res);
    }

    [HttpPost("AddEmpStamp")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] EmpStampAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpStampAddCmd { AddDto = addDto };
            var braId = await med.Send(command);
            return CreatedAtAction(nameof(GetEmpStamp), new { id = braId.Id }, braId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to upload EMPLOYEE STAMP", Details = ex.Message });
        }
    }



}
