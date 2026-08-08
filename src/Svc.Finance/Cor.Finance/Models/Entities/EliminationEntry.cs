// Models/Entities/EliminationEntry.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class EliminationEntry : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Type { get; set; } // 'Intercompany' | 'Investment' | 'Dividend' | 'Other'

    // From Entity (Source)
    public Guid? FromEntityId { get; set; }

    // To Entity (Destination)
    public Guid? ToEntityId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    [Column(TypeName = "decimal(18,6)")]
    public decimal ExchangeRate { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountInReportingCurrency { get; set; }

    [MaxLength(50)]
    public string? AccountCode { get; set; }

    [MaxLength(200)]
    public string? AccountName { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Draft' | 'Posted' | 'Reversed' | 'Cancelled'

    public Guid? ConsolidationGroupId { get; set; }

    [MaxLength(50)]
    public string? Period { get; set; }

    public DateTime? PostedAt { get; set; }

    [MaxLength(200)]
    public string? PostedBy { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
    // ❌ REMOVED duplicate Period property

    // Navigation
    [ForeignKey(nameof(FromEntityId))]
    public virtual Entity? FromEntity { get; set; }

    [ForeignKey(nameof(ToEntityId))]
    public virtual Entity? ToEntity { get; set; }

    [ForeignKey(nameof(ConsolidationGroupId))]
    public virtual ConsolidationGroup? ConsolidationGroup { get; set; }
}