// Commands/PurchaseOrderCommands.cs
using MediatR;
using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Helpers;
namespace Cor.Procurement.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class CreatePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public CreatePurchaseOrderDto CreateDto { get; set; } = new();
}

public class UpdatePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public UpdatePurchaseOrderDto UpdateDto { get; set; } = new();
}

public class DeletePurchaseOrderCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class UpdatePurchaseOrderStatusCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ReceivePurchaseOrderCommand : IRequest<PurchaseOrderDto>
{
    public Guid Id { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
}

public class CreatePurchaseOrderFromRequisitionCommand : IRequest<PurchaseOrderDto>
{
    public Guid RequisitionId { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreatePurchaseOrderCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreatePurchaseOrderCommandHandler(
        ProcurementDbContext context,
        ILogger<CreatePurchaseOrderCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new purchase order");

            if (request.CreateDto == null)
                throw new ArgumentException("Purchase order data is required");

            if (request.CreateDto.Lines == null || !request.CreateDto.Lines.Any())
                throw new ArgumentException("At least one line item is required");

            // Create purchase order
            var purchaseOrder = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                PurchaseOrderNumber = request.CreateDto.PurchaseOrderNumber ?? GeneratePurchaseOrderNumber(),
                VendorId = request.CreateDto.VendorId,
                VendorName = request.CreateDto.VendorName,
                OrderDate = request.CreateDto.OrderDate,
                ExpectedDeliveryDate = request.CreateDto.ExpectedDeliveryDate,
                Status = request.CreateDto.Status ?? "Draft",
                TotalAmount = request.CreateDto.TotalAmount,
                Currency = request.CreateDto.Currency ?? "USD",
                PaymentTerms = request.CreateDto.PaymentTerms ?? "Net 30",
                ShippingAddress = request.CreateDto.ShippingAddress,
                Description = request.CreateDto.Description,
                RequisitionId = request.CreateDto.RequisitionId,
                RequisitionNumber = request.CreateDto.RequisitionNumber,
                PeriodId = request.CreateDto.PeriodId,
                CreatedByUserId = request.CreateDto.CreatedByUserId,
                CreatedByUserName = request.CreateDto.CreatedByUserName,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            // Add line items
            foreach (var lineDto in request.CreateDto.Lines)
            {
                var line = new PurchaseOrderLine
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.Id,
                    Description = lineDto.Description,
                    Quantity = lineDto.Quantity,
                    UnitPrice = lineDto.UnitPrice,
                    TotalAmount = lineDto.Quantity * lineDto.UnitPrice,
                    Discount = lineDto.Discount ?? 0,
                    TaxRate = lineDto.TaxRate ?? 0,
                    TaxAmount = lineDto.TaxAmount ?? 0,
                    UnitOfMeasure = lineDto.UnitOfMeasure ?? "Each",
                    RequisitionLineId = lineDto.RequisitionLineId,
                    PeriodId = purchaseOrder.PeriodId,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                purchaseOrder.Lines.Add(line);
            }

            await _context.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync("purchaseorders_all", cancellationToken);

            _logger.LogInformation("Purchase order created successfully: {PurchaseOrderNumber}", purchaseOrder.PurchaseOrderNumber);

            return MapToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order");
            throw;
        }
    }

