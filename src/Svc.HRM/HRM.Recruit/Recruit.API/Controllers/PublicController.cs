// Recruit.API/Controllers/PublicController.cs

using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// Public endpoints for external applicants
/// </summary>
[ApiController]
[Route("api/public/recruit/v{version:apiVersion}")]
[ApiVersion("1.0")]
public class PublicController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new external applicant
    /// </summary>
    [HttpPost("RegisterApplicant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterApplicant([FromBody] ExternalApplicantRegistrationDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        var command = new RegisterExternalApplicantCmd { Dto = dto };
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Applicant registered successfully."));
    }

    /// <summary>
    /// Check if an applicant exists by email
    /// </summary>
    [HttpGet("CheckApplicantExists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckApplicantExists([FromQuery] string email)
    {
        var command = new CheckApplicantExistsQry { Email = email };
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get applicant by email or phone
    /// </summary>
    [HttpGet("GetApplicantByContact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplicantByContact([FromQuery] string email, [FromQuery] string? phone = null)
    {
        var command = new ApplicantByContactQry { Email = email, Phone = phone };
        var response = await _mediator.Send(command);
        if (response == null)
        {
            throw new DomainException("Applicant not found.");
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Apply for a job (external applicant)
    /// </summary>
    [HttpPost("ApplyExternal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApplyExternal([FromForm] JobAppExtAddDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        var command = new JobAppExtAddCmd { AddDto = dto };
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Application submitted successfully."));
    }

    /// <summary>
    /// Get all published vacancies for external applicants
    /// </summary>
    [HttpGet("PublishedVacancies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublishedVacancies()
    {
        var response = await _mediator.Send(new VacancyListQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get vacancy details by ID
    /// </summary>
    [HttpGet("VacancyDetail/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVacancyDetail(Guid id)
    {
        var response = await _mediator.Send(new VacancyDetailQry { Id = id });
        if (response == null)
        {
            throw new DomainException($"VACANCY with id [{id}] NOT FOUND.");
        }
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Get applications by external applicant
    /// </summary>
    [HttpGet("GetApplications/{applicantId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplications(Guid applicantId)
    {
        var response = await _mediator.Send(new JobAppByApplicantQry { ApplicantId = applicantId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    /// <summary>
    /// Update external applicant profile
    /// </summary>
    [HttpPut("UpdateApplicant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateApplicant([FromBody] ExternalApplicantUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            throw new ValException(errors);
        }

        var command = new UpdateExternalApplicantCmd { Dto = dto };
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Profile updated successfully."));
    }

    /// <summary>
    /// Withdraw application
    /// </summary>
    [HttpPut("WithdrawApplication/{applicationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> WithdrawApplication(Guid applicationId, [FromBody] WithdrawApplicationDto dto)
    {
        var command = new WithdrawApplicationCmd { ApplicationId = applicationId, Reason = dto.Reason };
        var response = await _mediator.Send(command);
        return Ok(ApiResponse<object>.Ok(response, "Application withdrawn successfully."));
    }
}