using MediatR;
using Cor.PlanDev.Models.DTOs;

namespace Cor.PlanDev.Queries;

public class GetTasksByProjectQuery : IRequest<List<TaskDto>>
{
    public Guid ProjectId { get; set; }
    public string? Status { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? Priority { get; set; }
}

public class GetTaskByIdQuery : IRequest<TaskDto>
{
    public Guid Id { get; set; }
}

public class GetTaskByParentQuery : IRequest<List<TaskDto>>
{
    public Guid ParentTaskId { get; set; }
}

public class SearchTasksQuery : IRequest<List<TaskDto>>
{
    public string? SearchTerm { get; set; }
    public Guid? ProjectId { get; set; }
    public string? Status { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class GetTaskStatusSummaryQuery : IRequest<TaskStatusSummaryDto>
{
    public Guid ProjectId { get; set; }
}

public class GetTaskByAssigneeQuery : IRequest<List<TaskDto>>
{
    public Guid AssigneeId { get; set; }
    public string? Status { get; set; }
}

public class TaskStatusSummaryDto
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Blocked { get; set; }
    public int Cancelled { get; set; }
    public decimal CompletionPercentage { get; set; }
    public int TotalEstimatedHours { get; set; }
    public int TotalActualHours { get; set; }
}