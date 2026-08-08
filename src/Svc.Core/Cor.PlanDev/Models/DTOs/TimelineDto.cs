// src/Svc.Core/Cor.PlanDev/Models/DTOs/TimelineDto.cs
namespace Cor.PlanDev.Models.DTOs;

public class TimelineDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Planning";
    public string? TimelineType { get; set; }
    public int Order { get; set; }
    public Guid? ParentTimelineId { get; set; }
}

public class CreateTimelineDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Status { get; set; }
    public string? TimelineType { get; set; }
    public int Order { get; set; }
    public Guid? ParentTimelineId { get; set; }
}

public class UpdateTimelineDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? TimelineType { get; set; }
    public int? Order { get; set; }
    public Guid? ParentTimelineId { get; set; }
}