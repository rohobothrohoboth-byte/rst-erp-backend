// Cor.CRM/Controllers/ActivityController.cs
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
public class ActivityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public ActivityController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new activity
    /// </summary>
    [HttpPost]
    [PerAuth("crm.activities.list.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateActivityDto dto)
    {
        try
        {
            var command = new ActivityAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Activity created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating activity");
            throw;
        }
    }

    /// <summary>
    /// Update an existing activity
    /// </summary>
    [HttpPut("{id:guid}")]
    [PerAuth("crm.activities.list.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateActivityDto dto)
    {
        try
        {
            var command = new ActivityUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Activity updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity: {ActivityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get activity by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [PerAuth("crm.activities.list.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ActivityByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Activity with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity: {ActivityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get all activities with filters
    /// </summary>
    [HttpGet]
    [PerAuth("crm.activities.list.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        [FromQuery] string? status,
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new ActivityAllQry
            {
                Type = type,
                Status = status,
                LeadId = leadId,
                CustomerId = customerId,
                OpportunityId = opportunityId,
                AssignedToUserId = assignedToUserId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activities");
            throw;
        }
    }

    /// <summary>
    /// Update activity status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [PerAuth("crm.activities.list.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateActivityStatusDto dto)
    {
        try
        {
            var command = new ActivityUpdateStatusCmd { Id = id, Status = dto.Status };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Activity status updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity status: {ActivityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Complete an activity
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [PerAuth("crm.activities.list.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new ActivityCompleteCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Activity completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing activity: {ActivityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete an activity (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [PerAuth("crm.activities.list.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new ActivityDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Activity with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting activity: {ActivityId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get activity statistics
    /// </summary>
    [HttpGet("stats")]
    [PerAuth("crm.activities.list.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new ActivityStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity stats");
            throw;
        }
    }
}

public class UpdateActivityStatusDto
{
    public string Status { get; set; } = string.Empty;
}