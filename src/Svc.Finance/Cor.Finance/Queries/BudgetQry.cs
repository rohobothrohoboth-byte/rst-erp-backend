using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cor.Finance.Queries;

// ==================== QUERIES ====================

public class GetAllBudgetsQry : IRequest<PaginatedResponse<BudgetDto>> // ✅ Changed from List<BudgetDto>
{
    public string? Status { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? FiscalYear { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? PeriodId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "StartDate";
    public string? SortDirection { get; set; } = "DESC";
}

public class GetBudgetByIdQry : IRequest<BudgetDto>
{
    public Guid Id { get; set; }
}

public class GetBudgetsByPeriodQry : IRequest<List<BudgetDto>>
{
    public Guid PeriodId { get; set; }
}

public class GetBudgetsByBranchQry : IRequest<List<BudgetDto>>
{
    public Guid BranchId { get; set; }
    public Guid? PeriodId { get; set; }
    public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
}

// ==================== INVALIDATE CACHE COMMAND ====================

public class InvalidateCacheCommand : IRequest<bool>
{
    public string EntityType { get; set; } = string.Empty;
    public string? CacheKey { get; set; }
    public bool ClearAll { get; set; } = false;
}

// ==================== GET ALL BUDGETS HANDLER ====================

// E:\untitled46\RST_ERP\src\Svc.Finance\Cor.Finance\Queries\BudgetQueries.cs

public class GetAllBudgetsHandler : IRequestHandler<GetAllBudgetsQry, PaginatedResponse<BudgetDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllBudgetsHandler> _logger;

    public GetAllBudgetsHandler(FinanceDbContext context, ILogger<GetAllBudgetsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<BudgetDto>> Handle(GetAllBudgetsQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("📊 GetAllBudgetsHandler - Page: {Page}, PageSize: {PageSize}",
                request.PageNumber, request.PageSize);

            var query = _context.Budgets
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.BudgetCode)  // ✅ Add this - Include BudgetCode navigation
                .Where(x => !x.IsDeleted)
                .AsSplitQuery();

            // ✅ Apply filters
            if (!string.IsNullOrEmpty(request.FiscalYear))
            {
                if (int.TryParse(request.FiscalYear, out var year))
                {
                    var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var endDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                    query = query.Where(x => x.StartDate >= startDate && x.EndDate <= endDate);
                }
            }

            if (request.BranchId.HasValue)
                query = query.Where(x => x.BranchId == request.BranchId.Value);

            if (request.DepartmentId.HasValue)
                query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
            {
                var fromDateUtc = DateTime.SpecifyKind(request.FromDate.Value, DateTimeKind.Utc);
                query = query.Where(x => x.StartDate >= fromDateUtc);
            }

            if (request.ToDate.HasValue)
            {
                var toDateUtc = DateTime.SpecifyKind(request.ToDate.Value, DateTimeKind.Utc);
                query = query.Where(x => x.EndDate <= toDateUtc);
            }

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(x => x.Status == request.Status);

            // ✅ Get total count BEFORE pagination
            var totalCount = await query.CountAsync(ct);
            _logger.LogInformation("📊 Total budgets found: {Count}", totalCount);

            // ✅ Apply sorting
            query = request.SortDirection?.ToUpper() == "ASC"
                ? query.OrderBy(x => EF.Property<object>(x, request.SortBy ?? "StartDate"))
                : query.OrderByDescending(x => EF.Property<object>(x, request.SortBy ?? "StartDate"));

            // ✅ Apply pagination
            query = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            // ✅ Execute query
            var budgets = await query.ToListAsync(ct);

            // ✅ Load budget lines separately
            var budgetIds = budgets.Select(b => b.Id).ToList();
            var budgetLines = await _context.BudgetLines
                .AsNoTracking()
                .Where(l => budgetIds.Contains(l.BudgetId) && !l.IsDeleted)
                .Include(l => l.Account)
                .ToListAsync(ct);

            // ✅ Map to DTOs - FIXED: Include BudgetCode fields
            var items = budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Status = b.Status,
                TotalAmount = b.TotalAmount,

                // ✅ ADD: BudgetCode fields

                BudgetCodeId = b.BudgetCodeId,
                BudgetCode = b.BudgetCode != null ? b.BudgetCode.Code.Trim() : string.Empty,
                PeriodId = b.PeriodId,
                PeriodName = b.Period?.Name ?? "Unknown Period",
                BranchId = b.BranchId,
                DepartmentId = b.DepartmentId,
                DateAdd = b.DateAdd,
                DateMod = b.DateMod,
                RowVersion = b.RowVersion ?? "",
                Lines = budgetLines
                    .Where(l => l.BudgetId == b.Id)
                    .Select(l => new BudgetLineDto
                    {
                        Id = l.Id,
                        AccountId = l.AccountId,
                        AccountCode = l.Account?.Code,
                        AccountName = l.Account?.Name,
                        AllocatedAmount = l.AllocatedAmount,
                        SpentAmount = l.SpentAmount,
                        RemainingAmount = l.AllocatedAmount - l.SpentAmount,
                        Description = l.Description,
                        PeriodId = l.PeriodId,
                        DateAdd = l.DateAdd,
                        DateMod = l.DateMod
                    }).ToList()
            }).ToList();

