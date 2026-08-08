// Cor.CRM/Commands/TaskCommands.cs
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class TaskAddCmd : IRequest<TaskDto>
{
    public CreateTaskDto Dto { get; set; } = default!;
}

public class TaskUpdateCmd : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public UpdateTaskDto Dto { get; set; } = default!;
}

public class TaskUpdateStatusCmd : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class TaskCompleteCmd : IRequest<TaskDto>
{
    public Guid Id { get; set; }
}

public class TaskDeleteCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// TASK ADD HANDLER
// ============================================================

public class TaskAddHandler : IRequestHandler<TaskAddCmd, TaskDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public TaskAddHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<TaskDto> Handle(TaskAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            var task = new Cor.CRM.Models.Entities.Task
            {
                Id = Guid.CreateVersion7(),
                Title = request.Dto.Title,
                Description = request.Dto.Description,
                Status = !string.IsNullOrEmpty(request.Dto.Status)
                    ? Enum.Parse<TaskState>(request.Dto.Status)
                    : TaskState.Pending,
                Priority = !string.IsNullOrEmpty(request.Dto.Priority)
                    ? Enum.Parse<TaskPriority>(request.Dto.Priority)
                    : TaskPriority.Medium,
                DueDate = request.Dto.DueDate.HasValue
                    ? DateTimeHelper.EnsureUtc(request.Dto.DueDate.Value)
                    : null,
                LeadId = request.Dto.LeadId,
                CustomerId = request.Dto.CustomerId,
                OpportunityId = request.Dto.OpportunityId,
                AssignedToUserId = request.Dto.AssignedToUserId,
                IsRecurring = request.Dto.IsRecurring,
                EstimatedHours = request.Dto.EstimatedHours ?? 0,
                CreatedByUserId = userId,
                CreatedByUserName = userName,
                UpdatedByUserId = userId,
                UpdatedByUserName = userName,
                CreatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow),
                IsDeleted = false,
                IsActive = true,
                CompletionPercentage = 0
            };

            await _uow.Add(task, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Task created successfully: {Title}", task.Title);

            return MapToDto(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to create task");
            await _uow.Rollback(ct);
            throw;
        }
    }

    private TaskDto MapToDto(Cor.CRM.Models.Entities.Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            CompletedDate = task.CompletedDate,
            LeadId = task.LeadId,
            CustomerId = task.CustomerId,
            OpportunityId = task.OpportunityId,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUserName = task.AssignedToUserName,
            IsRecurring = task.IsRecurring,
            CompletionPercentage = task.CompletionPercentage,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            CreatedByUserId = task.CreatedByUserId,
            CreatedByUserName = task.CreatedByUserName,
            UpdatedByUserId = task.UpdatedByUserId,
            UpdatedByUserName = task.UpdatedByUserName
        };
    }
}

// ============================================================
// TASK UPDATE HANDLER
// ============================================================

