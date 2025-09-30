using System.Data;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Module.App.Commands;
using Module.App.Queries;
using Module.Domain.DTOs;

namespace Module.API.Controllers;

/// <summary>
/// Period management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/module/v{version:apiVersion}/Period")]
[ApiVersion("1.0")]
public class PeriodController(IMediator med) : ControllerBase
{
    [HttpGet("AllPeriod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPeriod()
    {
        var response = await med.Send(new AllPeriodQry());
        return Ok(response);
    }

    [HttpGet("GetPeriod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPeriod(Guid id)
    {
        var response = await med.Send(new PeriodByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"PERIOD with Id {id} not found" });
        }
        return Ok(response);
    }
    
    [HttpPost("AddPeriod")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddPeriodDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddPeriodCmd { AddPeriodDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetPeriod), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create PERIOD", Details = ex.Message });
        }
    }

    [HttpPut("ModPeriod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditPeriodDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await med.Send(new ModPeriodCmd { EditPeriodDto = modDto });
            return Ok(response);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the PERIOD was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"PERIOD with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update PERIOD", Details = ex.Message });
        }
    }

    [HttpDelete("DelPeriod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelPeriodCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"PERIOD with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete PERIOD", Details = ex.Message });
        }
    }
}