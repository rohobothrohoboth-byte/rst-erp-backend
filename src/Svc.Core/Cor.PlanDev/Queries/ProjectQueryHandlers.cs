using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;

namespace Cor.PlanDev.Queries;

public class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetAllProjectsQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetAllProjectsQueryHandler(
        PlanDevDbContext context,
        ILogger<GetAllProjectsQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Try cache first
            var cacheKey = $"{CacheKeys.ProjectsAll}_{request.Status}_{request.Priority}_{request.ManagerId}_{request.Department}";
            var cached = await _cache.GetAsync<List<ProjectDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Projects ({Count} items)", cached.Count);
                return cached;
            }

            _logger.LogInformation("📦 Cache MISS: Projects - fetching from database");

            var query = _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Milestones)
                .Where(p => !p.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(p => p.Status == request.Status);

            if (!string.IsNullOrEmpty(request.Priority))
                query = query.Where(p => p.Priority == request.Priority);

            if (request.ManagerId.HasValue)
                query = query.Where(p => p.ManagerId == request.ManagerId.Value);

            if (!string.IsNullOrEmpty(request.Department))
                query = query.Where(p => p.Department == request.Department);

            if (request.FromDate.HasValue)
                query = query.Where(p => p.StartDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(p => p.EndDate <= request.ToDate.Value);

            var projects = await query
                .OrderByDescending(p => p.DateAdd)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Status = p.Status,
                    Priority = p.Priority,
                    Budget = p.Budget,
                    ActualCost = p.ActualCost,
                    Progress = p.Progress,
                    ProjectType = p.ProjectType,
                    Department = p.Department,
                    ManagerId = p.ManagerId,
                    ManagerName = p.ManagerName,
                    SponsorId = p.SponsorId,
                    SponsorName = p.SponsorName,
                    CompletionDate = p.CompletionDate,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == "Completed"),
                    MilestoneCount = p.Milestones.Count,
                    AchievedMilestones = p.Milestones.Count(m => m.Status == "Achieved"),
                    DateAdd = p.DateAdd,
                    DateMod = p.DateMod,
                    RowVersion = p.RowVersion
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, projects, TimeSpan.FromMinutes(15), cancellationToken);
            _logger.LogInformation("✅ Cached {Count} projects", projects.Count);

            return projects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching projects - falling back to database");

            // Fallback - direct query without cache
            var query = _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Milestones)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(p => p.Status == request.Status);

            if (!string.IsNullOrEmpty(request.Priority))
                query = query.Where(p => p.Priority == request.Priority);

            if (request.ManagerId.HasValue)
                query = query.Where(p => p.ManagerId == request.ManagerId.Value);

            if (!string.IsNullOrEmpty(request.Department))
                query = query.Where(p => p.Department == request.Department);

            if (request.FromDate.HasValue)
                query = query.Where(p => p.StartDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(p => p.EndDate <= request.ToDate.Value);

            return await query
                .OrderByDescending(p => p.DateAdd)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Status = p.Status,
                    Priority = p.Priority,
                    Budget = p.Budget,
                    ActualCost = p.ActualCost,
                    Progress = p.Progress,
                    ProjectType = p.ProjectType,
                    Department = p.Department,
                    ManagerId = p.ManagerId,
                    ManagerName = p.ManagerName,
                    SponsorId = p.SponsorId,
                    SponsorName = p.SponsorName,
                    CompletionDate = p.CompletionDate,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == "Completed"),
                    MilestoneCount = p.Milestones.Count,
                    AchievedMilestones = p.Milestones.Count(m => m.Status == "Achieved"),
                    DateAdd = p.DateAdd,
                    DateMod = p.DateMod,
                    RowVersion = p.RowVersion
                })
                .ToListAsync(cancellationToken);
        }
    }
}

