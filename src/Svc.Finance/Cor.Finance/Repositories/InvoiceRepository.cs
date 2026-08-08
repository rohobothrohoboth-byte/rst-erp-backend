// Repositories/InvoiceRepository.cs - FULLY CORRECTED WITH YOUR DTO PROPERTIES

using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Cor.Finance.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using System.Linq.Expressions;
using Helpers;
using MediatR;

namespace Cor.Finance.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly FinanceDbContext _writeContext;
    private readonly FinanceDbContextReadOnly _readContext;
    private readonly ICacheService _cache;
    private readonly ILogger<InvoiceRepository> _logger;
    private readonly IMediator _mediator;

    private const string CACHE_KEY_PREFIX = "invoices";
    private const int CACHE_DURATION_MINUTES = 5;

    public InvoiceRepository(
        FinanceDbContext writeContext,
        FinanceDbContextReadOnly readContext,
        ICacheService cache,
        ILogger<InvoiceRepository> logger,
        IMediator mediator)
    {
        _writeContext = writeContext;
        _readContext = readContext;
        _cache = cache;
        _logger = logger;
        _mediator = mediator;
    }

    // ============================================================
    // WRITE OPERATIONS
    // ============================================================

    public async Task<Invoice> AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("📝 Creating invoice: {InvoiceNumber}", invoice.InvoiceNumber);

            if (string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync(ct);
            }

            ValidateInvoice(invoice);

            await _writeContext.Invoices.AddAsync(invoice, ct);
            await _writeContext.SaveChangesAsync(ct);

            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Invoice created: {InvoiceNumber} (Id: {Id})",
                invoice.InvoiceNumber, invoice.Id);

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating invoice: {InvoiceNumber}", invoice.InvoiceNumber);
            throw;
        }
    }

    public async Task<Invoice> AddWithLinesAsync(Invoice invoice, List<InvoiceLine> lines, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("📝 Creating invoice with {LineCount} lines", lines.Count);

            if (string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                invoice.InvoiceNumber = await GenerateInvoiceNumberAsync(ct);
            }

            invoice.SubTotal = lines.Sum(l => l.UnitPrice * l.Quantity);
            invoice.TaxAmount = lines.Sum(l => l.TaxRate * l.UnitPrice * l.Quantity / 100);
            invoice.TotalAmount = invoice.SubTotal + invoice.TaxAmount - invoice.DiscountAmount;
            invoice.Lines = lines;

            ValidateInvoice(invoice);

            await _writeContext.Invoices.AddAsync(invoice, ct);
            await _writeContext.SaveChangesAsync(ct);

            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Invoice created with {LineCount} lines: {InvoiceNumber}",
                lines.Count, invoice.InvoiceNumber);

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating invoice with lines");
            throw;
        }
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("📝 Updating invoice: {InvoiceNumber}", invoice.InvoiceNumber);

            ValidateInvoice(invoice);

            _writeContext.Invoices.Update(invoice);
            await _writeContext.SaveChangesAsync(ct);

            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Invoice updated: {InvoiceNumber}", invoice.InvoiceNumber);

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating invoice: {InvoiceNumber}", invoice.InvoiceNumber);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🗑️ Deleting invoice: {Id}", id);

            var invoice = await _writeContext.Invoices
                .OrderBy(i => i.InvoiceNumber)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, ct);

            if (invoice == null)
            {
                _logger.LogWarning("⚠️ Invoice not found for deletion: {Id}", id);
                return false;
            }

            var hasPayments = await _writeContext.Payments
                .AnyAsync(p => p.InvoiceId == id && !p.IsDeleted, ct);

            if (hasPayments)
            {
                _logger.LogWarning("⚠️ Cannot delete invoice with payments: {Id}", id);
                return false;
            }

            invoice.IsDeleted = true;
            invoice.DateMod = DateTime.UtcNow;

            await _writeContext.SaveChangesAsync(ct);
            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Invoice deleted: {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting invoice: {Id}", id);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("📝 Updating invoice status: {Id} -> {Status}", id, status);

            var invoice = await _writeContext.Invoices
                .OrderBy(i => i.InvoiceNumber)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, ct);

            if (invoice == null)
            {
                _logger.LogWarning("⚠️ Invoice not found: {Id}", id);
                return false;
            }

            invoice.Status = status;
            invoice.DateMod = DateTime.UtcNow;

            await _writeContext.SaveChangesAsync(ct);
            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Invoice status updated: {Id} -> {Status}", id, status);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating invoice status: {Id}", id);
            throw;
        }
    }

    public async Task<bool> AddPaymentToInvoiceAsync(Guid invoiceId, Payment payment, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("💰 Adding payment to invoice: {InvoiceId}", invoiceId);

            var invoice = await _writeContext.Invoices
                .OrderBy(i => i.InvoiceNumber)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, ct);

            if (invoice == null)
            {
                _logger.LogWarning("⚠️ Invoice not found: {InvoiceId}", invoiceId);
                return false;
            }

            payment.InvoiceId = invoiceId;
            await _writeContext.Payments.AddAsync(payment, ct);

            invoice.PaidAmount += payment.Amount;
            invoice.DateMod = DateTime.UtcNow;

            if (invoice.PaidAmount >= invoice.TotalAmount)
            {
                invoice.Status = "Paid";
            }

            await _writeContext.SaveChangesAsync(ct);
            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Payment added to invoice: {InvoiceId}, Amount: {Amount}",
                invoiceId, payment.Amount);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error adding payment to invoice: {InvoiceId}", invoiceId);
            throw;
        }
    }

    // ============================================================
    // READ OPERATIONS (with caching)
    // ============================================================

    public async Task<InvoiceDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}:id:{id}";

            var cached = await _cache.GetAsync<InvoiceDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var invoice = await _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Include(i => i.Vendor)
                .Include(i => i.Customer)
                .Include(i => i.Lines.Where(l => !l.IsDeleted))
                .Include(i => i.Payments.Where(p => !p.IsDeleted))
                .Where(i => i.Id == id && !i.IsDeleted)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                    Lines = i.Lines.Select(l => new InvoiceLineDto
                    {
                        Id = l.Id,
                        Description = l.Description,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice,
                        Discount = l.Discount,
                        TaxRate = l.TaxRate,
                        TotalAmount = l.UnitPrice * l.Quantity,
                        DateAdd = l.DateAdd,
                        DateMod = l.DateMod
                    }).ToList(),
                    Payments = i.Payments.Select(p => new PaymentDto
                    {
                        Id = p.Id,
                        PaymentNumber = p.PaymentNumber,
                        Amount = p.Amount,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentMethod,
                        PaymentType = p.PaymentType,
                        Status = p.Status,
                        Description = p.Description,
                        Reference = p.Reference,
                        DateAdd = p.DateAdd,
                        DateMod = p.DateMod
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (invoice is not null)
            {
                await _cache.SetAsync(cacheKey, invoice, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
            }

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoice by ID: {Id}", id);
            throw;
        }
    }

    public async Task<InvoiceDto?> GetByNumberAsync(string invoiceNumber, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}:number:{invoiceNumber}";

            var cached = await _cache.GetAsync<InvoiceDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var invoice = await _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Include(i => i.Vendor)
                .Include(i => i.Customer)
                .Include(i => i.Lines.Where(l => !l.IsDeleted))
                .Include(i => i.Payments.Where(p => !p.IsDeleted))
                .Where(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                    Lines = i.Lines.Select(l => new InvoiceLineDto
                    {
                        Id = l.Id,
                        Description = l.Description,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice,
                        Discount = l.Discount,
                        TaxRate = l.TaxRate,
                        TotalAmount = l.UnitPrice * l.Quantity,
                        DateAdd = l.DateAdd,
                        DateMod = l.DateMod
                    }).ToList(),
                    Payments = i.Payments.Select(p => new PaymentDto
                    {
                        Id = p.Id,
                        PaymentNumber = p.PaymentNumber,
                        Amount = p.Amount,
                        PaymentDate = p.PaymentDate,
                        PaymentMethod = p.PaymentMethod,
                        PaymentType = p.PaymentType,
                        Status = p.Status,
                        Description = p.Description,
                        Reference = p.Reference,
                        DateAdd = p.DateAdd,
                        DateMod = p.DateMod
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (invoice is not null)
            {
                await _cache.SetAsync(cacheKey, invoice, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
            }

            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoice by number: {InvoiceNumber}", invoiceNumber);
            throw;
        }
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetAllAsync(GetAllInvoicesQry query, CancellationToken ct = default)
    {
        try
        {
            var shouldCache = query.Page == 1 &&
                             query.PageSize <= 50 &&
                             !query.FromDate.HasValue &&
                             !query.ToDate.HasValue &&
                             string.IsNullOrEmpty(query.Status) &&
                             string.IsNullOrEmpty(query.InvoiceType) &&
                             !query.VendorId.HasValue &&
                             !query.CustomerId.HasValue &&
                             !query.PeriodId.HasValue;

            var cacheKey = $"{CACHE_KEY_PREFIX}:all:page{query.Page}:size{query.PageSize}";

            if (shouldCache)
            {
                var cached = await _cache.GetAsync<PaginatedResponse<InvoiceDto>>(cacheKey);
                if (cached is not null)
                {
                    _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                    return cached;
                }
                _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);
            }

            var queryable = _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Include(i => i.Vendor)
                .Include(i => i.Customer)
                .Where(i => !i.IsDeleted);

            queryable = ApplyFilters(queryable, query);

            var totalCount = await queryable.CountAsync(ct);

            queryable = ApplySorting(queryable, query.SortBy, query.SortDirection);
            queryable = queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize);

            var items = await queryable
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                })
                .ToListAsync(ct);

            var result = new PaginatedResponse<InvoiceDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize),
                HasNextPage = query.Page < (int)Math.Ceiling((double)totalCount / query.PageSize),
                HasPreviousPage = query.Page > 1
            };

            if (shouldCache)
            {
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices");
            throw;
        }
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetByCustomerAsync(Guid customerId, GetAllInvoicesQry? query = null, CancellationToken ct = default)
    {
        try
        {
            query ??= new GetAllInvoicesQry();

            var cacheKey = $"{CACHE_KEY_PREFIX}:customer:{customerId}:page{query.Page}:size{query.PageSize}";

            var cached = await _cache.GetAsync<PaginatedResponse<InvoiceDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var queryable = _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Include(i => i.Customer)
                .Where(i => !i.IsDeleted && i.CustomerId == customerId);

            queryable = ApplyFilters(queryable, query);
            var totalCount = await queryable.CountAsync(ct);
            queryable = ApplySorting(queryable, query.SortBy, query.SortDirection);

            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                })
                .ToListAsync(ct);

            var result = CreatePaginatedResponse(items, totalCount, query.Page, query.PageSize);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices for customer: {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetByVendorAsync(Guid vendorId, GetAllInvoicesQry? query = null, CancellationToken ct = default)
    {
        try
        {
            query ??= new GetAllInvoicesQry();

            var cacheKey = $"{CACHE_KEY_PREFIX}:vendor:{vendorId}:page{query.Page}:size{query.PageSize}";

            var cached = await _cache.GetAsync<PaginatedResponse<InvoiceDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var queryable = _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Include(i => i.Vendor)
                .Where(i => !i.IsDeleted && i.VendorId == vendorId);

            queryable = ApplyFilters(queryable, query);
            var totalCount = await queryable.CountAsync(ct);
            queryable = ApplySorting(queryable, query.SortBy, query.SortDirection);

            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                })
                .ToListAsync(ct);

            var result = CreatePaginatedResponse(items, totalCount, query.Page, query.PageSize);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices for vendor: {VendorId}", vendorId);
            throw;
        }
    }

    public async Task<PaginatedResponse<InvoiceDto>> GetByTypeAsync(string invoiceType, GetAllInvoicesQry? query = null, CancellationToken ct = default)
    {
        try
        {
            query ??= new GetAllInvoicesQry();

            var cacheKey = $"{CACHE_KEY_PREFIX}:type:{invoiceType}:page{query.Page}:size{query.PageSize}";

            var cached = await _cache.GetAsync<PaginatedResponse<InvoiceDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var queryable = _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Where(i => !i.IsDeleted && i.InvoiceType == invoiceType);

            queryable = ApplyFilters(queryable, query);
            var totalCount = await queryable.CountAsync(ct);
            queryable = ApplySorting(queryable, query.SortBy, query.SortDirection);

            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                })
                .ToListAsync(ct);

            var result = CreatePaginatedResponse(items, totalCount, query.Page, query.PageSize);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoices by type: {InvoiceType}", invoiceType);
            throw;
        }
    }

    public async Task<List<InvoiceDto>> GetOverdueInvoicesAsync(DateTime? asOfDate = null, CancellationToken ct = default)
    {
        try
        {
            var date = asOfDate ?? DateTime.UtcNow;
            var cacheKey = $"{CACHE_KEY_PREFIX}:overdue:{date:yyyyMMdd}";

            var cached = await _cache.GetAsync<List<InvoiceDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var invoices = await _readContext.Invoices
                .AsNoTracking()
                .Include(i => i.Period)
                .Include(i => i.Vendor)
                .Include(i => i.Customer)
                .Where(i => !i.IsDeleted &&
                           i.DueDate < date &&
                           i.Status != "Paid" &&
                           i.PaidAmount < i.TotalAmount)
                .OrderBy(i => i.DueDate)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    SubTotal = i.SubTotal,
                    TaxAmount = i.TaxAmount,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    BalanceDue = i.TotalAmount - i.PaidAmount,
                    Status = i.Status,
                    InvoiceType = i.InvoiceType,
                    VendorId = i.VendorId,
                    CustomerId = i.CustomerId,
                    Notes = i.Notes,
                    BranchId = i.BranchId,
                    DepartmentId = i.DepartmentId,
                    EmployeeId = i.EmployeeId,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ReceivedDate = i.ReceivedDate,
                    SalesRep = i.SalesRep,
                    DeliveryDate = i.DeliveryDate,
                    PeriodId = i.PeriodId,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion ?? string.Empty,
                })
                .ToListAsync(ct);

            await _cache.SetAsync(cacheKey, invoices, TimeSpan.FromMinutes(15));

            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving overdue invoices");
            throw;
        }
    }

    public async Task<InvoiceSummaryDto> GetSummaryAsync(Guid? periodId = null, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}:summary:period:{periodId ?? Guid.Empty}";

            var cached = await _cache.GetAsync<InvoiceSummaryDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var query = _readContext.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted);

            if (periodId.HasValue)
            {
                query = query.Where(i => i.PeriodId == periodId.Value);
            }

            var summary = await query
                .GroupBy(i => 1)
                .Select(g => new InvoiceSummaryDto
                {
                    TotalInvoices = g.Count(),
                    DraftCount = g.Count(x => x.Status == "Draft"),
                    PostedCount = g.Count(x => x.Status == "Posted"),
                    PaidCount = g.Count(x => x.Status == "Paid"),
                    OverdueCount = g.Count(x => x.Status == "Overdue"),
                    TotalAmount = g.Sum(x => x.TotalAmount),
                    TotalPaid = g.Sum(x => x.PaidAmount),
                    TotalBalance = g.Sum(x => x.TotalAmount - x.PaidAmount),
                    AverageInvoiceAmount = g.Average(x => x.TotalAmount),
                    PeriodId = periodId
                })
                .FirstOrDefaultAsync(ct) ?? new InvoiceSummaryDto();

            await _cache.SetAsync(cacheKey, summary, TimeSpan.FromMinutes(10));

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving invoice summary");
            throw;
        }
    }

    public async Task<AgingReportDto> GetAgingReportAsync(DateTime asOfDate, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}:aging:{asOfDate:yyyyMMdd}";

            var cached = await _cache.GetAsync<AgingReportDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {Key}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {Key}", cacheKey);

            var invoices = await _readContext.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted && i.Status != "Paid" && i.PaidAmount < i.TotalAmount)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.DueDate,
                    i.TotalAmount,
                    i.PaidAmount,
                    Balance = i.TotalAmount - i.PaidAmount,
                    VendorName = i.Vendor != null ? i.Vendor.Name : null,
                    CustomerName = i.Customer != null ? i.Customer.Name : null,
                    DaysOverdue = i.DueDate.HasValue ? (asOfDate - i.DueDate.Value).Days : 0
                })
                .ToListAsync(ct);

            var agingReport = new AgingReportDto
            {
                Period = asOfDate.ToString("yyyy-MM-dd"),
                Details = new List<AgingDetailDto>()
            };

            foreach (var invoice in invoices)
            {
                var daysOverdue = Math.Max(0, invoice.DaysOverdue);
                var bucket = GetAgingBucket(daysOverdue);
                var partyName = invoice.VendorName ?? invoice.CustomerName ?? "Unknown";

                agingReport.Details.Add(new AgingDetailDto
                {
                    InvoiceId = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    PartyName = partyName,
                    InvoiceDate = invoice.InvoiceDate,
                    DueDate = invoice.DueDate ?? invoice.InvoiceDate,
                    Amount = invoice.Balance,
                    DaysOverdue = daysOverdue,
                    AgingBucket = bucket
                });

                switch (bucket)
                {
                    case "Current": agingReport.Current += invoice.Balance; break;
                    case "30-60": agingReport.Days30_60 += invoice.Balance; break;
                    case "60-90": agingReport.Days60_90 += invoice.Balance; break;
                    case "90+": agingReport.Days90Plus += invoice.Balance; break;
                }
                agingReport.Total += invoice.Balance;
            }

            await _cache.SetAsync(cacheKey, agingReport, TimeSpan.FromMinutes(30));

            return agingReport;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving aging report");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _readContext.Invoices
            .AnyAsync(i => i.Id == id && !i.IsDeleted, ct);
    }

    public async Task<bool> ExistsByNumberAsync(string invoiceNumber, CancellationToken ct = default)
    {
        return await _readContext.Invoices
            .AnyAsync(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted, ct);
    }

    public async Task<int> GetTotalCountAsync(Expression<Func<Invoice, bool>>? filter = null, CancellationToken ct = default)
    {
        var query = _readContext.Invoices.AsNoTracking().Where(i => !i.IsDeleted);

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(ct);
    }

    // ============================================================
    // BULK OPERATIONS
    // ============================================================

    public async Task<List<Invoice>> BulkAddAsync(List<Invoice> invoices, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("📝 Bulk creating {Count} invoices", invoices.Count);

            foreach (var invoice in invoices)
            {
                if (string.IsNullOrEmpty(invoice.InvoiceNumber))
                {
                    invoice.InvoiceNumber = await GenerateInvoiceNumberAsync(ct);
                }
                ValidateInvoice(invoice);
            }

            await _writeContext.Invoices.AddRangeAsync(invoices, ct);
            await _writeContext.SaveChangesAsync(ct);

            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Bulk created {Count} invoices", invoices.Count);

            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk creating invoices");
            throw;
        }
    }

    public async Task<bool> BulkDeleteAsync(List<Guid> ids, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🗑️ Bulk deleting {Count} invoices", ids.Count);

            var invoices = await _writeContext.Invoices
                .Where(i => ids.Contains(i.Id) && !i.IsDeleted)
                .ToListAsync(ct);

            if (!invoices.Any())
            {
                _logger.LogWarning("⚠️ No invoices found for bulk deletion");
                return false;
            }

            foreach (var invoice in invoices)
            {
                invoice.IsDeleted = true;
                invoice.DateMod = DateTime.UtcNow;
            }

            await _writeContext.SaveChangesAsync(ct);
            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Bulk deleted {Count} invoices", invoices.Count);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk deleting invoices");
            throw;
        }
    }

    public async Task<bool> BulkUpdateStatusAsync(List<Guid> ids, string status, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("📝 Bulk updating status for {Count} invoices -> {Status}", ids.Count, status);

            var invoices = await _writeContext.Invoices
                .Where(i => ids.Contains(i.Id) && !i.IsDeleted)
                .ToListAsync(ct);

            if (!invoices.Any())
            {
                _logger.LogWarning("⚠️ No invoices found for bulk status update");
                return false;
            }

            foreach (var invoice in invoices)
            {
                invoice.Status = status;
                invoice.DateMod = DateTime.UtcNow;
            }

            await _writeContext.SaveChangesAsync(ct);
            await InvalidateCacheAsync(null, ct);

            _logger.LogInformation("✅ Bulk status updated for {Count} invoices -> {Status}", invoices.Count, status);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk updating invoice status");
            throw;
        }
    }

    // ============================================================
    // CACHE MANAGEMENT
    // ============================================================

    public async Task InvalidateCacheAsync(string? pattern = null, CancellationToken ct = default)
    {
        try
        {
            var cachePattern = string.IsNullOrEmpty(pattern) ? $"{CACHE_KEY_PREFIX}:*" : pattern;
            await _cache.RemoveByPatternAsync(cachePattern, ct);
            _logger.LogDebug("🗑️ Cache invalidated: {Pattern}", cachePattern);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error invalidating cache");
        }
    }

    // ============================================================
    // PRIVATE HELPER METHODS
    // ============================================================

    private IQueryable<Invoice> ApplyFilters(IQueryable<Invoice> query, GetAllInvoicesQry filters)
    {
        if (filters.FromDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= filters.ToDate.Value);

        if (!string.IsNullOrEmpty(filters.Status))
            query = query.Where(i => i.Status == filters.Status);

        if (!string.IsNullOrEmpty(filters.InvoiceType))
            query = query.Where(i => i.InvoiceType == filters.InvoiceType);

        if (filters.VendorId.HasValue)
            query = query.Where(i => i.VendorId == filters.VendorId.Value);

        if (filters.CustomerId.HasValue)
            query = query.Where(i => i.CustomerId == filters.CustomerId.Value);

        if (filters.PeriodId.HasValue)
            query = query.Where(i => i.PeriodId == filters.PeriodId.Value);

        if (filters.MinAmount.HasValue)
            query = query.Where(i => i.TotalAmount >= filters.MinAmount.Value);

        if (filters.MaxAmount.HasValue)
            query = query.Where(i => i.TotalAmount <= filters.MaxAmount.Value);

        return query;
    }

    private IQueryable<Invoice> ApplySorting(IQueryable<Invoice> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
            sortBy = "InvoiceDate";

        return (sortDirection?.ToUpper() == "ASC")
            ? query.OrderBy(x => EF.Property<object>(x, sortBy))
            : query.OrderByDescending(x => EF.Property<object>(x, sortBy));
    }

    private PaginatedResponse<InvoiceDto> CreatePaginatedResponse(
        List<InvoiceDto> items, int totalCount, int page, int pageSize)
    {
        return new PaginatedResponse<InvoiceDto>
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

    private string GetAgingBucket(int daysOverdue)
    {
        if (daysOverdue <= 0) return "Current";
        if (daysOverdue <= 30) return "30-60";
        if (daysOverdue <= 60) return "60-90";
        return "90+";
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.ToString("yyyy");
        var prefix = "INV";

        var lastInvoice = await _readContext.Invoices
            .AsNoTracking()
            .Where(i => i.InvoiceNumber.StartsWith($"{prefix}{year}"))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync(ct);

        int sequence = 1;
        if (lastInvoice is not null)
        {
            var parts = lastInvoice.InvoiceNumber.Split('-');
            if (parts.Length >= 3 && int.TryParse(parts[2], out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{year}-{sequence:D6}";
    }

    private void ValidateInvoice(Invoice invoice)
    {
        if (invoice.TotalAmount < 0)
            throw new InvalidOperationException("Total amount cannot be negative");

        if (invoice.SubTotal < 0)
            throw new InvalidOperationException("Subtotal cannot be negative");

        if (invoice.TaxAmount < 0)
            throw new InvalidOperationException("Tax amount cannot be negative");

        if (invoice.PaidAmount < 0)
            throw new InvalidOperationException("Paid amount cannot be negative");

        if (invoice.PaidAmount > invoice.TotalAmount)
            throw new InvalidOperationException("Paid amount cannot exceed total amount");

        if (string.IsNullOrEmpty(invoice.InvoiceType))
            throw new InvalidOperationException("Invoice type is required");

        if (!new[] { "Purchase", "Sales" }.Contains(invoice.InvoiceType))
            throw new InvalidOperationException("Invoice type must be either 'Purchase' or 'Sales'");

        if (invoice.InvoiceType == "Purchase" && !invoice.VendorId.HasValue)
            throw new InvalidOperationException("Vendor is required for purchase invoices");

        if (invoice.InvoiceType == "Sales" && !invoice.CustomerId.HasValue)
            throw new InvalidOperationException("Customer is required for sales invoices");
    }
}