// Queries/PaymentQry.cs - Complete Fixed Version

using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
using System.Linq.Expressions;

namespace Cor.Finance.Queries;

// ============================================================
// EXCEPTION CLASSES
// ============================================================

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// ============================================================
// SHARED PROJECTION - Single source of truth
// ============================================================

public static class PaymentProjection
{
    public static Expression<Func<Payment, PaymentDto>> ToDto =>
        payment => new PaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            PaymentDate = payment.PaymentDate,
            PaymentType = payment.PaymentType,
            PaymentMethod = payment.PaymentMethod,
            Amount = payment.Amount,
            Description = payment.Description,
            Status = payment.Status,
            Reference = payment.Reference,
            PeriodId = payment.PeriodId,
            PeriodName = payment.Period != null ? payment.Period.Name : "Unknown Period",
            VendorId = payment.VendorId,
            VendorName = payment.Vendor != null ? payment.Vendor.Name : null,
            CustomerId = payment.CustomerId,
            CustomerName = payment.Customer != null ? payment.Customer.Name : null,
            InvoiceId = payment.InvoiceId,
            InvoiceNumber = payment.Invoice != null ? payment.Invoice.InvoiceNumber : null,
            BankAccountId = payment.BankAccountId,
            BankAccountName = payment.BankAccount != null ? payment.BankAccount.AccountName : null,
            JournalEntryId = payment.JournalEntryId,
            BranchId = payment.BranchId,
            EmployeeId = payment.EmployeeId,
            DateAdd = payment.DateAdd,
            DateMod = payment.DateMod,
            RowVersion = payment.RowVersion
        };
}

// ============================================================
// QUERIES
// ============================================================

