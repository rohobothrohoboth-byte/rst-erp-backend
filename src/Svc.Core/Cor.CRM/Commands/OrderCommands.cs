// Cor.CRM/Commands/OrderCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class OrderAddCmd : IRequest<OrderDto>
{
    public CreateOrderDto Dto { get; set; } = default!;
}

public class OrderUpdateCmd : IRequest<OrderDto>
{
    public Guid Id { get; set; }
    public UpdateOrderDto Dto { get; set; } = default!;
}

public class OrderSendCmd : IRequest<OrderDto>
{
    public Guid Id { get; set; }
}

public class OrderAcceptCmd : IRequest<OrderDto>
{
    public Guid Id { get; set; }
}

public class OrderRejectCmd : IRequest<OrderDto>
{
    public Guid Id { get; set; }
}

public class OrderDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class OrderCompleteCmd : IRequest<OrderDto>
{
    public Guid Id { get; set; }
}

public class OrderCancelCmd : IRequest<OrderDto>
{
    public Guid Id { get; set; }
}

// ============================================================
// ORDER ADD HANDLER
// ============================================================

public class OrderAddHandler : IRequestHandler<OrderAddCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {


            // Log each order line
            if (request.Dto.OrderLines != null)
            {
                foreach (var line in request.Dto.OrderLines)
                {
                    _logger.LogInformation("Line: Desc={Desc}, Qty={Qty}, Price={Price}, Disc={Disc}, Tax={Tax}, Total={Total}",
                        line.Description ?? "N/A",
                        line.Quantity,
                        line.UnitPrice,
                        line.Discount ?? 0,
                        line.TaxRate ?? 0,
                        line.TotalPrice);
                }
            }

            // Generate order number
            var orderNumber = await GenerateOrderNumber(ct);

            // Ensure all dates are UTC
            var orderDate = DateTimeHelper.EnsureUtc(request.Dto.OrderDate);
         var dueDate = request.Dto.DueDate.HasValue
             ? (DateTime?)DateTimeHelper.EnsureUtc(request.Dto.DueDate.Value)
             : null;

            var order = new SalesOrder
            {
                Id = Guid.CreateVersion7(),
                OrderNumber = orderNumber,
                CustomerId = request.Dto.CustomerId,
                OpportunityId = request.Dto.OpportunityId,
                QuoteId = request.Dto.QuoteId,
                OrderDate = orderDate,
                DueDate = dueDate,
                SubTotal = request.Dto.SubTotal,
                TaxAmount = request.Dto.TaxAmount,
                DiscountAmount = request.Dto.DiscountAmount,
                ShippingCost = request.Dto.ShippingCost,
                TotalAmount = request.Dto.TotalAmount,
                ShippingAddress = request.Dto.ShippingAddress,
                BillingAddress = request.Dto.BillingAddress,
                Terms = request.Dto.Terms,
                Notes = request.Dto.Notes,
                Currency = request.Dto.Currency ?? "USD",
                Status = OrderStatus.Draft,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false
            };

            // ✅ Add order first
            await _uow.Add(order, ct);

            // ✅ Add order lines
            int sortOrder = 0;
            var orderLines = new List<OrderLine>();

            if (request.Dto.OrderLines != null && request.Dto.OrderLines.Any())
            {
                _logger.LogInformation("Processing {LineCount} order lines", request.Dto.OrderLines.Count);

                foreach (var lineDto in request.Dto.OrderLines)
                {
                    var discount = lineDto.Discount ?? 0;
                    var taxRate = lineDto.TaxRate ?? 0;
                    var totalPrice = lineDto.Quantity * lineDto.UnitPrice * (1 - discount / 100) * (1 + taxRate / 100);

                    _logger.LogInformation("Order Line: Desc={Description}, Qty={Quantity}, Price={UnitPrice}, Disc={Discount}, Tax={TaxRate}, Total={TotalPrice}",
                        lineDto.Description, lineDto.Quantity, lineDto.UnitPrice, discount, taxRate, totalPrice);

                    var orderLine = new OrderLine
                    {
                        Id = Guid.CreateVersion7(),
                        OrderId = order.Id,
                        ProductId = lineDto.ProductId,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        Discount = discount,
                        TaxRate = taxRate,
                        TotalPrice = totalPrice,
                        SortOrder = sortOrder++,
                        Notes = lineDto.Notes,
                        CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        IsDeleted = false
                    };
                    orderLines.Add(orderLine);
                    await _uow.Add(orderLine, ct);
                }
            }
            else
            {
                _logger.LogWarning("No order lines provided for order {OrderNumber}", orderNumber);
            }

