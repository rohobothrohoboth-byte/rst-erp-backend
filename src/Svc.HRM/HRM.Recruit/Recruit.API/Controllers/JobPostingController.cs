using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using System.Security.Claims;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB POSTING management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobPosting")]
[ApiVersion("1.0")]
public class JobPostingController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.posting.view")]
    [HttpGet("AllJobPosting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AllJobPosting()
    {
        var response = await med.Send(new JobPostingAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// JOB POSTINGS by WORK FORCE PLAN Id
    /// </summary>
    [PerAuth("hr.recruit.posting.view")]
    [HttpGet("JobPostByWfp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JobPostByWfp(Guid id)
    {
        var response = await med.Send(new JobPostingByWfpIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB POSTING with Work force plan id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// JOB POSTINGS by Department Id
    /// </summary>
    [PerAuth("hr.recruit.posting.view")]
    [HttpGet("JobPostByDept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JobPostByDept()
    {

        var empId = User.FindFirstValue("employeeId");
        if (empId is { Length: <= 0 }) { throw new UnauthorizedException("AUTHORIZATION REQUIRED to gain access."); }

        var id = Guid.Parse(empId!);
        var response = await med.Send(new JobPostingByDeptIdQry { Id = id });
        if (response == null) { throw new DomainException("WORKFORCE PLANS for your department are NOT AVAILABLE."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.posting.view")]
    [HttpGet("GetJobPosting/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobPosting(Guid id)
    {
        var response = await med.Send(new JobPostingByIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB POSTING with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.posting.manage")]
    [HttpPost("AddJobPosting")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] JobPostingAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobPostingAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "New JOB POSTING successfully created."));
    }

    [PerAuth("hr.recruit.posting.manage")]
    [HttpPost("AddAllJobPosting")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAll([FromBody] JobPostingAddDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobPostingAddAllCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "All JOB POSTINGS successfully created."));
    }

    [PerAuth("hr.recruit.posting.manage")]
    [HttpPut("ModJobPosting/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] JobPostingModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new JobPostingModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Selected JOB POSTING successfully updated."));
    }

    [PerAuth("hr.recruit.posting.manage")]
    [HttpDelete("DelJobPosting/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new JobPostingDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"JOB POSTING with Id {id} successfully deleted."));
    }
}