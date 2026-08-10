// Cor.CRM/Controllers/SMSCampaignController.cs

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
public class SMSCampaignController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public SMSCampaignController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
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
            var query = new SMSCampaignAllQry
            {
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
            _logger.LogError(ex, "Error getting SMS campaigns");
            throw;
        }
    }

    [HttpGet("{id:guid}")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SMSCampaignByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"SMS campaign with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSMSCampaignDto dto)
    {
        try
        {
            var command = new SMSCampaignAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "SMS campaign created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating SMS campaign");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSMSCampaignDto dto)
    {
        try
        {
            var command = new SMSCampaignUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "SMS campaign updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpDelete("{id:guid}")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new SMSCampaignDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"SMS campaign with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/send")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SMSCampaignSendCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "SMS campaign sent successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/duplicate")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Duplicate(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SMSCampaignDuplicateCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "SMS campaign duplicated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/pause")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Pause(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SMSCampaignPauseCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "SMS campaign paused successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/resume")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Resume(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SMSCampaignResumeCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "SMS campaign resumed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpPost("{id:guid}/cancel")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new SMSCampaignCancelCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "SMS campaign cancelled successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling SMS campaign: {CampaignId}", id);
            throw;
        }
    }

    [HttpGet("stats")]
    [PerAuth("crm.marketing.sms.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(
        [FromQuery] Guid? campaignId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var query = new SMSCampaignStatsQry
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
            _logger.LogError(ex, "Error getting SMS campaign stats");
            throw;
        }
    }
}