// Shared/Helpers/Services/CachedReferenceDataService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Enums;
using Cor.Finance.Persistence;
using Cor.Finance.Helpers; // ✅ ADD THIS IMPORT
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Shared.Helpers.Services;

public class CachedReferenceDataService
{
    private readonly FinanceDbContext _context;
    private readonly RedisCacheService _cache;
    private readonly ILogger<CachedReferenceDataService> _logger;
    private readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(30);
    private const string COST_CENTERS_CACHE_KEY = "reference:costcenters";
    private const string ACCOUNTS_CACHE_KEY = "reference:accounts";
    private const string PERIODS_CACHE_KEY = "reference:periods";
    private const int CACHE_DURATION_MINUTES = 30;

    public CachedReferenceDataService(
        FinanceDbContext context,
        RedisCacheService cache,
        ILogger<CachedReferenceDataService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // ============================================================
    // GET CACHED ACCOUNTS ✅
    // ============================================================

    public async Task<List<ChartOfAccountsDto>> GetCachedAccountsAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<ChartOfAccountsDto>>(ACCOUNTS_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Accounts ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Accounts - fetching from database");

            var accounts = await _context.ChartOfAccounts
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name : null,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);

            await _cache.SetAsync(ACCOUNTS_CACHE_KEY, accounts, _defaultCacheDuration, ct);
            _logger.LogInformation("✅ Cached {Count} accounts", accounts.Count);

            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached accounts - falling back to database");

            return await _context.ChartOfAccounts
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Code)
                .Select(x => new ChartOfAccountsDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Name ?? string.Empty,
                    NameAm = x.NameAm,
                    Description = x.Description,
                    AccountType = x.AccountType ?? string.Empty,
                    AccountSubType = x.AccountSubType,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Level = x.Level,
                    OpeningBalance = x.OpeningBalance,
                    CurrentBalance = x.CurrentBalance,
                    OpeningBalanceDate = x.OpeningBalanceDate,
                    PeriodId = x.PeriodId,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name : null,
                    DateAdd = x.DateAdd,
                    DateMod = x.DateMod
                })
                .ToListAsync(ct);
        }
    }

    // ============================================================
    // GET CACHED COST CENTERS ✅
    // ============================================================

    public async Task<List<CostCenterDto>> GetCachedCostCentersAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<CostCenterDto>>(COST_CENTERS_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Cost Centers ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Cost Centers - fetching from database");

            var costCenters = await _context.CostCenters
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Code)
                .Select(x => new CostCenterDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
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

            await _cache.SetAsync(COST_CENTERS_CACHE_KEY, costCenters, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES), ct);
            _logger.LogInformation("✅ Cached {Count} cost centers", costCenters.Count);

            return costCenters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached cost centers - falling back to database");

            return await _context.CostCenters
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Code)
                .Select(x => new CostCenterDto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
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
        }
    }

    // ============================================================
    // GET CACHED FINANCIAL PERIODS ✅
    // ============================================================

    public async Task<List<FinancialPeriodDto>> GetCachedFinancialPeriodsAsync(CancellationToken ct = default)
    {
        try
        {
            var cached = await _cache.GetAsync<List<FinancialPeriodDto>>(PERIODS_CACHE_KEY, ct);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Financial Periods ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Financial Periods - fetching from database");

            // Get data first (without converting enums in SQL)
            var periodsData = await _context.FinancialPeriods
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.StartDate,
                    x.EndDate,
                    x.IsClosed,
                    Status = x.Status,
                    PeriodType = x.PeriodType,
                    x.Notes,
                    x.DateAdd,
                    x.DateMod,
                    x.ClosedDate,
                    x.ClosedBy
                })
                .ToListAsync(ct);

            // ✅ Use PeriodHelpers.GetStatusString
            var periods = periodsData.Select(x => new FinancialPeriodDto
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsClosed = x.IsClosed,
                Status = PeriodHelpers.GetStatusString(x.IsClosed, x.Status), // ✅ FIXED
                PeriodType = x.PeriodType.ToString(),
                Notes = x.Notes,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod,
                ClosedDate = x.ClosedDate,
                ClosedBy = x.ClosedBy
            }).ToList();

            await _cache.SetAsync(PERIODS_CACHE_KEY, periods, _defaultCacheDuration, ct);
            _logger.LogInformation("✅ Cached {Count} financial periods", periods.Count);

            return periods;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached financial periods - falling back to database");

            var periodsData = await _context.FinancialPeriods
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.StartDate,
                    x.EndDate,
                    x.IsClosed,
                    Status = x.Status,
                    PeriodType = x.PeriodType,
                    x.Notes,
                    x.DateAdd,
                    x.DateMod,
                    x.ClosedDate,
                    x.ClosedBy
                })
                .ToListAsync(ct);

            // ✅ Use PeriodHelpers.GetStatusString
            return periodsData.Select(x => new FinancialPeriodDto
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsClosed = x.IsClosed,
                Status = PeriodHelpers.GetStatusString(x.IsClosed, x.Status), // ✅ FIXED
                PeriodType = x.PeriodType.ToString(),
                Notes = x.Notes,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod,
                ClosedDate = x.ClosedDate,
                ClosedBy = x.ClosedBy
            }).ToList();
        }
    }

    // ============================================================
    // CACHE INVALIDATION METHODS
    // ============================================================

    public async Task InvalidateReferenceDataAsync(CancellationToken ct = default)
    {
        var keys = new[] { ACCOUNTS_CACHE_KEY, COST_CENTERS_CACHE_KEY, PERIODS_CACHE_KEY };
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

    public async Task InvalidateAccountsCacheAsync(CancellationToken ct = default)
    {
        try
        {
            await _cache.RemoveAsync(ACCOUNTS_CACHE_KEY, ct);
            _logger.LogInformation("🗑️ Accounts cache invalidated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating accounts cache");
        }
    }

    public async Task InvalidateCostCentersCacheAsync(CancellationToken ct = default)
    {
        try
        {
            await _cache.RemoveAsync(COST_CENTERS_CACHE_KEY, ct);
            _logger.LogInformation("🗑️ Cost centers cache invalidated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cost centers cache");
        }
    }

    public async Task InvalidatePeriodsCacheAsync(CancellationToken ct = default)
    {
        try
        {
            await _cache.RemoveAsync(PERIODS_CACHE_KEY, ct);
            _logger.LogInformation("🗑️ Periods cache invalidated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating periods cache");
        }
    }
}