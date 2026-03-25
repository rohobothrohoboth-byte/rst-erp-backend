using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// EVALUATION STEP management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/EvalStep")]
[ApiVersion("1.0")]
public class EvalStepController(IMediator med) : ControllerBase
{
    /// <summary>
    /// All EVALUATION STEP by EVALUATION FLOW id
    /// </summary>
    [HttpGet("EvalFlowAllStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EvalFlowAllStep(Guid id)
    {
        var response = await med.Send(new EvalStepByFlowIdQry { Id = id });
        if (response == null) { throw new DomainException($"EVALUATION STEPS with EVALUATION FLOW id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("AllEvalStep")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllEvalStep()
    {
        var response = await med.Send(new EvalStepAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetEvalStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEvalStep(Guid id)
    {
        var response = await med.Send(new EvalStepByIdQry { Id = id });
        if (response == null) { throw new DomainException($"EVALUATION STEP with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddEvalStep")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] EvalStepAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalStepAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New EVALUATION STEP successfully created."));
    }

    [HttpPut("ModEvalStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EvalStepModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new EvalStepModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected EVALUATION STEP successfully updated."));
    }

    [HttpDelete("DelEvalStep/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new EvalStepDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"EVALUATION STEP with Id {id} successfully deleted."));
    }
}