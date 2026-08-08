// Commands/PurchaseOrderCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities.Local;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ==================== COMMANDS ====================

public class AddPurchaseOrderCmd : IRequest<PurchaseOrderDto>
{
    public AddPurchaseOrderDto AddDto { get; set; } = default!;
}

public class EditPurchaseOrderCmd : IRequest<PurchaseOrderDto>
{
    public EditPurchaseOrderDto EditDto { get; set; } = default!;
}

public class DeletePurchaseOrderCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ReceivePurchaseOrderCmd : IRequest<bool>
{
    public Guid Id { get; set; }
    public DateTime ReceivedDate { get; set; }
    public Guid? ReceivedBy { get; set; }
}

// ==================== ADD PURCHASE ORDER HANDLER ====================

public class AddPurchaseOrderHandler : IRequestHandler<AddPurchaseOrderCmd, PurchaseOrderDto>
{
    private readonly FinanceDbContext _context;

    public AddPurchaseOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDto> Handle(AddPurchaseOrderCmd request, CancellationToken ct)
    {
        // ✅ STEP 1: VALIDATE PERIOD EXISTS AND IS OPEN
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {request.AddDto.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot create purchase order in a closed period: {period.Name}");

        // ✅ STEP 2: VALIDATE ORDER DATE IS WITHIN PERIOD RANGE
        if (request.AddDto.OrderDate < period.StartDate || request.AddDto.OrderDate > period.EndDate)
            throw new InvalidOperationException(
                $"Order date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

        // ✅ STEP 3: Validate vendor exists
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == request.AddDto.VendorId && !x.IsDeleted, ct);

        if (vendor == null)
            throw new InvalidOperationException($"Vendor with ID '{request.AddDto.VendorId}' not found");

        // ✅ STEP 4: Validate lines exist
        if (request.AddDto.Lines == null || !request.AddDto.Lines.Any())
            throw new InvalidOperationException("Purchase order must have at least one line");

        // ✅ STEP 5: Calculate total amount
        var totalAmount = request.AddDto.Lines.Sum(x => x.Quantity * x.UnitPrice);

        // ✅ STEP 6: Create purchase order
        var purchaseOrder = new PurchaseOrder
        {
            Id = Guid.NewGuid(),
            PurchaseOrderNumber = request.AddDto.PurchaseOrderNumber ?? GeneratePurchaseOrderNumber(),
            OrderDate = request.AddDto.OrderDate,
            ExpectedDeliveryDate = request.AddDto.ExpectedDeliveryDate,
            VendorId = request.AddDto.VendorId,
            VendorName = vendor.Name,
            Description = request.AddDto.Description,
            TotalAmount = totalAmount,
            Status = request.AddDto.Status ?? "Draft",
            Currency = request.AddDto.Currency ?? "USD",
            // ✅ PeriodId - REQUIRED
            PeriodId = period.Id,
            DateAdd = DateTime.UtcNow,
            DateMod = null,
            IsDeleted = false
        };

        // ✅ STEP 7: Add lines
        foreach (var lineDto in request.AddDto.Lines)
        {
            purchaseOrder.Lines.Add(new PurchaseOrderLine
            {
                Id = Guid.NewGuid(),
                PurchaseOrderId = purchaseOrder.Id,
                Description = lineDto.Description,
                Quantity = lineDto.Quantity,
                UnitPrice = lineDto.UnitPrice,
                TotalAmount = lineDto.Quantity * lineDto.UnitPrice,
                Discount = lineDto.Discount,
                TaxRate = lineDto.TaxRate,
                // ✅ PeriodId - REQUIRED (denormalized)
                PeriodId = period.Id,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            });
        }

        await _context.PurchaseOrders.AddAsync(purchaseOrder, ct);
        await _context.SaveChangesAsync(ct);

        return await MapToDto(purchaseOrder, period, ct);
    }

    private string GeneratePurchaseOrderNumber()
    {
        return $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    private async Task<PurchaseOrderDto> MapToDto(PurchaseOrder purchaseOrder, FinancialPeriod period, CancellationToken ct)
    {
        var lines = await _context.PurchaseOrderLines
            .Where(x => x.PurchaseOrderId == purchaseOrder.Id && !x.IsDeleted)
            .ToListAsync(ct);

        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            VendorId = purchaseOrder.VendorId,
            VendorName = purchaseOrder.VendorName,
            Description = purchaseOrder.Description,
            TotalAmount = purchaseOrder.TotalAmount,
            Status = purchaseOrder.Status,
            Currency = purchaseOrder.Currency,
            ReceivedDate = purchaseOrder.ReceivedDate,
            ReceivedBy = purchaseOrder.ReceivedBy,
            PeriodId = purchaseOrder.PeriodId,
            PeriodName = period?.Name,
            DateAdd = purchaseOrder.DateAdd,
            DateMod = purchaseOrder.DateMod,
            Lines = lines.Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = period?.Name
            }).ToList()
        };
    }
}

