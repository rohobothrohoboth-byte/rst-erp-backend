using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;

namespace Cor.PlanDev.Queries;

public class GetMilestoneByIdQueryHandler : IRequestHandler<GetMilestoneByIdQuery, MilestoneDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetMilestoneByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetMilestoneByIdQueryHandler(
        PlanDevDbContext context,
        ILogger<GetMilestoneByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<MilestoneDto> Handle(GetMilestoneByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"plandev:milestone:{request.Id}";
            var cached = await _cache.GetAsync<MilestoneDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var milestone = await _context.Milestones
                .Where(m => m.Id == request.Id && !m.IsDeleted)
                .Select(m => new MilestoneDto
                {
                    Id = m.Id,
                    ProjectId = m.ProjectId,
                    Name = m.Name,
                    Description = m.Description,
                    TargetDate = m.TargetDate,
                    AchievedDate = m.AchievedDate,
                    Status = m.Status,
                    Order = m.Order,
                    CompletionPercentage = m.CompletionPercentage,
                    MilestoneType = m.MilestoneType,
                    Deliverable = m.Deliverable,
                    IsCritical = m.IsCritical,
                    DateAdd = m.DateAdd,
                    DateMod = m.DateMod
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (milestone == null)
                throw new KeyNotFoundException($"Milestone with ID {request.Id} not found");

            await _cache.SetAsync(cacheKey, milestone, TimeSpan.FromMinutes(30), cancellationToken);
            return milestone;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting milestone by ID: {Id}", request.Id);
            throw;
        }
    }
}

public class GetMilestonesByProjectQueryHandler
    : IRequestHandler<GetMilestonesByProjectQuery, List<MilestoneDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetMilestonesByProjectQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetMilestonesByProjectQueryHandler(
        PlanDevDbContext context,
        ILogger<GetMilestonesByProjectQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<MilestoneDto>> Handle(GetMilestonesByProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = string.Format(CacheKeys.MilestonesByProject, request.ProjectId);
            var cached = await _cache.GetAsync<List<MilestoneDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var query = _context.Milestones
                .Where(m => m.ProjectId == request.ProjectId && !m.IsDeleted);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(m => m.Status == request.Status);

            var milestones = await query
                .OrderBy(m => m.Order)
                .Select(m => new MilestoneDto
                {
                    Id = m.Id,
                    ProjectId = m.ProjectId,
                    Name = m.Name,
                    Description = m.Description,
                    TargetDate = m.TargetDate,
                    AchievedDate = m.AchievedDate,
                    Status = m.Status,
                    Order = m.Order,
                    CompletionPercentage = m.CompletionPercentage,
                    MilestoneType = m.MilestoneType,
                    Deliverable = m.Deliverable,
                    IsCritical = m.IsCritical,
                    DateAdd = m.DateAdd,
                    DateMod = m.DateMod
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, milestones, TimeSpan.FromMinutes(15), cancellationToken);
            return milestones;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting milestones by project: {ProjectId}", request.ProjectId);
            throw;
        }
    }
}

public class GetUpcomingMilestonesQueryHandler
    : IRequestHandler<GetUpcomingMilestonesQuery, List<MilestoneDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetUpcomingMilestonesQueryHandler> _logger;

    public GetUpcomingMilestonesQueryHandler(
        PlanDevDbContext context,
        ILogger<GetUpcomingMilestonesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MilestoneDto>> Handle(GetUpcomingMilestonesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var targetDate = DateTime.UtcNow.AddDays(request.Days);

            var query = _context.Milestones
                .Where(m => !m.IsDeleted && m.Status == "Pending" && m.TargetDate <= targetDate);

            if (request.ProjectId.HasValue)
                query = query.Where(m => m.ProjectId == request.ProjectId.Value);

            return await query
                .OrderBy(m => m.TargetDate)
                .Select(m => new MilestoneDto
                {
                    Id = m.Id,
                    ProjectId = m.ProjectId,
                    Name = m.Name,
                    Description = m.Description,
                    TargetDate = m.TargetDate,
                    AchievedDate = m.AchievedDate,
                    Status = m.Status,
                    Order = m.Order,
                    CompletionPercentage = m.CompletionPercentage,
                    MilestoneType = m.MilestoneType,
                    Deliverable = m.Deliverable,
                    IsCritical = m.IsCritical,
                    DateAdd = m.DateAdd,
                    DateMod = m.DateMod
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming milestones");
            throw;
        }
    }
}

public class GetMilestoneStatusSummaryQueryHandler
    : IRequestHandler<GetMilestoneStatusSummaryQuery, MilestoneStatusSummaryDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetMilestoneStatusSummaryQueryHandler> _logger;

    public GetMilestoneStatusSummaryQueryHandler(
        PlanDevDbContext context,
        ILogger<GetMilestoneStatusSummaryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MilestoneStatusSummaryDto> Handle(GetMilestoneStatusSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var milestones = await _context.Milestones
                .Where(m => m.ProjectId == request.ProjectId && !m.IsDeleted)
                .ToListAsync(cancellationToken);

            var total = milestones.Count;
            var achieved = milestones.Count(m => m.Status == "Achieved");
            var pending = milestones.Count(m => m.Status == "Pending");
            var missed = milestones.Count(m => m.Status == "Missed");
            var cancelled = milestones.Count(m => m.Status == "Cancelled");
            var critical = milestones.Count(m => m.IsCritical);

            return new MilestoneStatusSummaryDto
            {
                Total = total,
                Achieved = achieved,
                Pending = pending,
                Missed = missed,
                Cancelled = cancelled,
                Critical = critical,
                CompletionPercentage = total > 0 ? ((decimal)achieved / total) * 100 : 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting milestone status summary");
            throw;
        }
    }
}