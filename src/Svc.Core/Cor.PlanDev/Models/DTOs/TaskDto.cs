namespace Cor.PlanDev.Models.DTOs;

public class TaskDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string? TaskNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public int Progress { get; set; }
    public Guid? ParentTaskId { get; set; }
    public int? Order { get; set; }
    public string? TaskType { get; set; }
    public List<TaskDto> Subtasks { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateTaskDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string Priority { get; set; } = "Medium";
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int EstimatedHours { get; set; }
    public Guid? ParentTaskId { get; set; }
    public string? TaskType { get; set; }
}

public class UpdateTaskDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? EndDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? Progress { get; set; }
    public string? RowVersion { get; set; }
}