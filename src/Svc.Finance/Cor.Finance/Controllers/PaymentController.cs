using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Helpers;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class PaymentController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IValidator<AddPaymentDto> _addValidator;
    private readonly IValidator<EditPaymentDto> _editValidator;

    public PaymentController(
        IMediator mediator,
        ILogger<PaymentController> logger,
        IValidator<AddPaymentDto> addValidator,
        IValidator<EditPaymentDto> editValidator)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _addValidator = addValidator;
        _editValidator = editValidator;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all payments with pagination and filtering
    /// </summary>
    [HttpGet("All")]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] {
        "fromDate", "toDate", "status", "invoiceId", "paymentMethod",
        "periodId", "minAmount", "maxAmount", "paymentType",
        "vendorId", "customerId", "page", "pageSize", "sortBy", "sortDirection"
    })]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? invoiceId,
        [FromQuery] string? paymentMethod,
        [FromQuery] Guid? periodId,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] string? paymentType,
        [FromQuery] Guid? vendorId,
        [FromQuery] Guid? customerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "PaymentDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        // ✅ Clamp page size at boundary
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        // ✅ Convert dates to UTC
        var fromDateUtc = fromDate.HasValue
            ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        var toDateUtc = toDate.HasValue
            ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        // ✅ Add check for empty result to prevent caching empty data
        var result = await _mediator.Send(new GetAllPaymentsQry
        {
            FromDate = fromDateUtc,
            ToDate = toDateUtc,
            Status = status,
            InvoiceId = invoiceId,
            PaymentMethod = paymentMethod,
            PeriodId = periodId,
            MinAmount = minAmount,
            MaxAmount = maxAmount,
            PaymentType = paymentType,
            VendorId = vendorId,
            CustomerId = customerId,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        });

        return Ok(result);
    }

    /// <summary>
    /// Get payments by invoice
    /// </summary>
    [HttpGet("ByInvoice/{invoiceId}")]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "page", "pageSize", "periodId", "sortBy", "sortDirection" })]
    public async Task<IActionResult> GetByInvoice(
        Guid invoiceId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "PaymentDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        // ✅ Clamp page size
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _mediator.Send(new GetPaymentsByInvoiceQry
        {
            InvoiceId = invoiceId,
            PeriodId = periodId,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        });

        return Ok(result);
    }

    /// <summary>
    /// Get payment summary with caching
    /// </summary>
    [HttpGet("Summary")]
    [ProducesResponseType(typeof(PaymentSummaryDto), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "fromDate", "toDate", "periodId" })]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? periodId)
    {
        var fromDateUtc = fromDate.HasValue
            ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        var toDateUtc = toDate.HasValue
            ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var result = await _mediator.Send(new GetPaymentSummaryQry
        {
            FromDate = fromDateUtc,
            ToDate = toDateUtc,
            PeriodId = periodId
        });

        return Ok(result);
    }

    /// <summary>
    /// Get purchase payments (Accounts Payable)
    /// </summary>
    [HttpGet("Purchase")]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] {
        "fromDate", "toDate", "status", "vendorId", "periodId",
        "page", "pageSize", "sortBy", "sortDirection"
    })]
    public async Task<IActionResult> GetPurchasePayments(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? vendorId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "PaymentDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var fromDateUtc = fromDate.HasValue
            ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        var toDateUtc = toDate.HasValue
            ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var result = await _mediator.Send(new GetPurchasePaymentsQry
        {
            FromDate = fromDateUtc,
            ToDate = toDateUtc,
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

    /// <summary>
    /// Get sales payments (Accounts Receivable)
    /// </summary>
    [HttpGet("Sales")]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] {
        "fromDate", "toDate", "status", "customerId", "periodId",
        "page", "pageSize", "sortBy", "sortDirection"
    })]
    public async Task<IActionResult> GetSalesPayments(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? status,
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "PaymentDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var fromDateUtc = fromDate.HasValue
            ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        var toDateUtc = toDate.HasValue
            ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var result = await _mediator.Send(new GetSalesPaymentsQry
        {
            FromDate = fromDateUtc,
            ToDate = toDateUtc,
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

    /// <summary>
    /// Get payments by type
    /// </summary>
    [HttpGet("ByType/{paymentType}")]
    [ProducesResponseType(typeof(PaginatedResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] {
        "periodId", "page", "pageSize", "sortBy", "sortDirection"
    })]
    public async Task<IActionResult> GetByType(
        string paymentType,
        [FromQuery] Guid? periodId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "PaymentDate",
        [FromQuery] string? sortDirection = "DESC")
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _mediator.Send(new GetPaymentsByTypeQry
        {
            PaymentType = paymentType,
            PeriodId = periodId,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        });

        return Ok(result);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPaymentByIdQry { Id = id });
        return Ok(result);
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a single payment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    public async Task<IActionResult> Create([FromBody] AddPaymentDto dto)
    {
        // ✅ Validate using FluentValidation
        var validationResult = await _addValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        var result = await _mediator.Send(new AddPaymentCmd { AddDto = dto });

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse<PaymentDto>
        {
            Success = true,
            Message = "Payment created successfully",
            Data = result
        });
    }

    /// <summary>
    /// Bulk create payments
    /// </summary>
    [HttpPost("Bulk")]
    [ProducesResponseType(typeof(ApiResponse<List<PaymentDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    public async Task<IActionResult> BulkCreate([FromBody] List<AddPaymentDto> dtos)
    {
        // ✅ Size cap
        if (dtos.Count > 500)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Maximum 500 payments per bulk request"
            });
        }

        // ✅ Validate all items
        var errors = new List<ValidationError>();
        for (int i = 0; i < dtos.Count; i++)
        {
            var validationResult = await _addValidator.ValidateAsync(dtos[i]);
            if (!validationResult.IsValid)
            {
                errors.Add(new ValidationError
                {
                    Index = i,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }
        }

        if (errors.Any())
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed for one or more items",
                Errors = errors
            });
        }

        var createdPayments = await _mediator.Send(new BulkAddPaymentCmd { AddDtos = dtos });

        return CreatedAtAction(nameof(GetAll), new { page = 1, pageSize = 50 }, new ApiResponse<List<PaymentDto>>
        {
            Success = true,
            Message = $"{createdPayments.Count} payments created successfully",
            Data = createdPayments
        });
    }

    // ============================================================
    // PUT/PATCH/DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a payment
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    public async Task<IActionResult> Update([FromBody] EditPaymentDto dto)
    {
        // ✅ Validate using FluentValidation
        var validationResult = await _editValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        var result = await _mediator.Send(new EditPaymentCmd { EditDto = dto });

        return Ok(new ApiResponse<PaymentDto>
        {
            Success = true,
            Message = "Payment updated successfully",
            Data = result
        });
    }

    /// <summary>
    /// Process a payment
    /// </summary>
    [HttpPost("{id}/process")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    public async Task<IActionResult> Process(Guid id)
    {
        var result = await _mediator.Send(new ProcessPaymentCmd { Id = id });

        if (!result)
            return NotFound(new ApiResponse<object> { Success = false, Message = $"Payment with ID '{id}' not found" });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Payment processed successfully"
        });
    }

    /// <summary>
    /// Cancel a payment
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Accountant,FinanceManager")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _mediator.Send(new CancelPaymentCmd { Id = id });

        if (!result)
            return NotFound(new ApiResponse<object> { Success = false, Message = $"Payment with ID '{id}' not found" });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Payment cancelled successfully"
        });
    }

    /// <summary>
    /// Delete a payment
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeletePaymentCmd { Id = id });

        if (!result)
            return NotFound(new ApiResponse<object> { Success = false, Message = $"Payment with ID '{id}' not found" });

        return NoContent();
    }
}

// ============================================================
// HELPER CLASSES
// ============================================================

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }
}

public class ValidationError
{
    public int Index { get; set; }
    public List<string> Errors { get; set; } = new();
}