// Queries/PurchaseOrderQueries.cs
using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Cor.Procurement.Models.Entities;
namespace Cor.Procurement.Queries;

// ============================================================
// QUERIES
// ============================================================

public class GetAllPurchaseOrdersQuery : IRequest<List<PurchaseOrderDto>>
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "OrderDate";
    public string? SortOrder { get; set; } = "DESC";
}

public class GetPurchaseOrderByIdQuery : IRequest<PurchaseOrderDto?>
{
    public Guid Id { get; set; }
}

public class GetPurchaseOrdersByRequisitionQuery : IRequest<List<PurchaseOrderDto>>
{
    public Guid RequisitionId { get; set; }
}

public class GetPurchaseOrdersByVendorQuery : IRequest<List<PurchaseOrderDto>>
{
    public Guid VendorId { get; set; }
}

public class GetPurchaseOrderSummaryQuery : IRequest<PurchaseOrderSummaryDto>
{
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetPurchaseOrderByNumberQuery : IRequest<PurchaseOrderDto?>
{
    public string PurchaseOrderNumber { get; set; } = string.Empty;
}

public class GetPurchaseOrdersByPeriodQuery : IRequest<List<PurchaseOrderDto>>
{
    public Guid PeriodId { get; set; }
}

// ============================================================
// DTOs
// ============================================================

public class PurchaseOrderSummaryDto
{
    public int TotalOrders { get; set; }
    public int DraftCount { get; set; }
    public int SentCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int ShippedCount { get; set; }
    public int DeliveredCount { get; set; }
    public int CancelledCount { get; set; }
    public int PartiallyReceivedCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? PeriodName { get; set; }
}

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

// ============================================================
// PAGINATED QUERY
// ============================================================

public class GetPagedPurchaseOrdersQuery : IRequest<PagedResult<PurchaseOrderDto>>
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "OrderDate";
    public string? SortOrder { get; set; } = "DESC";
}

// ============================================================
// HANDLERS
// ============================================================

// ==================== GET ALL PURCHASE ORDERS ====================

public class GetAllPurchaseOrdersHandler : IRequestHandler<GetAllPurchaseOrdersQuery, List<PurchaseOrderDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllPurchaseOrdersHandler> _logger;

    public GetAllPurchaseOrdersHandler(ProcurementDbContext context, ILogger<GetAllPurchaseOrdersHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.PurchaseOrders
                            .Include(x => x.Lines)
                            .Include(x => x.Vendor)  // ✅ ADD THIS
                            .Include(x => x.Period)  // ✅ ADD THIS for period name
                            .Where(x => !x.IsDeleted)
                            .AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(x => x.Status == request.Status);

            if (request.VendorId.HasValue)
                query = query.Where(x => x.VendorId == request.VendorId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.OrderDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.OrderDate <= request.ToDate.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.PurchaseOrderNumber.ToLower().Contains(search) ||
                    (x.VendorName != null && x.VendorName.ToLower().Contains(search)) ||
                    (x.Description != null && x.Description.ToLower().Contains(search))
                );
            }

            var purchaseOrders = await query
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);

            return purchaseOrders.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all purchase orders");
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
        {
            return new PurchaseOrderDto
            {
                Id = po.Id,
                PurchaseOrderNumber = po.PurchaseOrderNumber,
                VendorId = po.VendorId,
                VendorName = po.Vendor?.Name ?? po.VendorName ?? "Unknown",  // ✅ Use Vendor.Name first
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                Status = po.Status,
                TotalAmount = po.TotalAmount,
                Currency = po.Currency,
                PaymentTerms = po.PaymentTerms,
                ShippingAddress = po.ShippingAddress,
                Description = po.Description,
                RequisitionId = po.RequisitionId,
                RequisitionNumber = po.RequisitionNumber,
                PeriodId = po.PeriodId,
              //  PeriodName = po.Period?.Name ?? po.PeriodName,  // ✅ Use Period.Name
                CreatedByUserId = po.CreatedByUserId,
                CreatedByUserName = po.CreatedByUserName,
                UpdatedByUserId = po.UpdatedByUserId,
                UpdatedByUserName = po.UpdatedByUserName,
                DateAdd = po.DateAdd,
                DateMod = po.DateMod,
                RowVersion = po.RowVersion ?? string.Empty,
                Lines = po.Lines.Select(l => new PurchaseOrderLineDto
                {
                    Id = l.Id,
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    TotalAmount = l.TotalAmount,
                    Discount = l.Discount,
                    TaxRate = l.TaxRate,
                    TaxAmount = l.TaxAmount,
                    UnitOfMeasure = l.UnitOfMeasure,
                    RequisitionLineId = l.RequisitionLineId,
                    PeriodId = l.PeriodId
                }).ToList()
        };
    }
}

// ==================== GET PAGINATED PURCHASE ORDERS ====================

