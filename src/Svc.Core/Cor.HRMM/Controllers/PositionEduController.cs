using System.Data;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// Position Education end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionEdu")]
[ApiVersion("1.0")]

public class PositionEduController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of Position Education by PositionId
    /// </summary>
    [HttpGet("AllPositionEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionEdu(Guid id)
    {
        var response = await med.Send(new PositionEduAllQry { Id = id  });
        return Ok(response);
    }

    [HttpGet("GetPositionEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionEdu(Guid id)
    {
        var response = await med.Send(new PositionEduByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"Position Education with Id {id} not found" });
        }
        return Ok(response);
    }

    [HttpPost("AddPositionEdu")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionEduAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new PositionEduAddCmd { AddDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetPositionEdu), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Position Education", Details = ex.Message });
        }
    }

    [HttpPut("ModPositionEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionEduModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new PositionEduModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Position Education was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Education with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Position Education", Details = ex.Message });
        }
    }

    [HttpDelete("DelPositionEdu/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new PositionEduDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Position Education with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Position Education", Details = ex.Message });
        }
    }
}