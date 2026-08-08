using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Models.Entities;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;

namespace Cor.Inventory.Services;

public class WarehouseService : IWarehouseService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<WarehouseService> _logger;
    private readonly ICacheService _cache;

    public WarehouseService(
        InventoryDbContext context,
        ILogger<WarehouseService> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct = default)
    {
        var cacheKey = "warehouses_all";
        var cached = await _cache.GetAsync<List<WarehouseDto>>(cacheKey, ct);
        if (cached != null)
            return cached;

        var warehouses = await _context.Warehouses
            .Where(w => !w.IsDeleted && w.IsActive)
            .OrderBy(w => w.Name)
            .Select(w => MapToDto(w))
            .ToListAsync(ct);

        await _cache.SetAsync(cacheKey, warehouses, TimeSpan.FromMinutes(15), ct);
        return warehouses;
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = $"warehouse_{id}";
        var cached = await _cache.GetAsync<WarehouseDto>(cacheKey, ct);
        if (cached != null)
            return cached;

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, ct);

        if (warehouse == null)
            return null;

        var dto = MapToDto(warehouse);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), ct);
        return dto;
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto dto, CancellationToken ct = default)
    {
        // Check if code exists
        var exists = await _context.Warehouses
            .AnyAsync(w => w.Code == dto.Code && !w.IsDeleted, ct);

        if (exists)
            throw new InvalidOperationException($"Warehouse with code '{dto.Code}' already exists");

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Location = dto.Location,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            ZipCode = dto.ZipCode,
            Phone = dto.Phone,
            Email = dto.Email,
            WarehouseType = dto.WarehouseType,
            Status = dto.Status ?? "Active",
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow
        };

        warehouse.UpdateRowVersion();

        await _context.Warehouses.AddAsync(warehouse, ct);
        await _context.SaveChangesAsync(ct);

        // Invalidate cache
        await _cache.RemoveAsync("warehouses_all", ct);

        _logger.LogInformation("Created warehouse: {WarehouseName} ({WarehouseCode})", warehouse.Name, warehouse.Code);

        return MapToDto(warehouse);
    }

    public async Task<WarehouseDto> UpdateWarehouseAsync(UpdateWarehouseDto dto, CancellationToken ct = default)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == dto.Id && !w.IsDeleted, ct);

        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID '{dto.Id}' not found");

        // Check for concurrency
        if (!string.IsNullOrEmpty(dto.RowVersion) && dto.RowVersion != warehouse.RowVersion)
            throw new DbUpdateConcurrencyException("The warehouse was modified by another user");

        // Update fields
        if (!string.IsNullOrEmpty(dto.Name)) warehouse.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Code))
        {
            // Check if code is taken by another warehouse
            var exists = await _context.Warehouses
                .AnyAsync(w => w.Code == dto.Code && w.Id != dto.Id && !w.IsDeleted, ct);

            if (exists)
                throw new InvalidOperationException($"Warehouse with code '{dto.Code}' already exists");

            warehouse.Code = dto.Code;
        }
        if (!string.IsNullOrEmpty(dto.Location)) warehouse.Location = dto.Location;
        if (!string.IsNullOrEmpty(dto.Address)) warehouse.Address = dto.Address;
        if (!string.IsNullOrEmpty(dto.City)) warehouse.City = dto.City;
        if (!string.IsNullOrEmpty(dto.State)) warehouse.State = dto.State;
        if (!string.IsNullOrEmpty(dto.Country)) warehouse.Country = dto.Country;
        if (!string.IsNullOrEmpty(dto.ZipCode)) warehouse.ZipCode = dto.ZipCode;
        if (!string.IsNullOrEmpty(dto.Phone)) warehouse.Phone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.Email)) warehouse.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.WarehouseType)) warehouse.WarehouseType = dto.WarehouseType;
        if (!string.IsNullOrEmpty(dto.Status)) warehouse.Status = dto.Status;
        if (dto.IsActive.HasValue) warehouse.IsActive = dto.IsActive.Value;

        warehouse.DateMod = DateTime.UtcNow;
        warehouse.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        // Invalidate cache
        await _cache.RemoveAsync("warehouses_all", ct);
        await _cache.RemoveAsync($"warehouse_{dto.Id}", ct);

        _logger.LogInformation("Updated warehouse: {WarehouseName} ({WarehouseCode})", warehouse.Name, warehouse.Code);

        return MapToDto(warehouse);
    }

    public async Task<bool> DeleteWarehouseAsync(Guid id, CancellationToken ct = default)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, ct);

        if (warehouse == null)
            return false;

        warehouse.IsDeleted = true;
        warehouse.DateMod = DateTime.UtcNow;
        warehouse.UpdateRowVersion();

        await _context.SaveChangesAsync(ct);

        // Invalidate cache
        await _cache.RemoveAsync("warehouses_all", ct);
        await _cache.RemoveAsync($"warehouse_{id}", ct);

        _logger.LogInformation("Deleted warehouse: {WarehouseName} ({WarehouseCode})", warehouse.Name, warehouse.Code);

        return true;
    }

    private static WarehouseDto MapToDto(Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Location = warehouse.Location,
            Address = warehouse.Address,
            City = warehouse.City,
            State = warehouse.State,
            Country = warehouse.Country,
            ZipCode = warehouse.ZipCode,
            Phone = warehouse.Phone,
            Email = warehouse.Email,
            WarehouseType = warehouse.WarehouseType,
            Status = warehouse.Status,
            IsActive = warehouse.IsActive,
            DateAdd = warehouse.DateAdd,
            DateMod = warehouse.DateMod
        };
    }
}