using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
using System.Linq.Expressions;

namespace Cor.Finance.Queries;

// ============================================================
// SHARED PROJECTION - Single source of truth
// ============================================================

public static class InvoiceProjection
{
    public static Expression<Func<Invoice, InvoiceDto>> ToDto =>
        invoice => new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            BalanceDue = invoice.TotalAmount - invoice.PaidAmount,
            Status = invoice.Status,
            Notes = invoice.Notes,
            InvoiceType = invoice.InvoiceType,
            PeriodId = invoice.PeriodId,
            PeriodName = invoice.Period != null ? invoice.Period.Name : "Unknown Period",

            // Vendor
            VendorId = invoice.VendorId,
            VendorName = invoice.Vendor != null ? invoice.Vendor.Name : null,

            // Customer
            CustomerId = invoice.CustomerId,
            CustomerName = invoice.Customer != null ? invoice.Customer.Name : null,

            // ✅ Branch
            BranchId = invoice.BranchId,
            BranchName = invoice.Branch != null ? invoice.Branch.Name : null,

            // ✅ Department
            DepartmentId = invoice.DepartmentId,
            DepartmentName = invoice.Department != null ? invoice.Department.Name : null,

            // ✅ Employee
            EmployeeId = invoice.EmployeeId,
            EmployeeName = invoice.Employee != null ? $"{invoice.Employee.FirstName} {invoice.Employee.LastName}" : null,

            SalesRep = invoice.SalesRep,
            DeliveryDate = invoice.DeliveryDate,
            PurchaseOrderId = invoice.PurchaseOrderId,
            ReceivedDate = invoice.ReceivedDate,
            DateAdd = invoice.DateAdd,
            DateMod = invoice.DateMod,
            RowVersion = invoice.RowVersion,

            Lines = invoice.Lines
                .Where(l => !l.IsDeleted)
                .Select(l => new InvoiceLineDto
                {
                    Id = l.Id,
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    Discount = l.Discount,
                    TaxRate = l.TaxRate,
                    TotalAmount = l.TotalAmount,
                    PeriodId = l.PeriodId,
                    PeriodName = invoice.Period != null ? invoice.Period.Name : "Unknown Period",
                    DateAdd = l.DateAdd,
                    DateMod = l.DateMod
                }).ToList(),

            Payments = invoice.Payments
                .Where(p => !p.IsDeleted)
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    PaymentNumber = p.PaymentNumber,
                    PaymentDate = p.PaymentDate,
                    PaymentType = p.PaymentType,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    Description = p.Description,
                    Status = p.Status,
                    Reference = p.Reference,
                    VendorId = p.VendorId,
                    VendorName = p.Vendor != null ? p.Vendor.Name : null,
                    CustomerId = p.CustomerId,
                    CustomerName = p.Customer != null ? p.Customer.Name : null,
                    InvoiceId = p.InvoiceId,
                    InvoiceNumber = p.Invoice != null ? p.Invoice.InvoiceNumber : null,
                    JournalEntryId = p.JournalEntryId,
                    BankAccountId = p.BankAccountId,
                    BankAccountName = p.BankAccount != null ? p.BankAccount.AccountName : null,
                    BranchId = p.BranchId,
                    EmployeeId = p.EmployeeId,
                    PeriodId = p.PeriodId,
                    PeriodName = invoice.Period != null ? invoice.Period.Name : "Unknown Period",
                    DateAdd = p.DateAdd,
                    DateMod = p.DateMod
                }).ToList()
        };
}

// ============================================================
// QUERY CLASSES (DTOs/Requests)
// ============================================================

public class GetAllInvoicesQry : IRequest<PaginatedResponse<InvoiceDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public string? InvoiceType { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }        // ✅ Added
    public Guid? DepartmentId { get; set; }    // ✅ Added
    public Guid? PeriodId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "InvoiceDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetInvoiceByIdQry : IRequest<InvoiceDto>
{
    public Guid Id { get; set; }
}

public class GetInvoiceByNumberQry : IRequest<InvoiceDto>
{
    public string InvoiceNumber { get; set; } = string.Empty;
}

