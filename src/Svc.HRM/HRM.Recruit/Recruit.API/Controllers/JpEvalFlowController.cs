// Recruit.API/Controllers/JpEvalFlowController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB POST'S EVALUATION FLOW management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JpEvalFlow")]
[ApiVersion("1.0")]
public class JpEvalFlowController(IMediator med) : ControllerBase
{
   // Recruit.API/Controllers/JpEvalFlowController.cs

   [HttpPost("AllJpEvalFlow/{id:guid}")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<IActionResult> AllJpEvalFlow(Guid id)
   {
       var response = await med.Send(new JpEvalFlowByJpIdQry { Id = id });

       // ? response is now List<JpEvalFlowListDto>
       if (response == null || response.Count == 0)
       {
           return Ok(ApiResponse<object>.Ok(new List<JpEvalFlowListDto>()));
       }

       return Ok(ApiResponse<object>.Ok(response));
   }

    [HttpGet("GetJpEvalFlow/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJpEvalFlow(Guid id)
    {
        var response = await med.Send(new JpEvalFlowByIdQry { Id = id });
        if (response == null)
        {
            // ? Fix: Return null with 200 OK instead of throwing
            return Ok(ApiResponse<object>.Ok(null));
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddJpEvalFlow")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] JpEvalFlowAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JpEvalFlowAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "JOB POST'S EVALUATION FLOW successfully assigned."));
    }

    [HttpPut("ModJpEvalFlow/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] JpEvalFlowModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JpEvalFlowModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected JOB POST'S EVALUATION FLOW successfully updated."));
    }

    [HttpDelete("DelJpEvalFlow/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new JpEvalFlowDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"JOB POST'S EVALUATION FLOW with Id {id} successfully deleted."));
    }
}