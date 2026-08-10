// Cor.CRM/Controllers/CampaignController.cs

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
[Route("api/core/crm/v{version:apiVersion}/Campaign")]
[ApiVersion("1.0")]
public class CampaignController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public CampaignController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all campaigns with optional filters
    /// </summary>
    [HttpGet("AllCampaigns")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCampaigns([FromQuery] string? status, [FromQuery] string? type)
    {
        try
        {
            var response = await _mediator.Send(new CampaignAllQry { Status = status, Type = type });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all campaigns");
            throw;
        }
    }

    /// <summary>
    /// Get a single campaign by ID
    /// </summary>
    [HttpGet("GetCampaign/{id:guid}")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaign(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CampaignByIdQry { Id = id });
            if (response == null)
            {
                throw new DomainException($"Campaign with id [{id}] NOT FOUND.");
            }
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new campaign
    /// </summary>
    [HttpPost("AddCampaign")]
    [PerAuth("crm.marketing.campaigns.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto addDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new CampaignAddCmd { Dto = addDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign");
            throw;
        }
    }

    /// <summary>
    /// Update an existing campaign
    /// </summary>
    [HttpPut("ModCampaign/{id:guid}")]
    [PerAuth("crm.marketing.campaigns.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto modDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
            throw new ValException(errors);
        }

        try
        {
            var command = new CampaignModCmd { Id = id, Dto = modDto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a campaign
    /// </summary>
    [HttpDelete("DelCampaign/{id:guid}")]
    [PerAuth("crm.marketing.campaigns.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCampaign(Guid id)
    {
        try
        {
            var command = new CampaignDelCmd { Id = id };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, $"Campaign with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get campaign statistics
    /// </summary>
    [HttpGet("Stats")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new CampaignStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign stats");
            throw;
        }
    }

    /// <summary>
    /// Add leads to campaign
    /// </summary>
    [HttpPost("{id:guid}/AddLeads")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddLeadsToCampaign(Guid id, [FromBody] List<Guid> leadIds)
    {
        try
        {
            var command = new CampaignAddLeadsCmd { CampaignId = id, LeadIds = leadIds };
            await _mediator.Send(command);
            return Ok(ApiResponse<string>.Ok(null!, "Leads added to campaign successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding leads to campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get leads in campaign
    /// </summary>
    [HttpGet("{id:guid}/Leads")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampaignLeads(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CampaignLeadsQry { CampaignId = id });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign leads: {CampaignId}", id);
            throw;
        }
    }

    // ============================================================
    // CAMPAIGN STATUS ACTIONS
    // ============================================================

    /// <summary>
    /// Start a campaign (Draft/Scheduled -> Active)
    /// </summary>
    [HttpPost("{id:guid}/Start")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartCampaign(Guid id)
    {
        try
        {
            var command = new CampaignStartCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign started successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Pause a campaign (Active -> Paused)
    /// </summary>
    [HttpPost("{id:guid}/Pause")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PauseCampaign(Guid id)
    {
        try
        {
            var command = new CampaignPauseCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign paused successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pausing campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Resume a campaign (Paused -> Active)
    /// </summary>
    [HttpPost("{id:guid}/Resume")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResumeCampaign(Guid id)
    {
        try
        {
            var command = new CampaignResumeCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign resumed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Archive a campaign (Completed/Cancelled -> Archived)
    /// </summary>
    [HttpPost("{id:guid}/Archive")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveCampaign(Guid id)
    {
        try
        {
            var command = new CampaignArchiveCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign archived successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Cancel a campaign (Active/Paused/Draft/Scheduled -> Cancelled)
    /// </summary>
    [HttpPost("{id:guid}/Cancel")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelCampaign(Guid id)
    {
        try
        {
            var command = new CampaignCancelCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign cancelled successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling campaign: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Duplicate a campaign
    /// </summary>
    [HttpPost("{id:guid}/Duplicate")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DuplicateCampaign(Guid id)
    {
        try
        {
            var command = new CampaignDuplicateCmd { Id = id };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Campaign duplicated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error duplicating campaign: {CampaignId}", id);
            throw;
        }
    }

    // ============================================================
    // CAMPAIGN ANALYTICS
    // ============================================================

    /// <summary>
    /// Get campaign analytics
    /// </summary>
    [HttpGet("{id:guid}/Analytics")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampaignAnalytics(Guid id, [FromQuery] string? fromDate, [FromQuery] string? toDate)
    {
        try
        {
            var query = new CampaignAnalyticsQry {
                CampaignId = id,
                FromDate = fromDate,
                ToDate = toDate
            };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign analytics: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get campaign performance
    /// </summary>
    [HttpGet("{id:guid}/Performance")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampaignPerformance(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CampaignPerformanceQry { CampaignId = id });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign performance: {CampaignId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get campaign ROI
    /// </summary>
    [HttpGet("{id:guid}/ROI")]
    [PerAuth("crm.marketing.campaigns.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampaignROI(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CampaignROIQry { CampaignId = id });
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign ROI: {CampaignId}", id);
            throw;
        }
    }
}