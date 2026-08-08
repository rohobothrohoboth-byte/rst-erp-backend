using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Queries;


public class GetAllGoodsReceiptNotesQuery : IRequest<List<GoodsReceiptNoteDto>>
{
    public string? Status { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetGoodsReceiptNoteByIdQuery : IRequest<GoodsReceiptNoteDto>
{
    public Guid Id { get; set; }
}

public class GetGoodsReceiptNotesByPurchaseOrderQuery : IRequest<List<GoodsReceiptNoteDto>>
{
    public Guid PurchaseOrderId { get; set; }
}

public class GetAllGoodsReceiptNotesQueryHandler
    : IRequestHandler<GetAllGoodsReceiptNotesQuery, List<GoodsReceiptNoteDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetAllGoodsReceiptNotesQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllGoodsReceiptNotesQueryHandler(
        ProcurementDbContext context,
        ILogger<GetAllGoodsReceiptNotesQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<GoodsReceiptNoteDto>> Handle(
        GetAllGoodsReceiptNotesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching all GRNs with filters: {@Filters}", request);

            var query = _context.GoodsReceiptNotes
                .Include(g => g.Items)
                .Where(g => !g.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(g => g.Status == request.Status);
            }

            if (request.PurchaseOrderId.HasValue)
            {
                query = query.Where(g => g.PurchaseOrderId == request.PurchaseOrderId.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(g => g.ReceivedDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                var toDate = request.ToDate.Value.Date.AddDays(1);
                query = query.Where(g => g.ReceivedDate < toDate);
            }

            var grns = await query
                .OrderByDescending(g => g.ReceivedDate)
                .Select(g => new GoodsReceiptNoteDto
                {
                    Id = g.Id,
                    GrnNumber = g.GrnNumber,
                    PurchaseOrderId = g.PurchaseOrderId,
                    PurchaseOrderNumber = g.PurchaseOrderNumber,
                    DeliveryNoteNumber = g.DeliveryNoteNumber,
                    ReceivedDate = g.ReceivedDate,
                    WarehouseId = g.WarehouseId,
                    WarehouseName = g.WarehouseName,
                    ReceivedBy = g.ReceivedBy,
                    InspectedBy = g.InspectedBy,
                    Status = g.Status,
                    TotalReceived = g.TotalReceived,
                    TotalAccepted = g.TotalAccepted,
                    TotalRejected = g.TotalRejected,
                    Notes = g.Notes,
                    CompletedDate = g.CompletedDate,
                    DateAdd = g.DateAdd,
                    DateMod = g.DateMod,
                    RowVersion = g.RowVersion,
                    Items = g.Items.Select(i => new GoodsReceiptItemDto
                    {
                        Id = i.Id,
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        Description = i.Description,
                        QuantityReceived = i.QuantityReceived,
                        QuantityAccepted = i.QuantityAccepted,
                        QuantityRejected = i.QuantityRejected,
                        Condition = i.Condition,
                        RejectionReason = i.RejectionReason,
                        UnitPrice = i.UnitPrice,
                        TotalAmount = i.TotalAmount
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched {Count} GRNs", grns.Count);
            return grns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GRNs");
            throw;
        }
    }
}

public class GetGoodsReceiptNoteByIdQueryHandler
    : IRequestHandler<GetGoodsReceiptNoteByIdQuery, GoodsReceiptNoteDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetGoodsReceiptNoteByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetGoodsReceiptNoteByIdQueryHandler(
        ProcurementDbContext context,
        ILogger<GetGoodsReceiptNoteByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<GoodsReceiptNoteDto> Handle(
        GetGoodsReceiptNoteByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching GRN by ID: {Id}", request.Id);

            // Check cache first
            var cacheKey = $"grn_{request.Id}";
            var cached = await _cache.GetAsync<GoodsReceiptNoteDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("GRN found in cache: {Id}", request.Id);
                return cached;
            }

            var grn = await _context.GoodsReceiptNotes
                .Include(g => g.Items)
                .Where(g => g.Id == request.Id && !g.IsDeleted)
                .Select(g => new GoodsReceiptNoteDto
                {
                    Id = g.Id,
                    GrnNumber = g.GrnNumber,
                    PurchaseOrderId = g.PurchaseOrderId,
                    PurchaseOrderNumber = g.PurchaseOrderNumber,
                    DeliveryNoteNumber = g.DeliveryNoteNumber,
                    ReceivedDate = g.ReceivedDate,
                    WarehouseId = g.WarehouseId,
                    WarehouseName = g.WarehouseName,
                    ReceivedBy = g.ReceivedBy,
                    InspectedBy = g.InspectedBy,
                    Status = g.Status,
                    TotalReceived = g.TotalReceived,
                    TotalAccepted = g.TotalAccepted,
                    TotalRejected = g.TotalRejected,
                    Notes = g.Notes,
                    CompletedDate = g.CompletedDate,
                    DateAdd = g.DateAdd,
                    DateMod = g.DateMod,
                    RowVersion = g.RowVersion,
                    Items = g.Items.Select(i => new GoodsReceiptItemDto
                    {
                        Id = i.Id,
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        Description = i.Description,
                        QuantityReceived = i.QuantityReceived,
                        QuantityAccepted = i.QuantityAccepted,
                        QuantityRejected = i.QuantityRejected,
                        Condition = i.Condition,
                        RejectionReason = i.RejectionReason,
                        UnitPrice = i.UnitPrice,
                        TotalAmount = i.TotalAmount
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (grn == null)
            {
                _logger.LogWarning("GRN not found: {Id}", request.Id);
                throw new KeyNotFoundException($"GRN with ID '{request.Id}' not found");
            }

            // Cache the result
            await _cache.SetAsync(cacheKey, grn, TimeSpan.FromMinutes(15), cancellationToken);

            _logger.LogInformation("Fetched GRN: {GrnNumber}", grn.GrnNumber);
            return grn;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GRN {Id}", request.Id);
            throw;
        }
    }
}

public class GetGoodsReceiptNotesByPurchaseOrderQueryHandler
    : IRequestHandler<GetGoodsReceiptNotesByPurchaseOrderQuery, List<GoodsReceiptNoteDto>>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<GetGoodsReceiptNotesByPurchaseOrderQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetGoodsReceiptNotesByPurchaseOrderQueryHandler(
        ProcurementDbContext context,
        ILogger<GetGoodsReceiptNotesByPurchaseOrderQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<GoodsReceiptNoteDto>> Handle(
        GetGoodsReceiptNotesByPurchaseOrderQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching GRNs for PO: {PurchaseOrderId}", request.PurchaseOrderId);

            var cacheKey = $"grns_po_{request.PurchaseOrderId}";
            var cached = await _cache.GetAsync<List<GoodsReceiptNoteDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("GRNs found in cache for PO: {PurchaseOrderId}", request.PurchaseOrderId);
                return cached;
            }

            var grns = await _context.GoodsReceiptNotes
                .Include(g => g.Items)
                .Where(g => g.PurchaseOrderId == request.PurchaseOrderId && !g.IsDeleted)
                .OrderByDescending(g => g.ReceivedDate)
                .Select(g => new GoodsReceiptNoteDto
                {
                    Id = g.Id,
                    GrnNumber = g.GrnNumber,
                    PurchaseOrderId = g.PurchaseOrderId,
                    PurchaseOrderNumber = g.PurchaseOrderNumber,
                    DeliveryNoteNumber = g.DeliveryNoteNumber,
                    ReceivedDate = g.ReceivedDate,
                    WarehouseId = g.WarehouseId,
                    WarehouseName = g.WarehouseName,
                    ReceivedBy = g.ReceivedBy,
                    InspectedBy = g.InspectedBy,
                    Status = g.Status,
                    TotalReceived = g.TotalReceived,
                    TotalAccepted = g.TotalAccepted,
                    TotalRejected = g.TotalRejected,
                    Notes = g.Notes,
                    CompletedDate = g.CompletedDate,
                    DateAdd = g.DateAdd,
                    DateMod = g.DateMod,
                    RowVersion = g.RowVersion,
                    Items = g.Items.Select(i => new GoodsReceiptItemDto
                    {
                        Id = i.Id,
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        Description = i.Description,
                        QuantityReceived = i.QuantityReceived,
                        QuantityAccepted = i.QuantityAccepted,
                        QuantityRejected = i.QuantityRejected,
                        Condition = i.Condition,
                        RejectionReason = i.RejectionReason,
                        UnitPrice = i.UnitPrice,
                        TotalAmount = i.TotalAmount
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, grns, TimeSpan.FromMinutes(15), cancellationToken);

            _logger.LogInformation("Fetched {Count} GRNs for PO: {PurchaseOrderId}", grns.Count, request.PurchaseOrderId);
            return grns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GRNs for PO {PurchaseOrderId}", request.PurchaseOrderId);
            throw;
        }
    }
}