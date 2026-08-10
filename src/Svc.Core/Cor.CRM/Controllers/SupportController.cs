// Cor.CRM/Controllers/SupportController.cs
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
public class SupportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public SupportController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all tickets with filters
    /// </summary>
    [HttpGet("Tickets")]
    [PerAuth("crm.support.tickets.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTickets(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? source,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] string? category,
        [FromQuery] string? subCategory,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new TicketAllQry
            {
                Status = status,
                Priority = priority,
                Source = source,
                CustomerId = customerId,
                AssignedToUserId = assignedToUserId,
                Category = category,
                SubCategory = subCategory,
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
            _logger.LogError(ex, "Error getting tickets");
            throw;
        }
    }

    /// <summary>
    /// Get ticket by ID
    /// </summary>
    [HttpGet("Tickets/{id:guid}")]
    [PerAuth("crm.support.tickets.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new TicketByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Ticket with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new ticket
    /// </summary>
    [HttpPost("Tickets")]
    [PerAuth("crm.support.tickets.add")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
    {
        try
        {
            var command = new TicketAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTicketById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Ticket created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ticket");
            throw;
        }
    }

    /// <summary>
    /// Update a ticket
    /// </summary>
    [HttpPut("Tickets/{id:guid}")]
    [PerAuth("crm.support.tickets.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketDto dto)
    {
        try
        {
            var command = new TicketUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Ticket updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Update ticket status
    /// </summary>
    [HttpPatch("Tickets/{id:guid}/status")]
    [PerAuth("crm.support.tickets.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTicketStatus(Guid id, [FromBody] UpdateTicketStatusDto dto)
    {
        try
        {
            var command = new TicketUpdateStatusCmd { Id = id, Status = dto.Status };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Ticket status updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket status: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Assign a ticket to a user
    /// </summary>
    [HttpPost("Tickets/{id:guid}/assign")]
    [PerAuth("crm.support.tickets.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTicket(Guid id, [FromBody] Guid assignedToUserId)
    {
        try
        {
            var command = new TicketAssignCmd { Id = id, AssignedToUserId = assignedToUserId };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Ticket assigned successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning ticket: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Resolve a ticket
    /// </summary>
    [HttpPost("Tickets/{id:guid}/resolve")]
    [PerAuth("crm.support.tickets.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResolveTicket(Guid id, [FromBody] ResolveTicketDto dto)
    {
        try
        {
            var command = new TicketResolveCmd { Id = id, Resolution = dto.Resolution };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Ticket resolved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving ticket: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Close a ticket
    /// </summary>
    [HttpPost("Tickets/{id:guid}/close")]
    [PerAuth("crm.support.tickets.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseTicket(Guid id, [FromBody] CloseTicketDto dto)
    {
        try
        {
            var command = new TicketCloseCmd { Id = id, SatisfactionScore = dto.SatisfactionScore };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Ticket closed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing ticket: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a ticket (soft delete)
    /// </summary>
    [HttpDelete("Tickets/{id:guid}")]
    [PerAuth("crm.support.tickets.del")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        try
        {
            await _mediator.Send(new TicketDeleteCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Ticket with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ticket: {TicketId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get ticket statistics
    /// </summary>
    [HttpGet("Tickets/stats")]
    [PerAuth("crm.support.tickets.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTicketStats()
    {
        try
        {
            var response = await _mediator.Send(new TicketStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket stats");
            throw;
        }
    }
}

