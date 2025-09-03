using Asp.Versioning;
using Cor.App.Commands.FiscYear;
using Cor.App.Queries;
using Cor.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Cor.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/core/v{version:apiVersion}/fiscalyear")]
[ApiVersion("1.0")]
public class FiscalYearController(IMediator med) : ControllerBase
{
    [HttpPost("AddFiscalYear")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddFiscYearDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddFiscalYearCmd { AddFiscYearDto = addDto };
            var fYearId = await med.Send(command);
            return CreatedAtAction(nameof(GetFiscalYear), new { id = fYearId }, fYearId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create FISCAL YEAR", Details = ex.Message });
        }
    }

    [HttpGet("AllFiscalYear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllFiscalYears()
    {
        var comps = await med.Send(new AllCompsQry());
        return Ok(comps);
    }

    [HttpGet("GetFiscalYear/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFiscalYear(Guid id)
    {
        var fiscYear = await med.Send(new FiscalYearByIdQry { Id = id });
        if (fiscYear == null)
        {
            return NotFound(new { Error = $"FISCAL YEAR with Id {id} not found" });
        }
        return Ok(fiscYear);
    }

    [HttpPut("ModFiscalYear/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditFiscYearDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modFiscYear = await med.Send(new ModFiscalYearCmd { EditFiscYearDto = modDto });
            return Ok(modFiscYear);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the FISCAL YEAR was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"FISCAL YEAR with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update FISCAL YEAR", Details = ex.Message });
        }
    }

    [HttpDelete("DelFiscalYear/id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelFiscalYearCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"FISCAL YEAR with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete FISCAL YEAR", Details = ex.Message });
        }
    }
}
