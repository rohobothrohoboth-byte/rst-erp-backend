using System.Data;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// Job Grade Step/ እርከን end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/JgStep")]
[ApiVersion("1.0")]

public class JgStepController(IMediator med) : ControllerBase
{

    /// <summary>
    /// End point to get list of Job Grade Steps by JobGradeId
    /// </summary>
    [HttpGet("AllJgSteps/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJgSteps(Guid id)
    {
        var branches = await med.Send(new JgStepAllQry { Id = id });
        return Ok(branches);
    }
    
    [HttpGet("GetJgStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJgStep(Guid id)
    {
        var response = await med.Send(new JgStepByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"Job Grade Step with Id {id} not found" });
        }
        return Ok(response);
    }

    [HttpPost("AddJgStep")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] JgStepAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new JgStepAddCmd { AddDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetJgStep), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create Job Grade Step", Details = ex.Message });
        }
    }

    [HttpPut("ModJgStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] JgStepModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new JgStepModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Job Grade Step was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Job Grade Step with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Job Grade Step", Details = ex.Message });
        }
    }

    [HttpDelete("DelJgStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new JgStepDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Job Grade Step with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Job Grade Step", Details = ex.Message });
        }
    }
}