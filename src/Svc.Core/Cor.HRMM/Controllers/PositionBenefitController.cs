using Asp.Versioning;
using Cor.HRMM.Commands;
using Cor.HRMM.Helpers;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cor.HRMM.Controllers;

/// <summary>
/// Position Benefit end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionBenefit")]
[ApiVersion("1.0")]

public class PositionBenefitController(IMediator med) : ControllerBase
{
    /// <summary>
    /// End point to get list of Position Benefits by PositionId
    /// </summary>
    [HttpGet("AllPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionBenefit(Guid id)
    {
        var response = await med.Send(new PositionBenefitAllQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }
    
    [HttpGet("GetPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionBenefit(Guid id)
    {
        var response = await med.Send(new PositionBenefitByIdQry { Id = id });
        if (response == null) { throw new DomainException($"POSITION BENEFIT with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddPositionBenefit")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionBenefitAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new PositionBenefitAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New POSITION BENEFIT successfully created."));
    }

    [HttpPut("ModPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionBenefitModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var command = new PositionBenefitModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected POSITION BENEFIT successfully updated."));
    }

    [HttpDelete("DelPositionBenefit/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PositionBenefitDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"POSITION BENEFIT with Id {id} successfully deleted."));
    }
}