public class GetPagedPurchaseOrdersHandler : IRequestHandler<GetPagedPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPagedPurchaseOrdersHandler> _logger;

    public GetPagedPurchaseOrdersHandler(ProcurementDbContext context, ILogger<GetPagedPurchaseOrdersHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<PurchaseOrderDto>> Handle(GetPagedPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.PurchaseOrders
                .Include(x => x.Lines)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(x => x.Status == request.Status);

            if (request.VendorId.HasValue)
                query = query.Where(x => x.VendorId == request.VendorId.Value);

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.OrderDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.OrderDate <= request.ToDate.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.PurchaseOrderNumber.ToLower().Contains(search) ||
                    (x.VendorName != null && x.VendorName.ToLower().Contains(search)) ||
                    (x.Description != null && x.Description.ToLower().Contains(search))
                );
            }

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "purchaseordernumber" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.PurchaseOrderNumber)
                    : query.OrderBy(x => x.PurchaseOrderNumber),
                "vendorname" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.VendorName)
                    : query.OrderBy(x => x.VendorName),
                "totalamount" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.TotalAmount)
                    : query.OrderBy(x => x.TotalAmount),
                "status" => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.Status)
                    : query.OrderBy(x => x.Status),
                _ => request.SortOrder?.ToUpper() == "DESC"
                    ? query.OrderByDescending(x => x.OrderDate)
                    : query.OrderBy(x => x.OrderDate)
            };

            // Apply pagination
            var purchaseOrders = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} purchase orders (Page {Page}/{TotalPages})",
                purchaseOrders.Count, request.Page, (int)Math.Ceiling((double)totalCount / request.PageSize));

            return new PagedResult<PurchaseOrderDto>
            {
                Data = purchaseOrders.Select(MapToDto).ToList(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged purchase orders");
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            VendorId = po.VendorId,
            VendorName = po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            SentDate = po.SentDate,
            SentBy = po.SentBy,
            ConfirmedDate = po.ConfirmedDate,
            ConfirmedBy = po.ConfirmedBy,
            PeriodId = po.PeriodId,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}

// ==================== GET PURCHASE ORDER BY ID ====================

// Queries/PurchaseOrderQueries.cs - GetPurchaseOrderByIdHandler

public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto?>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPurchaseOrderByIdHandler> _logger;

    public GetPurchaseOrderByIdHandler(ProcurementDbContext context, ILogger<GetPurchaseOrderByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var po = await _context.PurchaseOrders
                .Include(x => x.Lines)
                .Include(x => x.Vendor)  // ✅ ADD THIS - Include Vendor
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (po == null)
                return null;

            return MapToDto(po);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase order by ID: {Id}", request.Id);
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            VendorId = po.VendorId,
            VendorName = po.Vendor?.Name ?? po.VendorName ?? "Unknown",  // ✅ Use Vendor.Name first
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            Status = po.Status,
            TotalAmount = po.TotalAmount,
            Currency = po.Currency,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            Description = po.Description,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PeriodId = po.PeriodId,
            PeriodName = po.Period != null ? po.Period.Name : null,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            RowVersion = po.RowVersion ?? string.Empty,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}

// ==================== GET PURCHASE ORDER BY NUMBER ====================

public class GetPurchaseOrderByNumberHandler : IRequestHandler<GetPurchaseOrderByNumberQuery, PurchaseOrderDto?>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPurchaseOrderByNumberHandler> _logger;

    public GetPurchaseOrderByNumberHandler(ProcurementDbContext context, ILogger<GetPurchaseOrderByNumberHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PurchaseOrderDto?> Handle(GetPurchaseOrderByNumberQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var po = await _context.PurchaseOrders
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.PurchaseOrderNumber == request.PurchaseOrderNumber && !x.IsDeleted, cancellationToken);

            if (po == null)
                return null;

            return MapToDto(po);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase order by number: {Number}", request.PurchaseOrderNumber);
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            VendorId = po.VendorId,
            VendorName = po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            SentDate = po.SentDate,
            SentBy = po.SentBy,
            ConfirmedDate = po.ConfirmedDate,
            ConfirmedBy = po.ConfirmedBy,
            PeriodId = po.PeriodId,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}

// ==================== GET PURCHASE ORDERS BY REQUISITION ====================

public class GetPurchaseOrdersByRequisitionHandler : IRequestHandler<GetPurchaseOrdersByRequisitionQuery, List<PurchaseOrderDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPurchaseOrdersByRequisitionHandler> _logger;

    public GetPurchaseOrdersByRequisitionHandler(ProcurementDbContext context, ILogger<GetPurchaseOrdersByRequisitionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersByRequisitionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pos = await _context.PurchaseOrders
                .Include(x => x.Lines)
                .Where(x => x.RequisitionId == request.RequisitionId && !x.IsDeleted)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);

            return pos.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase orders by requisition: {RequisitionId}", request.RequisitionId);
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            VendorId = po.VendorId,
            VendorName = po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            SentDate = po.SentDate,
            SentBy = po.SentBy,
            ConfirmedDate = po.ConfirmedDate,
            ConfirmedBy = po.ConfirmedBy,
            PeriodId = po.PeriodId,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}

// ==================== GET PURCHASE ORDERS BY VENDOR ====================