public class GetInvoicesByCustomerQry : IRequest<PaginatedResponse<InvoiceDto>>
{
    public Guid CustomerId { get; set; }
    public Guid? BranchId { get; set; }        // ✅ Added
    public Guid? DepartmentId { get; set; }    // ✅ Added
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "InvoiceDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetInvoicesByVendorQry : IRequest<PaginatedResponse<InvoiceDto>>
{
    public Guid VendorId { get; set; }
    public Guid? BranchId { get; set; }        // ✅ Added
    public Guid? DepartmentId { get; set; }    // ✅ Added
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "InvoiceDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetInvoicesByTypeQry : IRequest<PaginatedResponse<InvoiceDto>>
{
    public string InvoiceType { get; set; } = "Purchase";
    public Guid? BranchId { get; set; }        // ✅ Added
    public Guid? DepartmentId { get; set; }    // ✅ Added
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "InvoiceDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetSalesInvoicesQry : IRequest<PaginatedResponse<InvoiceDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }        // ✅ Added
    public Guid? DepartmentId { get; set; }    // ✅ Added
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "InvoiceDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetPurchaseInvoicesQry : IRequest<PaginatedResponse<InvoiceDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? BranchId { get; set; }        // ✅ Added
    public Guid? DepartmentId { get; set; }    // ✅ Added
    public Guid? PeriodId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "InvoiceDate";
    public string? SortDirection { get; set; } = "DESC";
}

// ============================================================
// BASE HANDLER - Shared logic
// ============================================================

public abstract class BaseInvoiceHandler
{
    protected readonly FinanceDbContext _context;
    protected readonly ILogger _logger;

    protected BaseInvoiceHandler(FinanceDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected IQueryable<Invoice> BuildBaseQuery(
        DateTime? fromDate,
        DateTime? toDate,
        string? status,
        string? invoiceType,
        Guid? periodId,
        Guid? vendorId,
        Guid? customerId,
        decimal? minAmount,
        decimal? maxAmount)
    {
        return BuildBaseQuery(
            fromDate, toDate, status, invoiceType,
            periodId, vendorId, customerId,
            null, null,
            minAmount, maxAmount);
    }

    protected IQueryable<Invoice> BuildBaseQuery(
        DateTime? fromDate,
        DateTime? toDate,
        string? status,
        string? invoiceType,
        Guid? periodId,
        Guid? vendorId,
        Guid? customerId,
        Guid? branchId,
        Guid? departmentId,
        decimal? minAmount,
        decimal? maxAmount)
    {
        var query = _context.Invoices
            .AsNoTracking()
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .Include(x => x.Customer)
            .Include(x => x.Branch)        // ✅ Added
            .Include(x => x.Department)    // ✅ Added
            .Include(x => x.Employee)      // ✅ Added
            .Include(x => x.Lines.Where(l => !l.IsDeleted))
            .Include(x => x.Payments.Where(p => !p.IsDeleted))
            .AsSplitQuery()
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (periodId.HasValue)
            query = query.Where(x => x.PeriodId == periodId.Value);

        if (fromDate.HasValue)
            query = query.Where(x => x.InvoiceDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.InvoiceDate <= toDate.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(invoiceType))
            query = query.Where(x => x.InvoiceType == invoiceType);

        if (vendorId.HasValue)
            query = query.Where(x => x.VendorId == vendorId.Value);

        if (customerId.HasValue)
            query = query.Where(x => x.CustomerId == customerId.Value);

        // ✅ Added branch and department filtering
        if (branchId.HasValue)
            query = query.Where(x => x.BranchId == branchId.Value);

        if (departmentId.HasValue)
            query = query.Where(x => x.DepartmentId == departmentId.Value);

        if (minAmount.HasValue)
            query = query.Where(x => x.TotalAmount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(x => x.TotalAmount <= maxAmount.Value);

        return query;
    }

    protected IQueryable<Invoice> ApplySorting(IQueryable<Invoice> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
            sortBy = "InvoiceDate";

        return (sortDirection?.ToUpper() == "ASC")
            ? query.OrderBy(x => EF.Property<object>(x, sortBy))
            : query.OrderByDescending(x => EF.Property<object>(x, sortBy));
    }

    protected async Task<PaginatedResponse<InvoiceDto>> GetPaginatedResponseAsync(
        IQueryable<Invoice> query,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(InvoiceProjection.ToDto)
            .ToListAsync(ct);

        return new PaginatedResponse<InvoiceDto>
        {
            Data = items,  // ✅ Changed from Items to Data
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
// GET ALL INVOICES HANDLER
// ============================================================

public class GetAllInvoicesHandler : BaseInvoiceHandler, IRequestHandler<GetAllInvoicesQry, PaginatedResponse<InvoiceDto>>
{
    public GetAllInvoicesHandler(FinanceDbContext context, ILogger<GetAllInvoicesHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<InvoiceDto>> Handle(GetAllInvoicesQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetAllInvoicesHandler - Page: {Page}, PageSize: {PageSize}",
                request.Page, request.PageSize);

            var query = BuildBaseQuery(
                request.FromDate,
                request.ToDate,
                request.Status,
                request.InvoiceType,
                request.PeriodId,
                request.VendorId,
                request.CustomerId,
                request.BranchId,      // ✅ Added
                request.DepartmentId,  // ✅ Added
                request.MinAmount,
                request.MaxAmount);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices");
            throw;
        }
    }
}

// ============================================================
// GET INVOICE BY ID HANDLER
// ============================================================

public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQry, InvoiceDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetInvoiceByIdHandler> _logger;

    public GetInvoiceByIdHandler(FinanceDbContext context, ILogger<GetInvoiceByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(GetInvoiceByIdQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📄 GetInvoiceByIdHandler - Id: {Id}", request.Id);

            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.Vendor)
                .Include(x => x.Customer)
                .Include(x => x.Branch)        // ✅ Added
                .Include(x => x.Department)    // ✅ Added
                .Include(x => x.Employee)      // ✅ Added
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                .Include(x => x.Payments.Where(p => !p.IsDeleted))
                .AsSplitQuery()
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(InvoiceProjection.ToDto)
                .FirstOrDefaultAsync(ct);

            if (invoice == null)
                throw new NotFoundException($"Invoice with ID '{request.Id}' not found");

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoice {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET INVOICE BY NUMBER HANDLER
// ============================================================

public class GetInvoiceByNumberHandler : IRequestHandler<GetInvoiceByNumberQry, InvoiceDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetInvoiceByNumberHandler> _logger;

    public GetInvoiceByNumberHandler(FinanceDbContext context, ILogger<GetInvoiceByNumberHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InvoiceDto> Handle(GetInvoiceByNumberQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📄 GetInvoiceByNumberHandler - Number: {InvoiceNumber}", request.InvoiceNumber);

            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.Vendor)
                .Include(x => x.Customer)
                .Include(x => x.Branch)        // ✅ Added
                .Include(x => x.Department)    // ✅ Added
                .Include(x => x.Employee)      // ✅ Added
                .Include(x => x.Lines.Where(l => !l.IsDeleted))
                .Include(x => x.Payments.Where(p => !p.IsDeleted))
                .AsSplitQuery()
                .Where(x => x.InvoiceNumber == request.InvoiceNumber && !x.IsDeleted)
                .Select(InvoiceProjection.ToDto)
                .FirstOrDefaultAsync(ct);

            if (invoice == null)
                throw new NotFoundException($"Invoice with number '{request.InvoiceNumber}' not found");

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoice by number {InvoiceNumber}", request.InvoiceNumber);
            throw;
        }
    }
}

// ============================================================
// GET INVOICES BY CUSTOMER HANDLER
// ============================================================

public class GetInvoicesByCustomerHandler : BaseInvoiceHandler, IRequestHandler<GetInvoicesByCustomerQry, PaginatedResponse<InvoiceDto>>
{
    public GetInvoicesByCustomerHandler(FinanceDbContext context, ILogger<GetInvoicesByCustomerHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<InvoiceDto>> Handle(GetInvoicesByCustomerQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetInvoicesByCustomerHandler - CustomerId: {CustomerId}", request.CustomerId);

            var query = BuildBaseQuery(
                null, null, null, null,
                request.PeriodId,
                null,
                request.CustomerId,
                request.BranchId,      // ✅ Added
                request.DepartmentId,  // ✅ Added
                null, null);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices for customer {CustomerId}", request.CustomerId);
            throw;
        }
    }
}

// ============================================================
// GET INVOICES BY VENDOR HANDLER
// ============================================================

public class GetInvoicesByVendorHandler : BaseInvoiceHandler, IRequestHandler<GetInvoicesByVendorQry, PaginatedResponse<InvoiceDto>>
{
    public GetInvoicesByVendorHandler(FinanceDbContext context, ILogger<GetInvoicesByVendorHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<InvoiceDto>> Handle(GetInvoicesByVendorQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetInvoicesByVendorHandler - VendorId: {VendorId}", request.VendorId);

            var query = BuildBaseQuery(
                null, null, null, null,
                request.PeriodId,
                request.VendorId,
                null,
                request.BranchId,      // ✅ Added
                request.DepartmentId,  // ✅ Added
                null, null);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices for vendor {VendorId}", request.VendorId);
            throw;
        }
    }
}

// ============================================================
// GET INVOICES BY TYPE HANDLER
// ============================================================

public class GetInvoicesByTypeHandler : BaseInvoiceHandler, IRequestHandler<GetInvoicesByTypeQry, PaginatedResponse<InvoiceDto>>
{
    public GetInvoicesByTypeHandler(FinanceDbContext context, ILogger<GetInvoicesByTypeHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<InvoiceDto>> Handle(GetInvoicesByTypeQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetInvoicesByTypeHandler - Type: {InvoiceType}", request.InvoiceType);

            var query = BuildBaseQuery(
                null, null, null,
                request.InvoiceType,
                request.PeriodId,
                null, null,
                request.BranchId,      // ✅ Added
                request.DepartmentId,  // ✅ Added
                null, null);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices by type {InvoiceType}", request.InvoiceType);
            throw;
        }
    }
}

// ============================================================
// GET SALES INVOICES HANDLER
// ============================================================

public class GetSalesInvoicesHandler : BaseInvoiceHandler, IRequestHandler<GetSalesInvoicesQry, PaginatedResponse<InvoiceDto>>
{
    public GetSalesInvoicesHandler(FinanceDbContext context, ILogger<GetSalesInvoicesHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<InvoiceDto>> Handle(GetSalesInvoicesQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetSalesInvoicesHandler - CustomerId: {CustomerId}", request.CustomerId);

            var query = BuildBaseQuery(
                request.FromDate,
                request.ToDate,
                request.Status,
                "Sales",
                request.PeriodId,
                null,
                request.CustomerId,
                request.BranchId,      // ✅ Added
                request.DepartmentId,  // ✅ Added
                null, null);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving sales invoices");
            throw;
        }
    }
}

// ============================================================
// GET PURCHASE INVOICES HANDLER
// ============================================================

public class GetPurchaseInvoicesHandler : BaseInvoiceHandler, IRequestHandler<GetPurchaseInvoicesQry, PaginatedResponse<InvoiceDto>>
{
    public GetPurchaseInvoicesHandler(FinanceDbContext context, ILogger<GetPurchaseInvoicesHandler> logger)
        : base(context, logger) { }

    public async Task<PaginatedResponse<InvoiceDto>> Handle(GetPurchaseInvoicesQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetPurchaseInvoicesHandler - VendorId: {VendorId}", request.VendorId);

            var query = BuildBaseQuery(
                request.FromDate,
                request.ToDate,
                request.Status,
                "Purchase",
                request.PeriodId,
                request.VendorId,
                null,
                request.BranchId,      // ✅ Added
                request.DepartmentId,  // ✅ Added
                null, null);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await GetPaginatedResponseAsync(query, request.Page, request.PageSize, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving purchase invoices");
            throw;
        }
    }
}