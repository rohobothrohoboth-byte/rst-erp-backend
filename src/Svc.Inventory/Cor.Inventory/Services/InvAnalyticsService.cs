using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class InvAnalyticsService : IInvAnalyticsService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InvAnalyticsService> _logger;

    public InvAnalyticsService(InventoryDbContext context, ILogger<InvAnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StockSummaryDto> GetStockSummaryAsync(CancellationToken ct = default)
    {
        var levels = await _context.StockLevels.AsNoTracking().ToListAsync(ct);

        return new StockSummaryDto
        {
            ProductCount = levels.Select(l => l.ProductId).Distinct().Count(),
            TotalOnHand = levels.Sum(l => l.QuantityOnHand),
            TotalValue = levels.Sum(l => l.QuantityOnHand * (l.AverageCost ?? 0m)),
            LowStockCount = levels.Count(l => l.QuantityOnHand <= l.ReorderLevel)
        };
    }

    public async Task<MovementAnalysisDto> GetMovementAnalysisAsync(int top = 5, CancellationToken ct = default)
    {
        if (top <= 0) top = 5;

        var aggregates = await _context.StockMovements.AsNoTracking()
            .GroupBy(m => new { m.ProductId, m.ProductName })
            .Select(g => new MovementAnalysisItemDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                TotalQuantity = g.Sum(m => m.Quantity),
                MovementCount = g.Count()
            })
            .ToListAsync(ct);

        return new MovementAnalysisDto
        {
            FastMovers = aggregates
                .OrderByDescending(a => a.TotalQuantity)
                .Take(top)
                .ToList(),
            SlowMovers = aggregates
                .OrderBy(a => a.TotalQuantity)
                .Take(top)
                .ToList()
        };
    }

    public async Task<ForecastDto> GetForecastAsync(int months = 3, CancellationToken ct = default)
    {
        if (months <= 0) months = 3;

        var since = DateTime.UtcNow.AddMonths(-months);

        var outbound = await _context.StockMovements.AsNoTracking()
            .Where(m => m.Type == "Outbound" && m.MovementDate >= since)
            .GroupBy(m => new { m.ProductId, m.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                TotalOutbound = g.Sum(m => m.Quantity)
            })
            .ToListAsync(ct);

        var onHand = await _context.StockLevels.AsNoTracking()
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, OnHand = g.Sum(s => s.QuantityOnHand) })
            .ToListAsync(ct);

        var onHandMap = onHand.ToDictionary(o => o.ProductId, o => o.OnHand);

        var items = outbound.Select(o =>
        {
            var avgMonthly = Math.Round((decimal)o.TotalOutbound / months, 2);
            var currentOnHand = onHandMap.TryGetValue(o.ProductId, out var qty) ? qty : 0;

            return new ForecastItemDto
            {
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                AverageMonthlyOutbound = avgMonthly,
                ProjectedNextMonthOutbound = avgMonthly,
                CurrentOnHand = currentOnHand,
                ProjectedEndOfMonthOnHand = currentOnHand - avgMonthly
            };
        })
        .OrderByDescending(i => i.AverageMonthlyOutbound)
        .ToList();

        return new ForecastDto
        {
            MonthsAnalyzed = months,
            Items = items
        };
    }
}
