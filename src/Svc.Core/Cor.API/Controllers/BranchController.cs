using Asp.Versioning;
using Cor.App.Commands.BranchOff;
using Cor.App.Queries;
using Cor.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cor.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/core/v{version:apiVersion}/branch")]
[ApiVersion("1.0")]
public class BranchController(IMediator med) : ControllerBase
{
    [HttpPost("AddBranch")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddBranchDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddBranchCmd{ AddBranchDto = addDto };
            var braId = await med.Send(command);
            return CreatedAtAction(nameof(GetBranch), new { id = braId }, braId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create BRANCH", Details = ex.Message });
        }
    }

    [HttpGet("AllBranch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllBranches()
    {
        var branches = await med.Send(new AllBranchesQry());
        return Ok(branches);
    }
    
    [HttpGet("BranchComp/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CompBranches(Guid id)
    {
        var branches = await med.Send(new BranchByCompQry { Id = id });
        return Ok(branches);
    }

    [HttpGet("GetBranch/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch(Guid id)
    {
        var bra = await med.Send(new BranchByIdQry { Id = id });
        if (bra == null)
        {
            return NotFound(new { Error = $"BRANCH with Id {id} not found" });
        }
        return Ok(bra);
    }

    [HttpPut("ModBranch/id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditBranchDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modBra = await med.Send(new ModBranchCmd { EditBranchDto = modDto });
            return Ok(modBra);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the BRANCH was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"BRANCH with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update BRANCH", Details = ex.Message });
        }
    }

    [HttpDelete("DelBranch/id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelBranchCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"BRANCH with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete BRANCH", Details = ex.Message });
        }
    }
}
