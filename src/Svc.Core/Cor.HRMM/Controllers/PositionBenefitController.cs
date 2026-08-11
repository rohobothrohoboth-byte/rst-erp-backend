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
/// Position Benefit end points
/// </summary>

[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionBenefit")]
[ApiVersion("1.0")]

public class PositionBenefitController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of Position Benefits by PositionId
    /// </summary>
    [HttpGet("AllPositionBenefit/{id:guid}")]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionBenefit(Guid id)
    {
        var response = await med.Send(new PosBenefitAllQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }
    
    [HttpGet("GetPositionBenefit/{id:guid}")]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionBenefit(Guid id)
    {
        var response = await med.Send(new PosBenefitByIdQry { Id = id });
        if (response == null) { throw new DomainException($"POSITION BENEFIT with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddPositionBenefit")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PosBenefitAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PosBenefitAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New POSITION BENEFIT successfully created."));
    }

    [HttpPut("ModPositionBenefit/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PosBenefitModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PosBenefitModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected POSITION BENEFIT successfully updated."));
    }

    [HttpDelete("DelPositionBenefit/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PosBenefitDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"POSITION BENEFIT with Id {id} successfully deleted."));
    }
}


