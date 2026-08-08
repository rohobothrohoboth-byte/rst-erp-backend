using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class ProjectTask : BaseEntity
{
    [Required]
    public Guid ProjectId { get; set; }

    [MaxLength(50)]
    public string? TaskNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Blocked, Cancelled

    [MaxLength(50)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }

    public int Progress { get; set; } // 0-100

    public Guid? ParentTaskId { get; set; }
    public int? Order { get; set; }

    [MaxLength(50)]
    public string? TaskType { get; set; } // Development, Testing, Documentation, Review

    [Column(TypeName = "jsonb")]
    public string? Metadata { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }

    [ForeignKey(nameof(ParentTaskId))]
    public virtual ProjectTask? ParentTask { get; set; }

    public virtual ICollection<ProjectTask> Subtasks { get; set; } = new List<ProjectTask>();
}