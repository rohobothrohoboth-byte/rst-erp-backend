using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Queries;

public class GetAllInvoicesQueryHandler
    : IRequestHandler<GetAllInvoicesQuery, List<InvoiceDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllInvoicesQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllInvoicesQueryHandler(
        ProcurementDbContext context,
        ILogger<GetAllInvoicesQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Try cache first
            var cacheKey = $"invoices_all_{request.Status}_{request.VendorId}_{request.PurchaseOrderId}";
            var cached = await _cache.GetAsync<List<InvoiceDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Invoices ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Invoices - fetching from database");

            var query = _context.Invoices
                .Include(i => i.LineItems)
                .Where(i => !i.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(i => i.Status == request.Status);
            }

            if (request.VendorId.HasValue)
            {
                query = query.Where(i => i.VendorId == request.VendorId.Value);
            }

            if (request.PurchaseOrderId.HasValue)
            {
                query = query.Where(i => i.PurchaseOrderId == request.PurchaseOrderId.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(i => i.InvoiceDate < toDate);
            }

            var invoices = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseOrderNumber = i.PurchaseOrderNumber,
                    VendorId = i.VendorId,
                    VendorName = i.VendorName,
                    Title = i.Title,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    ReceivedDate = i.ReceivedDate,
                    NetAmount = i.NetAmount,
                    TaxAmount = i.TaxAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    PaymentTerms = i.PaymentTerms,
                    Notes = i.Notes,
                    ApprovedBy = i.ApprovedBy,
                    ApprovedDate = i.ApprovedDate,
                    PaidBy = i.PaidBy,
                    PaidDate = i.PaidDate,
                    AttachmentCount = i.AttachmentCount,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion,
                    LineItems = i.LineItems
                        .Where(li => !li.IsDeleted)
                        .Select(li => new InvoiceLineItemDto
                        {
                            Id = li.Id,
                            PurchaseOrderItemId = li.PurchaseOrderItemId,
                            Description = li.Description,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TotalAmount = li.TotalAmount,
                            Discount = li.Discount,
                            TaxAmount = li.TaxAmount
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            // Cache the result
            await _cache.SetAsync(cacheKey, invoices, TimeSpan.FromMinutes(15), cancellationToken);
            _logger.LogInformation("✅ Cached {Count} invoices", invoices.Count);

            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching invoices - falling back to database");

            // Fallback - direct query without cache
            var query = _context.Invoices
                .Include(i => i.LineItems)
                .Where(i => !i.IsDeleted);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(i => i.Status == request.Status);

            if (request.VendorId.HasValue)
                query = query.Where(i => i.VendorId == request.VendorId.Value);

            if (request.PurchaseOrderId.HasValue)
                query = query.Where(i => i.PurchaseOrderId == request.PurchaseOrderId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(i => i.InvoiceDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(i => i.InvoiceDate < toDate);
            }

            return await query
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseOrderNumber = i.PurchaseOrderNumber,
                    VendorId = i.VendorId,
                    VendorName = i.VendorName,
                    Title = i.Title,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    ReceivedDate = i.ReceivedDate,
                    NetAmount = i.NetAmount,
                    TaxAmount = i.TaxAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    PaymentTerms = i.PaymentTerms,
                    Notes = i.Notes,
                    ApprovedBy = i.ApprovedBy,
                    ApprovedDate = i.ApprovedDate,
                    PaidBy = i.PaidBy,
                    PaidDate = i.PaidDate,
                    AttachmentCount = i.AttachmentCount,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion,
                    LineItems = i.LineItems
                        .Where(li => !li.IsDeleted)
                        .Select(li => new InvoiceLineItemDto
                        {
                            Id = li.Id,
                            PurchaseOrderItemId = li.PurchaseOrderItemId,
                            Description = li.Description,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TotalAmount = li.TotalAmount,
                            Discount = li.Discount,
                            TaxAmount = li.TaxAmount
                        }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}

public class GetInvoiceByIdQueryHandler
    : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetInvoiceByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetInvoiceByIdQueryHandler(
        ProcurementDbContext context,
        ILogger<GetInvoiceByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching invoice by ID: {Id}", request.Id);

            // Check cache first
            var cacheKey = $"invoice_{request.Id}";
            var cached = await _cache.GetAsync<InvoiceDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Invoice {Id}", request.Id);
                return cached;
            }

            var invoice = await _context.Invoices
                .Include(i => i.LineItems)
                .Where(i => i.Id == request.Id && !i.IsDeleted)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseOrderNumber = i.PurchaseOrderNumber,
                    VendorId = i.VendorId,
                    VendorName = i.VendorName,
                    Title = i.Title,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    ReceivedDate = i.ReceivedDate,
                    NetAmount = i.NetAmount,
                    TaxAmount = i.TaxAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    PaymentTerms = i.PaymentTerms,
                    Notes = i.Notes,
                    ApprovedBy = i.ApprovedBy,
                    ApprovedDate = i.ApprovedDate,
                    PaidBy = i.PaidBy,
                    PaidDate = i.PaidDate,
                    AttachmentCount = i.AttachmentCount,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion,
                    LineItems = i.LineItems
                        .Where(li => !li.IsDeleted)
                        .Select(li => new InvoiceLineItemDto
                        {
                            Id = li.Id,
                            PurchaseOrderItemId = li.PurchaseOrderItemId,
                            Description = li.Description,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TotalAmount = li.TotalAmount,
                            Discount = li.Discount,
                            TaxAmount = li.TaxAmount
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (invoice == null)
            {
                _logger.LogWarning("Invoice not found: {Id}", request.Id);
                throw new KeyNotFoundException($"Invoice with ID '{request.Id}' not found");
            }

            // Cache the result
            await _cache.SetAsync(cacheKey, invoice, TimeSpan.FromMinutes(15), cancellationToken);

            _logger.LogInformation("✅ Fetched invoice: {InvoiceNumber}", invoice.InvoiceNumber);
            return invoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching invoice {Id}", request.Id);
            throw;
        }
    }
}

public class GetInvoicesByPurchaseOrderQueryHandler
    : IRequestHandler<GetInvoicesByPurchaseOrderQuery, List<InvoiceDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetInvoicesByPurchaseOrderQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetInvoicesByPurchaseOrderQueryHandler(
        ProcurementDbContext context,
        ILogger<GetInvoicesByPurchaseOrderQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<InvoiceDto>> Handle(GetInvoicesByPurchaseOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching invoices for PO: {PurchaseOrderId}", request.PurchaseOrderId);

            var cacheKey = $"invoices_po_{request.PurchaseOrderId}";
            var cached = await _cache.GetAsync<List<InvoiceDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Invoices for PO {PurchaseOrderId}", request.PurchaseOrderId);
                return cached;
            }

            var invoices = await _context.Invoices
                .Include(i => i.LineItems)
                .Where(i => i.PurchaseOrderId == request.PurchaseOrderId && !i.IsDeleted)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseOrderNumber = i.PurchaseOrderNumber,
                    VendorId = i.VendorId,
                    VendorName = i.VendorName,
                    Title = i.Title,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    ReceivedDate = i.ReceivedDate,
                    NetAmount = i.NetAmount,
                    TaxAmount = i.TaxAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    PaymentTerms = i.PaymentTerms,
                    Notes = i.Notes,
                    ApprovedBy = i.ApprovedBy,
                    ApprovedDate = i.ApprovedDate,
                    PaidBy = i.PaidBy,
                    PaidDate = i.PaidDate,
                    AttachmentCount = i.AttachmentCount,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion,
                    LineItems = i.LineItems
                        .Where(li => !li.IsDeleted)
                        .Select(li => new InvoiceLineItemDto
                        {
                            Id = li.Id,
                            PurchaseOrderItemId = li.PurchaseOrderItemId,
                            Description = li.Description,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TotalAmount = li.TotalAmount,
                            Discount = li.Discount,
                            TaxAmount = li.TaxAmount
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, invoices, TimeSpan.FromMinutes(15), cancellationToken);

            _logger.LogInformation("✅ Fetched {Count} invoices for PO {PurchaseOrderId}", invoices.Count, request.PurchaseOrderId);
            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching invoices for PO {PurchaseOrderId}", request.PurchaseOrderId);
            throw;
        }
    }
}
public class SearchInvoicesQueryHandler
    : IRequestHandler<SearchInvoicesQuery, List<InvoiceDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<SearchInvoicesQueryHandler> _logger;

    public SearchInvoicesQueryHandler(
        ProcurementDbContext context,
        ILogger<SearchInvoicesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<InvoiceDto>> Handle(SearchInvoicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Searching invoices with term: {SearchTerm}", request.SearchTerm);

            var query = _context.Invoices
                .Include(i => i.LineItems)
                .Where(i => !i.IsDeleted);

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(i => i.Status == request.Status);
            }

            // Apply search term if provided
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower().Trim();
                query = query.Where(i =>
                    i.InvoiceNumber.ToLower().Contains(searchTerm) ||
                    (i.PurchaseOrderNumber != null && i.PurchaseOrderNumber.ToLower().Contains(searchTerm)) ||
                    (i.VendorName != null && i.VendorName.ToLower().Contains(searchTerm)) ||
                    i.Id.ToString().Contains(searchTerm)
                );
            }

            var invoices = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseOrderNumber = i.PurchaseOrderNumber,
                    VendorId = i.VendorId,
                    VendorName = i.VendorName,
                    Title = i.Title,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    ReceivedDate = i.ReceivedDate,
                    NetAmount = i.NetAmount,
                    TaxAmount = i.TaxAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    PaymentTerms = i.PaymentTerms,
                    Notes = i.Notes,
                    ApprovedBy = i.ApprovedBy,
                    ApprovedDate = i.ApprovedDate,
                    PaidBy = i.PaidBy,
                    PaidDate = i.PaidDate,
                    AttachmentCount = i.AttachmentCount,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion,
                    LineItems = i.LineItems
                        .Where(li => !li.IsDeleted)
                        .Select(li => new InvoiceLineItemDto
                        {
                            Id = li.Id,
                            PurchaseOrderItemId = li.PurchaseOrderItemId,
                            Description = li.Description,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TotalAmount = li.TotalAmount,
                            Discount = li.Discount,
                            TaxAmount = li.TaxAmount
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("✅ Found {Count} invoices matching search", invoices.Count);
            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching invoices");
            throw;
        }
    }
}
public class GetInvoicesByVendorQueryHandler
    : IRequestHandler<GetInvoicesByVendorQuery, List<InvoiceDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetInvoicesByVendorQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetInvoicesByVendorQueryHandler(
        ProcurementDbContext context,
        ILogger<GetInvoicesByVendorQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<InvoiceDto>> Handle(GetInvoicesByVendorQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching invoices for vendor: {VendorId}", request.VendorId);

            var cacheKey = $"invoices_vendor_{request.VendorId}";
            var cached = await _cache.GetAsync<List<InvoiceDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Invoices for vendor {VendorId}", request.VendorId);
                return cached;
            }

            var invoices = await _context.Invoices
                .Include(i => i.LineItems)
                .Where(i => i.VendorId == request.VendorId && !i.IsDeleted)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    PurchaseOrderId = i.PurchaseOrderId,
                    PurchaseOrderNumber = i.PurchaseOrderNumber,
                    VendorId = i.VendorId,
                    VendorName = i.VendorName,
                    Title = i.Title,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    ReceivedDate = i.ReceivedDate,
                    NetAmount = i.NetAmount,
                    TaxAmount = i.TaxAmount,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    PaymentTerms = i.PaymentTerms,
                    Notes = i.Notes,
                    ApprovedBy = i.ApprovedBy,
                    ApprovedDate = i.ApprovedDate,
                    PaidBy = i.PaidBy,
                    PaidDate = i.PaidDate,
                    AttachmentCount = i.AttachmentCount,
                    DateAdd = i.DateAdd,
                    DateMod = i.DateMod,
                    RowVersion = i.RowVersion,
                    LineItems = i.LineItems
                        .Where(li => !li.IsDeleted)
                        .Select(li => new InvoiceLineItemDto
                        {
                            Id = li.Id,
                            PurchaseOrderItemId = li.PurchaseOrderItemId,
                            Description = li.Description,
                            Quantity = li.Quantity,
                            UnitPrice = li.UnitPrice,
                            TotalAmount = li.TotalAmount,
                            Discount = li.Discount,
                            TaxAmount = li.TaxAmount
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, invoices, TimeSpan.FromMinutes(15), cancellationToken);

            _logger.LogInformation("✅ Fetched {Count} invoices for vendor {VendorId}", invoices.Count, request.VendorId);
            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching invoices for vendor {VendorId}", request.VendorId);
            throw;
        }
    }
}