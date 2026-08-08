// Shared/Helpers/Services/CachedReferenceDataService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cor.Inventory.Models.DTOs;
using Cor.Inventory.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.Services;

public class CachedReferenceDataService
{
    private readonly InventoryDbContext _context;
    private readonly RedisCacheService _cache;
    private readonly ILogger<CachedReferenceDataService> _logger;
    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(30);

    // Cache Keys
    private const string WAREHOUSES_CACHE_KEY = "reference:warehouses";
    private const string STOCK_LEVELS_CACHE_KEY = "reference:stocklevels";

    public CachedReferenceDataService(
        InventoryDbContext context,
        RedisCacheService cache,
        ILogger<CachedReferenceDataService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // ============================================================
    // GET CACHED WAREHOUSES ✅
    // ============================================================

    public async Task<List<WarehouseDto>> GetCachedWarehousesAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<WarehouseDto>>(WAREHOUSES_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Warehouses ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Warehouses - fetching from database");

            var warehouses = await _context.Warehouses
                .Where(w => !w.IsDeleted && w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Location = w.Location,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    Country = w.Country,
                    ZipCode = w.ZipCode,
                    Phone = w.Phone,
                    Email = w.Email,
                    WarehouseType = w.WarehouseType,
                    Status = w.Status,
                    IsActive = w.IsActive,
                    DateAdd = w.DateAdd,
                    DateMod = w.DateMod
                })
                .ToListAsync(ct);

            await _cache.SetAsync(WAREHOUSES_CACHE_KEY, warehouses, _defaultCacheDuration, ct);
            _logger.LogInformation("✅ Cached {Count} warehouses", warehouses.Count);

            return warehouses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached warehouses - falling back to database");

            return await _context.Warehouses
                .Where(w => !w.IsDeleted && w.IsActive)
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Location = w.Location,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    Country = w.Country,
                    ZipCode = w.ZipCode,
                    Phone = w.Phone,
                    Email = w.Email,
                    WarehouseType = w.WarehouseType,
                    Status = w.Status,
                    IsActive = w.IsActive,
                    DateAdd = w.DateAdd,
                    DateMod = w.DateMod
                })
                .ToListAsync(ct);
        }
    }

    // ============================================================
    // GET CACHED STOCK LEVELS ✅
    // ============================================================

    public async Task<List<StockLevelDto>> GetCachedStockLevelsAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<StockLevelDto>>(STOCK_LEVELS_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Stock Levels ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Stock Levels - fetching from database");

            var stockLevels = await _context.StockLevels
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.ProductCode)
                .Select(s => new StockLevelDto
                {
                    Id = s.Id,
                    WarehouseId = s.WarehouseId,
                    ProductId = s.ProductId,
                    ProductCode = s.ProductCode ?? string.Empty,
                    ProductName = s.ProductName ?? string.Empty,
                    QuantityOnHand = s.QuantityOnHand,
                    QuantityReserved = s.QuantityReserved,
                    QuantityAvailable = s.QuantityOnHand - s.QuantityReserved,
                    ReorderLevel = s.ReorderLevel,
                    ReorderQuantity = s.ReorderQuantity,
                    LastReceivedDate = s.LastReceivedDate,
                    LastIssuedDate = s.LastIssuedDate,
                    AverageCost = s.AverageCost,
                    LastUnitCost = s.LastUnitCost,
                    BinLocation = s.BinLocation,
                    ShelfNumber = s.ShelfNumber,
                    RackNumber = s.RackNumber,
                    DateAdd = s.DateAdd,
                    DateMod = s.DateMod
                })
                .ToListAsync(ct);

            await _cache.SetAsync(STOCK_LEVELS_CACHE_KEY, stockLevels, _defaultCacheDuration, ct);
            _logger.LogInformation("✅ Cached {Count} stock levels", stockLevels.Count);

            return stockLevels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached stock levels - falling back to database");

            return await _context.StockLevels
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.ProductCode)
                .Select(s => new StockLevelDto
                {
                    Id = s.Id,
                    WarehouseId = s.WarehouseId,
                    ProductId = s.ProductId,
                    ProductCode = s.ProductCode ?? string.Empty,
                    ProductName = s.ProductName ?? string.Empty,
                    QuantityOnHand = s.QuantityOnHand,
                    QuantityReserved = s.QuantityReserved,
                    QuantityAvailable = s.QuantityOnHand - s.QuantityReserved,
                    ReorderLevel = s.ReorderLevel,
                    ReorderQuantity = s.ReorderQuantity,
                    LastReceivedDate = s.LastReceivedDate,
                    LastIssuedDate = s.LastIssuedDate,
                    AverageCost = s.AverageCost,
                    LastUnitCost = s.LastUnitCost,
                    BinLocation = s.BinLocation,
                    ShelfNumber = s.ShelfNumber,
                    RackNumber = s.RackNumber,
                    DateAdd = s.DateAdd,
                    DateMod = s.DateMod
                })
                .ToListAsync(ct);
        }
    }

    // ============================================================
    // GET CACHED WAREHOUSE BY ID ✅
    // ============================================================

    public async Task<WarehouseDto?> GetCachedWarehouseByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var warehouses = await GetCachedWarehousesAsync(ct);
            return warehouses.FirstOrDefault(w => w.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached warehouse by ID: {Id}", id);

            return await _context.Warehouses
                .Where(w => w.Id == id && !w.IsDeleted && w.IsActive)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Code = w.Code,
                    Location = w.Location,
                    Address = w.Address,
                    City = w.City,
                    State = w.State,
                    Country = w.Country,
                    ZipCode = w.ZipCode,
                    Phone = w.Phone,
                    Email = w.Email,
                    WarehouseType = w.WarehouseType,
                    Status = w.Status,
                    IsActive = w.IsActive,
                    DateAdd = w.DateAdd,
                    DateMod = w.DateMod
                })
                .FirstOrDefaultAsync(ct);
        }
    }

    // ============================================================
    // CACHE INVALIDATION METHODS
    // ============================================================

    public async Task InvalidateReferenceDataAsync(CancellationToken ct = default)
    {
        var keys = new[] { WAREHOUSES_CACHE_KEY, STOCK_LEVELS_CACHE_KEY };
        foreach (var key in keys)
        {
            try
            {
                await _cache.RemoveAsync(key, ct);
                _logger.LogInformation("🗑️ Cache invalidated: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate cache for key: {Key}", key);
            }
        }
    }

    public async Task InvalidateWarehousesCacheAsync(CancellationToken ct = default)
    {
        try
        {
            await _cache.RemoveAsync(WAREHOUSES_CACHE_KEY, ct);
            _logger.LogInformation("🗑️ Warehouses cache invalidated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating warehouses cache");
        }
    }

    public async Task InvalidateStockLevelsCacheAsync(CancellationToken ct = default)
    {
        try
        {
            await _cache.RemoveAsync(STOCK_LEVELS_CACHE_KEY, ct);
            _logger.LogInformation("🗑️ Stock levels cache invalidated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating stock levels cache");
        }
    }
}