// ==================== EDIT PURCHASE ORDER HANDLER ====================

public class EditPurchaseOrderHandler : IRequestHandler<EditPurchaseOrderCmd, PurchaseOrderDto>
{
    private readonly FinanceDbContext _context;

    public EditPurchaseOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDto> Handle(EditPurchaseOrderCmd request, CancellationToken ct)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (purchaseOrder == null)
            throw new InvalidOperationException($"Purchase order with ID '{request.EditDto.Id}' not found");

        if (purchaseOrder.Status == "Received")
            throw new InvalidOperationException("Cannot edit a received purchase order.");

        // Validate Period
        if (purchaseOrder.PeriodId != request.EditDto.PeriodId)
        {
            var newPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    p => p.Id == request.EditDto.PeriodId &&
                         !p.IsDeleted,
                    ct);

            if (newPeriod == null)
                throw new InvalidOperationException(
                    $"Period with ID '{request.EditDto.PeriodId}' not found.");

            if (newPeriod.IsClosed)
                throw new InvalidOperationException(
                    $"Cannot move purchase order to a closed period: {newPeriod.Name}");

            if (request.EditDto.OrderDate < newPeriod.StartDate ||
                request.EditDto.OrderDate > newPeriod.EndDate)
            {
                throw new InvalidOperationException(
                    $"Order date must be between {newPeriod.StartDate:yyyy-MM-dd} and {newPeriod.EndDate:yyyy-MM-dd}");
            }

