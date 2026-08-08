using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class IFRSReport : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Standard { get; set; } = string.Empty; // 'IFRS 9' | 'IFRS 15' | 'IFRS 16' | 'IFRS 7' | 'IFRS 8'

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Period { get; set; } // 'Q1 2024', 'FY 2024', etc.

    public DateTime? ReportDate { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Generated' | 'InProgress' | 'Scheduled' | 'Error'

    [MaxLength(50)]
    public string? Format { get; set; } // 'PDF' | 'Excel' | 'HTML'

    [MaxLength(200)]
    public string? GeneratedBy { get; set; }

    [MaxLength(500)]
    public string? FilePath { get; set; }

    [MaxLength(50)]
    public string? FileSize { get; set; }

    [MaxLength(1000)]
    public string? Summary { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    public virtual ICollection<IFRSMetric> Metrics { get; set; } = new List<IFRSMetric>();
}