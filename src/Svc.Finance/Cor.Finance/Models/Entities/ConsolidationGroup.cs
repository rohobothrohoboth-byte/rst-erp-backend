// Models/Entities/ConsolidationGroup.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ConsolidationGroup : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid? ParentEntityId { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Draft' | 'InProgress' | 'Completed' | 'Approved'

    public DateTime? ConsolidationDate { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
    // ❌ REMOVED duplicate: public Guid? PeriodId { get; set; }
    // ❌ REMOVED duplicate: public virtual FinancialPeriod? Period { get; set; }

    // Financial totals
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalRevenue { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalExpenses { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalProfit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAssets { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalLiabilities { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalEquity { get; set; }

    // Navigation
    [ForeignKey(nameof(ParentEntityId))]
    public virtual Entity? ParentEntity { get; set; }

    public virtual ICollection<ConsolidationGroupEntity> Entities { get; set; } = new List<ConsolidationGroupEntity>();
    public virtual ICollection<EliminationEntry> EliminationEntries { get; set; } = new List<EliminationEntry>();
}