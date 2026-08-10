// Cor.CRM/Controllers/InteractionController.cs

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
public class InteractionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public InteractionController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all interactions with optional filters
    /// </summary>
    [HttpGet]
    [PerAuth("crm.contacts.interactions.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? contactId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder)
    {
        try
        {
            var query = new InteractionAllQry
            {
                Type = type,
                Status = status,
                Priority = priority,
                LeadId = leadId,
                CustomerId = customerId,
                ContactId = contactId,
                OpportunityId = opportunityId,
                AssignedToUserId = assignedToUserId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all interactions");
            throw;
        }
    }

    /// <summary>
    /// Get a single interaction by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [PerAuth("crm.contacts.interactions.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new InteractionByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Interaction with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interaction: {InteractionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new interaction
    /// </summary>
    [HttpPost]
    [PerAuth("crm.contacts.interactions.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInteractionDto dto)
    {
        try
        {
            var command = new InteractionAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Interaction created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating interaction");
            throw;
        }
    }

    /// <summary>
    /// Update an existing interaction
    /// </summary>
    [HttpPut("{id:guid}")]
    [PerAuth("crm.contacts.interactions.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInteractionDto dto)
    {
        try
        {
            var command = new InteractionModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Interaction updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating interaction: {InteractionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete an interaction
    /// </summary>
    [HttpDelete("{id:guid}")]
    [PerAuth("crm.contacts.interactions.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new InteractionDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Interaction with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting interaction: {InteractionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get interaction statistics
    /// </summary>
    [HttpGet("stats")]
    [PerAuth("crm.contacts.interactions.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new InteractionStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interaction stats");
            throw;
        }
    }
}