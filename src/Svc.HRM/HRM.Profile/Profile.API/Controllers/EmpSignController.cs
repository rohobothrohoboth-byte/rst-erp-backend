using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Signature end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpSign")]
[ApiVersion("1.0")]
public class EmpSignController(IMediator med) : ControllerBase
{

    [HttpGet("EmpSign/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpSign(Guid id)
    {
        var res = await med.Send(new EmpSignByIdQry { Id = id });
        if (res == null)
        {
            return NotFound(new { Error = $"EMPLOYEE SIGNATURE with Id {id} NOT FOUND" });
        }
        return Ok(res);
    }

    [HttpPost("AddEmpSign")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] EmpSignAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpSignAddCmd { AddDto = addDto };
            var braId = await med.Send(command);
            return CreatedAtAction(nameof(GetEmpSign), new { id = braId.Id }, braId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to upload EMPLOYEE SIGNATURE", Details = ex.Message });
        }
    }



}
