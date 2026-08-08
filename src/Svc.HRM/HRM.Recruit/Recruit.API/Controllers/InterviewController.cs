// Recruit.API/Controllers/InterviewController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// Interview management end points
/// </summary>
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/Interview")]
[ApiVersion("1.0")]
public class InterviewController(IMediator med) : ControllerBase
{
    [HttpGet("ByApplicant/{applicantId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInterviewsByApplicant(Guid applicantId)
    {
        var response = await med.Send(new InterviewByApplicantQry { ApplicantId = applicantId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByJobPosting/{jobPostingId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInterviewsByJobPosting(Guid jobPostingId)
    {
        var response = await med.Send(new InterviewByJobPostingQry { JobPostingId = jobPostingId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("GetInterview/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInterview(Guid id)
    {
        var response = await med.Send(new InterviewByIdQry { Id = id });
        if (response == null)
        {
            throw new DomainException($"INTERVIEW with id [{id}] NOT FOUND.");
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

    // ✅ Add this endpoint
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllInterviews()
    {
        var response = await med.Send(new InterviewAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddInterview([FromBody] InterviewAddDto addDto)
    {
        // ✅ Validate required fields
        if (addDto.ApplicantId == Guid.Empty)
        {
            throw new DomainException("Applicant ID is required.");
        }
        if (addDto.JobPostingId == Guid.Empty)
        {
            throw new DomainException("Job Posting ID is required.");
        }
        if (string.IsNullOrEmpty(addDto.InterviewType))
        {
            throw new DomainException("Interview Type is required.");
        }
        if (addDto.ScheduledDate == DateTime.MinValue)
        {
            throw new DomainException("Scheduled Date is required.");
        }

        // ✅ Log the incoming data for debugging
        Console.WriteLine($"Received InterviewAddDto: {System.Text.Json.JsonSerializer.Serialize(addDto)}");

        var command = new InterviewAddCmd { AddDto = addDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Interview successfully created."));
    }

    [HttpPut("Mod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInterview(Guid id, [FromBody] InterviewModDto modDto)
    {
        if (!ModelState.IsValid || modDto.Id != id)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new InterviewModCmd { ModDto = modDto };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Interview successfully updated."));
    }

    [HttpDelete("Del/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInterview(Guid id)
    {
        var command = new InterviewDelCmd { Id = id };
        await med.Send(command);
        return Ok(ApiResponse<string>.Ok(null!, $"INTERVIEW with Id {id} successfully deleted."));
    }

    // Recruit.API/Controllers/InterviewController.cs

    [HttpPatch("UpdateStatus/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInterviewStatus(Guid id, [FromBody] InterviewStatusUpdateDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        var command = new InterviewStatusUpdateCmd { Id = id, Status = statusDto.Status };
        var response = await med.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Interview status successfully updated."));
    }
}