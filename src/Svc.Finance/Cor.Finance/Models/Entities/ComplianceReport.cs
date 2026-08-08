// Models/Entities/ComplianceReport.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ComplianceReport : BaseEntity
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
    public string? Type { get; set; } // 'Internal' | 'External' | 'Audit' | 'Regulatory' | 'Management'

    [MaxLength(50)]
    public string? Category { get; set; } // 'Financial' | 'DataPrivacy' | 'Labor' | 'Environmental' | 'Industry' | 'Corporate' | 'Tax'

    [MaxLength(50)]
    public string? Status { get; set; } // 'Generated' | 'InProgress' | 'Scheduled' | 'Error'

    [MaxLength(50)]
    public string? Format { get; set; } // 'PDF' | 'Excel' | 'HTML'

    public DateTime? GeneratedDate { get; set; }

    [MaxLength(200)]
    public string? GeneratedBy { get; set; }

    [MaxLength(500)]
    public string? FilePath { get; set; }

    [MaxLength(50)]
    public string? FileSize { get; set; }

    [MaxLength(1000)]
    public string? Summary { get; set; }

    public int Findings { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public int PartiallyPassed { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal ComplianceScore { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
    // No additional PeriodId needed
}