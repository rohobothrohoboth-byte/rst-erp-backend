// Svc.Finance.Handlers - PurchaseOrderHandlers.cs

 using MediatR;
 using Microsoft.EntityFrameworkCore;
 using Cor.Finance.Models.DTOs;
 using Cor.Finance.Models.Entities;
 using Cor.Finance.Persistence;
 using Cor.Finance.Queries;
using Cor.Finance.Models.Entities.Local;
 namespace Cor.Finance.Handlers;



 // ✅ Handler for Get All
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
             .Where(x => !x.IsDeleted)
             .Include(x => x.Lines)
             .AsQueryable();

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
             VendorName = po.VendorName,
             Description = po.Description,
             TotalAmount = po.TotalAmount,
             Status = po.Status,
             Currency = po.Currency,
             ReceivedDate = po.ReceivedDate,
             ReceivedBy = po.ReceivedBy,
             DateAdd = po.DateAdd,
             DateMod = po.DateMod,
             Lines = po.Lines.Select(line => new PurchaseOrderLineDto
             {
                 Id = line.Id,
                 Description = line.Description,
                 Quantity = line.Quantity,
                 UnitPrice = line.UnitPrice,
                 TotalAmount = line.TotalAmount,
                 Discount = line.Discount,
                 TaxRate = line.TaxRate
             }).ToList()
         };
     }
 }

 // ✅ Handler for Get By ID
 public class GetPurchaseOrderByIdHandler : IRequestHandler<GetPurchaseOrderByIdQry, PurchaseOrderDto>
 {
     private readonly FinanceDbContext _context;

     public GetPurchaseOrderByIdHandler(FinanceDbContext context)
     {
         _context = context;
     }

     public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByIdQry request, CancellationToken ct)
     {
         var purchaseOrder = await _context.PurchaseOrders
             .Include(x => x.Lines)
             .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

         if (purchaseOrder == null)
             throw new InvalidOperationException($"Purchase Order with ID '{request.Id}' not found");

         return MapToDto(purchaseOrder);
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
             DateAdd = po.DateAdd,
             DateMod = po.DateMod,
             Lines = po.Lines.Select(line => new PurchaseOrderLineDto
             {
                 Id = line.Id,
                 Description = line.Description,
                 Quantity = line.Quantity,
                 UnitPrice = line.UnitPrice,
                 TotalAmount = line.TotalAmount,
                 Discount = line.Discount,
                 TaxRate = line.TaxRate
             }).ToList()
         };
     }
 }

 // ✅ Handler for Get By Vendor
 public class GetPurchaseOrdersByVendorHandler : IRequestHandler<GetPurchaseOrdersByVendorQry, List<PurchaseOrderDto>>
 {
     private readonly FinanceDbContext _context;

     public GetPurchaseOrdersByVendorHandler(FinanceDbContext context)
     {
         _context = context;
     }

     public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersByVendorQry request, CancellationToken ct)
     {
         var purchaseOrders = await _context.PurchaseOrders
             .Where(x => x.VendorId == request.VendorId && !x.IsDeleted)
             .Include(x => x.Lines)
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
             VendorName = po.VendorName,
             Description = po.Description,
             TotalAmount = po.TotalAmount,
             Status = po.Status,
             Currency = po.Currency,
             ReceivedDate = po.ReceivedDate,
             ReceivedBy = po.ReceivedBy,
             DateAdd = po.DateAdd,
             DateMod = po.DateMod,
             Lines = po.Lines.Select(line => new PurchaseOrderLineDto
             {
                 Id = line.Id,
                 Description = line.Description,
                 Quantity = line.Quantity,
                 UnitPrice = line.UnitPrice,
                 TotalAmount = line.TotalAmount,
                 Discount = line.Discount,
                 TaxRate = line.TaxRate
             }).ToList()
         };
     }
 }

 // ✅ Handler for Get By Number
 public class GetPurchaseOrderByNumberHandler : IRequestHandler<GetPurchaseOrderByNumberQry, PurchaseOrderDto>
 {
     private readonly FinanceDbContext _context;

     public GetPurchaseOrderByNumberHandler(FinanceDbContext context)
     {
         _context = context;
     }

     public async Task<PurchaseOrderDto> Handle(GetPurchaseOrderByNumberQry request, CancellationToken ct)
     {
         var purchaseOrder = await _context.PurchaseOrders
             .Include(x => x.Lines)
             .FirstOrDefaultAsync(x => x.PurchaseOrderNumber == request.PurchaseOrderNumber && !x.IsDeleted, ct);

         if (purchaseOrder == null)
             throw new InvalidOperationException($"Purchase Order with number '{request.PurchaseOrderNumber}' not found");

         return MapToDto(purchaseOrder);
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
             DateAdd = po.DateAdd,
             DateMod = po.DateMod,
             Lines = po.Lines.Select(line => new PurchaseOrderLineDto
             {
                 Id = line.Id,
                 Description = line.Description,
                 Quantity = line.Quantity,
                 UnitPrice = line.UnitPrice,
                 TotalAmount = line.TotalAmount,
                 Discount = line.Discount,
                 TaxRate = line.TaxRate
             }).ToList()
         };
     }
 }