public class GetPurchaseOrdersByVendorHandler : IRequestHandler<GetPurchaseOrdersByVendorQuery, List<PurchaseOrderDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPurchaseOrdersByVendorHandler> _logger;

    public GetPurchaseOrdersByVendorHandler(ProcurementDbContext context, ILogger<GetPurchaseOrdersByVendorHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersByVendorQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pos = await _context.PurchaseOrders
                .Include(x => x.Lines)
                .Where(x => x.VendorId == request.VendorId && !x.IsDeleted)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);

            return pos.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase orders by vendor: {VendorId}", request.VendorId);
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            VendorId = po.VendorId,
            VendorName = po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            SentDate = po.SentDate,
            SentBy = po.SentBy,
            ConfirmedDate = po.ConfirmedDate,
            ConfirmedBy = po.ConfirmedBy,
            PeriodId = po.PeriodId,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}

// ==================== GET PURCHASE ORDERS BY PERIOD ====================

public class GetPurchaseOrdersByPeriodHandler : IRequestHandler<GetPurchaseOrdersByPeriodQuery, List<PurchaseOrderDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPurchaseOrdersByPeriodHandler> _logger;

    public GetPurchaseOrdersByPeriodHandler(ProcurementDbContext context, ILogger<GetPurchaseOrdersByPeriodHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersByPeriodQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pos = await _context.PurchaseOrders
                .Include(x => x.Lines)
                .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);

            return pos.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase orders by period: {PeriodId}", request.PeriodId);
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            VendorId = po.VendorId,
            VendorName = po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            SentDate = po.SentDate,
            SentBy = po.SentBy,
            ConfirmedDate = po.ConfirmedDate,
            ConfirmedBy = po.ConfirmedBy,
            PeriodId = po.PeriodId,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}
// Queries/PurchaseOrderQueries.cs - Add this at the end

public class ExportPurchaseOrdersQuery : IRequest<List<PurchaseOrderDto>>
{
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
}

public class ExportPurchaseOrdersHandler : IRequestHandler<ExportPurchaseOrdersQuery, List<PurchaseOrderDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<ExportPurchaseOrdersHandler> _logger;

    public ExportPurchaseOrdersHandler(ProcurementDbContext context, ILogger<ExportPurchaseOrdersHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PurchaseOrderDto>> Handle(ExportPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.PurchaseOrders
                .Include(x => x.Lines)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(x => x.Status == request.Status);

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.OrderDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.OrderDate <= request.ToDate.Value);

            var orders = await query
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync(cancellationToken);

            return orders.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting purchase orders");
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            VendorId = po.VendorId,
            VendorName = po.VendorName,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            Status = po.Status,
            TotalAmount = po.TotalAmount,
            Currency = po.Currency,
            PaymentTerms = po.PaymentTerms,
            ShippingAddress = po.ShippingAddress,
            Description = po.Description,
            RequisitionId = po.RequisitionId,
            RequisitionNumber = po.RequisitionNumber,
            PeriodId = po.PeriodId,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            CreatedByUserId = po.CreatedByUserId,
            CreatedByUserName = po.CreatedByUserName,
            UpdatedByUserId = po.UpdatedByUserId,
            UpdatedByUserName = po.UpdatedByUserName,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            RowVersion = po.RowVersion ?? string.Empty,
            Lines = po.Lines.Select(l => new PurchaseOrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TotalAmount = l.TotalAmount,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TaxAmount = l.TaxAmount,
                UnitOfMeasure = l.UnitOfMeasure,
                RequisitionLineId = l.RequisitionLineId,
                PeriodId = l.PeriodId
            }).ToList()
        };
    }
}
// ==================== GET PURCHASE ORDER SUMMARY ====================

public class GetPurchaseOrderSummaryHandler : IRequestHandler<GetPurchaseOrderSummaryQuery, PurchaseOrderSummaryDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetPurchaseOrderSummaryHandler> _logger;

    public GetPurchaseOrderSummaryHandler(ProcurementDbContext context, ILogger<GetPurchaseOrderSummaryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PurchaseOrderSummaryDto> Handle(GetPurchaseOrderSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.PurchaseOrders
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.OrderDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.OrderDate <= request.ToDate.Value);

            var purchaseOrders = await query.ToListAsync(cancellationToken);

            var totalOrders = purchaseOrders.Count;
            var totalAmount = purchaseOrders.Sum(x => x.TotalAmount);

            return new PurchaseOrderSummaryDto
            {
                TotalOrders = totalOrders,
                DraftCount = purchaseOrders.Count(x => x.Status == "Draft"),
                SentCount = purchaseOrders.Count(x => x.Status == "Sent"),
                ConfirmedCount = purchaseOrders.Count(x => x.Status == "Confirmed"),
                ShippedCount = purchaseOrders.Count(x => x.Status == "Shipped"),
                DeliveredCount = purchaseOrders.Count(x => x.Status == "Delivered"),
                CancelledCount = purchaseOrders.Count(x => x.Status == "Cancelled"),
                PartiallyReceivedCount = purchaseOrders.Count(x => x.Status == "PartiallyReceived"),
                TotalAmount = totalAmount,
                AverageAmount = totalOrders > 0 ? totalAmount / totalOrders : 0,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting purchase order summary");
            throw;
        }
    }
}