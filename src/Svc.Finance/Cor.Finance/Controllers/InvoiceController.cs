using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MessagePack;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class InvoiceController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(IMediator mediator, ILogger<InvoiceController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("All")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] {
        "status", "invoiceType", "customerId", "vendorId", "periodId",
        "fromDate", "toDate", "minAmount", "maxAmount", "page", "pageSize"
    })]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] string? invoiceType,
        [FromQuery] Guid? vendorId,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? periodId,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "InvoiceDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        try
        {
            _logger.LogInformation("📊 GetAllInvoices - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            // Clamp page size
            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            // Convert dates to UTC
            var fromDateUtc = fromDate.HasValue
                ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;
            var toDateUtc = toDate.HasValue
                ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            var result = await _mediator.Send(new GetAllInvoicesQry
            {
                FromDate = fromDateUtc,
                ToDate = toDateUtc,
                Status = status,
                InvoiceType = invoiceType,
                VendorId = vendorId,
                CustomerId = customerId,
                PeriodId = periodId,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            });

            // Check if client accepts MessagePack
            var acceptHeader = Request.Headers["Accept"].ToString();
            if (acceptHeader.Contains("msgpack"))
            {
                // MessagePack will be automatically handled by the formatter
                return Ok(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetAllInvoices");
            return HandleException(ex, "GetAllInvoices");
        }
    }

    [HttpGet("{id}")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("📄 GetInvoiceById - Id: {Id}", id);
            var result = await _mediator.Send(new GetInvoiceByIdQry { Id = id });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetInvoiceById - Id: {Id}", id);
            return HandleException(ex, "GetInvoiceById", id);
        }
    }

    [HttpGet("ByNumber/{invoiceNumber}")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumber(string invoiceNumber)
    {
        try
        {
            _logger.LogInformation("📄 GetInvoiceByNumber - Number: {InvoiceNumber}", invoiceNumber);
            var result = await _mediator.Send(new GetInvoiceByNumberQry { InvoiceNumber = invoiceNumber });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetInvoiceByNumber - Number: {InvoiceNumber}", invoiceNumber);
            return HandleException(ex, "GetInvoiceByNumber", invoiceNumber);
        }
    }

    [HttpGet("ByCustomer/{customerId}")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCustomer(
        Guid customerId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "InvoiceDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        try
        {
            _logger.LogInformation("📊 GetInvoicesByCustomer - CustomerId: {CustomerId}", customerId);

            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var result = await _mediator.Send(new GetInvoicesByCustomerQry
            {
                CustomerId = customerId,
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetInvoicesByCustomer - CustomerId: {CustomerId}", customerId);
            return HandleException(ex, "GetInvoicesByCustomer", customerId);
        }
    }

    [HttpGet("ByVendor/{vendorId}")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVendor(
        Guid vendorId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "InvoiceDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        try
        {
            _logger.LogInformation("📊 GetInvoicesByVendor - VendorId: {VendorId}", vendorId);

            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var result = await _mediator.Send(new GetInvoicesByVendorQry
            {
                VendorId = vendorId,
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetInvoicesByVendor - VendorId: {VendorId}", vendorId);
            return HandleException(ex, "GetInvoicesByVendor", vendorId);
        }
    }

    [HttpGet("ByType/{invoiceType}")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByType(
        string invoiceType,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "InvoiceDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        try
        {
            _logger.LogInformation("📊 GetInvoicesByType - Type: {InvoiceType}", invoiceType);

            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var result = await _mediator.Send(new GetInvoicesByTypeQry
            {
                InvoiceType = invoiceType,
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetInvoicesByType - Type: {InvoiceType}", invoiceType);
            return HandleException(ex, "GetInvoicesByType", invoiceType);
        }
    }

    [HttpGet("Sales")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSalesInvoices(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "InvoiceDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        try
        {
            _logger.LogInformation("📊 GetSalesInvoices - CustomerId: {CustomerId}", customerId);

            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var result = await _mediator.Send(new GetSalesInvoicesQry
            {
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                CustomerId = customerId,
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetSalesInvoices");
            return HandleException(ex, "GetSalesInvoices");
        }
    }

    [HttpGet("Purchase")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(PaginatedResponse<InvoiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseInvoices(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? vendorId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "InvoiceDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        try
        {
            _logger.LogInformation("📊 GetPurchaseInvoices - VendorId: {VendorId}", vendorId);

            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var result = await _mediator.Send(new GetPurchaseInvoicesQry
            {
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                VendorId = vendorId,
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetPurchaseInvoices");
            return HandleException(ex, "GetPurchaseInvoices");
        }
    }

    [HttpPost]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddInvoiceDto dto)
    {
        try
        {
            _logger.LogInformation("📝 CreateInvoice - Type: {InvoiceType}", dto.InvoiceType);

            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new AddInvoiceCmd { AddDto = dto });

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Invoice created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Invalid operation in CreateInvoice");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in CreateInvoice");
            return HandleException(ex, "CreateInvoice");
        }
    }

    [HttpPut]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditInvoiceDto dto)
    {
        try
        {
            _logger.LogInformation("📝 UpdateInvoice - Id: {Id}", dto.Id);

            if (dto.PeriodId == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            var result = await _mediator.Send(new EditInvoiceCmd { EditDto = dto });

            return Ok(new
            {
                success = true,
                message = "Invoice updated successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Invalid operation in UpdateInvoice - Id: {Id}", dto.Id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in UpdateInvoice - Id: {Id}", dto.Id);
            return HandleException(ex, "UpdateInvoice", dto.Id);
        }
    }

    [HttpPatch("{id}/status")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] InvoiceStatusUpdateDto dto)
    {
        try
        {
            _logger.LogInformation("📝 UpdateInvoiceStatus - Id: {Id}, Status: {Status}", id, dto.Status);

            if (string.IsNullOrEmpty(dto.Status))
                return HandleBadRequest("Status is required");

            var result = await _mediator.Send(new UpdateInvoiceStatusCmd { Id = id, Status = dto.Status });

            if (!result)
                return HandleNotFound("Invoice", id);

            return SuccessResponse($"Invoice status updated to '{dto.Status}'");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Invalid operation in UpdateInvoiceStatus - Id: {Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in UpdateInvoiceStatus - Id: {Id}", id);
            return HandleException(ex, "UpdateInvoiceStatus", id);
        }
    }

    [HttpPost("Bulk")]
    [Produces("application/json", "application/x-msgpack")]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] List<AddInvoiceDto> dtos)
    {
        try
        {
            _logger.LogInformation("📝 BulkCreateInvoices - Count: {Count}", dtos?.Count ?? 0);

            if (dtos == null || !dtos.Any())
                return HandleBadRequest("No invoices provided");

            foreach (var dto in dtos)
            {
                if (dto.PeriodId == Guid.Empty)
                    return HandleBadRequest("PeriodId is required for all invoices");
            }

            var result = await _mediator.Send(new BulkAddInvoiceCmd { AddDtos = dtos });

            return Ok(new
            {
                success = true,
                message = $"{result.Count} invoices created successfully",
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in BulkCreateInvoices");
            return HandleException(ex, "BulkCreateInvoices");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("🗑️ DeleteInvoice - Id: {Id}", id);

            var result = await _mediator.Send(new DeleteInvoiceCmd { Id = id });

            if (!result)
                return HandleNotFound("Invoice", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Invalid operation in DeleteInvoice - Id: {Id}", id);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in DeleteInvoice - Id: {Id}", id);
            return HandleException(ex, "DeleteInvoice", id);
        }
    }
}