            purchaseOrder.PeriodId = newPeriod.Id;
        }
        else
        {
            var currentPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    p => p.Id == purchaseOrder.PeriodId &&
                         !p.IsDeleted,
                    ct);

            if (currentPeriod != null && currentPeriod.IsClosed)
                throw new InvalidOperationException(
                    $"Cannot update purchase order in a closed period: {currentPeriod.Name}");
        }

        // Validate Vendor
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(
                x => x.Id == request.EditDto.VendorId &&
                     !x.IsDeleted,
                ct);

        if (vendor == null)
            throw new InvalidOperationException(
                $"Vendor with ID '{request.EditDto.VendorId}' not found");

        // Update Purchase Order
        purchaseOrder.PurchaseOrderNumber = request.EditDto.PurchaseOrderNumber;
        purchaseOrder.OrderDate = request.EditDto.OrderDate;
        purchaseOrder.ExpectedDeliveryDate = request.EditDto.ExpectedDeliveryDate;
        purchaseOrder.VendorId = request.EditDto.VendorId;
        purchaseOrder.VendorName = vendor.Name;
        purchaseOrder.Description = request.EditDto.Description;
        purchaseOrder.Currency = request.EditDto.Currency ?? "USD";
        purchaseOrder.DateMod = DateTime.UtcNow;

        // Update Lines
        if (request.EditDto.Lines != null && request.EditDto.Lines.Any())
        {
            var existingLines = await _context.PurchaseOrderLines
                .Where(x => x.PurchaseOrderId == purchaseOrder.Id && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var line in existingLines)
            {
                line.IsDeleted = true;
                line.DateMod = DateTime.UtcNow;
            }

            decimal totalAmount = 0;

            foreach (var lineDto in request.EditDto.Lines)
            {
                var lineTotal = lineDto.Quantity * lineDto.UnitPrice;

                totalAmount += lineTotal;

                purchaseOrder.Lines.Add(new PurchaseOrderLine
                {
                    Id = Guid.NewGuid(),
                    PurchaseOrderId = purchaseOrder.Id,
                    Description = lineDto.Description,
                    Quantity = lineDto.Quantity,
                    UnitPrice = lineDto.UnitPrice,
                    TotalAmount = lineTotal,
                    Discount = lineDto.Discount,
                    TaxRate = lineDto.TaxRate,
                    PeriodId = purchaseOrder.PeriodId,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                });
            }

            purchaseOrder.TotalAmount = totalAmount;
        }

        await _context.SaveChangesAsync(ct);

        var purchaseOrderPeriod = await _context.FinancialPeriods
            .FirstOrDefaultAsync(
                p => p.Id == purchaseOrder.PeriodId &&
                     !p.IsDeleted,
                ct);

        return await MapToDto(
            purchaseOrder,
            purchaseOrderPeriod,
            ct);
    }


    private async Task<PurchaseOrderDto> MapToDto(PurchaseOrder purchaseOrder, FinancialPeriod? period, CancellationToken ct)
    {
        var lines = await _context.PurchaseOrderLines
            .Where(x => x.PurchaseOrderId == purchaseOrder.Id && !x.IsDeleted)
            .ToListAsync(ct);

        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            VendorId = purchaseOrder.VendorId,
            VendorName = purchaseOrder.VendorName,
            Description = purchaseOrder.Description,
            TotalAmount = purchaseOrder.TotalAmount,
            Status = purchaseOrder.Status,
            Currency = purchaseOrder.Currency,
            ReceivedDate = purchaseOrder.ReceivedDate,
            ReceivedBy = purchaseOrder.ReceivedBy,
            PeriodId = purchaseOrder.PeriodId,
            PeriodName = period?.Name,
            DateAdd = purchaseOrder.DateAdd,
            DateMod = purchaseOrder.DateMod,
            Lines = lines.Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = period?.Name
            }).ToList()
        };
    }
}

// ==================== DELETE PURCHASE ORDER HANDLER ====================

public class DeletePurchaseOrderHandler : IRequestHandler<DeletePurchaseOrderCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeletePurchaseOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePurchaseOrderCmd request, CancellationToken ct)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (purchaseOrder == null)
            return false;

        if (purchaseOrder.Status == "Received")
            throw new InvalidOperationException("Cannot delete a received purchase order.");

        // ✅ VALIDATE PERIOD IS OPEN BEFORE DELETING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == purchaseOrder.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {purchaseOrder.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot delete purchase order in a closed period: {period.Name}");

        // Soft delete the purchase order
        purchaseOrder.IsDeleted = true;
        purchaseOrder.DateMod = DateTime.UtcNow;

        // Soft delete all lines
        var lines = await _context.PurchaseOrderLines
            .Where(x => x.PurchaseOrderId == purchaseOrder.Id && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var line in lines)
        {
            line.IsDeleted = true;
            line.DateMod = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ==================== RECEIVE PURCHASE ORDER HANDLER ====================

public class ReceivePurchaseOrderHandler : IRequestHandler<ReceivePurchaseOrderCmd, bool>
{
    private readonly FinanceDbContext _context;

    public ReceivePurchaseOrderHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReceivePurchaseOrderCmd request, CancellationToken ct)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (purchaseOrder == null)
            return false;

        if (purchaseOrder.Status == "Received")
            throw new InvalidOperationException("Purchase order is already received.");

        // ✅ VALIDATE PERIOD IS OPEN
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == purchaseOrder.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {purchaseOrder.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot receive purchase order in a closed period: {period.Name}");

        purchaseOrder.Status = "Received";
        purchaseOrder.ReceivedDate = request.ReceivedDate;
        purchaseOrder.ReceivedBy = request.ReceivedBy;
        purchaseOrder.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}