using Asp.Versioning;
using Cor.CRM.Commands;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Queries;
using Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.CRM.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace Cor.CRM.Controllers;

[Authorize]
[ApiController]
[Route("api/core/crm/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SalesOrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogService _logger;

    public SalesOrderController(IMediator mediator, ILogService logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new sales order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var command = new OrderAddCmd { Dto = dto };
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.Id },
                ApiResponse<object>.Ok(response, "Order created successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sales order");
            throw;
        }
    }

    /// <summary>
    /// Update an existing order
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderDto dto)
    {
        try
        {
            var command = new OrderUpdateCmd { Id = id, Dto = dto };
            var response = await _mediator.Send(command);
            return Ok(ApiResponse<object>.Ok(response, "Order updated successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 120, VaryByQueryKeys = new[] { "id" })]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OrderByIdQry { Id = id });
            if (response == null)
                throw new DomainException($"Order with id [{id}] NOT FOUND.");

            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get all orders with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "customerId", "opportunityId", "quoteId", "status", "fromDate", "toDate", "page", "pageSize" })]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? opportunityId,
        [FromQuery] Guid? quoteId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new OrderAllQry
            {
                CustomerId = customerId,
                OpportunityId = opportunityId,
                QuoteId = quoteId,
                Status = status,
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
            _logger.LogError(ex, "Error getting orders");
            throw;
        }
    }

    /// <summary>
    /// Get order statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30)] // Cache for 30 seconds
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var response = await _mediator.Send(new OrderStatsQry());
            return Ok(ApiResponse<object>.Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order stats");
            throw;
        }
    }

    /// <summary>
    /// Send an order (change status to Pending)
    /// </summary>
    [HttpPost("{id:guid}/send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Send(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OrderSendCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Order sent successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Accept an order (change status to Processing)
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Accept(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OrderAcceptCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Order accepted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Reject an order (change status to Cancelled)
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Reject(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OrderRejectCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Order rejected successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Complete an order (change status to Completed)
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Complete(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OrderCompleteCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Order completed successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Cancel an order (change status to Cancelled)
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new OrderCancelCmd { Id = id });
            return Ok(ApiResponse<object>.Ok(response, "Order cancelled successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order: {OrderId}", id);
            throw;
        }
    }

    /// <summary>
    /// Delete an order (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _mediator.Send(new OrderDelCmd { Id = id });
            return Ok(ApiResponse<string>.Ok(null!, $"Order with Id {id} successfully deleted."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order: {OrderId}", id);
            throw;
        }
    }



    /// <summary>
    /// Get sales pipeline data for dashboard
    /// </summary>
    [HttpGet("Pipeline")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPipeline([FromQuery] string period = "quarter")
    {
        try
        {
            // You can implement actual pipeline logic here
            // For now, return empty pipeline data
            var pipeline = new
            {
                stages = new[]
                {
                    new { stage = "Lead", value = 0, count = 0 },
                    new { stage = "Qualified", value = 0, count = 0 },
                    new { stage = "Proposal", value = 0, count = 0 },
                    new { stage = "Negotiation", value = 0, count = 0 },
                    new { stage = "Closed Won", value = 0, count = 0 },
                    new { stage = "Closed Lost", value = 0, count = 0 }
                },
                totalValue = 0,
                period = period,
                opportunities = new List<object>(),
                byStage = new Dictionary<string, object>()
            };

            return Ok(ApiResponse<object>.Ok(pipeline, "Pipeline data retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pipeline data");
            throw;
        }
    }
}