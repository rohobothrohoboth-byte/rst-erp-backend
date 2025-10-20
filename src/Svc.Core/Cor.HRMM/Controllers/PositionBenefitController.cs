using System.Data;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// Position Benefit end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionBenefit")]
[ApiVersion("1.0")]

public class PositionBenefitController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of Position Benefits by PositionId
    /// </summary>
    [HttpGet("AllPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionBenefit(Guid id)
    {
        var response = await med.Send(new PositionBenefitAllQry { Id = id  });
        return Ok(response);
    }
    
    [HttpGet("GetPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionBenefit(Guid id)
    {
        var response = await med.Send(new PositionBenefitByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"Position Benefit with Id {id} not found" });
        }
        return Ok(response);
    }

    [HttpPost("AddPositionBenefit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionBenefitAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new PositionBenefitAddCmd { AddDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetPositionBenefit), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Position Benefit", Details = ex.Message });
        }
    }

    [HttpPut("ModPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionBenefitModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new PositionBenefitModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Position Benefit was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Benefit with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Position Benefit", Details = ex.Message });
        }
    }

    [HttpDelete("DelPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new PositionBenefitDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Benefit with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Position Benefit", Details = ex.Message });
        }
    }
}