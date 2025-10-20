using System.Data;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// Position Experience end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionExp")]
[ApiVersion("1.0")]

public class PositionExpController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of Position Experience by PositionId
    /// </summary>
    [HttpGet("AllPositionExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionExp(Guid id)
    {
        var response = await med.Send(new PositionExpAllQry { Id = id  });
        return Ok(response);
    }

    [HttpGet("GetPositionExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionExp(Guid id)
    {
        var response = await med.Send(new PositionExpByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"Position Experience with Id {id} not found" });
        }
        return Ok(response);
    }

    [HttpPost("AddPositionExp")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionExpAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new PositionExpAddCmd { AddDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetPositionExp), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Position Experience", Details = ex.Message });
        }
    }

    [HttpPut("ModPositionExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionExpModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new PositionExpModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Position Experience was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Experience with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Position Experience", Details = ex.Message });
        }
    }

    [HttpDelete("DelPositionExp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new PositionExpDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Experience with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Position Experience", Details = ex.Message });
        }
    }
}