    private string GeneratePurchaseOrderNumber()
    {
        return $"PO-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            VendorId = purchaseOrder.VendorId,
            VendorName = purchaseOrder.VendorName,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            Status = purchaseOrder.Status,
            TotalAmount = purchaseOrder.TotalAmount,
            Currency = purchaseOrder.Currency,
            PaymentTerms = purchaseOrder.PaymentTerms,
            ShippingAddress = purchaseOrder.ShippingAddress,
            Description = purchaseOrder.Description,
            RequisitionId = purchaseOrder.RequisitionId,
            RequisitionNumber = purchaseOrder.RequisitionNumber,
            PeriodId = purchaseOrder.PeriodId,
            CreatedByUserId = purchaseOrder.CreatedByUserId,
            CreatedByUserName = purchaseOrder.CreatedByUserName,
            DateAdd = purchaseOrder.DateAdd,
            DateMod = purchaseOrder.DateMod,
            RowVersion = purchaseOrder.RowVersion ?? string.Empty,
            Lines = purchaseOrder.Lines.Select(l => new PurchaseOrderLineDto
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

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<UpdatePurchaseOrderCommandHandler> _logger;
    private readonly ICacheService _cache;

    public UpdatePurchaseOrderCommandHandler(
        ProcurementDbContext context,
        ILogger<UpdatePurchaseOrderCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<PurchaseOrderDto> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating purchase order: {PurchaseOrderId}", request.UpdateDto.Id);

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Lines)
                .FirstOrDefaultAsync(po => po.Id == request.UpdateDto.Id && !po.IsDeleted, cancellationToken);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID '{request.UpdateDto.Id}' not found");

            if (purchaseOrder.Status != "Draft")
                throw new InvalidOperationException($"Cannot update a purchase order with status '{purchaseOrder.Status}'");

            // Update fields
            purchaseOrder.VendorId = request.UpdateDto.VendorId ?? purchaseOrder.VendorId;
            purchaseOrder.VendorName = request.UpdateDto.VendorName ?? purchaseOrder.VendorName;
            purchaseOrder.ExpectedDeliveryDate = request.UpdateDto.ExpectedDeliveryDate ?? purchaseOrder.ExpectedDeliveryDate;
            purchaseOrder.PaymentTerms = request.UpdateDto.PaymentTerms ?? purchaseOrder.PaymentTerms;
            purchaseOrder.ShippingAddress = request.UpdateDto.ShippingAddress ?? purchaseOrder.ShippingAddress;
            purchaseOrder.Description = request.UpdateDto.Description ?? purchaseOrder.Description;
            purchaseOrder.RequisitionId = request.UpdateDto.RequisitionId ?? purchaseOrder.RequisitionId;
            purchaseOrder.RequisitionNumber = request.UpdateDto.RequisitionNumber ?? purchaseOrder.RequisitionNumber;
            purchaseOrder.PeriodId = request.UpdateDto.PeriodId ?? purchaseOrder.PeriodId;
            purchaseOrder.UpdatedByUserId = request.UpdateDto.UpdatedByUserId;
            purchaseOrder.UpdatedByUserName = request.UpdateDto.UpdatedByUserName;
            purchaseOrder.DateMod = DateTime.UtcNow;

            // Update lines if provided
            if (request.UpdateDto.Lines != null && request.UpdateDto.Lines.Any())
            {
                // Remove existing lines
                _context.PurchaseOrderLines.RemoveRange(purchaseOrder.Lines);
                purchaseOrder.Lines.Clear();

                // Add new lines
                foreach (var lineDto in request.UpdateDto.Lines)
                {
                    var line = new PurchaseOrderLine
                    {
                        Id = lineDto.Id ?? Guid.NewGuid(),
                        PurchaseOrderId = purchaseOrder.Id,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        TotalAmount = lineDto.Quantity * lineDto.UnitPrice,
                        Discount = lineDto.Discount ?? 0,
                        TaxRate = lineDto.TaxRate ?? 0,
                        TaxAmount = lineDto.TaxAmount ?? 0,
                        UnitOfMeasure = lineDto.UnitOfMeasure ?? "Each",
                        RequisitionLineId = lineDto.RequisitionLineId,
                        PeriodId = purchaseOrder.PeriodId,
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    purchaseOrder.Lines.Add(line);
                }

                purchaseOrder.TotalAmount = purchaseOrder.Lines.Sum(l => l.TotalAmount);
            }

            purchaseOrder.RowVersion = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync($"purchaseorder_{purchaseOrder.Id}", cancellationToken);
            await _cache.RemoveAsync("purchaseorders_all", cancellationToken);

            _logger.LogInformation("Purchase order updated successfully: {PurchaseOrderNumber}", purchaseOrder.PurchaseOrderNumber);

            return MapToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating purchase order");
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            VendorId = purchaseOrder.VendorId,
            VendorName = purchaseOrder.VendorName,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            Status = purchaseOrder.Status,
            TotalAmount = purchaseOrder.TotalAmount,
            Currency = purchaseOrder.Currency,
            PaymentTerms = purchaseOrder.PaymentTerms,
            ShippingAddress = purchaseOrder.ShippingAddress,
            Description = purchaseOrder.Description,
            RequisitionId = purchaseOrder.RequisitionId,
            RequisitionNumber = purchaseOrder.RequisitionNumber,
            PeriodId = purchaseOrder.PeriodId,
            UpdatedByUserId = purchaseOrder.UpdatedByUserId,
            UpdatedByUserName = purchaseOrder.UpdatedByUserName,
            DateAdd = purchaseOrder.DateAdd,
            DateMod = purchaseOrder.DateMod,
            RowVersion = purchaseOrder.RowVersion ?? string.Empty,
            Lines = purchaseOrder.Lines.Select(l => new PurchaseOrderLineDto
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

public class DeletePurchaseOrderCommandHandler : IRequestHandler<DeletePurchaseOrderCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<DeletePurchaseOrderCommandHandler> _logger;
    private readonly ICacheService _cache;

    public DeletePurchaseOrderCommandHandler(
        ProcurementDbContext context,
        ILogger<DeletePurchaseOrderCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting purchase order: {PurchaseOrderId}", request.Id);

            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == request.Id && !po.IsDeleted, cancellationToken);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID '{request.Id}' not found");

            if (purchaseOrder.Status != "Draft")
                throw new InvalidOperationException($"Cannot delete a purchase order with status '{purchaseOrder.Status}'");

            purchaseOrder.IsDeleted = true;
            purchaseOrder.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync($"purchaseorder_{request.Id}", cancellationToken);
            await _cache.RemoveAsync("purchaseorders_all", cancellationToken);

            _logger.LogInformation("Purchase order deleted successfully: {PurchaseOrderId}", request.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting purchase order");
            throw;
        }
    }
}

public class UpdatePurchaseOrderStatusCommandHandler : IRequestHandler<UpdatePurchaseOrderStatusCommand, bool>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<UpdatePurchaseOrderStatusCommandHandler> _logger;
    private readonly ICacheService _cache;

    public UpdatePurchaseOrderStatusCommandHandler(
        ProcurementDbContext context,
        ILogger<UpdatePurchaseOrderStatusCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdatePurchaseOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating purchase order status: {PurchaseOrderId} to {Status}",
                request.Id, request.Status);

            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == request.Id && !po.IsDeleted, cancellationToken);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID '{request.Id}' not found");

            // Validate status transition
            ValidateStatusTransition(purchaseOrder.Status, request.Status);

            // Update status
            purchaseOrder.Status = request.Status;
            purchaseOrder.DateMod = DateTime.UtcNow;

            // Handle specific status changes
            switch (request.Status)
            {
                case "Sent":
                    purchaseOrder.SentDate = DateTime.UtcNow;
                    break;
                case "Confirmed":
                    purchaseOrder.ConfirmedDate = DateTime.UtcNow;
                    break;
                case "Shipped":
                    purchaseOrder.ShippedDate = DateTime.UtcNow;
                    break;
                case "Delivered":
                    purchaseOrder.DeliveredDate = DateTime.UtcNow;
                    purchaseOrder.ReceivedDate = DateTime.UtcNow;
                    break;
                case "Cancelled":
                    purchaseOrder.CancelledDate = DateTime.UtcNow;
                    break;
            }

            purchaseOrder.RowVersion = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync($"purchaseorder_{purchaseOrder.Id}", cancellationToken);
            await _cache.RemoveAsync("purchaseorders_all", cancellationToken);

            _logger.LogInformation("Purchase order status updated successfully: {PurchaseOrderNumber} -> {Status}",
                purchaseOrder.PurchaseOrderNumber, purchaseOrder.Status);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating purchase order status");
            throw;
        }
    }

