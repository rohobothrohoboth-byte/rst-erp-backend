// Models/Entities/ComplianceRequirementControl.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ComplianceRequirementControl : BaseEntity  // ✅ Inherit from BaseEntity
{
    [Required]
    public Guid ComplianceRequirementId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ControlName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? ControlType { get; set; } // 'Preventive' | 'Detective' | 'Corrective'

    [MaxLength(50)]
    public string? Frequency { get; set; } // 'Daily' | 'Weekly' | 'Monthly' | 'Quarterly' | 'Annually'

    public bool IsImplemented { get; set; } = false;

    public DateTime? ImplementationDate { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ComplianceRequirementId))]
    public virtual ComplianceRequirement ComplianceRequirement { get; set; } = null!;
}