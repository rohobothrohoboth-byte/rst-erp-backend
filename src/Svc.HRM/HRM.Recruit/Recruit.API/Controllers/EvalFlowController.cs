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
/// EVALUATION FLOW management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/EvalFlow")]
[ApiVersion("1.0")]
public class EvalFlowController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.evaluation.view")]
    [HttpGet("AllEvalFlow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEvalFlow()
    {
        var response = await med.Send(new EvalFlowAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.evaluation.view")]
    [HttpGet("GetEvalFlow/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvalFlow(Guid id)
    {
        var response = await med.Send(new EvalFlowByIdQry { Id = id });
        if (response == null) { throw new DomainException($"EVALUATION FLOW with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpPost("StatEvalFlow")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStat([FromBody] StatChangeDto statDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalFlowStatCmd { StatDto = statDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "EVALUATION FLOW status successfully changed."));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpPost("AddEvalFlow")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EvalFlowAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalFlowAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EVALUATION FLOW successfully created."));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpPut("ModEvalFlow/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EvalFlowModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalFlowModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected EVALUATION FLOW successfully updated."));
    }

    [PerAuth("hr.recruit.evaluation.manage")]
    [HttpDelete("DelEvalFlow/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EvalFlowDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"EVALUATION FLOW with Id {id} successfully deleted."));
    }
}