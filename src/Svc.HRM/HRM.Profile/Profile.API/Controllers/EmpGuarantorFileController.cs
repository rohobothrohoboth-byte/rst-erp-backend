using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Profile.App.Commands;
using Profile.App.Queries;
using Profile.Domain.DTOs;

namespace Profile.API.Controllers;

/// <summary>
/// Employee Guarantor File end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/profile/v{version:apiVersion}/EmpGuarantorFile")]
[ApiVersion("1.0")]
public class EmpGuarantorFileController(IMediator med) : ControllerBase
{

    [HttpGet("EmpGuarantorFile/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpGuarantorFile(Guid id)
    {
        var res = await med.Send(new EmpGuarantorFileByIdQry { Id = id });
        if (res == null)
        {
            return NotFound(new { Error = $"EMPLOYEE GUARANTOR FILE with Id {id} NOT FOUND" });
        }
        return Ok(res);
    }

    [HttpPost("AddEmpGuarantorFile")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromForm] EmpGuarantorFileAddDto addDto)
    {
        if (!ModelState.IsValid) { return BadRequest(ModelState); }

        try
        {
            var command = new EmpGuarantorFileAddCmd { AddDto = addDto };
            var braId = await med.Send(command);
            return CreatedAtAction(nameof(GetEmpGuarantorFile), new { id = braId.Id }, braId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = "Failed to upload EMPLOYEE GUARANTOR FILE", Details = ex.Message });
        }
    }



}