public class GetAllPaymentsQry : IRequest<PaginatedResponse<PaymentDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? PaymentType { get; set; }
    public Guid? PeriodId { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? CustomerId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "PaymentDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetPaymentByIdQry : IRequest<PaymentDto>
{
    public Guid Id { get; set; }
}

public class GetPaymentsByInvoiceQry : IRequest<PaginatedResponse<PaymentDto>>
{
    public Guid InvoiceId { get; set; }
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "PaymentDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetPaymentSummaryQry : IRequest<PaymentSummaryDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? PeriodId { get; set; }
}

public class GetPurchasePaymentsQry : IRequest<PaginatedResponse<PaymentDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "PaymentDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetSalesPaymentsQry : IRequest<PaginatedResponse<PaymentDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "PaymentDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetPaymentsByTypeQry : IRequest<PaginatedResponse<PaymentDto>>
{
    public string PaymentType { get; set; } = string.Empty;
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "PaymentDate";
    public string? SortDirection { get; set; } = "DESC";
}

// ============================================================
// BASE HANDLER - Shared logic for all payment queries
// ============================================================

public abstract class BasePaymentHandler
{
    protected readonly FinanceDbContext _context;
    protected readonly ILogger _logger;

    protected BasePaymentHandler(FinanceDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    // ? Shared method to build base query with includes
    protected IQueryable<Payment> BuildBaseQuery(
        DateTime? fromDate,
        DateTime? toDate,
        string? status,
        Guid? periodId,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        string? paymentType = null,
        string? paymentMethod = null)
    {
        var query = _context.Payments
            .AsNoTracking()
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .Include(x => x.Customer)
            .Include(x => x.Invoice)
            .Include(x => x.BankAccount)
            .Where(x => !x.IsDeleted);

        if (periodId.HasValue)
            query = query.Where(x => x.PeriodId == periodId.Value);

        if (fromDate.HasValue)
            query = query.Where(x => x.PaymentDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.PaymentDate <= toDate.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(paymentType))
            query = query.Where(x => x.PaymentType == paymentType);

        if (!string.IsNullOrEmpty(paymentMethod))
            query = query.Where(x => x.PaymentMethod == paymentMethod);

        if (minAmount.HasValue)
            query = query.Where(x => x.Amount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(x => x.Amount <= maxAmount.Value);

        return query;
    }

    // ? Apply sorting
    protected IQueryable<Payment> ApplySorting(IQueryable<Payment> query, string? sortBy, string? sortDirection)
    {
        return (sortDirection?.ToUpper() == "ASC")
            ? query.OrderBy(p => EF.Property<object>(p, sortBy ?? "PaymentDate"))
            : query.OrderByDescending(p => EF.Property<object>(p, sortBy ?? "PaymentDate"));
    }

    // ? Apply pagination
    protected IQueryable<Payment> ApplyPagination(IQueryable<Payment> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    // ? Get total count and return paginated response
    protected async Task<PaginatedResponse<PaymentDto>> GetPaginatedResponseAsync(
        IQueryable<Payment> query,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Select(PaymentProjection.ToDto)
            .ToListAsync(ct);

        return new PaginatedResponse<PaymentDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
            HasPreviousPage = page > 1
        };
    }
}

// ============================================================
// GET ALL PAYMENTS HANDLER
// ============================================================

public class GetAllPaymentsHandler : BasePaymentHandler, IRequestHandler<GetAllPaymentsQry, PaginatedResponse<PaymentDto>>
{
    public GetAllPaymentsHandler(FinanceDbContext context, ILogger<GetAllPaymentsHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<PaymentDto>> Handle(GetAllPaymentsQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetAllPaymentsHandler - Page: {Page}, PageSize: {PageSize}",
                request.Page, request.PageSize);

            var query = BuildBaseQuery(
                request.FromDate,
                request.ToDate,
                request.Status,
                request.PeriodId,
                request.MinAmount,
                request.MaxAmount,
                request.PaymentType,
                request.PaymentMethod);

            if (request.CustomerId.HasValue)
                query = query.Where(x => x.CustomerId == request.CustomerId.Value);

            if (request.VendorId.HasValue)
                query = query.Where(x => x.VendorId == request.VendorId.Value);

            if (request.InvoiceId.HasValue)
                query = query.Where(x => x.InvoiceId == request.InvoiceId.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);
            query = ApplyPagination(query, request.Page, request.PageSize);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving payments");
            throw;
        }
    }
}

// ============================================================
// GET PAYMENT BY ID HANDLER
// ============================================================

public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQry, PaymentDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetPaymentByIdHandler> _logger;

    public GetPaymentByIdHandler(FinanceDbContext context, ILogger<GetPaymentByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetPaymentByIdHandler - Id: {Id}", request.Id);

            var payment = await _context.Payments
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.Vendor)
                .Include(x => x.Customer)
                .Include(x => x.Invoice)
                .Include(x => x.BankAccount)
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(PaymentProjection.ToDto)
                .FirstOrDefaultAsync(ct);

            if (payment == null)
                throw new NotFoundException($"Payment with ID '{request.Id}' not found");

            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving payment {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET PAYMENTS BY INVOICE HANDLER
// ============================================================

public class GetPaymentsByInvoiceHandler : BasePaymentHandler, IRequestHandler<GetPaymentsByInvoiceQry, PaginatedResponse<PaymentDto>>
{
    public GetPaymentsByInvoiceHandler(FinanceDbContext context, ILogger<GetPaymentsByInvoiceHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<PaymentDto>> Handle(GetPaymentsByInvoiceQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetPaymentsByInvoiceHandler - InvoiceId: {InvoiceId}", request.InvoiceId);

            var query = _context.Payments
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.Vendor)
                .Include(x => x.Customer)
                .Include(x => x.Invoice)
                .Include(x => x.BankAccount)
                .Where(x => x.InvoiceId == request.InvoiceId && !x.IsDeleted);

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);
            query = ApplyPagination(query, request.Page, request.PageSize);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving payments for invoice {InvoiceId}", request.InvoiceId);
            throw;
        }
    }
}

// ============================================================
// GET PURCHASE PAYMENTS HANDLER
// ============================================================

public class GetPurchasePaymentsHandler : BasePaymentHandler, IRequestHandler<GetPurchasePaymentsQry, PaginatedResponse<PaymentDto>>
{
    public GetPurchasePaymentsHandler(FinanceDbContext context, ILogger<GetPurchasePaymentsHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<PaymentDto>> Handle(GetPurchasePaymentsQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetPurchasePaymentsHandler started");

            var query = BuildBaseQuery(
                request.FromDate,
                request.ToDate,
                request.Status,
                request.PeriodId);

            query = query.Where(x => x.PaymentType == "Purchase");

            if (request.VendorId.HasValue)
                query = query.Where(x => x.VendorId == request.VendorId.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);
            query = ApplyPagination(query, request.Page, request.PageSize);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving purchase payments");
            throw;
        }
    }
}

// ============================================================
// GET SALES PAYMENTS HANDLER
// ============================================================

public class GetSalesPaymentsHandler : BasePaymentHandler, IRequestHandler<GetSalesPaymentsQry, PaginatedResponse<PaymentDto>>
{
    public GetSalesPaymentsHandler(FinanceDbContext context, ILogger<GetSalesPaymentsHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<PaymentDto>> Handle(GetSalesPaymentsQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetSalesPaymentsHandler started");

            var query = BuildBaseQuery(
                request.FromDate,
                request.ToDate,
                request.Status,
                request.PeriodId);

            query = query.Where(x => x.PaymentType == "Sales");

            if (request.CustomerId.HasValue)
                query = query.Where(x => x.CustomerId == request.CustomerId.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);
            query = ApplyPagination(query, request.Page, request.PageSize);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving sales payments");
            throw;
        }
    }
}

// ============================================================
// GET PAYMENTS BY TYPE HANDLER
// ============================================================

public class GetPaymentsByTypeHandler : BasePaymentHandler, IRequestHandler<GetPaymentsByTypeQry, PaginatedResponse<PaymentDto>>
{
    public GetPaymentsByTypeHandler(FinanceDbContext context, ILogger<GetPaymentsByTypeHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<PaymentDto>> Handle(GetPaymentsByTypeQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetPaymentsByTypeHandler - Type: {PaymentType}", request.PaymentType);

            var query = _context.Payments
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.Vendor)
                .Include(x => x.Customer)
                .Include(x => x.Invoice)
                .Include(x => x.BankAccount)
                .Where(x => x.PaymentType == request.PaymentType && !x.IsDeleted);

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);
            query = ApplyPagination(query, request.Page, request.PageSize);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving payments by type");
            throw;
        }
    }
}

