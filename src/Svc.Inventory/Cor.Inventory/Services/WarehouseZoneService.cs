using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cor.Inventory.Services;

public class WarehouseZoneService : IWarehouseZoneService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<WarehouseZoneService> _logger;

    public WarehouseZoneService(InventoryDbContext context, ILogger<WarehouseZoneService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ============= Zones =============

    public async Task<List<WarehouseZoneDto>> GetZonesAsync(Guid? warehouseId, CancellationToken ct = default)
    {
        var query = _context.WarehouseZones.AsNoTracking().AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(z => z.WarehouseId == warehouseId.Value);

        var zones = await query.OrderBy(z => z.Name).ToListAsync(ct);
        return zones.Select(MapZone).ToList();
    }

    public async Task<WarehouseZoneDto?> GetZoneByIdAsync(Guid id, CancellationToken ct = default)
    {
        var zone = await _context.WarehouseZones.AsNoTracking().FirstOrDefaultAsync(z => z.Id == id, ct);
        return zone == null ? null : MapZone(zone);
    }

    public async Task<WarehouseZoneDto> CreateZoneAsync(CreateWarehouseZoneDto dto, CancellationToken ct = default)
    {
        var zone = new WarehouseZone
        {
            Id = Guid.NewGuid(),
            WarehouseId = dto.WarehouseId,
            Name = dto.Name,
            Code = dto.Code,
            ZoneType = dto.ZoneType,
            Description = dto.Description,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };
        zone.UpdateRowVersion();

        await _context.WarehouseZones.AddAsync(zone, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created warehouse zone {ZoneName} for warehouse {WarehouseId}", zone.Name, zone.WarehouseId);
        return MapZone(zone);
    }

    public async Task<WarehouseZoneDto> UpdateZoneAsync(UpdateWarehouseZoneDto dto, CancellationToken ct = default)
    {
        var zone = await _context.WarehouseZones.FirstOrDefaultAsync(z => z.Id == dto.Id, ct);
        if (zone == null)
            throw new KeyNotFoundException($"Warehouse zone with ID '{dto.Id}' not found");

        if (!string.IsNullOrEmpty(dto.RowVersion) && dto.RowVersion != zone.RowVersion)
            throw new DbUpdateConcurrencyException("The warehouse zone was modified by another user");

        if (!string.IsNullOrEmpty(dto.Name)) zone.Name = dto.Name;
        if (dto.Code != null) zone.Code = dto.Code;
        if (dto.ZoneType != null) zone.ZoneType = dto.ZoneType;
        if (dto.Description != null) zone.Description = dto.Description;
        if (dto.IsActive.HasValue) zone.IsActive = dto.IsActive.Value;

        zone.DateMod = DateTime.UtcNow;
        zone.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Updated warehouse zone {ZoneId}", zone.Id);
        return MapZone(zone);
    }

    public async Task<bool> DeleteZoneAsync(Guid id, CancellationToken ct = default)
    {
        var zone = await _context.WarehouseZones.FirstOrDefaultAsync(z => z.Id == id, ct);
        if (zone == null)
            return false;

        zone.IsDeleted = true;
        zone.DateMod = DateTime.UtcNow;
        zone.UpdateRowVersion();

        // Soft-delete child bins as well.
        var bins = await _context.Bins.Where(b => b.ZoneId == id).ToListAsync(ct);
        foreach (var bin in bins)
        {
            bin.IsDeleted = true;
            bin.DateMod = DateTime.UtcNow;
            bin.UpdateRowVersion();
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted warehouse zone {ZoneId}", id);
        return true;
    }

    // ============= Bins =============

    public async Task<List<BinDto>> GetBinsAsync(Guid zoneId, CancellationToken ct = default)
    {
        var bins = await _context.Bins.AsNoTracking()
            .Where(b => b.ZoneId == zoneId)
            .OrderBy(b => b.Code)
            .ToListAsync(ct);

        return bins.Select(MapBin).ToList();
    }

    public async Task<BinDto> CreateBinAsync(Guid zoneId, CreateBinDto dto, CancellationToken ct = default)
    {
        var zone = await _context.WarehouseZones.FirstOrDefaultAsync(z => z.Id == zoneId, ct);
        if (zone == null)
            throw new KeyNotFoundException($"Warehouse zone with ID '{zoneId}' not found");

        var bin = new Bin
        {
            Id = Guid.NewGuid(),
            ZoneId = zoneId,
            Code = dto.Code,
            Description = dto.Description,
            Capacity = dto.Capacity,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };
        bin.UpdateRowVersion();

        await _context.Bins.AddAsync(bin, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Created bin {BinCode} in zone {ZoneId}", bin.Code, zoneId);
        return MapBin(bin);
    }

    // ============= Layout =============

    public async Task<WarehouseLayoutDto> GetLayoutAsync(Guid warehouseId, CancellationToken ct = default)
    {
        var zones = await _context.WarehouseZones.AsNoTracking()
            .Where(z => z.WarehouseId == warehouseId)
            .OrderBy(z => z.Name)
            .ToListAsync(ct);

        var zoneIds = zones.Select(z => z.Id).ToList();

        var bins = await _context.Bins.AsNoTracking()
            .Where(b => zoneIds.Contains(b.ZoneId))
            .OrderBy(b => b.Code)
            .ToListAsync(ct);

        var layout = new WarehouseLayoutDto
        {
            WarehouseId = warehouseId,
            Zones = zones.Select(z => new WarehouseLayoutZoneDto
            {
                Id = z.Id,
                WarehouseId = z.WarehouseId,
                Name = z.Name,
                Code = z.Code,
                ZoneType = z.ZoneType,
                Description = z.Description,
                IsActive = z.IsActive,
                Bins = bins.Where(b => b.ZoneId == z.Id).Select(MapBin).ToList()
            }).ToList()
        };

        return layout;
    }

    // ============= Helpers =============

    private static WarehouseZoneDto MapZone(WarehouseZone z) => new()
    {
        Id = z.Id,
        WarehouseId = z.WarehouseId,
        Name = z.Name,
        Code = z.Code,
        ZoneType = z.ZoneType,
        Description = z.Description,
        IsActive = z.IsActive,
        DateAdd = z.DateAdd,
        DateMod = z.DateMod
    };

    private static BinDto MapBin(Bin b) => new()
    {
        Id = b.Id,
        ZoneId = b.ZoneId,
        Code = b.Code,
        Description = b.Description,
        Capacity = b.Capacity,
        IsActive = b.IsActive,
        DateAdd = b.DateAdd,
        DateMod = b.DateMod
    };
}
