namespace Cor.PlanDev.Models.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal ActualCost { get; set; }
    public int Progress { get; set; }
    public string? ProjectType { get; set; }
    public string? Department { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public Guid? SponsorId { get; set; }
    public string? SponsorName { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTasks { get; set; }
    public int MilestoneCount { get; set; }
    public int AchievedMilestones { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? RowVersion { get; set; }
    public List<TaskDto> Tasks { get; set; } = new();
    public List<MilestoneDto> Milestones { get; set; } = new();
    public List<BudgetDto> Budgets { get; set; } = new();
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public decimal Budget { get; set; }
    public string? ProjectType { get; set; }
    public string? Department { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public Guid? SponsorId { get; set; }
    public string? SponsorName { get; set; }
}

public class UpdateProjectDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public decimal? Budget { get; set; }
    public string? ProjectType { get; set; }
    public string? Department { get; set; }
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public Guid? SponsorId { get; set; }
    public string? SponsorName { get; set; }
    public string? RowVersion { get; set; }
}

public class UpdateProjectProgressDto
{
    public Guid Id { get; set; }
    public int Progress { get; set; }
    public string? RowVersion { get; set; }
}