// ============================================================
// GET PAYMENT SUMMARY HANDLER
// ============================================================

public class GetPaymentSummaryHandler : IRequestHandler<GetPaymentSummaryQry, PaymentSummaryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetPaymentSummaryHandler> _logger;

    public GetPaymentSummaryHandler(FinanceDbContext context, ILogger<GetPaymentSummaryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentSummaryDto> Handle(GetPaymentSummaryQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("?? GetPaymentSummaryHandler started");

            var query = _context.Payments
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.PaymentDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.PaymentDate <= request.ToDate.Value);

            // ? Use aggregation for better performance
            var summary = await query
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TotalAmount = g.Sum(x => x.Amount),
                    TotalPosted = g.Where(x => x.Status == "Posted" || x.Status == "Completed").Sum(x => x.Amount),
                    TotalPending = g.Where(x => x.Status == "Pending").Sum(x => x.Amount),
                    TotalCancelled = g.Where(x => x.Status == "Cancelled").Sum(x => x.Amount),
                    TotalVendorPayments = g.Where(x => x.PaymentType == "Purchase").Sum(x => x.Amount),
                    TotalCustomerPayments = g.Where(x => x.PaymentType == "Sales").Sum(x => x.Amount),
                    PaymentCount = g.Count()
                })
                .FirstOrDefaultAsync(ct);

            // Get period name
            string periodName = "All Periods";
            if (request.PeriodId.HasValue)
            {
                periodName = await _context.FinancialPeriods
                    .Where(p => p.Id == request.PeriodId.Value && !p.IsDeleted)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync(ct) ?? "Unknown Period";
            }

            // Get payment methods (separate query for grouping)
            var methods = await query
                .GroupBy(x => x.PaymentMethod)
                .Select(g => new { Key = g.Key, Sum = g.Sum(x => x.Amount) })
                .ToDictionaryAsync(g => g.Key ?? "Unknown", g => g.Sum, ct);

            var statuses = await query
                .GroupBy(x => x.Status)
                .Select(g => new { Key = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Key ?? "Unknown", g => g.Count, ct);

            return new PaymentSummaryDto
            {
                TotalPayments = summary?.TotalAmount ?? 0,
                TotalProcessed = summary?.TotalPosted ?? 0,
                TotalPending = summary?.TotalPending ?? 0,
                TotalCancelled = summary?.TotalCancelled ?? 0,
                PaymentCount = summary?.PaymentCount ?? 0,
                PeriodId = request.PeriodId ?? Guid.Empty,
                PeriodName = periodName,
                TotalVendorPayments = summary?.TotalVendorPayments ?? 0,
                TotalCustomerPayments = summary?.TotalCustomerPayments ?? 0,
                PaymentsByMethod = methods,
                PaymentsByStatus = statuses
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving payment summary");
            throw;
        }
    }
}