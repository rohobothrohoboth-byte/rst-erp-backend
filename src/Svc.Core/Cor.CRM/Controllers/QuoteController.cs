// Cor.CRM/Controllers/QuoteController.cs

using Asp.Versioning;
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
public class QuoteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public QuoteController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all quotes with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            var query = new QuoteAllQry
            {
                Status = status,
                CustomerId = customerId,
                LeadId = leadId,
                OpportunityId = opportunityId,
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
            _logger.LogError(ex, "Error getting all quotes");
            throw;
        }
    }

    /// <summary>
    /// Get quote by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new QuoteByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Quote with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new quote
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateQuoteDto dto)
    {
        try
        {
            var command = new QuoteAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Quote created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quote");
            throw;
        }
    }

    /// <summary>
    /// Update an existing quote
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuoteDto dto)
    {
        try
        {
            var command = new QuoteModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Quote updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Send a quote
    /// </summary>
    [HttpPost("{id:guid}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new QuoteSendCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Quote sent successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Accept a quote
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accept(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new QuoteAcceptCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Quote accepted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Reject a quote
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new QuoteRejectCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Quote rejected successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Convert quote to invoice
    /// </summary>
    [HttpPost("{id:guid}/convert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Convert(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new QuoteConvertCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Quote converted to invoice successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete a quote
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new QuoteDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Quote with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quote: {QuoteId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get quote statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new QuoteStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quote stats");
            throw;
        }
    }
}