using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class StockService : IStockService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<StockService> _logger;

    public StockService(InventoryDbContext context, ILogger<StockService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ============= Movements =============

    public async Task<List<StockMovementDto>> GetMovementsAsync(string? type, Guid? warehouseId, CancellationToken ct = default)
    {
        var query = _context.StockMovements.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(m => m.Type == type);

        if (warehouseId.HasValue)
            query = query.Where(m => m.WarehouseId == warehouseId.Value || m.ToWarehouseId == warehouseId.Value);

        var movements = await query
            .OrderByDescending(m => m.MovementDate)
            .ToListAsync(ct);

        return movements.Select(MapMovement).ToList();
    }

    public async Task<List<StockLevel>> GetStockLevelsAsync(Guid? warehouseId, CancellationToken ct = default)
    {
        var query = _context.StockLevels.AsNoTracking().AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == warehouseId.Value);

        return await query
            .OrderBy(s => s.ProductName)
            .ToListAsync(ct);
    }

    public async Task<StockMovementDto> InboundAsync(InboundStockDto dto, CancellationToken ct = default)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        var product = await GetProductAsync(dto.ProductId, ct);

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            Type = "Inbound",
            ProductId = dto.ProductId,
            ProductName = product?.Name,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            Reference = dto.Reference,
            Reason = dto.Reason,
            Notes = dto.Notes,
            Status = "Posted",
            MovementDate = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        };
        movement.UpdateRowVersion();

        var level = await GetOrCreateLevelAsync(dto.WarehouseId, dto.ProductId, product, ct);

        // Weighted average cost using existing on-hand / average cost.
        if (dto.UnitCost.HasValue)
        {
            var existingQty = level.QuantityOnHand;
            var existingAvg = level.AverageCost ?? dto.UnitCost.Value;
            var newQty = existingQty + dto.Quantity;

            if (newQty > 0)
            {
                var totalCost = existingQty * existingAvg + dto.Quantity * dto.UnitCost.Value;
                level.AverageCost = Math.Round(totalCost / newQty, 2);
            }

            level.LastUnitCost = dto.UnitCost.Value;
        }

        level.QuantityOnHand += dto.Quantity;
        level.LastReceivedDate = DateTime.UtcNow;
        TouchLevel(level);

        await _context.StockMovements.AddAsync(movement, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Inbound {Quantity} of product {ProductId} to warehouse {WarehouseId}",
            dto.Quantity, dto.ProductId, dto.WarehouseId);

        return MapMovement(movement);
    }

    public async Task<StockMovementDto> OutboundAsync(OutboundStockDto dto, CancellationToken ct = default)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        var product = await GetProductAsync(dto.ProductId, ct);

        var level = await GetLevelAsync(dto.WarehouseId, dto.ProductId, ct);
        if (level == null || level.QuantityAvailable < dto.Quantity)
            throw new InvalidOperationException("Insufficient available stock for outbound movement.");

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            Type = "Outbound",
            ProductId = dto.ProductId,
            ProductName = product?.Name,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            Reference = dto.Reference,
            Reason = dto.Reason,
            Notes = dto.Notes,
            Status = "Posted",
            MovementDate = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        };
        movement.UpdateRowVersion();

        level.QuantityOnHand -= dto.Quantity;
        level.LastIssuedDate = DateTime.UtcNow;
        TouchLevel(level);

        await _context.StockMovements.AddAsync(movement, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Outbound {Quantity} of product {ProductId} from warehouse {WarehouseId}",
            dto.Quantity, dto.ProductId, dto.WarehouseId);

        return MapMovement(movement);
    }

    public async Task<StockMovementDto> TransferAsync(TransferStockDto dto, CancellationToken ct = default)
    {
        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        if (dto.WarehouseId == dto.ToWarehouseId)
            throw new InvalidOperationException("Source and destination warehouses must differ.");

        var product = await GetProductAsync(dto.ProductId, ct);

        var source = await GetLevelAsync(dto.WarehouseId, dto.ProductId, ct);
        if (source == null || source.QuantityAvailable < dto.Quantity)
            throw new InvalidOperationException("Insufficient available stock for transfer.");

        var destination = await GetOrCreateLevelAsync(dto.ToWarehouseId, dto.ProductId, product, ct);

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            Type = "Transfer",
            ProductId = dto.ProductId,
            ProductName = product?.Name,
            WarehouseId = dto.WarehouseId,
            ToWarehouseId = dto.ToWarehouseId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            Reference = dto.Reference,
            Reason = dto.Reason,
            Notes = dto.Notes,
            Status = "Posted",
            MovementDate = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        };
        movement.UpdateRowVersion();

        source.QuantityOnHand -= dto.Quantity;
        source.LastIssuedDate = DateTime.UtcNow;
        TouchLevel(source);

        destination.QuantityOnHand += dto.Quantity;
        destination.LastReceivedDate = DateTime.UtcNow;
        if (dto.UnitCost.HasValue)
            destination.LastUnitCost = dto.UnitCost.Value;
        TouchLevel(destination);

        await _context.StockMovements.AddAsync(movement, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Transfer {Quantity} of product {ProductId} from warehouse {From} to {To}",
            dto.Quantity, dto.ProductId, dto.WarehouseId, dto.ToWarehouseId);

        return MapMovement(movement);
    }

    public async Task<StockMovementDto> AdjustmentAsync(AdjustmentStockDto dto, CancellationToken ct = default)
    {
        var product = await GetProductAsync(dto.ProductId, ct);

        var level = await GetOrCreateLevelAsync(dto.WarehouseId, dto.ProductId, product, ct);

        var newQuantity = dto.IsDelta ? level.QuantityOnHand + dto.Quantity : dto.Quantity;
        if (newQuantity < 0)
            throw new InvalidOperationException("Adjustment would result in negative stock.");

        var movement = new StockMovement
        {
            Id = Guid.NewGuid(),
            Type = "Adjustment",
            ProductId = dto.ProductId,
            ProductName = product?.Name,
            WarehouseId = dto.WarehouseId,
            Quantity = Math.Abs(newQuantity - level.QuantityOnHand),
            UnitCost = dto.UnitCost,
            Reference = dto.Reference,
            Reason = dto.Reason,
            Notes = dto.Notes,
            Status = "Posted",
            MovementDate = DateTime.UtcNow,
            DateAdd = DateTime.UtcNow
        };
        movement.UpdateRowVersion();

        level.QuantityOnHand = newQuantity;
        if (dto.UnitCost.HasValue)
            level.LastUnitCost = dto.UnitCost.Value;
        TouchLevel(level);

        await _context.StockMovements.AddAsync(movement, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Adjustment of product {ProductId} in warehouse {WarehouseId} to {NewQuantity}",
            dto.ProductId, dto.WarehouseId, newQuantity);

        return MapMovement(movement);
    }

    // ============= Stock counts =============

    public async Task<StockCountDto> CreateCountAsync(CreateStockCountDto dto, CancellationToken ct = default)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == dto.WarehouseId && !w.IsDeleted, ct);

        var count = new StockCount
        {
            Id = Guid.NewGuid(),
            WarehouseId = dto.WarehouseId,
            WarehouseName = warehouse?.Name,
            Status = "Open",
            ScheduledDate = dto.ScheduledDate,
            Notes = dto.Notes,
            DateAdd = DateTime.UtcNow
        };
        count.UpdateRowVersion();

        var levels = await _context.StockLevels
            .Where(s => s.WarehouseId == dto.WarehouseId)
            .ToListAsync(ct);

        var lines = levels.Select(level => new StockCountLine
        {
            Id = Guid.NewGuid(),
            StockCountId = count.Id,
            ProductId = level.ProductId,
            ProductName = level.ProductName,
            SystemQuantity = level.QuantityOnHand,
            CountedQuantity = 0,
            Variance = 0,
            DateAdd = DateTime.UtcNow
        }).ToList();

        foreach (var line in lines)
            line.UpdateRowVersion();

        await _context.StockCounts.AddAsync(count, ct);
        await _context.StockCountLines.AddRangeAsync(lines, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created stock count {CountId} for warehouse {WarehouseId} with {LineCount} lines",
            count.Id, dto.WarehouseId, lines.Count);

        return MapCount(count, lines);
    }

    public async Task<List<StockCountDto>> GetCountsAsync(CancellationToken ct = default)
    {
        var counts = await _context.StockCounts
            .AsNoTracking()
            .OrderByDescending(c => c.DateAdd)
            .ToListAsync(ct);

        return counts.Select(c => MapCount(c, null)).ToList();
    }

    public async Task<StockCountDto?> GetCountAsync(Guid id, CancellationToken ct = default)
    {
        var count = await _context.StockCounts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (count == null)
            return null;

        var lines = await _context.StockCountLines
            .AsNoTracking()
            .Where(l => l.StockCountId == id)
            .OrderBy(l => l.ProductName)
            .ToListAsync(ct);

        return MapCount(count, lines);
    }

    public async Task<StockCountDto> RecordCountAsync(Guid id, RecordStockCountDto dto, CancellationToken ct = default)
    {
        var count = await _context.StockCounts.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (count == null)
            throw new KeyNotFoundException($"Stock count with ID '{id}' not found");

        var lines = await _context.StockCountLines
            .Where(l => l.StockCountId == id)
            .ToListAsync(ct);

        foreach (var input in dto.Lines)
        {
            var line = lines.FirstOrDefault(l => l.ProductId == input.ProductId);
            if (line == null)
                continue;

            line.CountedQuantity = input.CountedQuantity;
            line.Variance = input.CountedQuantity - line.SystemQuantity;
            line.DateMod = DateTime.UtcNow;
            line.UpdateRowVersion();
        }

        count.Status = "Counted";
        count.DateMod = DateTime.UtcNow;
        count.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Recorded counted quantities for stock count {CountId}", id);

        return MapCount(count, lines);
    }

    public async Task<StockCountDto> ReconcileCountAsync(Guid id, CancellationToken ct = default)
    {
        var count = await _context.StockCounts.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (count == null)
            throw new KeyNotFoundException($"Stock count with ID '{id}' not found");

        var lines = await _context.StockCountLines
            .Where(l => l.StockCountId == id)
            .ToListAsync(ct);

        foreach (var line in lines)
        {
            if (line.Variance == 0)
                continue;

            var product = await GetProductAsync(line.ProductId, ct);
            var level = await GetOrCreateLevelAsync(count.WarehouseId, line.ProductId, product, ct);

            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                Type = "Adjustment",
                ProductId = line.ProductId,
                ProductName = line.ProductName ?? product?.Name,
                WarehouseId = count.WarehouseId,
                Quantity = Math.Abs(line.Variance),
                Reason = $"Stock count reconciliation ({count.Id})",
                Reference = count.Id.ToString(),
                Status = "Posted",
                MovementDate = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow
            };
            movement.UpdateRowVersion();

            level.QuantityOnHand = line.CountedQuantity;
            TouchLevel(level);

            await _context.StockMovements.AddAsync(movement, ct);
        }

        count.Status = "Reconciled";
        count.DateMod = DateTime.UtcNow;
        count.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Reconciled stock count {CountId}", id);

        return MapCount(count, lines);
    }

    // ============= Helpers =============

    private async Task<Product?> GetProductAsync(Guid productId, CancellationToken ct)
    {
        return await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, ct);
    }

    private async Task<StockLevel?> GetLevelAsync(Guid warehouseId, Guid productId, CancellationToken ct)
    {
        return await _context.StockLevels
            .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.ProductId == productId, ct);
    }

    private async Task<StockLevel> GetOrCreateLevelAsync(Guid warehouseId, Guid productId, Product? product, CancellationToken ct)
    {
        var level = await GetLevelAsync(warehouseId, productId, ct);
        if (level != null)
            return level;

        level = new StockLevel
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            ProductId = productId,
            ProductCode = product?.Sku,
            ProductName = product?.Name,
            QuantityOnHand = 0,
            DateAdd = DateTime.UtcNow
        };
        level.UpdateRowVersion();

        await _context.StockLevels.AddAsync(level, ct);
        return level;
    }

    private static void TouchLevel(StockLevel level)
    {
        level.DateMod = DateTime.UtcNow;
        level.UpdateRowVersion();
    }

    private static StockMovementDto MapMovement(StockMovement m) => new()
    {
        Id = m.Id,
        Type = m.Type,
        ProductId = m.ProductId,
        ProductName = m.ProductName,
        WarehouseId = m.WarehouseId,
        ToWarehouseId = m.ToWarehouseId,
        Quantity = m.Quantity,
        UnitCost = m.UnitCost,
        Reference = m.Reference,
        Reason = m.Reason,
        Notes = m.Notes,
        Status = m.Status,
        MovementDate = m.MovementDate,
        DateAdd = m.DateAdd,
        DateMod = m.DateMod
    };

    private static StockCountDto MapCount(StockCount c, List<StockCountLine>? lines) => new()
    {
        Id = c.Id,
        WarehouseId = c.WarehouseId,
        WarehouseName = c.WarehouseName,
        Status = c.Status,
        ScheduledDate = c.ScheduledDate,
        Notes = c.Notes,
        DateAdd = c.DateAdd,
        DateMod = c.DateMod,
        Lines = lines?.Select(MapCountLine).ToList() ?? new List<StockCountLineDto>()
    };

    private static StockCountLineDto MapCountLine(StockCountLine l) => new()
    {
        Id = l.Id,
        StockCountId = l.StockCountId,
        ProductId = l.ProductId,
        ProductName = l.ProductName,
        SystemQuantity = l.SystemQuantity,
        CountedQuantity = l.CountedQuantity,
        Variance = l.Variance
    };
}
