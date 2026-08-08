// src/Svc.Core/Cor.PlanDev/Models/Entities/Timeline.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class Timeline : BaseEntity
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Planning"; // Planning, Active, Completed, Cancelled

    [MaxLength(50)]
    public string? TimelineType { get; set; } // Project, Phase, Milestone, Task

    public int Order { get; set; }

    public Guid? ParentTimelineId { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Dependencies { get; set; } // JSON array of dependency IDs

    [Column(TypeName = "jsonb")]
    public string? Metadata { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }

    [ForeignKey(nameof(ParentTimelineId))]
    public virtual Timeline? ParentTimeline { get; set; }

    public virtual ICollection<Timeline> Subtimelines { get; set; } = new List<Timeline>();
}