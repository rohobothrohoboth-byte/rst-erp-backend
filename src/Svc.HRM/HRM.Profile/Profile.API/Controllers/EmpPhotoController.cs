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
    [HttpGet("EmpPhoto/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpPhoto(Guid id)
    {
        var res = await med.Send(new EmpPhotoByIdQry { Id = id });
        if (res == null) { return NotFound(new { Error = $"EMPLOYEE PHOTO with Id {id} NOT FOUND" }); }
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
            var res = await med.Send(command);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to upload EMPLOYEE PHOTO", Details = ex.Message });
        }
    }

}