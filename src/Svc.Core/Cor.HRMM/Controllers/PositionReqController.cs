// Cor.HRMM/Controllers/PositionReqController.cs

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
/// Position Requirement end points
/// </summary>
[Authorize(AuthenticationSchemes = "ApiKey,Bearer")]
[ApiController]
[Route("api/core/hrmm/v{version:apiVersion}/PositionReq")]
[ApiVersion("1.0")]
public class PositionReqController(IMediator med) : ControllerBase
{
    /// <summary>
    /// ✅ GET ALL Position Requirements (No ID required)
    /// </summary>
    [HttpGet]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPositionRequirements()
    {
        var response = await med.Send(new PositionReqGetAllQry());  // ✅ Use the new query
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// End point to get list of Position Requirement by PositionId
    /// </summary>
    [HttpGet("AllPositionReq/{id:guid}")]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllPositionReq(Guid id)
    {
        var response = await med.Send(new PositionReqAllQry { Id = id });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetPositionReq/{id:guid}")]
    [PerAuth("position.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPositionReq(Guid id)
    {
        var response = await med.Send(new PositionReqByIdQry { Id = id });
        if (response == null) { throw new DomainException($"POSITION REQUIREMENT with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddPositionReq")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PositionReqAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PosReqAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New POSITION REQUIREMENT successfully created."));
    }

    [HttpPut("ModPositionReq/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionReqModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new PosReqModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected POSITION REQUIREMENT successfully updated."));
    }

    [HttpDelete("DelPositionReq/{id:guid}")]
    [PerAuth("position.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new PosReqDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"POSITION REQUIREMENT with Id {id} successfully deleted."));
    }
}