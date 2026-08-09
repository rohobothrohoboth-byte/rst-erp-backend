using Asp.Versioning;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Recruit.App.Commands;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;

namespace Recruit.API.Controllers;

/// <summary>
/// JOB OFFER lifecycle: draft → approve → extend → accept/decline → hire
/// </summary>
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/JobOffer")]
[ApiVersion("1.0")]
public class JobOfferController(IMediator med) : ControllerBase
{
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> All()
    {
        var response = await med.Send(new JobOfferAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("Get/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new JobOfferByIdQry { Id = id });
        if (response == null)
            throw new DomainException($"JOB OFFER with id [{id}] NOT FOUND.");
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpGet("ByApplication/{jobApplicationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ByApplication(Guid jobApplicationId)
    {
        var response = await med.Send(new JobOfferByApplicationQry { JobApplicationId = jobApplicationId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [HttpPost("Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Add([FromBody] JobOfferAddDto addDto)
    {
        EnsureValid();
        var response = await med.Send(new JobOfferAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "Job offer created."));
    }

    [HttpPut("Mod/{id:guid}")]
    public async Task<IActionResult> Mod(Guid id, [FromBody] JobOfferModDto modDto)
    {
        EnsureValid();
        if (modDto.Id != id)
            throw new ValException("ID mismatch between route and body.");
        var response = await med.Send(new JobOfferModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Job offer updated."));
    }

    [HttpDelete("Del/{id:guid}")]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new JobOfferDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"Job offer {id} deleted."));
    }

    [HttpPost("Submit/{id:guid}")]
    public async Task<IActionResult> Submit(Guid id)
    {
        var response = await med.Send(new JobOfferSubmitCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Job offer submitted for approval."));
    }

    [HttpPost("Approve")]
    public async Task<IActionResult> Approve([FromBody] JobOfferApproveDto dto)
    {
        EnsureValid();
        var response = await med.Send(new JobOfferApproveCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Job offer approval recorded."));
    }

    [HttpPost("Reject")]
    public async Task<IActionResult> Reject([FromBody] JobOfferRejectDto dto)
    {
        EnsureValid();
        var response = await med.Send(new JobOfferRejectCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Job offer rejected."));
    }

    [HttpPost("Extend/{id:guid}")]
    public async Task<IActionResult> Extend(Guid id)
    {
        var response = await med.Send(new JobOfferExtendCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Job offer extended to candidate."));
    }

    [HttpPost("Accept")]
    public async Task<IActionResult> Accept([FromBody] JobOfferRespondDto dto)
    {
        EnsureValid();
        var response = await med.Send(new JobOfferAcceptCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Job offer accepted."));
    }

    [HttpPost("Decline")]
    public async Task<IActionResult> Decline([FromBody] JobOfferRespondDto dto)
    {
        EnsureValid();
        var response = await med.Send(new JobOfferDeclineCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Job offer declined."));
    }

    [HttpPost("Withdraw/{id:guid}")]
    public async Task<IActionResult> Withdraw(Guid id)
    {
        var response = await med.Send(new JobOfferWithdrawCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Job offer withdrawn."));
    }

    /// <summary>
    /// Convert an accepted offer into a Profile employee and optionally assign onboarding tasks.
    /// </summary>
    [HttpPost("Hire")]
    public async Task<IActionResult> Hire([FromBody] HireFromOfferDto dto)
    {
        EnsureValid();
        var response = await med.Send(new HireFromOfferCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Candidate hired and employee created in Profile."));
    }

    private void EnsureValid()
    {
        if (ModelState.IsValid) return;
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        throw new ValException(errors);
    }
}
