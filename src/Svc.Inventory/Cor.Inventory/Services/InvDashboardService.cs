using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class InvDashboardService : IInvDashboardService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InvDashboardService> _logger;

    public InvDashboardService(InventoryDbContext context, ILogger<InvDashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken ct = default)
    {
        var productCount = await _context.Products.AsNoTracking().CountAsync(ct);
        var warehouseCount = await _context.Warehouses.AsNoTracking().CountAsync(ct);

        var levels = await _context.StockLevels.AsNoTracking().ToListAsync(ct);
        var totalStockValue = levels.Sum(l => l.QuantityOnHand * (l.AverageCost ?? 0m));
        var lowStockCount = levels.Count(l => l.QuantityOnHand <= l.ReorderLevel);

        var recentMovements = await _context.StockMovements.AsNoTracking()
            .OrderByDescending(m => m.MovementDate)
            .Take(10)
            .ToListAsync(ct);

        return new DashboardStatsDto
        {
            ProductCount = productCount,
            WarehouseCount = warehouseCount,
            TotalStockValue = totalStockValue,
            LowStockCount = lowStockCount,
            RecentMovements = recentMovements.Select(MapMovement).ToList()
        };
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
}
