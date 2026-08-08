// Models/Entities/InternalOrder.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class InternalOrder : BaseEntity
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
    public string? Type { get; set; } // 'Investment' | 'Maintenance' | 'Project' | 'Event' | 'Research'

    [Column(TypeName = "decimal(18,2)")]
    public decimal BudgetAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ActualAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CommittedAmount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [MaxLength(20)]
    public string? Priority { get; set; } // 'Low' | 'Medium' | 'High' | 'Critical'

    [MaxLength(50)]
    public string? Status { get; set; } // 'Planning' | 'Active' | 'Completed' | 'Cancelled' | 'Closed'

    [MaxLength(200)]
    public string? ResponsiblePerson { get; set; }

    [MaxLength(200)]
    public string? ProjectManager { get; set; }

    public Guid? CostCenterId { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ❌ REMOVED duplicate: public Guid? PeriodId { get; set; }
    // ❌ REMOVED duplicate: public virtual FinancialPeriod? Period { get; set; }

    // Navigation
    [ForeignKey(nameof(CostCenterId))]
    public virtual CostCenter? CostCenter { get; set; }
}