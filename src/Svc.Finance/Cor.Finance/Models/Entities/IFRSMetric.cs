using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class IFRSMetric : BaseEntity
{
    [Required]
    public Guid ReportId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Value { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PreviousValue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Change { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? ChangePercentage { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Positive' | 'Negative' | 'Neutral'

    [MaxLength(20)]
    public string? Unit { get; set; } // 'USD' | 'EUR' | 'GBP' | '%' | 'Number'

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(ReportId))]
    public virtual IFRSReport? Report { get; set; }
}