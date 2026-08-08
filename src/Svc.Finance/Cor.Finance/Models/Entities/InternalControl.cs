// Models/Entities/InternalControl.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class InternalControl : BaseEntity
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
    public string? Type { get; set; } // 'Preventive' | 'Detective' | 'Corrective'

    [MaxLength(50)]
    public string? Category { get; set; } // 'Financial' | 'Operational' | 'Compliance' | 'IT' | 'Fraud'

    [MaxLength(50)]
    public string? Frequency { get; set; } // 'Continuous' | 'Daily' | 'Weekly' | 'Monthly' | 'Quarterly' | 'Annually'

    [MaxLength(200)]
    public string? Owner { get; set; }

    [MaxLength(200)]
    public string? Department { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Active' | 'Inactive' | 'UnderReview' | 'RequiresUpdate'

    [MaxLength(50)]
    public string? Effectiveness { get; set; } // 'High' | 'Medium' | 'Low' | 'NotTested'

    public DateTime? LastTestedDate { get; set; }
    public DateTime? NextTestDate { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
}