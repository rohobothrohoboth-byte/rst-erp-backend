// Models/Entities/ComplianceRequirement.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ComplianceRequirement : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Regulation { get; set; } // 'SOX' | 'GDPR' | 'PCI-DSS' | 'HIPAA' | 'IFRS' | 'Local' | 'Internal'

    [MaxLength(100)]
    public string? Section { get; set; }

    [MaxLength(50)]
    public string? ComplianceStatus { get; set; } // 'Compliant' | 'NonCompliant' | 'PartiallyCompliant' | 'NotAssessed' | 'InProgress'

    public DateTime? Deadline { get; set; }

    [MaxLength(200)]
    public string? Owner { get; set; }

    [MaxLength(20)]
    public string? RiskLevel { get; set; } // 'Critical' | 'High' | 'Medium' | 'Low'

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
    // No additional PeriodId needed

    // Navigation
    public virtual ICollection<ComplianceRequirementControl> Controls { get; set; } = new List<ComplianceRequirementControl>();
    public virtual ICollection<ComplianceRequirementEvidence> Evidence { get; set; } = new List<ComplianceRequirementEvidence>();
}