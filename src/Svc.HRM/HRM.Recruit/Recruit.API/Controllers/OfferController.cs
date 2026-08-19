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
/// Job offer management endpoints (create, send to candidate, respond, list).
/// </summary>
[ApiController]
[Route("api/hrm/recruit/v{version:apiVersion}/Offer")]
[ApiVersion("1.0")]
public class OfferController(IMediator med) : ControllerBase
{
    [PerAuth("hr.recruit.offer.view")]
    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> All()
    {
        var response = await med.Send(new OfferAllQry());
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.offer.view")]
    [HttpGet("ByApplicant/{applicantId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ByApplicant(Guid applicantId)
    {
        var response = await med.Send(new OfferByApplicantQry { ApplicantId = applicantId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.offer.view")]
    [HttpGet("ByJobPosting/{jobPostingId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ByJobPosting(Guid jobPostingId)
    {
        var response = await med.Send(new OfferByJobPostingQry { JobPostingId = jobPostingId });
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.offer.view")]
    [HttpGet("Get/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await med.Send(new OfferByIdQry { Id = id });
        if (response == null) { throw new DomainException($"OFFER with id [{id}] NOT FOUND."); }
        return Ok(ApiResponse<object>.Ok(response));
    }

    [PerAuth("hr.recruit.offer.manage")]
    [HttpPost("Add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] OfferAddDto addDto)
    {
        var response = await med.Send(new OfferAddCmd { AddDto = addDto });
        return Ok(ApiResponse<object>.Ok(response, "Offer successfully created."));
    }

    [PerAuth("hr.recruit.offer.manage")]
    [HttpPut("Mod/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Mod(Guid id, [FromBody] OfferModDto modDto)
    {
        if (modDto.Id != id) { throw new DomainException("Offer id mismatch."); }
        var response = await med.Send(new OfferModCmd { ModDto = modDto });
        return Ok(ApiResponse<object>.Ok(response, "Offer successfully updated."));
    }

    [PerAuth("hr.recruit.offer.manage")]
    [HttpPost("Send/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(Guid id)
    {
        var response = await med.Send(new OfferSendCmd { Id = id });
        return Ok(ApiResponse<object>.Ok(response, "Offer sent to candidate."));
    }

    [PerAuth("hr.recruit.offer.manage")]
    [HttpPost("Respond")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Respond([FromBody] OfferResponseDto dto)
    {
        var response = await med.Send(new OfferRespondCmd { Dto = dto });
        return Ok(ApiResponse<object>.Ok(response, "Offer response recorded."));
    }

    [PerAuth("hr.recruit.offer.manage")]
    [HttpDelete("Del/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Del(Guid id)
    {
        await med.Send(new OfferDelCmd { Id = id });
        return Ok(ApiResponse<string>.Ok(null!, $"OFFER with Id {id} successfully deleted."));
    }
}
