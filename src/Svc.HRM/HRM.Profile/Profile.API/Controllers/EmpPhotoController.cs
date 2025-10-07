using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Images end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpPhoto")]
[ApiVersion("1.0")]
public class EmpPhotoController(IMediator med) : ControllerBase
{
    //[HttpGet("AllEmpPhoto")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> AllBranch()
    //{
    //    var branches = await med.Send(new EmpPhotoAllQry());
    //    return Ok(branches);
    //}

    [HttpGet("EmpPhoto/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpPhoto(Guid id)
    {
        var res = await med.Send(new EmpPhotoByIdQry { Id = id });
        if (res == null)
        {
            return NotFound(new { Error = $"EMPLOYEE PHOTO with Id {id} NOT FOUND" });
        }
        return Ok(res);
    }
    
    [HttpPost("AddEmpPhoto")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] EmpPhotoAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpPhotoAddCmd { AddDto = addDto };
            var braId = await med.Send(command);
            return CreatedAtAction(nameof(GetEmpPhoto), new { id = braId.Id }, braId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to upload EMPLOYEE PHOTO", Details = ex.Message });
        }
    }

    //[HttpPut("ModBranch/{id:guid}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status409Conflict)]
    //public async Task<IActionResult> Update(Guid id, [FromBody] EditBranchDto modDto)
    //{
    //    if (!ModelState.IsValid || modDto.Id != id)
    //    {
    //        return BadRequest(ModelState);
    //    }

    //    try
    //    {
    //        var modBra = await med.Send(new ModBranchCmd { EditBranchDto = modDto });
    //        return Ok(modBra);
    //    }
    //    catch (DBConcurrencyException ex)
    //    {
    //        return Conflict(new { Error = "Concurrency conflict: the BRANCH was modified by another user", Details = ex.Message });
    //    }
    //    catch (KeyNotFoundException)
    //    {
    //        return NotFound(new { Error = $"BRANCH with Id {id} not found" });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { Error = "Failed to update BRANCH", Details = ex.Message });
    //    }
    //}

    //[HttpDelete("DelBranch/{id:guid}")]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> Delete(Guid id)
    //{
    //    try
    //    {
    //        await med.Send(new DelBranchCmd { Id = id });
    //        return NoContent();
    //    }
    //    catch (KeyNotFoundException)
    //    {
    //        return NotFound(new { Error = $"BRANCH with Id {id} not found" });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { Error = "Failed to delete BRANCH", Details = ex.Message });
    //    }
    //}
}