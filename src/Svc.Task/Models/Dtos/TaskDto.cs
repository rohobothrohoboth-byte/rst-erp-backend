namespace Svc.Task.Models.Dtos;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "pending";
    public string Priority { get; set; } = "medium";
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public Guid AssignedTo { get; set; }
    public Guid AssignedBy { get; set; }
    public string? Category { get; set; }
    public string? Module { get; set; }
    public DateTime? LastOverdueNotified { get; set; } // ✅ Add this
}

public class TaskAddDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "medium";
    public DateTime DueDate { get; set; }
    public string? Category { get; set; }
    public string? Module { get; set; }
    public Guid AssignedTo { get; set; }
    public Guid AssignedBy { get; set; }
}

public class TaskModDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Category { get; set; }
    public string? Module { get; set; }
    public Guid? AssignedTo { get; set; }
}

public class UpdateTaskStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class TaskStatsDto
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int InProgress { get; set; }
    public int Pending { get; set; }
    public int Overdue { get; set; }
    public int CompletionRate { get; set; }
}