    private void ValidateStatusTransition(string currentStatus, string newStatus)
    {
        var validTransitions = new Dictionary<string, List<string>>
        {
            { "Draft", new List<string> { "Sent", "Cancelled" } },
            { "Sent", new List<string> { "Confirmed", "Cancelled" } },
            { "Confirmed", new List<string> { "Shipped", "Cancelled" } },
            { "Shipped", new List<string> { "Delivered", "Cancelled" } },
            { "Delivered", new List<string> { "Cancelled" } },
            { "Cancelled", new List<string>() }
        };

        if (!validTransitions.ContainsKey(currentStatus))
            throw new InvalidOperationException($"Invalid current status: '{currentStatus}'");

        if (!validTransitions[currentStatus].Contains(newStatus))
            throw new InvalidOperationException($"Cannot transition from '{currentStatus}' to '{newStatus}'");
    }
}

public class ReceivePurchaseOrderCommandHandler : IRequestHandler<ReceivePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<ReceivePurchaseOrderCommandHandler> _logger;
    private readonly ICacheService _cache;

    public ReceivePurchaseOrderCommandHandler(
        ProcurementDbContext context,
        ILogger<ReceivePurchaseOrderCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<PurchaseOrderDto> Handle(ReceivePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Receiving purchase order: {PurchaseOrderId}", request.Id);

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Lines)
                .FirstOrDefaultAsync(po => po.Id == request.Id && !po.IsDeleted, cancellationToken);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID '{request.Id}' not found");

            if (purchaseOrder.Status == "Delivered")
                throw new InvalidOperationException("Purchase order is already delivered");

            if (purchaseOrder.Status == "Cancelled")
                throw new InvalidOperationException("Cannot receive a cancelled purchase order");

            // Update status to Delivered
            purchaseOrder.Status = "Delivered";
            purchaseOrder.ReceivedDate = request.ReceivedDate;
            purchaseOrder.ReceivedBy = request.ReceivedBy;
            purchaseOrder.DeliveredDate = DateTime.UtcNow;
            purchaseOrder.DateMod = DateTime.UtcNow;
            purchaseOrder.RowVersion = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync($"purchaseorder_{purchaseOrder.Id}", cancellationToken);
            await _cache.RemoveAsync("purchaseorders_all", cancellationToken);

            _logger.LogInformation("Purchase order received successfully: {PurchaseOrderNumber}", purchaseOrder.PurchaseOrderNumber);

            return MapToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving purchase order");
            throw;
        }
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            VendorId = purchaseOrder.VendorId,
            VendorName = purchaseOrder.VendorName,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            Status = purchaseOrder.Status,
            TotalAmount = purchaseOrder.TotalAmount,
            Currency = purchaseOrder.Currency,
            PaymentTerms = purchaseOrder.PaymentTerms,
            ShippingAddress = purchaseOrder.ShippingAddress,
            Description = purchaseOrder.Description,
            RequisitionId = purchaseOrder.RequisitionId,
            RequisitionNumber = purchaseOrder.RequisitionNumber,
            PeriodId = purchaseOrder.PeriodId,
            ReceivedDate = purchaseOrder.ReceivedDate,
            ReceivedBy = purchaseOrder.ReceivedBy,
            CreatedByUserId = purchaseOrder.CreatedByUserId,
            CreatedByUserName = purchaseOrder.CreatedByUserName,
            DateAdd = purchaseOrder.DateAdd,
            DateMod = purchaseOrder.DateMod,
            RowVersion = purchaseOrder.RowVersion ?? string.Empty,
            Lines = purchaseOrder.Lines.Select(l => new PurchaseOrderLineDto
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

public class CreatePurchaseOrderFromRequisitionCommandHandler : IRequestHandler<CreatePurchaseOrderFromRequisitionCommand, PurchaseOrderDto>
{
    private readonly ProcurementDbContext _context;
    private readonly ILogger<CreatePurchaseOrderFromRequisitionCommandHandler> _logger;
    private readonly ICacheService _cache;

    public CreatePurchaseOrderFromRequisitionCommandHandler(
        ProcurementDbContext context,
        ILogger<CreatePurchaseOrderFromRequisitionCommandHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderFromRequisitionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating purchase order from requisition: {RequisitionId}", request.RequisitionId);

            var requisition = await _context.Requisitions
                .Include(r => r.Lines)
                .FirstOrDefaultAsync(r => r.Id == request.RequisitionId && !r.IsDeleted, cancellationToken);

            if (requisition == null)
                throw new KeyNotFoundException($"Requisition with ID '{request.RequisitionId}' not found");

            if (requisition.Status != "Approved")
                throw new InvalidOperationException($"Cannot create purchase order from requisition with status '{requisition.Status}'");

            // Check if purchase order already exists for this requisition
            var existingPO = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.RequisitionId == request.RequisitionId && !po.IsDeleted, cancellationToken);

            if (existingPO != null)
                throw new InvalidOperationException($"A purchase order already exists for requisition '{requisition.RequisitionNumber}'");

            // Create purchase order from requisition
            var purchaseOrder = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                PurchaseOrderNumber = GeneratePurchaseOrderNumber(),
                RequisitionId = requisition.Id,
                RequisitionNumber = requisition.RequisitionNumber,
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = requisition.RequiredDate,
                Description = requisition.Description,
                Status = "Draft",
                TotalAmount = requisition.TotalAmount,
                Currency = "USD",
                PaymentTerms = "Net 30",
                PeriodId = requisition.PeriodId,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            // Copy lines from requisition
            foreach (var reqLine in requisition.Lines.Where(l => !l.IsDeleted))
            {
                var poLine = new PurchaseOrderLine
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.Id,
                    Description = reqLine.Description,
                    Quantity = reqLine.Quantity,
                    UnitPrice = reqLine.UnitPrice,
                    TotalAmount = reqLine.Quantity * reqLine.UnitPrice,
                    UnitOfMeasure = reqLine.UnitOfMeasure,
                    RequisitionLineId = reqLine.Id,
                    PeriodId = purchaseOrder.PeriodId,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                purchaseOrder.Lines.Add(poLine);
            }

            await _context.PurchaseOrders.AddAsync(purchaseOrder, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync("purchaseorders_all", cancellationToken);

            _logger.LogInformation("Purchase order created from requisition successfully: {PurchaseOrderNumber}", purchaseOrder.PurchaseOrderNumber);

            return MapToDto(purchaseOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating purchase order from requisition");
            throw;
        }
    }

    private string GeneratePurchaseOrderNumber()
    {
        return $"PO-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            VendorId = purchaseOrder.VendorId,
            VendorName = purchaseOrder.VendorName,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            Status = purchaseOrder.Status,
            TotalAmount = purchaseOrder.TotalAmount,
            Currency = purchaseOrder.Currency,
            PaymentTerms = purchaseOrder.PaymentTerms,
            ShippingAddress = purchaseOrder.ShippingAddress,
            Description = purchaseOrder.Description,
            RequisitionId = purchaseOrder.RequisitionId,
            RequisitionNumber = purchaseOrder.RequisitionNumber,
            PeriodId = purchaseOrder.PeriodId,
            CreatedByUserId = purchaseOrder.CreatedByUserId,
            CreatedByUserName = purchaseOrder.CreatedByUserName,
            DateAdd = purchaseOrder.DateAdd,
            DateMod = purchaseOrder.DateMod,
            RowVersion = purchaseOrder.RowVersion ?? string.Empty,
            Lines = purchaseOrder.Lines.Select(l => new PurchaseOrderLineDto
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