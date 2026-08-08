// Cor.CRM/Controllers/InvoiceController.cs

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
public class InvoiceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public InvoiceController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all invoices with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? type,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? leadId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = false)
    {
        try
        {
            var query = new InvoiceAllQry
            {
                Status = status,
                Type = type,
                CustomerId = customerId,
                LeadId = leadId,
                OpportunityId = opportunityId,
                FromDate = fromDate,
                ToDate = toDate,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
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
            _logger.LogError(ex, "Error getting all invoices");
            throw;
        }
    }

    /// <summary>
    /// Get invoice by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new InvoiceByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Invoice with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Create a new invoice
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var command = new InvoiceAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Invoice created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            throw;
        }
    }

    /// <summary>
    /// Update an existing invoice
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceDto dto)
    {
        try
        {
            var command = new InvoiceModCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Invoice updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Send an invoice
    /// </summary>
    [HttpPost("{id:guid}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new InvoiceSendCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Invoice sent successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending invoice: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Mark invoice as paid
    /// </summary>
    [HttpPost("{id:guid}/mark-paid")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkPaid(Guid id, [FromBody] MarkPaidDto dto)
    {
        try
        {
            var command = new InvoiceMarkPaidCmd { Id = id, AmountPaid = dto.AmountPaid };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Invoice marked as paid successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking invoice as paid: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Cancel an invoice
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new InvoiceCancelCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Invoice cancelled successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling invoice: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Refund an invoice
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Refund(Guid id, [FromBody] RefundDto dto)
    {
        try
        {
            var command = new InvoiceRefundCmd { Id = id, Amount = dto.Amount };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Invoice refunded successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding invoice: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete an invoice
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new InvoiceDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Invoice with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice: {InvoiceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get invoice statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats([FromQuery] Guid? customerId)
    {
        try
        {
            var query = new InvoiceStatsQry { CustomerId = customerId };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice stats");
            throw;
        }
    }

    /// <summary>
    /// Get invoices by customer
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        try
        {
            var query = new InvoiceByCustomerQry { CustomerId = customerId };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by customer: {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Get invoices by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(string status)
    {
        try
        {
            var query = new InvoiceByStatusQry { Status = status };
            var response = await _mediator.Send(query);
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by status: {Status}", status);
            throw;
        }
    }
}

