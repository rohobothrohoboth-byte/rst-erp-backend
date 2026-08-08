using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class Project : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

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
    public string Status { get; set; } = "Planning"; // Planning, Active, OnHold, Completed, Cancelled

    [MaxLength(50)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

    [Column(TypeName = "decimal(18,2)")]
    public decimal Budget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualCost { get; set; }

    public int Progress { get; set; } // 0-100

    [MaxLength(50)]
    public string? ProjectType { get; set; } // Development, Maintenance, Research, Implementation

    [MaxLength(50)]
    public string? Department { get; set; }

    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }

    public Guid? SponsorId { get; set; }
    public string? SponsorName { get; set; }

    public DateTime? CompletionDate { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Metadata { get; set; }

    // Navigation
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public virtual ICollection<Risk> Risks { get; set; } = new List<Risk>();
}