public class TaskUpdateHandler : IRequestHandler<TaskUpdateCmd, TaskDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public TaskUpdateHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<TaskDto> Handle(TaskUpdateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var task = await _uow.Set<Cor.CRM.Models.Entities.Task>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (task == null)
                throw new DomainException($"Task with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            if (!string.IsNullOrEmpty(request.Dto.Title))
                task.Title = request.Dto.Title;

            if (request.Dto.Description != null)
                task.Description = request.Dto.Description;

            if (!string.IsNullOrEmpty(request.Dto.Status))
                task.Status = Enum.Parse<TaskState>(request.Dto.Status);

            if (!string.IsNullOrEmpty(request.Dto.Priority))
                task.Priority = Enum.Parse<TaskPriority>(request.Dto.Priority);

            if (request.Dto.DueDate.HasValue)
                task.DueDate = DateTimeHelper.EnsureUtc(request.Dto.DueDate.Value);

            if (request.Dto.LeadId.HasValue)
                task.LeadId = request.Dto.LeadId;

            if (request.Dto.CustomerId.HasValue)
                task.CustomerId = request.Dto.CustomerId;

            if (request.Dto.OpportunityId.HasValue)
                task.OpportunityId = request.Dto.OpportunityId;

            if (request.Dto.AssignedToUserId.HasValue)
                task.AssignedToUserId = request.Dto.AssignedToUserId;

            if (request.Dto.EstimatedHours.HasValue)
                task.EstimatedHours = request.Dto.EstimatedHours.Value;

            task.UpdatedByUserId = userId;
            task.UpdatedByUserName = userName;
            task.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(task);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Task updated: {Title}", task.Title);

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                LeadId = task.LeadId,
                CustomerId = task.CustomerId,
                OpportunityId = task.OpportunityId,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUserName,
                IsRecurring = task.IsRecurring,
                CompletionPercentage = task.CompletionPercentage,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUserName,
                UpdatedByUserId = task.UpdatedByUserId,
                UpdatedByUserName = task.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to update task: {TaskId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// TASK UPDATE STATUS HANDLER
// ============================================================

public class TaskUpdateStatusHandler : IRequestHandler<TaskUpdateStatusCmd, TaskDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public TaskUpdateStatusHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<TaskDto> Handle(TaskUpdateStatusCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var task = await _uow.Set<Cor.CRM.Models.Entities.Task>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (task == null)
                throw new DomainException($"Task with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            var newStatus = Enum.Parse<TaskState>(request.Status);
            task.Status = newStatus;

            if (newStatus == TaskState.Completed)
            {
                task.CompletedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
                task.CompletionPercentage = 100;
            }
            else if (newStatus == TaskState.InProgress && task.StartedDate == null)
            {
                task.StartedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            }

            task.UpdatedByUserId = userId;
            task.UpdatedByUserName = userName;
            task.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(task);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Task status updated: {Title} -> {Status}", task.Title, request.Status);

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                LeadId = task.LeadId,
                CustomerId = task.CustomerId,
                OpportunityId = task.OpportunityId,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUserName,
                IsRecurring = task.IsRecurring,
                CompletionPercentage = task.CompletionPercentage,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUserName,
                UpdatedByUserId = task.UpdatedByUserId,
                UpdatedByUserName = task.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to update task status: {TaskId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// TASK COMPLETE HANDLER
// ============================================================

public class TaskCompleteHandler : IRequestHandler<TaskCompleteCmd, TaskDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public TaskCompleteHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<TaskDto> Handle(TaskCompleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var task = await _uow.Set<Cor.CRM.Models.Entities.Task>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (task == null)
                throw new DomainException($"Task with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            task.Status = TaskState.Completed;
            task.CompletedDate = DateTimeHelper.EnsureUtc(DateTime.UtcNow);
            task.CompletionPercentage = 100;
            task.UpdatedByUserId = userId;
            task.UpdatedByUserName = userName;
            task.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(task);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Task completed: {Title}", task.Title);

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                LeadId = task.LeadId,
                CustomerId = task.CustomerId,
                OpportunityId = task.OpportunityId,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUserName,
                IsRecurring = task.IsRecurring,
                CompletionPercentage = task.CompletionPercentage,
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUserName,
                UpdatedByUserId = task.UpdatedByUserId,
                UpdatedByUserName = task.UpdatedByUserName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to complete task: {TaskId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// TASK DELETE HANDLER
// ============================================================

public class TaskDeleteHandler : IRequestHandler<TaskDeleteCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;
    private readonly IUserContextService _userContext;

    public TaskDeleteHandler(IUnitOfWork uow, ILogService logger, IUserContextService userContext)
    {
        _uow = uow;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task Handle(TaskDeleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var task = await _uow.Set<Cor.CRM.Models.Entities.Task>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (task == null)
                throw new DomainException($"Task with id [{request.Id}] NOT FOUND.");

            var userId = _userContext.GetCurrentUserId();
            var userName = _userContext.GetCurrentUserName();

            task.IsDeleted = true;
            task.IsActive = false;
            task.UpdatedByUserId = userId;
            task.UpdatedByUserName = userName;
            task.UpdatedAt = DateTimeHelper.EnsureUtc(DateTime.UtcNow);

            await _uow.Update(task);
            await _uow.Commit(ct);

            _logger.LogInformation("✅ Task deleted: {Title}", task.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to delete task: {TaskId}", request.Id);
            await _uow.Rollback(ct);
            throw;
        }
    }
}