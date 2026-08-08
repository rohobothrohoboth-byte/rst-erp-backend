using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.PlanDev.Models.Entities;

public class Budget : BaseEntity
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PlannedAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Variance => PlannedAmount - ActualAmount;

    public int PlannedQuantity { get; set; }
    public int ActualQuantity { get; set; }

    [MaxLength(50)]
    public string? Unit { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Approved, InProgress, Completed

    [MaxLength(50)]
    public string? BudgetType { get; set; } // Personnel, Equipment, Materials, Travel, Training

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }
}