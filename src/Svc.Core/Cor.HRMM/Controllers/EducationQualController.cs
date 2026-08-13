using Asp.Versioning;
using Common;
using Cor.HRMM.Commands;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace Cor.HRMM.Controllers;

/// <summary>
/// Education Qualification end points
/// </summary>

[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/EducationQual")]
[ApiVersion("1.0")]

public class EducationQualController(IMediator med) : ControllerBase
{
    [PerAuth("hr.emp.view|hr.db.view|hr.recruit.requisition.view")]
    [HttpGet("AllEducationQual")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEducationQual()
    {
        var response = await med.Send(new EducationQualAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.emp.view|hr.db.view|hr.recruit.requisition.view")]
    [HttpGet("GetEducationQual/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEducationQual(Guid id)
    {
        var response = await med.Send(new EducationQualByIdQry { Id = id });
        if (response == null) { throw new DomainException($"EDUCATION QUALIFICATION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddEducationQual")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EducationQualAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EducationQualAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EDUCATION QUALIFICATION successfully created."));
    }

    [HttpPut("ModEducationQual/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EducationQualModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EducationQualModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected EDUCATION QUALIFICATION successfully updated."));
    }

    [HttpDelete("DelEducationQual/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EducationQualDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"EDUCATION QUALIFICATION with Id {id} successfully deleted."));
    }
}


