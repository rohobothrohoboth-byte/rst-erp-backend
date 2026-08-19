// Recruit.API/Controllers/ApplicantController.cs

using Asp.Versioning;
using Common;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;  // ? Add this for the command
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
namespace Recruit.API.Controllers;

/// <summary>
/// JOB APPLICANTS management end points
/// </summary>

//[Authorize]
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/Applicant")]
[ApiVersion("1.0")]
public class ApplicantController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.applicant.manage")]
    [HttpPost("AllIntApp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AllIntApp()
    {
        var response = await med.Send(new JobAppAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.applicant.view")]
    [HttpGet("GetIntApp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIntApp(Guid id)
    {
        var response = await med.Send(new JobAppByIdQry { Id = id });
        if (response == null) { throw new DomainException($"JOB APPLICANT with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.applicant.view")]
    [HttpGet("JobPostAllIntApp/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JobPostAllIntApp(Guid id)
    {
        var response = await med.Send(new JobAppByJobPostQry { Id = id });
        if (response == null) { throw new DomainException($"JOB APPLICANTS with Job Post id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ? Add the UpdateStatus endpoint
    [PerAuth("hr.recruit.applicant.manage")]
    [HttpPut("UpdateStatus/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateApplicantStatusDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new UpdateApplicantStatusCmd
        {
            Id = id,
            Status = statusDto.Status,
            Reason = statusDto.Reason
        };

        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, $"Applicant status updated to {statusDto.Status}."));
    }
}