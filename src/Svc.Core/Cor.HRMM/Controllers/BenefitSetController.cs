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
/// Benefit Setting end points
/// </summary>

[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/BenefitSet")]
[ApiVersion("1.0")]

public class BenefitSetController(IMediator med) : ControllerBase
{
    [HttpGet("AllBenefitSet")]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllBenefitSet()
    {
        var response = await med.Send(new BenefitSetAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetBenefitSet/{id:guid}")]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBenefitSet(Guid id)
    {
        var response = await med.Send(new BenefitSetByIdQry { Id = id });
        if (response == null) { throw new DomainException($"BENEFIT SETTING with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddBenefitSet")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] BenefitSetAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new BenefitSetAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New BENEFIT SETTING successfully created."));
    }

    [HttpPut("ModBenefitSet/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] BenefitSetModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new BenefitSetModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected BENEFIT SETTING successfully updated."));
    }

    [HttpDelete("DelBenefitSet/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new BenefitSetDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"BENEFIT SETTING with Id {id} successfully deleted."));
    }
}


