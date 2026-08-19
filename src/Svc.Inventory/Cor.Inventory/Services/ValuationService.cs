using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class ValuationService : IValuationService
{
    private static readonly string[] AllowedMethods = { "AVG", "FIFO", "LIFO" };

    private readonly InventoryDbContext _context;
    private readonly ILogger<ValuationService> _logger;

    public ValuationService(InventoryDbContext context, ILogger<ValuationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ValuationMethodDto> GetMethodAsync(CancellationToken ct = default)
    {
        var setting = await _context.ValuationSettings.AsNoTracking()
            .OrderByDescending(v => v.DateAdd)
            .FirstOrDefaultAsync(ct);

        return new ValuationMethodDto { Method = setting?.Method ?? "AVG" };
    }

    public async Task<ValuationMethodDto> SetMethodAsync(ValuationMethodDto dto, CancellationToken ct = default)
    {
        var method = (dto.Method ?? "AVG").ToUpperInvariant();
        if (!AllowedMethods.Contains(method))
            throw new InvalidOperationException($"Invalid valuation method '{dto.Method}'. Allowed: AVG, FIFO, LIFO.");

        var setting = await _context.ValuationSettings
            .OrderByDescending(v => v.DateAdd)
            .FirstOrDefaultAsync(ct);

        if (setting == null)
        {
            setting = new ValuationSetting
            {
                Id = Guid.NewGuid(),
                Method = method,
                DateAdd = DateTime.UtcNow
            };
            setting.UpdateRowVersion();
            await _context.ValuationSettings.AddAsync(setting, ct);
        }
        else
        {
            setting.Method = method;
            setting.DateMod = DateTime.UtcNow;
            setting.UpdateRowVersion();
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Valuation method set to {Method}", method);
        return new ValuationMethodDto { Method = method };
    }

    public async Task<ValuationReportDto> GetReportAsync(Guid? warehouseId, CancellationToken ct = default)
    {
        var method = await GetMethodAsync(ct);

        var query = _context.StockLevels.AsNoTracking().AsQueryable();
        if (warehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == warehouseId.Value);

        var levels = await query.OrderBy(s => s.ProductName).ToListAsync(ct);

        var lines = levels.Select(s =>
        {
            var avg = s.AverageCost ?? 0m;
            return new ValuationReportLineDto
            {
                ProductId = s.ProductId,
                ProductName = s.ProductName,
                WarehouseId = s.WarehouseId,
                QuantityOnHand = s.QuantityOnHand,
                AverageCost = avg,
                Value = s.QuantityOnHand * avg
            };
        }).ToList();

        return new ValuationReportDto
        {
            Method = method.Method,
            WarehouseId = warehouseId,
            Lines = lines,
            TotalQuantity = lines.Sum(l => l.QuantityOnHand),
            TotalValue = lines.Sum(l => l.Value)
        };
    }
}
