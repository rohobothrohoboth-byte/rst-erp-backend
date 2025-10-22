using System.Data;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Pension Card end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpPensionCard")]
[ApiVersion("1.0")]
public class EmpPensionCardController(IMediator med) : ControllerBase
{
    [HttpGet("GetEmpPensionCard/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpPensionCard(Guid id)
    {
        var response = await med.Send(new EmpPensionCardByIdQry { Id = id });
        if (response == null) { return NotFound(new { Error = $"Employee Pension Card with Id {id} not found" }); }
        return Ok(response);
    }

    [HttpPost("AddEmpPensionCard")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EmpPensionCardAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpPensionCardAddCmd { AddDto = addDto };
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Employee Pension Card", Details = ex.Message });
        }
    }

    [HttpPut("ModEmpPensionCard/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmpPensionCardModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new EmpPensionCardModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Employee Pension Card was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Employee Pension Card with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Employee Pension Card", Details = ex.Message });
        }
    }

    [HttpDelete("DelEmpPensionCard/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new EmpPensionCardDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Employee Pension Card with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Employee Pension Card", Details = ex.Message });
        }
    }
}