// Controllers/PurchaseOrderController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cor.Procurement.Commands;
using Cor.Procurement.Queries;
using Cor.Procurement.Models.DTOs;
using Asp.Versioning;
using Microsoft.Extensions.Logging;

namespace Cor.Procurement.Controllers;

[ApiController]
[Route("api/procurement/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class PurchaseOrderController : BaseApiController
{
    private readonly IMediator _mediator;

    public PurchaseOrderController(IMediator mediator, ILogger<PurchaseOrderController> logger)
        : base(logger)
    {
        _mediator = mediator;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all purchase orders with pagination (CACHED)
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(typeof(PagedResult<PurchaseOrderDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any,
        VaryByQueryKeys = new[] { "page", "pageSize", "status", "vendorId", "periodId", "fromDate", "toDate", "searchTerm", "sortBy", "sortOrder" })]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] Guid? vendorId = null,
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "OrderDate",
        [FromQuery] string? sortOrder = "DESC")
    {
        try
        {
            var result = await _mediator.Send(new GetPagedPurchaseOrdersQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status,
                VendorId = vendorId,
                PeriodId = periodId,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortOrder = sortOrder
            });

            _logger.LogInformation("✅ Retrieved {Count} purchase orders (Page {Page}/{TotalPages})",
                result.Data?.Count ?? 0, page, result.TotalPages);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetAll));
        }
    }

    /// <summary>
    /// Get purchase order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrderByIdQuery { Id = id });
            if (result == null)
                return NotFound(new { message = $"Purchase Order with ID '{id}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetById));
        }
    }

    /// <summary>
    /// Get purchase order by number
    /// </summary>
    [HttpGet("ByNumber/{number}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumber(string number)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrderByNumberQuery { PurchaseOrderNumber = number });
            if (result == null)
                return NotFound(new { message = $"Purchase Order with number '{number}' not found" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByNumber));
        }
    }

    /// <summary>
    /// Get purchase orders by vendor
    /// </summary>
    [HttpGet("ByVendor/{vendorId}")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVendor(Guid vendorId)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrdersByVendorQuery { VendorId = vendorId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByVendor));
        }
    }

    /// <summary>
    /// Get purchase orders by period
    /// </summary>
    [HttpGet("ByPeriod/{periodId}")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPeriod(Guid periodId)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrdersByPeriodQuery { PeriodId = periodId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByPeriod));
        }
    }

    /// <summary>
    /// Get purchase orders by requisition
    /// </summary>
    [HttpGet("ByRequisition/{requisitionId}")]
    [ProducesResponseType(typeof(List<PurchaseOrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRequisition(Guid requisitionId)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrdersByRequisitionQuery { RequisitionId = requisitionId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetByRequisition));
        }
    }

    /// <summary>
    /// Get purchase order summary
    /// </summary>
    [HttpGet("Summary")]
    [ProducesResponseType(typeof(PurchaseOrderSummaryDto), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any,
        VaryByQueryKeys = new[] { "periodId", "fromDate", "toDate" })]
    public async Task<IActionResult> GetSummary(
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var result = await _mediator.Send(new GetPurchaseOrderSummaryQuery
            {
                PeriodId = periodId,
                FromDate = fromDate,
                ToDate = toDate
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(GetSummary));
        }
    }

    /// <summary>
    /// Export purchase orders
    /// </summary>
    [HttpGet("Export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? status = null,
        [FromQuery] string format = "csv")
    {
        try
        {
            var data = await _mediator.Send(new ExportPurchaseOrdersQuery
            {
                PeriodId = periodId,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status
            });

            if (format.ToLower() == "csv")
            {
                var csv = ConvertPurchaseOrdersToCsv(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"purchase-orders-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            else if (format.ToLower() == "json")
            {
                var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", $"purchase-orders-{DateTime.UtcNow:yyyyMMdd}.json");
            }

            return BadRequest(new { message = "Unsupported format. Use 'csv' or 'json'." });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Export));
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new purchase order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto createDto)
    {
        try
        {
            // Validate
            if (createDto.Lines == null || !createDto.Lines.Any())
                return BadRequest(new { message = "Purchase order must have at least one line" });

            if (string.IsNullOrEmpty(createDto.PurchaseOrderNumber))
                return BadRequest(new { message = "Purchase order number is required" });

            if (!createDto.PeriodId.HasValue)
                return BadRequest(new { message = "Period ID is required" });

            var command = new CreatePurchaseOrderCommand { CreateDto = createDto };
            var result = await _mediator.Send(command);

            // Invalidate cache
            // await _cachedService.InvalidatePurchaseOrdersCacheAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Create));
        }
    }

    /// <summary>
    /// Create purchase order from requisition
    /// </summary>
    [HttpPost("FromRequisition/{requisitionId}")]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFromRequisition(Guid requisitionId)
    {
        try
        {
            var command = new CreatePurchaseOrderFromRequisitionCommand { RequisitionId = requisitionId };
            var result = await _mediator.Send(command);

            // Invalidate cache
            // await _cachedService.InvalidatePurchaseOrdersCacheAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(CreateFromRequisition));
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a purchase order
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(PurchaseOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdatePurchaseOrderDto updateDto)
    {
        try
        {
            if (updateDto.Lines == null || !updateDto.Lines.Any())
                return BadRequest(new { message = "Purchase order must have at least one line" });

            if (string.IsNullOrEmpty(updateDto.PurchaseOrderNumber))
                return BadRequest(new { message = "Purchase order number is required" });

            var command = new UpdatePurchaseOrderCommand { UpdateDto = updateDto };
            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(new { message = $"Purchase Order with ID '{updateDto.Id}' not found" });

            // Invalidate cache
            // await _cachedService.InvalidatePurchaseOrdersCacheAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Update));
        }
    }

    // ============================================================
    // PATCH ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update purchase order status
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] PurchaseOrderStatusUpdateDto statusDto)
    {
        try
        {
            if (string.IsNullOrEmpty(statusDto.Status))
                return BadRequest(new { message = "Status is required" });

            var command = new UpdatePurchaseOrderStatusCommand
            {
                Id = id,
                Status = statusDto.Status,
                Notes = statusDto.Notes
            };

            var result = await _mediator.Send(command);
            if (result==null)
                return NotFound(new { message = $"Purchase Order with ID '{id}' not found" });

            // Invalidate cache
            // await _cachedService.InvalidatePurchaseOrdersCacheAsync();

            return Ok(new { success = true, message = $"Status updated to {statusDto.Status}" });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(UpdateStatus));
        }
    }

    /// <summary>
    /// Receive a purchase order
    /// </summary>
    [HttpPatch("{id}/receive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceivePurchaseOrderDto receiveDto)
    {
        try
        {
            var command = new ReceivePurchaseOrderCommand
            {
                Id = id,
                ReceivedDate = receiveDto.ReceivedDate ?? DateTime.UtcNow,
                ReceivedBy = receiveDto.ReceivedBy,
                Notes = receiveDto.Notes
            };

            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound(new { message = $"Purchase Order with ID '{id}' not found" });

            // Invalidate cache
            // await _cachedService.InvalidatePurchaseOrdersCacheAsync();

            return Ok(new { success = true, message = "Purchase order received successfully", data = result });
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Receive));
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a purchase order
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var command = new DeletePurchaseOrderCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new { message = $"Purchase Order with ID '{id}' not found" });

            // Invalidate cache
            // await _cachedService.InvalidatePurchaseOrdersCacheAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, nameof(Delete));
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private string ConvertPurchaseOrdersToCsv(List<PurchaseOrderDto> orders)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("PurchaseOrderNumber,VendorName,Status,TotalAmount,Currency,OrderDate,ExpectedDelivery,CreatedBy,DateAdd");

        foreach (var order in orders)
        {
            sb.AppendLine($"{order.PurchaseOrderNumber}," +
                         $"{order.VendorName}," +
                         $"{order.Status}," +
                         $"{order.TotalAmount}," +
                         $"{order.Currency}," +
                         $"{order.OrderDate:yyyy-MM-dd}," +
                         $"{order.ExpectedDeliveryDate:yyyy-MM-dd}," +
                         $"{order.CreatedByUserName}," +
                         $"{order.DateAdd:yyyy-MM-dd HH:mm:ss}");
        }

        return sb.ToString();
    }
}