// Cor.CRM/Controllers/SocialMediaController.cs

using Asp.Versioning;
using Common;
using Cor.CRM.Commands;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;

namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SocialMediaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public SocialMediaController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? platform,
        [FromQuery] string? status,
        [FromQuery] Guid? campaignId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            var query = new SocialMediaAllQry
            {
                Platform = platform,
                Status = status,
                CampaignId = campaignId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting social media posts");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SocialMediaByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Social media post with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting social media post: {PostId}", id);
            throw;
        }
    }

    [HttpPost]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSocialMediaPostDto dto)
    {
        try
        {
            var command = new SocialMediaAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Social media post created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating social media post");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [PerAuth("crm.marketing.social.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSocialMediaPostDto dto)
    {
        try
        {
            var command = new SocialMediaUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Social media post updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating social media post: {PostId}", id);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new SocialMediaDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Social media post with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting social media post: {PostId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/publish")]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SocialMediaPublishCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Social media post published successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing social media post: {PostId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/duplicate")]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Duplicate(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SocialMediaDuplicateCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Social media post duplicated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating social media post: {PostId}", id);
            throw;
        }
    }

    [HttpGet("stats")]
    [PerAuth("crm.marketing.social.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(
        [FromQuery] Guid? campaignId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var query = new SocialMediaStatsQry
            {
                CampaignId = campaignId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting social media stats");
            throw;
        }
    }
}