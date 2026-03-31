using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB REQUISITION management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobReq")]
[ApiVersion("1.0")]
public class JobReqController(IMediator med) : ControllerBase
{
    [HttpGet("AllJobReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllJobReq(Guid id)
    {
        var response = await med.Send(new JobReqAllQry { Id = id });
        if (response == null) { throw new DomainException($"JOB REQUISITIONS with Worfk force plan id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetJobReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobReq(Guid id)
    {
        var response = await med.Send(new JobReqByIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB REQUISITION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("AddJobReq")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] JobReqAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobRequisitionAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New JOB REQUISITION successfully created."));
    }

    [HttpPut("ModJobReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] JobReqModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobRequisitionModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected JOB REQUISITION successfully updated."));
    }

    [HttpDelete("DelJobReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new JobRequisitionDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"JOB REQUISITION with Id {id} successfully deleted."));
    }
}