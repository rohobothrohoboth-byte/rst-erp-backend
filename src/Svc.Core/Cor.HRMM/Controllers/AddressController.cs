using System.Data;
using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/address")]
[ApiVersion("1.0")]

public class AddressController(IMediator med) : ControllerBase
{
    [HttpGet("AllAddress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllAddress()
    {
        var response = await med.Send(new AllAddressQry());
        return Ok(response);
    }
    
    [HttpGet("GetAddress/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddress(Guid id)
    {
        var response = await med.Send(new AddressByIdQry { Id = id });
        if (response == null)
        {
            return NotFound(new { Error = $"ADDRESS with Id {id} not found" });
        }
        return Ok(response);
    }

    [HttpPost("AddAddress")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddAddressDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new AddAddressCmd { AddAddressDto = addDto };
            var response = await med.Send(command);
            return CreatedAtAction(nameof(GetAddress), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to create ADDRESS", Details = ex.Message });
        }
    }

    [HttpPut("ModAddress/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditAddressDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new ModAddressCmd { EditAddressDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the ADDRESS was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"ADDRESS with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update ADDRESS", Details = ex.Message });
        }
    }

    [HttpDelete("DelAddress/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new DelAddressCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"ADDRESS with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete ADDRESS", Details = ex.Message });
        }
    }
}