public class GetProjectByIdQueryHandler
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetProjectByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetProjectByIdQueryHandler(
        PlanDevDbContext context,
        ILogger<GetProjectByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching project by ID: {Id}", request.Id);

            // Check cache first
            var cacheKey = string.Format(CacheKeys.ProjectById, request.Id);
            var cached = await _cache.GetAsync<ProjectDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("📦 Cache HIT: Project {Id}", request.Id);
                return cached;
            }

            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Milestones)
                .Include(p => p.Budgets)
                .Include(p => p.Risks)
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

            if (project == null)
            {
                _logger.LogWarning("Project not found: {Id}", request.Id);
                throw new KeyNotFoundException($"Project with ID '{request.Id}' not found");
            }

            var dto = new ProjectDto
            {
                Id = project.Id,
                Code = project.Code,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Priority = project.Priority,
                Budget = project.Budget,
                ActualCost = project.ActualCost,
                Progress = project.Progress,
                ProjectType = project.ProjectType,
                Department = project.Department,
                ManagerId = project.ManagerId,
                ManagerName = project.ManagerName,
                SponsorId = project.SponsorId,
                SponsorName = project.SponsorName,
                CompletionDate = project.CompletionDate,
                TaskCount = project.Tasks.Count,
                CompletedTasks = project.Tasks.Count(t => t.Status == "Completed"),
                MilestoneCount = project.Milestones.Count,
                AchievedMilestones = project.Milestones.Count(m => m.Status == "Achieved"),
                DateAdd = project.DateAdd,
                DateMod = project.DateMod,
                RowVersion = project.RowVersion,
                Tasks = project.Tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    Progress = t.Progress
                }).ToList(),
                Milestones = project.Milestones.Select(m => new MilestoneDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Status = m.Status,
                    TargetDate = m.TargetDate
                }).ToList(),
                Budgets = project.Budgets.Select(b => new BudgetDto
                {
                    Id = b.Id,
                    Category = b.Category,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = b.ActualAmount
                }).ToList()
            };

            // Cache the result
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);

            _logger.LogInformation("✅ Fetched project: {ProjectCode} - {ProjectName}", project.Code, project.Name);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching project {Id}", request.Id);
            throw;
        }
    }
}

public class GetProjectByCodeQueryHandler
    : IRequestHandler<GetProjectByCodeQuery, ProjectDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetProjectByCodeQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetProjectByCodeQueryHandler(
        PlanDevDbContext context,
        ILogger<GetProjectByCodeQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ProjectDto> Handle(GetProjectByCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching project by code: {Code}", request.Code);

            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Milestones)
                .FirstOrDefaultAsync(p => p.Code == request.Code && !p.IsDeleted, cancellationToken);

            if (project == null)
            {
                _logger.LogWarning("Project not found: {Code}", request.Code);
                throw new KeyNotFoundException($"Project with code '{request.Code}' not found");
            }

            var cacheKey = string.Format(CacheKeys.ProjectById, project.Id);
            var cached = await _cache.GetAsync<ProjectDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var dto = new ProjectDto
            {
                Id = project.Id,
                Code = project.Code,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Priority = project.Priority,
                Budget = project.Budget,
                ActualCost = project.ActualCost,
                Progress = project.Progress,
                ProjectType = project.ProjectType,
                Department = project.Department,
                ManagerId = project.ManagerId,
                ManagerName = project.ManagerName,
                SponsorId = project.SponsorId,
                SponsorName = project.SponsorName,
                CompletionDate = project.CompletionDate,
                TaskCount = project.Tasks.Count,
                CompletedTasks = project.Tasks.Count(t => t.Status == "Completed"),
                MilestoneCount = project.Milestones.Count,
                AchievedMilestones = project.Milestones.Count(m => m.Status == "Achieved"),
                DateAdd = project.DateAdd,
                DateMod = project.DateMod,
                RowVersion = project.RowVersion
            };

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching project by code {Code}", request.Code);
            throw;
        }
    }
}

public class SearchProjectsQueryHandler
    : IRequestHandler<SearchProjectsQuery, List<ProjectDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<SearchProjectsQueryHandler> _logger;

    public SearchProjectsQueryHandler(
        PlanDevDbContext context,
        ILogger<SearchProjectsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ProjectDto>> Handle(SearchProjectsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Searching projects with term: {SearchTerm}", request.SearchTerm);

            var query = _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Milestones)
                .Where(p => !p.IsDeleted);

            // Apply status filter
            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(p => p.Status == request.Status);

            // Apply search term
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Code.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.Department != null && p.Department.ToLower().Contains(searchTerm))
                );
            }

            var projects = await query
                .OrderByDescending(p => p.DateAdd)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Status = p.Status,
                    Priority = p.Priority,
                    Budget = p.Budget,
                    ActualCost = p.ActualCost,
                    Progress = p.Progress,
                    ProjectType = p.ProjectType,
                    Department = p.Department,
                    ManagerId = p.ManagerId,
                    ManagerName = p.ManagerName,
                    SponsorId = p.SponsorId,
                    SponsorName = p.SponsorName,
                    CompletionDate = p.CompletionDate,
                    TaskCount = p.Tasks.Count,
                    CompletedTasks = p.Tasks.Count(t => t.Status == "Completed"),
                    MilestoneCount = p.Milestones.Count,
                    AchievedMilestones = p.Milestones.Count(m => m.Status == "Achieved"),
                    DateAdd = p.DateAdd,
                    DateMod = p.DateMod,
                    RowVersion = p.RowVersion
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("✅ Found {Count} projects matching search", projects.Count);
            return projects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching projects");
            throw;
        }
    }
}