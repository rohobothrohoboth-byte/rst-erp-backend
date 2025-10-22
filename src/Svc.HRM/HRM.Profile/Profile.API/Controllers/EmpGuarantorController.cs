using System.Data;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Guarantors end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpGuarantor")]
[ApiVersion("1.0")]
public class EmpGuarantorController(IMediator med) : ControllerBase
{
    [HttpGet("GetEmpGuarantor/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpGuarantor(Guid id)
    {
        var response = await med.Send(new EmpGuarantorByIdQry { Id = id });
        if (response == null) { return NotFound(new { Error = $"Employee Guarantor with Id {id} not found" }); }
        return Ok(response);
    }

    [HttpPost("AddEmpGuarantor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmpGuarantorAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpGuarantorAddCmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee Guarantor", Details = ex.Message });
        }
    }

    [HttpPut("ModEmpGuarantor/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmpGuarantorModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new EmpGuarantorModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Employee Guarantor was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Employee Guarantor with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Employee Guarantor", Details = ex.Message });
        }
    }

    [HttpDelete("DelEmpGuarantor/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new EmpGuarantorDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Employee Guarantor with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Employee Guarantor", Details = ex.Message });
        }
    }
}