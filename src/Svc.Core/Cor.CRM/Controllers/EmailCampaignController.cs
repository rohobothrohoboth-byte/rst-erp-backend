// Cor.CRM/Controllers/EmailCampaignController.cs

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
public class EmailCampaignController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public EmailCampaignController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] Guid? campaignId,
        [FromQuery] Guid? templateId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            var query = new EmailCampaignAllQry
            {
                Status = status,
                CampaignId = campaignId,
                TemplateId = templateId,
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
            _logger.LogError(ex, "Error getting email campaigns");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmailCampaignByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Email campaign with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEmailCampaignDto dto)
    {
        try
        {
            var command = new EmailCampaignAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Email campaign created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating email campaign");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmailCampaignDto dto)
    {
        try
        {
            var command = new EmailCampaignUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Email campaign updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new EmailCampaignDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Email campaign with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/send")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmailCampaignSendCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Email campaign sent successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/duplicate")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Duplicate(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmailCampaignDuplicateCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Email campaign duplicated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/pause")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pause(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmailCampaignPauseCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Email campaign paused successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/resume")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resume(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmailCampaignResumeCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Email campaign resumed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/cancel")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new EmailCampaignCancelCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Email campaign cancelled successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling email campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpGet("stats")]
    [PerAuth("crm.marketing.email.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(
        [FromQuery] Guid? campaignId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var query = new EmailCampaignStatsQry
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
            _logger.LogError(ex, "Error getting email campaign stats");
            throw;
        }
    }
}