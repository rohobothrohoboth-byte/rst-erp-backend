namespace Cor.PlanDev.Models.DTOs;

public class MilestoneDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime TargetDate { get; set; }
    public DateTime? AchievedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Order { get; set; }
    public decimal CompletionPercentage { get; set; }
    public string? MilestoneType { get; set; }
    public string? Deliverable { get; set; }
    public bool IsCritical { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class CreateMilestoneDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime TargetDate { get; set; }
    public int Order { get; set; }
    public string? MilestoneType { get; set; }
    public string? Deliverable { get; set; }
    public bool IsCritical { get; set; }
}

public class UpdateMilestoneDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? TargetDate { get; set; }
    public string? Status { get; set; }
    public int? Order { get; set; }
    public string? Deliverable { get; set; }
    public bool? IsCritical { get; set; }
    public string? RowVersion { get; set; }
}