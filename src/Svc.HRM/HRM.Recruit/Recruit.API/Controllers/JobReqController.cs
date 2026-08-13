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
/// JOB REQUISITION management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobReq")]
[ApiVersion("1.0")]
public class JobReqController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("AllJobReq")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllJobReq()
    {
        var response = await med.Send(new JobReqAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// All JOB REQUISITION of selected Work force plan.
    /// </summary>
    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("AllWfpJobReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AllWfpJobReq(Guid id)
    {
        var response = await med.Send(new JobReqAllByWfpIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB REQUISITIONS with Worfk force plan id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("GetJobReq/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobReq(Guid id)
    {
        var response = await med.Send(new JobReqByIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB REQUISITION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.requisition.view")]
    [HttpGet("GetJobReqDetail/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobReqDetail(Guid id)
    {
        var response = await med.Send(new JobReqDetailQry { Id = id });
        if (response == null) { throw new DomainException($"JOB REQUISITION with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.requisition.manage")]
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

    [PerAuth("hr.recruit.requisition.manage")]
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

    [PerAuth("hr.recruit.requisition.manage")]
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