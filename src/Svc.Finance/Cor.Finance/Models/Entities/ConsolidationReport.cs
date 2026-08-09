// Models/Entities/ConsolidationReport.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ConsolidationReport : BaseEntity
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
    public string? Type { get; set; } // 'Financial' | 'Management' | 'Custom' | 'Compliance'

    [MaxLength(50)]
    public new string? Period { get; set; } // 'Monthly' | 'Quarterly' | 'Yearly' | 'Custom'

    public Guid? ConsolidationGroupId { get; set; }

    [MaxLength(50)]
    public string? Format { get; set; } // 'PDF' | 'Excel' | 'HTML'

    [MaxLength(50)]
    public string? Status { get; set; } // 'Generated' | 'InProgress' | 'Scheduled' | 'Error'

    public DateTime? GeneratedDate { get; set; }

    [MaxLength(200)]
    public string? GeneratedBy { get; set; }

    [MaxLength(50)]
    public string? FileSize { get; set; }

    [MaxLength(500)]
    public string? FilePath { get; set; }

    [MaxLength(1000)]
    public string? Summary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAssets { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalLiabilities { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalEquity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetIncome { get; set; }

    public int Adjustments { get; set; }
    public int Eliminations { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ConsolidationGroupId))]
    public virtual ConsolidationGroup? ConsolidationGroup { get; set; }
}