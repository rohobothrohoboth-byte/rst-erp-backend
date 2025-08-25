using Asp.Versioning;
using Cor.App.Commands.Hier;
using Cor.App.Queries;
using Cor.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cor.API.Controllers;

[Authorize]
[ApiController]
[Route("api/core/v{version:apiVersion}/hierarchy")]
[ApiVersion("1.0")]
public class HierarchyController(IMediator med) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddHierDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddHierCmd { AddHierDto = addDto };
            var hierId = await med.Send(command);
            return CreatedAtAction(nameof(GetHierarchy), new { id = hierId }, new { Id = hierId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create HIERARCHY", Details = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllHierarchies()
    {
        var comps = await med.Send(new AllCompsQry());
        return Ok(comps);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHierarchy(Guid id)
    {
        var hier = await med.Send(new HierByIdQry { Id = id });
        if (hier == null)
        {
            return NotFound(new { Error = $"HIERARCHY with Id {id} not found" });
        }
        return Ok(hier);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditHierDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modHier = await med.Send(new ModHierCmd { EditHierDto = modDto });
            return Ok(modHier);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the HIERARCHY was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"HIERARCHY with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update HIERARCHY", Details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelHierCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"HIERARCHY with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete HIERARCHY", Details = ex.Message });
        }
    }
}