            _logger.LogInformation("✅ Retrieved {Count} budgets", items.Count);

            return new PaginatedResponse<BudgetDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.PageNumber > 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets");
            throw;
        }
    }
}

// ==================== GET BUDGET BY ID HANDLER ====================

public class GetBudgetByIdHandler : IRequestHandler<GetBudgetByIdQry, BudgetDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetBudgetByIdHandler> _logger;

    public GetBudgetByIdHandler(FinanceDbContext context, ILogger<GetBudgetByIdHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BudgetDto> Handle(GetBudgetByIdQry request, CancellationToken ct)
    {
        try
        {
            var budget = await _context.Budgets
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.BudgetCode)  // ✅ Add this
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (budget == null)
                throw new InvalidOperationException($"Budget with ID '{request.Id}' not found");

            var lines = await _context.BudgetLines
                .AsNoTracking()
                .Where(x => x.BudgetId == budget.Id && !x.IsDeleted)
                .Include(x => x.Account)
                .ToListAsync(ct);

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,

                // ✅ ADD: BudgetCode fields

                 BudgetCodeId = budget.BudgetCodeId,
                 BudgetCode = budget.BudgetCode != null ? budget.BudgetCode.Code.Trim() : string.Empty,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                TotalAmount = budget.TotalAmount,
                Status = budget.Status,
                Description = budget.Description,
                PeriodId = budget.PeriodId,
                PeriodName = budget.Period?.Name,
                BranchId = budget.BranchId,
                DepartmentId = budget.DepartmentId,
                Lines = lines.Select(line => new BudgetLineDto
                {
                    Id = line.Id,
                    AccountId = line.AccountId,
                    AccountName = line.Account?.Name,
                    AccountCode = line.Account?.Code,
                    AllocatedAmount = line.AllocatedAmount,
                    SpentAmount = line.SpentAmount,
                    RemainingAmount = line.AllocatedAmount - line.SpentAmount,
                    Description = line.Description,
                    PeriodId = line.PeriodId,
                    PeriodName = budget.Period?.Name,
                    DateAdd = line.DateAdd,
                    DateMod = line.DateMod
                }).ToList(),
                DateAdd = budget.DateAdd,
                DateMod = budget.DateMod,
                RowVersion = budget.RowVersion ?? ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budget {BudgetId}", request.Id);
            throw;
        }
    }
}

// ==================== GET BUDGETS BY PERIOD HANDLER ====================

public class GetBudgetsByPeriodHandler : IRequestHandler<GetBudgetsByPeriodQry, List<BudgetDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetBudgetsByPeriodHandler> _logger;

    public GetBudgetsByPeriodHandler(FinanceDbContext context, ILogger<GetBudgetsByPeriodHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BudgetDto>> Handle(GetBudgetsByPeriodQry request, CancellationToken ct)
    {
        try
        {
            var budgets = await _context.Budgets
                .AsNoTracking()
                .Include(x => x.Period)
                .Include(x => x.BudgetCode)  // ✅ Add this
                .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(ct);

            var budgetIds = budgets.Select(b => b.Id).ToList();
            var budgetLines = await _context.BudgetLines
                .AsNoTracking()
                .Where(l => budgetIds.Contains(l.BudgetId) && !l.IsDeleted)
                .Include(l => l.Account)
                .ToListAsync(ct);

            var result = budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,

                // ✅ ADD: BudgetCode fields


                BudgetCodeId = b.BudgetCodeId,

                BudgetCode = b.BudgetCode != null ? b.BudgetCode.Code.Trim() : string.Empty,

                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalAmount = b.TotalAmount,
                Status = b.Status,
                Description = b.Description,
                PeriodId = b.PeriodId,
                PeriodName = b.Period?.Name ?? "Unknown Period",
                BranchId = b.BranchId,
                DepartmentId = b.DepartmentId,
                DateAdd = b.DateAdd,
                DateMod = b.DateMod,
                RowVersion = b.RowVersion ?? "",
                Lines = budgetLines
                    .Where(l => l.BudgetId == b.Id)
                    .Select(l => new BudgetLineDto
                    {
                        Id = l.Id,
                        AccountId = l.AccountId,
                        AccountName = l.Account?.Name,
                        AccountCode = l.Account?.Code,
                        AllocatedAmount = l.AllocatedAmount,
                        SpentAmount = l.SpentAmount,
                        RemainingAmount = l.AllocatedAmount - l.SpentAmount,
                        Description = l.Description,
                        PeriodId = l.PeriodId,
                        DateAdd = l.DateAdd,
                        DateMod = l.DateMod
                    }).ToList()
            }).ToList();

            _logger.LogInformation("✅ Retrieved {Count} budgets for period {PeriodId}", result.Count, request.PeriodId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets for period {PeriodId}", request.PeriodId);
            throw;
        }
    }
}

