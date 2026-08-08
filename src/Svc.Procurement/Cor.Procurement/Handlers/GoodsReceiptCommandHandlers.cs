using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Procurement.Commands;

public class CreateGoodsReceiptNoteCommandHandler : IRequestHandler<CreateGoodsReceiptNoteCommand, GoodsReceiptNoteDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreateGoodsReceiptNoteCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreateGoodsReceiptNoteCommandHandler(
        ProcurementDbContext context,
        ILogger<CreateGoodsReceiptNoteCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<GoodsReceiptNoteDto> Handle(CreateGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new Goods Receipt Note");

            if (request.CreateDto == null)
                throw new ArgumentException("GRN data is required");

            if (request.CreateDto.Items == null || !request.CreateDto.Items.Any())
                throw new ArgumentException("At least one item is required");

            // Validate Purchase Order exists
            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == request.CreateDto.PurchaseOrderId && !po.IsDeleted, cancellationToken);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase Order with ID '{request.CreateDto.PurchaseOrderId}' not found");

            var grnNumber = GenerateGrnNumber();

            var grn = new GoodsReceiptNote
            {
                Id = Guid.NewGuid(),
                GrnNumber = grnNumber,
                PurchaseOrderId = request.CreateDto.PurchaseOrderId,
                PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
                DeliveryNoteNumber = request.CreateDto.DeliveryNoteNumber,
                ReceivedDate = request.CreateDto.ReceivedDate != DateTime.MinValue ? request.CreateDto.ReceivedDate : DateTime.UtcNow,
                WarehouseId = request.CreateDto.WarehouseId,
                WarehouseName = request.CreateDto.WarehouseName,
                ReceivedBy = request.CreateDto.ReceivedBy,
                InspectedBy = request.CreateDto.InspectedBy,
                Status = "Draft",
                Notes = request.CreateDto.Notes,
                TotalReceived = 0,
                TotalAccepted = 0,
                TotalRejected = 0,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            decimal totalReceived = 0;
            decimal totalAccepted = 0;
            decimal totalRejected = 0;

            foreach (var itemDto in request.CreateDto.Items)
            {
                var item = new GoodsReceiptItem
                {
                    Id = Guid.NewGuid(),
                    GoodsReceiptNoteId = grn.Id,
                    PurchaseOrderItemId = itemDto.PurchaseOrderItemId,
                    Description = itemDto.Description,
                    QuantityReceived = itemDto.QuantityReceived,
                    QuantityAccepted = itemDto.QuantityAccepted,
                    QuantityRejected = itemDto.QuantityRejected,
                    Condition = itemDto.Condition ?? "Good",
                    RejectionReason = itemDto.RejectionReason,
                    UnitPrice = itemDto.UnitPrice,
                    TotalAmount = (itemDto.QuantityAccepted + itemDto.QuantityRejected) * (itemDto.UnitPrice ?? 0),
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                grn.Items.Add(item);
                totalReceived += itemDto.QuantityReceived;
                totalAccepted += itemDto.QuantityAccepted;
                totalRejected += itemDto.QuantityRejected;
            }

            grn.TotalReceived = totalReceived;
            grn.TotalAccepted = totalAccepted;
            grn.TotalRejected = totalRejected;

            await _context.GoodsReceiptNotes.AddAsync(grn, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync("grns_all", cancellationToken);

            _logger.LogInformation("GRN created successfully: {GrnNumber}", grnNumber);

            return MapToDto(grn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating GRN");
            throw;
        }
    }

    private string GenerateGrnNumber()
    {
        var today = DateTime.UtcNow;
        return $"GRN-{today:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private GoodsReceiptNoteDto MapToDto(GoodsReceiptNote grn)
    {
        return new GoodsReceiptNoteDto
        {
            Id = grn.Id,
            GrnNumber = grn.GrnNumber,
            PurchaseOrderId = grn.PurchaseOrderId,
            PurchaseOrderNumber = grn.PurchaseOrderNumber,
            DeliveryNoteNumber = grn.DeliveryNoteNumber,
            ReceivedDate = grn.ReceivedDate,
            WarehouseId = grn.WarehouseId,
            WarehouseName = grn.WarehouseName,
            ReceivedBy = grn.ReceivedBy,
            InspectedBy = grn.InspectedBy,
            Status = grn.Status,
            TotalReceived = grn.TotalReceived,
            TotalAccepted = grn.TotalAccepted,
            TotalRejected = grn.TotalRejected,
            Notes = grn.Notes,
            CompletedDate = grn.CompletedDate,
            DateAdd = grn.DateAdd,
            DateMod = grn.DateMod,
            RowVersion = grn.RowVersion,
            Items = grn.Items.Select(i => new GoodsReceiptItemDto
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
        };
    }
}

public class UpdateGoodsReceiptNoteCommandHandler : IRequestHandler<UpdateGoodsReceiptNoteCommand, GoodsReceiptNoteDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<UpdateGoodsReceiptNoteCommandHandler> _logger;
    private readonly ICacheService _cache;

    public UpdateGoodsReceiptNoteCommandHandler(
        ProcurementDbContext context,
        ILogger<UpdateGoodsReceiptNoteCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<GoodsReceiptNoteDto> Handle(UpdateGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating GRN: {GrnId}", request.UpdateDto.Id);

            var grn = await _context.GoodsReceiptNotes
                .Include(g => g.Items)
                .FirstOrDefaultAsync(g => g.Id == request.UpdateDto.Id && !g.IsDeleted, cancellationToken);

            if (grn == null)
                throw new KeyNotFoundException($"GRN with ID '{request.UpdateDto.Id}' not found");

            if (grn.Status == "Completed")
                throw new InvalidOperationException("Cannot update a completed GRN");

            // Update fields
            grn.DeliveryNoteNumber = request.UpdateDto.DeliveryNoteNumber ?? grn.DeliveryNoteNumber;
            grn.ReceivedDate = request.UpdateDto.ReceivedDate != DateTime.MinValue ? request.UpdateDto.ReceivedDate : grn.ReceivedDate;
            grn.WarehouseId = request.UpdateDto.WarehouseId;
            grn.WarehouseName = request.UpdateDto.WarehouseName;
            grn.ReceivedBy = request.UpdateDto.ReceivedBy ?? grn.ReceivedBy;
            grn.InspectedBy = request.UpdateDto.InspectedBy ?? grn.InspectedBy;
            grn.Notes = request.UpdateDto.Notes ?? grn.Notes;
            grn.DateMod = DateTime.UtcNow;

            // Update items
            if (request.UpdateDto.Items != null && request.UpdateDto.Items.Any())
            {
                // Remove existing items
                _context.GoodsReceiptItems.RemoveRange(grn.Items);
                grn.Items.Clear();

                decimal totalReceived = 0;
                decimal totalAccepted = 0;
                decimal totalRejected = 0;

                foreach (var itemDto in request.UpdateDto.Items)
                {
                    var item = new GoodsReceiptItem
                    {
                        Id = itemDto.Id ?? Guid.NewGuid(),
                        GoodsReceiptNoteId = grn.Id,
                        PurchaseOrderItemId = itemDto.PurchaseOrderItemId,
                        Description = itemDto.Description,
                        QuantityReceived = itemDto.QuantityReceived,
                        QuantityAccepted = itemDto.QuantityAccepted,
                        QuantityRejected = itemDto.QuantityRejected,
                        Condition = itemDto.Condition ?? "Good",
                        RejectionReason = itemDto.RejectionReason,
                        UnitPrice = itemDto.UnitPrice,
                        TotalAmount = (itemDto.QuantityAccepted + itemDto.QuantityRejected) * (itemDto.UnitPrice ?? 0),
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    grn.Items.Add(item);
                    totalReceived += itemDto.QuantityReceived;
                    totalAccepted += itemDto.QuantityAccepted;
                    totalRejected += itemDto.QuantityRejected;
                }

                grn.TotalReceived = totalReceived;
                grn.TotalAccepted = totalAccepted;
                grn.TotalRejected = totalRejected;
            }

            grn.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"grn_{grn.Id}", cancellationToken);
            await _cache.RemoveAsync("grns_all", cancellationToken);

            _logger.LogInformation("GRN updated successfully: {GrnNumber}", grn.GrnNumber);

            return MapToDto(grn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating GRN");
            throw;
        }
    }

    private GoodsReceiptNoteDto MapToDto(GoodsReceiptNote grn)
    {
        return new GoodsReceiptNoteDto
        {
            Id = grn.Id,
            GrnNumber = grn.GrnNumber,
            PurchaseOrderId = grn.PurchaseOrderId,
            PurchaseOrderNumber = grn.PurchaseOrderNumber,
            DeliveryNoteNumber = grn.DeliveryNoteNumber,
            ReceivedDate = grn.ReceivedDate,
            WarehouseId = grn.WarehouseId,
            WarehouseName = grn.WarehouseName,
            ReceivedBy = grn.ReceivedBy,
            InspectedBy = grn.InspectedBy,
            Status = grn.Status,
            TotalReceived = grn.TotalReceived,
            TotalAccepted = grn.TotalAccepted,
            TotalRejected = grn.TotalRejected,
            Notes = grn.Notes,
            CompletedDate = grn.CompletedDate,
            DateAdd = grn.DateAdd,
            DateMod = grn.DateMod,
            RowVersion = grn.RowVersion,
            Items = grn.Items.Select(i => new GoodsReceiptItemDto
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
        };
    }
}

public class DeleteGoodsReceiptNoteCommandHandler : IRequestHandler<DeleteGoodsReceiptNoteCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DeleteGoodsReceiptNoteCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeleteGoodsReceiptNoteCommandHandler(
        ProcurementDbContext context,
        ILogger<DeleteGoodsReceiptNoteCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting GRN: {GrnId}", request.Id);

            var grn = await _context.GoodsReceiptNotes
                .FirstOrDefaultAsync(g => g.Id == request.Id && !g.IsDeleted, cancellationToken);

            if (grn == null)
                return false;

            if (grn.Status == "Completed")
                throw new InvalidOperationException("Cannot delete a completed GRN");

            grn.IsDeleted = true;
            grn.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"grn_{request.Id}", cancellationToken);
            await _cache.RemoveAsync("grns_all", cancellationToken);

            _logger.LogInformation("GRN deleted successfully: {GrnNumber}", grn.GrnNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting GRN");
            throw;
        }
    }
}

public class CompleteGoodsReceiptNoteCommandHandler : IRequestHandler<CompleteGoodsReceiptNoteCommand, GoodsReceiptNoteDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CompleteGoodsReceiptNoteCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CompleteGoodsReceiptNoteCommandHandler(
        ProcurementDbContext context,
        ILogger<CompleteGoodsReceiptNoteCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<GoodsReceiptNoteDto> Handle(CompleteGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Completing GRN: {GrnId}", request.Id);

            var grn = await _context.GoodsReceiptNotes
                .Include(g => g.Items)
                .FirstOrDefaultAsync(g => g.Id == request.Id && !g.IsDeleted, cancellationToken);

            if (grn == null)
                throw new KeyNotFoundException($"GRN with ID '{request.Id}' not found");

            if (grn.Status == "Completed")
            {
                _logger.LogInformation("GRN already completed: {GrnNumber}", grn.GrnNumber);
                return MapToDto(grn);
            }

            if (grn.TotalReceived <= 0)
                throw new InvalidOperationException("Cannot complete GRN with zero items received");

            grn.Status = "Completed";
            grn.CompletedDate = DateTime.UtcNow;
            grn.DateMod = DateTime.UtcNow;
            grn.UpdateRowVersion();

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync($"grn_{grn.Id}", cancellationToken);
            await _cache.RemoveAsync("grns_all", cancellationToken);

            _logger.LogInformation("GRN completed successfully: {GrnNumber}", grn.GrnNumber);

            return MapToDto(grn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing GRN");
            throw;
        }
    }

    private GoodsReceiptNoteDto MapToDto(GoodsReceiptNote grn)
    {
        return new GoodsReceiptNoteDto
        {
            Id = grn.Id,
            GrnNumber = grn.GrnNumber,
            PurchaseOrderId = grn.PurchaseOrderId,
            PurchaseOrderNumber = grn.PurchaseOrderNumber,
            DeliveryNoteNumber = grn.DeliveryNoteNumber,
            ReceivedDate = grn.ReceivedDate,
            WarehouseId = grn.WarehouseId,
            WarehouseName = grn.WarehouseName,
            ReceivedBy = grn.ReceivedBy,
            InspectedBy = grn.InspectedBy,
            Status = grn.Status,
            TotalReceived = grn.TotalReceived,
            TotalAccepted = grn.TotalAccepted,
            TotalRejected = grn.TotalRejected,
            Notes = grn.Notes,
            CompletedDate = grn.CompletedDate,
            DateAdd = grn.DateAdd,
            DateMod = grn.DateMod,
            RowVersion = grn.RowVersion,
            Items = grn.Items.Select(i => new GoodsReceiptItemDto
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
        };
    }
}