            await _uow.Commit(ct);

            _logger.LogInformation("✅ Order created successfully: {OrderNumber} with {LineCount} lines",
                order.OrderNumber, orderLines.Count);

            // Return the DTO from the already loaded entities
            return MapToDto(order, orderLines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to create order");
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task<string> GenerateOrderNumber(CancellationToken ct)
    {
        var count = await _uow.Set<SalesOrder>().CountAsync(ct) + 1;
        return $"SO-{DateTime.UtcNow:yyyyMMdd}-{count:D4}";
    }

    public static OrderDto MapToDto(SalesOrder order, List<OrderLine> orderLines)
    {
        var dto = new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            OpportunityId = order.OpportunityId,
            QuoteId = order.QuoteId,
            OrderDate = order.OrderDate,
            DueDate = order.DueDate,
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            DiscountAmount = order.DiscountAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            BillingAddress = order.BillingAddress,
            Terms = order.Terms,
            Notes = order.Notes,
            Currency = order.Currency,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            OrderLines = orderLines.Where(l => !l.IsDeleted).Select(l => new OrderLineDto
            {
                Id = l.Id,
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                Discount = l.Discount,
                TaxRate = l.TaxRate,
                TotalPrice = l.TotalPrice,
                SortOrder = l.SortOrder,
                ProductId = l.ProductId,
                ProductName = l.Product?.Name,
                Notes = l.Notes
            }).ToList()
        };

        return dto;
    }
}

// ============================================================
// ORDER UPDATE HANDLER
// ============================================================