// ==================== GET BUDGETS BY BRANCH HANDLER ====================

public class GetBudgetsByBranchHandler : IRequestHandler<GetBudgetsByBranchQry, List<BudgetDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetBudgetsByBranchHandler> _logger;

    public GetBudgetsByBranchHandler(FinanceDbContext context, ILogger<GetBudgetsByBranchHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BudgetDto>> Handle(GetBudgetsByBranchQry request, CancellationToken ct)
    {
        try
        {
            var query = _context.Budgets
                .AsNoTracking()
                .Include(x => x.Period)
                .Where(x => x.BranchId == request.BranchId && !x.IsDeleted);

            if (request.PeriodId.HasValue)
            {
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);
            }

            var budgets = await query
                .OrderByDescending(x => x.StartDate)
                .ToListAsync(ct);

            var budgetIds = budgets.Select(b => b.Id).ToList();
            var budgetLines = await _context.BudgetLines
                .AsNoTracking()
                .Where(l => budgetIds.Contains(l.BudgetId) && !l.IsDeleted)
                .Include(l => l.Account)
                .ToListAsync(ct);

            var result = budgets.Select(b => new BudgetDto
            {
                Id = b.Id,
                Name = b.Name,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                TotalAmount = b.TotalAmount,
                Status = b.Status,
                Description = b.Description,
                PeriodId = b.PeriodId,
                PeriodName = b.Period?.Name ?? "Unknown Period",
                BranchId = b.BranchId,
                DepartmentId = b.DepartmentId,
                DateAdd = b.DateAdd,
                DateMod = b.DateMod,
                Lines = budgetLines
                    .Where(l => l.BudgetId == b.Id)
                    .Select(l => new BudgetLineDto
                    {
                        Id = l.Id,
                        AccountId = l.AccountId,
                        AccountName = l.Account?.Name,
                        AccountCode = l.Account?.Code,
                        AllocatedAmount = l.AllocatedAmount,
                        SpentAmount = l.SpentAmount,
                        RemainingAmount = l.AllocatedAmount - l.SpentAmount,
                        Description = l.Description,
                        PeriodId = l.PeriodId,
                        DateAdd = l.DateAdd,
                        DateMod = l.DateMod
                    }).ToList()
            }).ToList();

            _logger.LogInformation("? Retrieved {Count} budgets for branch {BranchId}", result.Count, request.BranchId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets for branch {BranchId}", request.BranchId);
            throw;
        }
    }
}

// ==================== INVALIDATE CACHE HANDLER ====================

public class InvalidateCacheHandler : IRequestHandler<InvalidateCacheCommand, bool>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<InvalidateCacheHandler> _logger;

    public InvalidateCacheHandler(IMemoryCache cache, ILogger<InvalidateCacheHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<bool> Handle(InvalidateCacheCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ClearAll)
            {
                // Clear all budget caches
                _cache.Remove("Budgets_");
                _cache.Remove("Budgets_Branch_");
                _cache.Remove("Budgets_Period_");
                _cache.Remove("Budget_");
                _logger.LogInformation("?? Cleared all budget caches");
            }
            else if (!string.IsNullOrEmpty(request.CacheKey))
            {
                _cache.Remove(request.CacheKey);
                _logger.LogInformation("?? Removed specific cache key: {CacheKey}", request.CacheKey);
            }
            else if (!string.IsNullOrEmpty(request.EntityType))
            {
                // Clear all caches for entity type
                _cache.Remove("Budgets_");
                _cache.Remove("Budgets_Branch_");
                _cache.Remove("Budgets_Period_");
                _cache.Remove("Budget_");
                _logger.LogInformation("?? Cleared all cache for entity: {EntityType}", request.EntityType);
            }

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
            return Task.FromResult(false);
        }
    }
}