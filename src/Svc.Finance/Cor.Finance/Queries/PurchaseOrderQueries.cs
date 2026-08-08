// Svc.Finance.Queries - PurchaseOrderQueries.cs

using MediatR;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Entities.Local;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ==================== QUERIES ====================

public class GetAllPurchaseOrdersQry : IRequest<List<PurchaseOrderDto>>
{
    public Guid? VendorId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? PeriodId { get; set; }  // ✅ ADDED
}

public class GetPurchaseOrderByIdQry : IRequest<PurchaseOrderDto>
{
    public Guid Id { get; set; }
}

public class GetPurchaseOrdersByVendorQry : IRequest<List<PurchaseOrderDto>>
{
    public Guid VendorId { get; set; }
    public Guid? PeriodId { get; set; }  // ✅ ADDED
}

public class GetPurchaseOrdersByPeriodQry : IRequest<List<PurchaseOrderDto>>  // ✅ NEW
{
    public Guid PeriodId { get; set; }
}

public class GetPurchaseOrderByNumberQry : IRequest<PurchaseOrderDto>
{
    public string PurchaseOrderNumber { get; set; } = string.Empty;
}

// ==================== HANDLERS ====================

public class GetAllPurchaseOrdersHandler : IRequestHandler<GetAllPurchaseOrdersQry, List<PurchaseOrderDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllPurchaseOrdersHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQry request, CancellationToken ct)
    {
        var query = _context.PurchaseOrders
            .Include(x => x.Period)  // ✅ Include Period for PeriodName
            .Include(x => x.Vendor)
            .Include(x => x.Lines)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        // ✅ Filter by PeriodId
        if (request.PeriodId.HasValue)
        {
            query = query.Where(x => x.PeriodId == request.PeriodId.Value);
        }

        if (request.VendorId.HasValue)
            query = query.Where(x => x.VendorId == request.VendorId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.OrderDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.OrderDate <= request.ToDate.Value);

        var purchaseOrders = await query
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync(ct);

        return purchaseOrders.Select(MapToDto).ToList();
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
            VendorName = po.Vendor?.Name ?? po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            PeriodId = po.PeriodId,
            PeriodName = po.Period?.Name,  // ✅ Get PeriodName
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines?.Where(x => !x.IsDeleted).Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = po.Period?.Name
            }).ToList() ?? new List<PurchaseOrderLineDto>()
        };
    }
}

// ==================== GET PURCHASE ORDER BY ID HANDLER ====================

public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdQry, PurchaseOrderDto>
{
    private readonly FinanceDbContext _context;

    public GetPurchaseOrderByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByIdQry request, CancellationToken ct)
    {
        var po = await _context.PurchaseOrders
            .Include(x => x.Period)  // ✅ Include Period for PeriodName
            .Include(x => x.Vendor)
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (po == null)
            throw new InvalidOperationException($"Purchase order with ID '{request.Id}' not found");

        return MapToDto(po);
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
            VendorName = po.Vendor?.Name ?? po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            PeriodId = po.PeriodId,
            PeriodName = po.Period?.Name,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines?.Where(x => !x.IsDeleted).Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = po.Period?.Name
            }).ToList() ?? new List<PurchaseOrderLineDto>()
        };
    }
}

// ==================== GET PURCHASE ORDERS BY VENDOR HANDLER ====================

public class GetPurchaseOrdersByVendorHandler : IRequestHandler<GetPurchaseOrdersByVendorQry, List<PurchaseOrderDto>>
{
    private readonly FinanceDbContext _context;

    public GetPurchaseOrdersByVendorHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersByVendorQry request, CancellationToken ct)
    {
        var query = _context.PurchaseOrders
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .Include(x => x.Lines)
            .Where(x => x.VendorId == request.VendorId && !x.IsDeleted)
            .AsQueryable();

        // ✅ Filter by PeriodId
        if (request.PeriodId.HasValue)
        {
            query = query.Where(x => x.PeriodId == request.PeriodId.Value);
        }

        var purchaseOrders = await query
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync(ct);

        return purchaseOrders.Select(MapToDto).ToList();
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
            VendorName = po.Vendor?.Name ?? po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            PeriodId = po.PeriodId,
            PeriodName = po.Period?.Name,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines?.Where(x => !x.IsDeleted).Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = po.Period?.Name
            }).ToList() ?? new List<PurchaseOrderLineDto>()
        };
    }
}

// ==================== GET PURCHASE ORDERS BY PERIOD HANDLER ====================

public class GetPurchaseOrdersByPeriodHandler : IRequestHandler<GetPurchaseOrdersByPeriodQry, List<PurchaseOrderDto>>
{
    private readonly FinanceDbContext _context;

    public GetPurchaseOrdersByPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersByPeriodQry request, CancellationToken ct)
    {
        var purchaseOrders = await _context.PurchaseOrders
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .Include(x => x.Lines)
            .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync(ct);

        return purchaseOrders.Select(MapToDto).ToList();
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
            VendorName = po.Vendor?.Name ?? po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            PeriodId = po.PeriodId,
            PeriodName = po.Period?.Name,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines?.Where(x => !x.IsDeleted).Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = po.Period?.Name
            }).ToList() ?? new List<PurchaseOrderLineDto>()
        };
    }
}

// ==================== GET PURCHASE ORDER BY NUMBER HANDLER ====================

public class GetPurchaseOrderByNumberHandler : IRequestHandler<GetPurchaseOrderByNumberQry, PurchaseOrderDto>
{
    private readonly FinanceDbContext _context;

    public GetPurchaseOrderByNumberHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByNumberQry request, CancellationToken ct)
    {
        var po = await _context.PurchaseOrders
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.PurchaseOrderNumber == request.PurchaseOrderNumber && !x.IsDeleted, ct);

        if (po == null)
            throw new InvalidOperationException($"Purchase order with number '{request.PurchaseOrderNumber}' not found");

        return MapToDto(po);
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
            VendorName = po.Vendor?.Name ?? po.VendorName,
            Description = po.Description,
            TotalAmount = po.TotalAmount,
            Status = po.Status,
            Currency = po.Currency,
            ReceivedDate = po.ReceivedDate,
            ReceivedBy = po.ReceivedBy,
            PeriodId = po.PeriodId,
            PeriodName = po.Period?.Name,
            DateAdd = po.DateAdd,
            DateMod = po.DateMod,
            Lines = po.Lines?.Where(x => !x.IsDeleted).Select(line => new PurchaseOrderLineDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TotalAmount = line.TotalAmount,
                Discount = line.Discount,
                TaxRate = line.TaxRate,
                PeriodId = line.PeriodId,
                PeriodName = po.Period?.Name
            }).ToList() ?? new List<PurchaseOrderLineDto>()
        };
    }
}