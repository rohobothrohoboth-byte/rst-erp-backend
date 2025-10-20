using System.Data;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// Position Requirement end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionReq")]
[ApiVersion("1.0")]

public class PositionReqController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of Position Requirement by PositionId
    /// </summary>
    [HttpGet("AllPositionReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionReq(Guid id)
    {
        var response = await med.Send(new PositionReqAllQry { Id = id  });
        return Ok(response);
    }

    [HttpGet("GetPositionReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionReq(Guid id)
    {
        var response = await med.Send(new PositionReqByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"Position Requirement with Id {id} not found" });
        }
        return Ok(response);
    }

    [HttpPost("AddPositionReq")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionReqAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new PositionReqAddCmd { AddDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetPositionReq), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Position Requirement", Details = ex.Message });
        }
    }

    [HttpPut("ModPositionReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionReqModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new PositionReqModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Position Requirement was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Requirement with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Position Requirement", Details = ex.Message });
        }
    }

    [HttpDelete("DelPositionReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new PositionReqDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Requirement with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Position Requirement", Details = ex.Message });
        }
    }
}