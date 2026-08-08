using MediatR;
using Cor.PlanDev.Models.DTOs;
using Cor.PlanDev.Models.Entities;
using Cor.PlanDev.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Helpers.Services;
using Cor.PlanDev.Constants;
using Cor.PlanDev.Queries;

namespace Cor.PlanDev.Commands;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<CreateTaskCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public CreateTaskCommandHandler(
        PlanDevDbContext context,
        ILogger<CreateTaskCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new task for project: {ProjectId}", request.CreateDto.ProjectId);

            // Generate task number
            var taskNumber = await GenerateTaskNumberAsync(request.CreateDto.ProjectId, cancellationToken);

            var task = new ProjectTask
            {
                Id = Guid.NewGuid(),
                ProjectId = request.CreateDto.ProjectId,
                TaskNumber = taskNumber,
                Title = request.CreateDto.Title,
                Description = request.CreateDto.Description,
                AssignedToUserId = request.CreateDto.AssignedToUserId,
                AssignedToUserName = request.CreateDto.AssignedToUserName,
                Status = "Pending",
                Priority = request.CreateDto.Priority,
                StartDate = request.CreateDto.StartDate,
                EndDate = request.CreateDto.EndDate,
                EstimatedHours = request.CreateDto.EstimatedHours,
                ActualHours = 0,
                Progress = 0,
                ParentTaskId = request.CreateDto.ParentTaskId,
                TaskType = request.CreateDto.TaskType,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            // Set order
            var maxOrder = await _context.Tasks
                .Where(t => t.ProjectId == request.CreateDto.ProjectId && t.ParentTaskId == request.CreateDto.ParentTaskId)
                .MaxAsync(t => (int?)t.Order) ?? 0;
            task.Order = maxOrder + 1;

            task.UpdateRowVersion();

            await _context.Tasks.AddAsync(task, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.TasksByProject, request.CreateDto.ProjectId), cancellationToken);

            _logger.LogInformation("✅ Task created successfully: {TaskTitle} ({TaskNumber})", task.Title, task.TaskNumber);

            // Use MediatR to get the created task
            return await _mediator.Send(new GetTaskByIdQuery { Id = task.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            throw;
        }
    }

    private async Task<string> GenerateTaskNumberAsync(Guid projectId, CancellationToken ct)
    {
        var count = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .CountAsync(ct) + 1;

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId, ct);

        return $"{project?.Code ?? "TASK"}-{count:D3}";
    }
}

public class UpdateTaskProgressCommandHandler
    : IRequestHandler<UpdateTaskProgressCommand, TaskDto>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<UpdateTaskProgressCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public UpdateTaskProgressCommandHandler(
        PlanDevDbContext context,
        ILogger<UpdateTaskProgressCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<TaskDto> Handle(UpdateTaskProgressCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating task progress: {TaskId} to {Progress}%", request.Id, request.Progress);

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID '{request.Id}' not found");

            if (request.Progress < 0 || request.Progress > 100)
                throw new ArgumentException("Progress must be between 0 and 100");

            task.Progress = request.Progress;
            task.DateMod = DateTime.UtcNow;
            task.UpdateRowVersion();

            if (task.Progress >= 100)
            {
                task.Status = "Completed";
                task.CompletedDate = DateTime.UtcNow;
            }
            else if (task.Status == "Pending" && task.Progress > 0)
            {
                task.Status = "InProgress";
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _cache.RemoveAsync(string.Format(CacheKeys.TasksByProject, task.ProjectId), cancellationToken);
            await _cache.RemoveAsync(string.Format(CacheKeys.TaskById, task.Id), cancellationToken);

            _logger.LogInformation("✅ Task progress updated: {TaskTitle} - {Progress}%", task.Title, task.Progress);

            // Use MediatR to get the updated task
            return await _mediator.Send(new GetTaskByIdQuery { Id = task.Id }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task progress");
            throw;
        }
    }
}

// ============================================================
// ✅ ADD THIS: BULK CREATE TASKS HANDLER
// ============================================================

public class BulkCreateTasksCommandHandler
    : IRequestHandler<BulkCreateTasksCommand, List<TaskDto>>
{
    private readonly PlanDevDbContext _context;
    private readonly ILogger<BulkCreateTasksCommandHandler> _logger;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    public BulkCreateTasksCommandHandler(
        PlanDevDbContext context,
        ILogger<BulkCreateTasksCommandHandler> logger,
        ICacheService cache,
        IMediator mediator)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<List<TaskDto>> Handle(BulkCreateTasksCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var firstTask = request.Tasks.FirstOrDefault();
            if (firstTask == null)
                throw new ArgumentException("No tasks provided");

            var projectId = firstTask.ProjectId;
            _logger.LogInformation("Bulk creating {Count} tasks for project: {ProjectId}",
                request.Tasks.Count, projectId);

            // Get project code for task numbering
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            var projectCode = project?.Code ?? "TASK";

            // Get current task count
            var currentCount = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .CountAsync(cancellationToken);

            var tasks = new List<ProjectTask>();
            var order = 1;

            foreach (var dto in request.Tasks)
            {
                currentCount++;
                var taskNumber = $"{projectCode}-{currentCount:D3}";

                var task = new ProjectTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = dto.ProjectId,
                    TaskNumber = taskNumber,
                    Title = dto.Title,
                    Description = dto.Description,
                    AssignedToUserId = dto.AssignedToUserId,
                    AssignedToUserName = dto.AssignedToUserName,
                    Status = "Pending",
                    Priority = dto.Priority,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    EstimatedHours = dto.EstimatedHours,
                    ActualHours = 0,
                    Progress = 0,
                    ParentTaskId = dto.ParentTaskId,
                    TaskType = dto.TaskType,
                    Order = order,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };

                task.UpdateRowVersion();
                tasks.Add(task);
                order++;
            }

            await _context.Tasks.AddRangeAsync(tasks, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cache.RemoveAsync(string.Format(CacheKeys.TasksByProject, projectId), cancellationToken);

            _logger.LogInformation("✅ {Count} tasks created successfully", tasks.Count);

            // Return the created tasks
            var result = new List<TaskDto>();
            foreach (var task in tasks)
            {
                var dto = await _mediator.Send(new GetTaskByIdQuery { Id = task.Id }, cancellationToken);
                result.Add(dto);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk creating tasks");
            throw;
        }
    }
}