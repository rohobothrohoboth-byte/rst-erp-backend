// Queries/CostCenterQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.Finance.Models.Entities;
namespace Cor.Finance.Queries;

// ============================================================
// QUERY CLASSES (Requests)
// ============================================================

public class GetAllCostCentersQry : IRequest<List<CostCenterDto>>
{
    public bool? IsActive { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "Name";
    public string? SortDirection { get; set; } = "ASC";
}

public class GetCostCenterByIdQry : IRequest<CostCenterDto>
{
    public Guid Id { get; set; }
}

public class GetCostCentersByDepartmentQry : IRequest<List<CostCenterDto>>
{
    public Guid DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}

// ============================================================
// GET ALL COST CENTERS HANDLER
// ============================================================

public class GetAllCostCentersHandler : IRequestHandler<GetAllCostCentersQry, List<CostCenterDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetAllCostCentersHandler> _logger;
    private const string CACHE_KEY_PREFIX = "costcenters";
    private const int CACHE_DURATION_MINUTES = 10;

    public GetAllCostCentersHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetAllCostCentersHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<CostCenterDto>> Handle(GetAllCostCentersQry request, CancellationToken ct)
    {
        try
        {
            // ✅ Build cache key based on request parameters
            var cacheKey = $"{CACHE_KEY_PREFIX}:all:active:{request.IsActive ?? false}:dept:{request.DepartmentId ?? Guid.Empty}:search:{request.SearchTerm ?? "all"}:page:{request.Page}:size:{request.PageSize}";

            // ✅ Try to get from cache
            var cached = await _cache.GetAsync<List<CostCenterDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {CacheKey}", cacheKey);

            // ✅ Use AsNoTracking() for read-only queries
            var query = _context.CostCenters
                .AsNoTracking()
                .Include(x => x.Parent)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // ✅ Apply filters
            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            if (request.DepartmentId.HasValue)
                query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(searchTerm) ||
                    x.Code.ToLower().Contains(searchTerm) ||
                    (x.Description != null && x.Description.ToLower().Contains(searchTerm)));
            }

            // ✅ Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // ✅ Apply pagination
            query = query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);

            // ✅ Execute query with projection
            var result = await query
                .Select(x => new CostCenterDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    DepartmentId = x.DepartmentId,
                    BudgetHolder = x.BudgetHolder ?? string.Empty,
                    ParentId = x.ParentId,
                    ParentName = x.Parent != null ? x.Parent.Name : null,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            // ✅ Cache the result
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving cost centers");
            throw;
        }
    }

    private IQueryable<CostCenter> ApplySorting(IQueryable<CostCenter> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrEmpty(sortBy))
            sortBy = "Name";

        return (sortDirection?.ToUpper() == "ASC")
            ? query.OrderBy(x => EF.Property<object>(x, sortBy))
            : query.OrderByDescending(x => EF.Property<object>(x, sortBy));
    }
}

// ============================================================
// GET COST CENTER BY ID HANDLER
// ============================================================

public class GetCostCenterByIdHandler : IRequestHandler<GetCostCenterByIdQry, CostCenterDto>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetCostCenterByIdHandler> _logger;
    private const string CACHE_KEY_PREFIX = "costcenter";

    public GetCostCenterByIdHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetCostCenterByIdHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CostCenterDto> Handle(GetCostCenterByIdQry request, CancellationToken ct)
    {
        try
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}:{request.Id}";

            // ✅ Try to get from cache
            var cached = await _cache.GetAsync<CostCenterDto>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {CacheKey}", cacheKey);

            // ✅ Use AsNoTracking() for read-only query
            var center = await _context.CostCenters
                .AsNoTracking()
                .Include(x => x.Parent)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (center == null)
                throw new KeyNotFoundException($"Cost center with ID '{request.Id}' not found");

            var result = new CostCenterDto
            {
                Id = center.Id,
                Code = center.Code,
                Name = center.Name,
                NameAm = center.NameAm,
                Description = center.Description,
                IsActive = center.IsActive,
                DepartmentId = center.DepartmentId,
                BudgetHolder = center.BudgetHolder ?? string.Empty,
                ParentId = center.ParentId,
                ParentName = center.Parent?.Name,
                DateAdd = center.DateAdd,
                DateMod = center.DateMod
            };

            // ✅ Cache the result
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving cost center {Id}", request.Id);
            throw;
        }
    }
}

// ============================================================
// GET COST CENTERS BY DEPARTMENT HANDLER
// ============================================================

public class GetCostCentersByDepartmentHandler : IRequestHandler<GetCostCentersByDepartmentQry, List<CostCenterDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<GetCostCentersByDepartmentHandler> _logger;
    private const string CACHE_KEY_PREFIX = "costcenters:dept";

    public GetCostCentersByDepartmentHandler(
        FinanceDbContext context,
        ICacheService cache,
        ILogger<GetCostCentersByDepartmentHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<CostCenterDto>> Handle(GetCostCentersByDepartmentQry request, CancellationToken ct)
    {
        try
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}:{request.DepartmentId}:active:{request.IsActive ?? false}";

            var cached = await _cache.GetAsync<List<CostCenterDto>>(cacheKey);
            if (cached is not null)
            {
                _logger.LogDebug("📦 Cache HIT: {CacheKey}", cacheKey);
                return cached;
            }

            _logger.LogDebug("📦 Cache MISS: {CacheKey}", cacheKey);

            var query = _context.CostCenters
                .AsNoTracking()
                .Include(x => x.Parent)
                .Where(x => !x.IsDeleted && x.DepartmentId == request.DepartmentId)
                .AsQueryable();

            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);

            var result = await query
                .OrderBy(x => x.Name)
                .Select(x => new CostCenterDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    DepartmentId = x.DepartmentId,
                    BudgetHolder = x.BudgetHolder ?? string.Empty,
                    ParentId = x.ParentId,
                    ParentName = x.Parent != null ? x.Parent.Name : null,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error retrieving cost centers for department {DepartmentId}", request.DepartmentId);
            throw;
        }
    }
}