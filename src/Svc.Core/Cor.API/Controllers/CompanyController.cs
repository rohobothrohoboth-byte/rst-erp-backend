using Asp.Versioning;
using Cor.App.Commands.Comp;
using Cor.App.Queries;
using Cor.Domain.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Cor.API.Controllers;

[ApiController]
[Route("core/api/v{version:apiVersion}/company")]
[ApiVersion("1.0")]
public class CompanyController(IMediator med) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddCompDto branchDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var command = new AddCompCmd { AddCompDto = branchDto };
            var branchId = await med.Send(command);
            return CreatedAtAction(nameof(GetComp), new { id = branchId }, new { Id = branchId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create COMPANY", Details = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComps()
    {
        var branches = await med.Send(new GetCompsQry());
        return Ok(branches);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComp(Guid id)
    {
        var branch = await med.Send(new GetCompByIdQry { Id = id });
        if (branch == null)
        {
            return NotFound(new { Error = $"COMPANY with Id {id} not found" });
        }
        return Ok(branch);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditCompDto branchDto)
    {
        if (!ModelState.IsValid || branchDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedBranch = await med.Send(new UpdateCompCmd { EditCompDto = branchDto });
            return Ok(updatedBranch);
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

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DeleteCompCmd { Id = id });
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