public class OrderUpdateHandler : IRequestHandler<OrderUpdateCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderUpdateHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Pending)
                throw new DomainException($"Cannot update order with status [{order.Status}]");

            if (request.Dto.DueDate.HasValue)
                order.DueDate = DateTimeHelper.EnsureUtc(request.Dto.DueDate.Value);

            if (request.Dto.ShippingAddress != null)
                order.ShippingAddress = request.Dto.ShippingAddress;

            if (request.Dto.BillingAddress != null)
                order.BillingAddress = request.Dto.BillingAddress;

            if (request.Dto.Terms != null)
                order.Terms = request.Dto.Terms;

            if (request.Dto.Notes != null)
                order.Notes = request.Dto.Notes;

            if (request.Dto.DiscountAmount.HasValue)
                order.DiscountAmount = request.Dto.DiscountAmount.Value;

            if (request.Dto.ShippingCost.HasValue)
                order.ShippingCost = request.Dto.ShippingCost.Value;

            if (request.Dto.OrderLines != null && request.Dto.OrderLines.Any())
            {
                var existingLines = await _uow.Set<OrderLine>()
                    .Where(x => x.OrderId == order.Id && !x.IsDeleted)
                    .ToListAsync(ct);

                foreach (var line in existingLines)
                {
                    line.IsDeleted = true;
                    line.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                    await _uow.Update(line);
                }

                decimal subTotal = 0;
                decimal taxTotal = 0;
                int sortOrder = 0;
                var newOrderLines = new List<OrderLine>();

                foreach (var lineDto in request.Dto.OrderLines)
                {
                    var discount = lineDto.Discount ?? 0;
                    var taxRate = lineDto.TaxRate ?? 0;
                    var totalPrice = lineDto.Quantity * lineDto.UnitPrice * (1 - discount / 100) * (1 + taxRate / 100);

                    subTotal += lineDto.Quantity * lineDto.UnitPrice;
                    taxTotal += totalPrice - (lineDto.Quantity * lineDto.UnitPrice);

                    var orderLine = new OrderLine
                    {
                        Id = Guid.CreateVersion7(),
                        OrderId = order.Id,
                        ProductId = lineDto.ProductId,
                        Description = lineDto.Description,
                        Quantity = lineDto.Quantity,
                        UnitPrice = lineDto.UnitPrice,
                        Discount = discount,
                        TaxRate = taxRate,
                        TotalPrice = totalPrice,
                        SortOrder = sortOrder++,
                        Notes = lineDto.Notes,
                        CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                        IsDeleted = false
                    };
                    newOrderLines.Add(orderLine);
                    await _uow.Add(orderLine, ct);
                }

                order.SubTotal = subTotal;
                order.TaxAmount = taxTotal;
                order.TotalAmount = subTotal + taxTotal - order.DiscountAmount + order.ShippingCost;
            }

            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order updated: {OrderNumber}", order.OrderNumber);

            var activeLines = order.OrderLines.Where(l => !l.IsDeleted).ToList();
            return OrderAddHandler.MapToDto(order, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ORDER SEND HANDLER
// ============================================================

public class OrderSendHandler : IRequestHandler<OrderSendCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderSendHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderSendCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status != OrderStatus.Draft)
                throw new DomainException($"Cannot send order with status [{order.Status}]");

            order.Status = OrderStatus.Pending;
            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order sent: {OrderNumber}", order.OrderNumber);

            var activeLines = order.OrderLines.Where(l => !l.IsDeleted).ToList();
            return OrderAddHandler.MapToDto(order, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ORDER ACCEPT HANDLER
// ============================================================

public class OrderAcceptHandler : IRequestHandler<OrderAcceptCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderAcceptHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderAcceptCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
                throw new DomainException($"Cannot accept order with status [{order.Status}]");

            order.Status = OrderStatus.Processing;
            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order accepted: {OrderNumber}", order.OrderNumber);

            var activeLines = order.OrderLines.Where(l => !l.IsDeleted).ToList();
            return OrderAddHandler.MapToDto(order, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ORDER REJECT HANDLER
// ============================================================

public class OrderRejectHandler : IRequestHandler<OrderRejectCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderRejectHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderRejectCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status != OrderStatus.Pending)
                throw new DomainException($"Cannot reject order with status [{order.Status}]");

            order.Status = OrderStatus.Cancelled;
            order.CancelledDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order rejected: {OrderNumber}", order.OrderNumber);

            var activeLines = order.OrderLines.Where(l => !l.IsDeleted).ToList();
            return OrderAddHandler.MapToDto(order, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ORDER DELETE HANDLER
// ============================================================

public class OrderDelHandler : IRequestHandler<OrderDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(OrderDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Pending)
                throw new DomainException($"Cannot delete order with status [{order.Status}]");

            order.IsDeleted = true;
            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order deleted: {OrderNumber}", order.OrderNumber);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ORDER COMPLETE HANDLER
// ============================================================

public class OrderCompleteHandler : IRequestHandler<OrderCompleteCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderCompleteHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderCompleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status != OrderStatus.Shipped && order.Status != OrderStatus.Delivered)
                throw new DomainException($"Cannot complete order with status [{order.Status}]");

            order.Status = OrderStatus.Completed;
            order.CompletedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order completed: {OrderNumber}", order.OrderNumber);

            var activeLines = order.OrderLines.Where(l => !l.IsDeleted).ToList();
            return OrderAddHandler.MapToDto(order, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// ORDER CANCEL HANDLER
// ============================================================

public class OrderCancelHandler : IRequestHandler<OrderCancelCmd, OrderDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OrderCancelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OrderDto> Handle(OrderCancelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var order = await _uow.Set<SalesOrder>()
                .Include(x => x.OrderLines)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (order == null)
                throw new DomainException($"Order with id [{request.Id}] NOT FOUND.");

            if (order.Status == OrderStatus.Completed)
                throw new DomainException($"Cannot cancel completed order");

            order.Status = OrderStatus.Cancelled;
            order.CancelledDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            order.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(order);
            await _uow.Commit(ct);

            _logger.LogInformation("Order cancelled: {OrderNumber}", order.OrderNumber);

            var activeLines = order.OrderLines.Where(l => !l.IsDeleted).ToList();
            return OrderAddHandler.MapToDto(order, activeLines);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}