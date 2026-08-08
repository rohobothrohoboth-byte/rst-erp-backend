using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class Milestone : BaseEntity
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime TargetDate { get; set; }

    public DateTime? AchievedDate { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Achieved, Missed, Cancelled

    public int Order { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal CompletionPercentage { get; set; }

    [MaxLength(50)]
    public string? MilestoneType { get; set; } // Phase, Deliverable, Review, Approval

    [MaxLength(500)]
    public string? Deliverable { get; set; }

    public bool IsCritical { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }
}