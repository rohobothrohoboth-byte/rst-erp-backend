using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Commands;

// ============================================================
// CREATE TASK
// ============================================================
public class CreateTaskCommand : IRequest<TaskDto>
{
    public CreateTaskDto CreateDto { get; set; } = new();
}

// ============================================================
// UPDATE TASK
// ============================================================
public class UpdateTaskCommand : IRequest<TaskDto>
{
    public UpdateTaskDto UpdateDto { get; set; } = new();
}

// ============================================================
// DELETE TASK
// ============================================================
public class DeleteTaskCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============================================================
// UPDATE TASK STATUS
// ============================================================
public class UpdateTaskStatusCommand : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, InProgress, Completed, Blocked, Cancelled
}

// ============================================================
// UPDATE TASK PROGRESS
// ============================================================
public class UpdateTaskProgressCommand : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public int Progress { get; set; }
}

// ============================================================
// ASSIGN TASK
// ============================================================
public class AssignTaskCommand : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
}

// ============================================================
// BULK CREATE TASKS
// ============================================================
public class BulkCreateTasksCommand : IRequest<List<TaskDto>>
{
    public List<CreateTaskDto> Tasks { get; set; } = new();
}

// ============================================================
// REORDER TASKS
// ============================================================
public class ReorderTasksCommand : IRequest<bool>
{
    public Guid ProjectId { get; set; }
    public List<Guid> TaskIds { get; set; } = new();
}

// ============================================================
// UPDATE TASK HOURS
// ============================================================
public class UpdateTaskHoursCommand : IRequest<TaskDto>
{
    public Guid Id { get; set; }
    public int ActualHours { get; set; }
}