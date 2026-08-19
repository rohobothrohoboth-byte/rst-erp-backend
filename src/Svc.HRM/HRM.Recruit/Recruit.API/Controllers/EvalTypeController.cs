using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// EVALUATION TYPE management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/EvalType")]
[ApiVersion("1.0")]
public class EvalTypeController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.evaluation.view")]
    [HttpGet("AllEvalType")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEvalType()
    {
        var response = await med.Send(new EvalTypeAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.evaluation.view")]
    [HttpGet("GetEvalType/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvalType(Guid id)
    {
        var response = await med.Send(new EvalTypeByIdQry { Id = id });
        if (response == null) { throw new DomainException($"EVALUATION TYPE with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpPost("StatEvalType")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStat([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalTypeStatCmd { StatDto = statDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "EVALUATION TYPE status successfully changed."));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpPost("AddEvalType")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EvalTypeAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalTypeAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EVALUATION TYPE successfully created."));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpPut("ModEvalType/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EvalTypeModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalTypeModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected EVALUATION TYPE successfully updated."));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpDelete("DelEvalType/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EvalTypeDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"EVALUATION TYPE with Id {id} successfully deleted."));
    }
}