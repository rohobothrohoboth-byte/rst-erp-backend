using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;

namespace Cor.PlanDev.Queries;

public class GetTasksByProjectQueryHandler
    : IRequestHandler<GetTasksByProjectQuery, List<TaskDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetTasksByProjectQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetTasksByProjectQueryHandler(
        PlanDevDbContext context,
        ILogger<GetTasksByProjectQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = string.Format(CacheKeys.TasksByProject, request.ProjectId);
            var cached = await _cache.GetAsync<List<TaskDto>>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var query = _context.Tasks
                .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(t => t.Status == request.Status);

            if (request.AssignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);

            if (!string.IsNullOrEmpty(request.Priority))
                query = query.Where(t => t.Priority == request.Priority);

            var tasks = await query
                .OrderBy(t => t.Order)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    TaskNumber = t.TaskNumber,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUserName,
                    Status = t.Status,
                    Priority = t.Priority,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    CompletedDate = t.CompletedDate,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    Progress = t.Progress,
                    ParentTaskId = t.ParentTaskId,
                    Order = t.Order,
                    TaskType = t.TaskType,
                    DateAdd = t.DateAdd,
                    DateMod = t.DateMod
                })
                .ToListAsync(cancellationToken);

            await _cache.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(15), cancellationToken);
            return tasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks by project");
            throw;
        }
    }
}

public class GetTaskByIdQueryHandler
    : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetTaskByIdQueryHandler> _logger;
    private readonly ICacheService _cache;

    public GetTaskByIdQueryHandler(
        PlanDevDbContext context,
        ILogger<GetTaskByIdQueryHandler> logger,
        ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = string.Format(CacheKeys.TaskById, request.Id);
            var cached = await _cache.GetAsync<TaskDto>(cacheKey, cancellationToken);
            if (cached != null)
                return cached;

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID '{request.Id}' not found");

            var dto = new TaskDto
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                TaskNumber = task.TaskNumber,
                Title = task.Title,
                Description = task.Description,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUserName,
                Status = task.Status,
                Priority = task.Priority,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                CompletedDate = task.CompletedDate,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                Progress = task.Progress,
                ParentTaskId = task.ParentTaskId,
                Order = task.Order,
                TaskType = task.TaskType,
                DateAdd = task.DateAdd,
                DateMod = task.DateMod
            };

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15), cancellationToken);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task by id");
            throw;
        }
    }
}

public class SearchTasksQueryHandler
    : IRequestHandler<SearchTasksQuery, List<TaskDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<SearchTasksQueryHandler> _logger;

    public SearchTasksQueryHandler(
        PlanDevDbContext context,
        ILogger<SearchTasksQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TaskDto>> Handle(SearchTasksQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Tasks
                .Where(t => !t.IsDeleted);

            if (request.ProjectId.HasValue)
                query = query.Where(t => t.ProjectId == request.ProjectId.Value);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(t => t.Status == request.Status);

            if (request.AssignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower().Trim();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchTerm) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                    (t.AssignedToUserName != null && t.AssignedToUserName.ToLower().Contains(searchTerm))
                );
            }

            var tasks = await query
                .OrderByDescending(t => t.DateAdd)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    TaskNumber = t.TaskNumber,
                    Title = t.Title,
                    Description = t.Description,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToUserName = t.AssignedToUserName,
                    Status = t.Status,
                    Priority = t.Priority,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    CompletedDate = t.CompletedDate,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    Progress = t.Progress,
                    ParentTaskId = t.ParentTaskId,
                    Order = t.Order,
                    TaskType = t.TaskType,
                    DateAdd = t.DateAdd,
                    DateMod = t.DateMod
                })
                .ToListAsync(cancellationToken);

            return tasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tasks");
            throw;
        }
    }
}

public class GetTaskStatusSummaryQueryHandler
    : IRequestHandler<GetTaskStatusSummaryQuery, TaskStatusSummaryDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<GetTaskStatusSummaryQueryHandler> _logger;

    public GetTaskStatusSummaryQueryHandler(
        PlanDevDbContext context,
        ILogger<GetTaskStatusSummaryQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskStatusSummaryDto> Handle(GetTaskStatusSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted)
                .ToListAsync(cancellationToken);

            var total = tasks.Count;
            var pending = tasks.Count(t => t.Status == "Pending");
            var inProgress = tasks.Count(t => t.Status == "InProgress");
            var completed = tasks.Count(t => t.Status == "Completed");
            var blocked = tasks.Count(t => t.Status == "Blocked");
            var cancelled = tasks.Count(t => t.Status == "Cancelled");

            return new TaskStatusSummaryDto
            {
                Total = total,
                Pending = pending,
                InProgress = inProgress,
                Completed = completed,
                Blocked = blocked,
                Cancelled = cancelled,
                CompletionPercentage = total > 0 ? ((decimal)completed / total) * 100 : 0,
                TotalEstimatedHours = tasks.Sum(t => t.EstimatedHours),
                TotalActualHours = tasks.Sum(t => t.ActualHours)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task status summary");
            throw;
        }
    }
}