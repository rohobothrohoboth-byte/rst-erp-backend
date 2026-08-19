using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;

namespace Cor.PlanDev.Queries;

public class GetBudgetsByProjectQueryHandler
    : IRequestHandler<GetBudgetsByProjectQuery, List<BudgetDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetBudgetsByProjectQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetBudgetsByProjectQueryHandler(
        PlanDevDbContext context,
        ILogger<GetBudgetsByProjectQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<BudgetDto>> Handle(GetBudgetsByProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = string.Format(CacheKeys.BudgetsByProject, request.ProjectId);
            var cached = await _cache.GetAsync<List<BudgetDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var budgets = await _context.Budgets
                .Where(b => b.ProjectId == request.ProjectId && !b.IsDeleted)
                .OrderBy(b => b.Category)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    ProjectId = b.ProjectId,
                    Category = b.Category,
                    Description = b.Description,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    PlannedQuantity = b.PlannedQuantity,
                    ActualQuantity = b.ActualQuantity,
                    Unit = b.Unit,
                    Status = b.Status,
                    BudgetType = b.BudgetType,
                    DateAdd = b.DateAdd,
                    DateMod = b.DateMod
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, budgets, TimeSpan.FromMinutes(15), cancellationToken);
            return budgets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budgets by project");
            throw;
        }
    }
}

public class GetAllBudgetsQueryHandler
    : IRequestHandler<GetAllBudgetsQuery, List<BudgetDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetAllBudgetsQueryHandler> _logger;

    public GetAllBudgetsQueryHandler(PlanDevDbContext context, ILogger<GetAllBudgetsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BudgetDto>> Handle(GetAllBudgetsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var q = _context.Budgets.Where(b => !b.IsDeleted);
            if (!string.IsNullOrWhiteSpace(request.BudgetType))
                q = q.Where(b => b.BudgetType == request.BudgetType);

            return await q
                .OrderByDescending(b => b.DateAdd)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    ProjectId = b.ProjectId,
                    Category = b.Category,
                    Description = b.Description,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    PlannedQuantity = b.PlannedQuantity,
                    ActualQuantity = b.ActualQuantity,
                    Unit = b.Unit,
                    Status = b.Status,
                    BudgetType = b.BudgetType,
                    DateAdd = b.DateAdd,
                    DateMod = b.DateMod
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all budgets");
            throw;
        }
    }
}

// FIXED: This handler was using properties that don't exist in Budget or BudgetDto
public class GetBudgetByProjectQueryHandler : IRequestHandler<GetBudgetByProjectQuery, BudgetDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetBudgetByProjectQueryHandler> _logger;

    public GetBudgetByProjectQueryHandler(PlanDevDbContext context, ILogger<GetBudgetByProjectQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BudgetDto> Handle(GetBudgetByProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var budget = await _context.Budgets
                .Where(b => b.ProjectId == request.ProjectId && !b.IsDeleted)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    ProjectId = b.ProjectId,
                    Category = b.Category,
                    Description = b.Description,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    PlannedQuantity = b.PlannedQuantity,
                    ActualQuantity = b.ActualQuantity,
                    Unit = b.Unit,
                    Status = b.Status,
                    BudgetType = b.BudgetType,
                    DateAdd = b.DateAdd,
                    DateMod = b.DateMod
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (budget == null)
                throw new KeyNotFoundException($"Budget for project {request.ProjectId} not found");

            return budget;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget by project");
            throw;
        }
    }
}

public class GetBudgetByIdQueryHandler
    : IRequestHandler<GetBudgetByIdQuery, BudgetDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetBudgetByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetBudgetByIdQueryHandler(
        PlanDevDbContext context,
        ILogger<GetBudgetByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<BudgetDto> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = string.Format(CacheKeys.BudgetById, request.Id);
            var cached = await _cache.GetAsync<BudgetDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var budget = await _context.Budgets
                .Where(b => b.Id == request.Id && !b.IsDeleted)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    ProjectId = b.ProjectId,
                    Category = b.Category,
                    Description = b.Description,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    PlannedQuantity = b.PlannedQuantity,
                    ActualQuantity = b.ActualQuantity,
                    Unit = b.Unit,
                    Status = b.Status,
                    BudgetType = b.BudgetType,
                    DateAdd = b.DateAdd,
                    DateMod = b.DateMod
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (budget == null)
                throw new KeyNotFoundException($"Budget with ID '{request.Id}' not found");

            await _cache.SetAsync(cacheKey, budget, TimeSpan.FromMinutes(15), cancellationToken);
            return budget;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget by id");
            throw;
        }
    }
}

public class GetBudgetSummaryQueryHandler
    : IRequestHandler<GetBudgetSummaryQuery, BudgetSummaryDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetBudgetSummaryQueryHandler> _logger;

    public GetBudgetSummaryQueryHandler(
        PlanDevDbContext context,
        ILogger<GetBudgetSummaryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BudgetSummaryDto> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var budgets = await _context.Budgets
                .Where(b => b.ProjectId == request.ProjectId && !b.IsDeleted)
                .ToListAsync(cancellationToken);

            var totalPlanned = budgets.Sum(b => b.PlannedAmount);
            var totalActual = budgets.Sum(b => b.ActualAmount);

            return new BudgetSummaryDto
            {
                ProjectId = request.ProjectId,
                TotalPlanned = totalPlanned,
                TotalActual = totalActual,
                TotalVariance = totalPlanned - totalActual,
                UtilizationPercentage = totalPlanned > 0 ? (totalActual / totalPlanned) * 100 : 0,
                BudgetItems = budgets.Select(b => new BudgetDto
                {
                    Id = b.Id,
                    ProjectId = b.ProjectId,
                    Category = b.Category,
                    Description = b.Description,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = b.ActualAmount,
                    Variance = b.Variance,
                    PlannedQuantity = b.PlannedQuantity,
                    ActualQuantity = b.ActualQuantity,
                    Unit = b.Unit,
                    Status = b.Status,
                    BudgetType = b.BudgetType,
                    DateAdd = b.DateAdd,
                    DateMod = b.DateMod
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget summary");
            throw;
        }
    }
}