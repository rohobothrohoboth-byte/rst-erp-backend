using Asp.Versioning;
using Cor.App.Commands.Comp;
using Cor.App.Queries;
using Cor.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Cor.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/core/v{version:apiVersion}/company")]
[ApiVersion("1.0")]
public class CompanyController(IMediator med) : ControllerBase
{
    [HttpPost("AddCompany")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddCompDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddCompCmd { AddCompDto = addDto };
            var compId = await med.Send(command);
            return CreatedAtAction(nameof(GetCompany), new { id = compId }, compId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create COMPANY", Details = ex.Message });
        }
    }
    
    [HttpGet("AllCompany")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllCompanies()
    {
        var comps = await med.Send(new AllCompsQry());
        return Ok(comps);
    }
    
    [HttpGet("GetCompany/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var comp = await med.Send(new CompByIdQry { Id = id });
        if (comp == null)
        {
            return NotFound(new { Error = $"COMPANY with Id {id} not found" });
        }
        return Ok(comp);
    }

    [HttpPut("ModCompany/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditCompDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new ModCompCmd { EditCompDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the COMPANY was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"COMPANY with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update COMPANY", Details = ex.Message });
        }
    }
    
    [HttpDelete("DelCompany/id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelCompCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"COMPANY with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete COMPANY", Details = ex.Message });
        }
    }
}
