using System.Data;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Emergency Contacts end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmContact")]
[ApiVersion("1.0")]
public class EmContactController(IMediator med) : ControllerBase
{
    [HttpGet("AllEmContact/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEmContact(Guid id)
    {
        var response = await med.Send(new EmContactAllQry { Id = id });
        return Ok(response);
    }

    [HttpGet("GetEmContact/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmContact(Guid id)
    {
        var response = await med.Send(new EmContactByIdQry { Id = id });
        if (response == null) { return NotFound(new { Error = $"Emergency Contact with Id {id} not found" }); }
        return Ok(response);
    }
    
    [HttpPut("ModEmContact/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmContactModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var modComp = await med.Send(new EmContactModCmd { ModDto = modDto });
            return Ok(modComp);
        }
        catch (DBConcurrencyException ex)
        {
            return Conflict(new { Error = "Concurrency conflict: the Emergency Contact was modified by another user", Details = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Emergency Contact with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to update Emergency Contact", Details = ex.Message });
        }
    }

    [HttpDelete("DelEmContact/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await med.Send(new EmContactDelCmd { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Error = $"Emergency Contact with Id {id} not found" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to delete Emergency Contact", Details = ex.Message });
        }
    }
}