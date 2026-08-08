using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class Risk : BaseEntity
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Column(TypeName = "decimal(3,2)")]
    public decimal Probability { get; set; } // 0.00 - 1.00

    [Column(TypeName = "decimal(3,2)")]
    public decimal Impact { get; set; } // 0.00 - 1.00

    [Column(TypeName = "decimal(5,2)")]
    public decimal RiskScore => Probability * Impact * 100;

    [MaxLength(50)]
    public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical

    [MaxLength(50)]
    public string Status { get; set; } = "Identified"; // Identified, Mitigated, Accepted, Resolved

    [MaxLength(500)]
    public string? MitigationPlan { get; set; }

    public DateTime? IdentifiedDate { get; set; }
    public DateTime? MitigatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }
}