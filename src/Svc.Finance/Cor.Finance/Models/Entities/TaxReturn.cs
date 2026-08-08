// Models/Entities/TaxReturn.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class TaxReturn : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TaxType { get; set; } // 'VAT' | 'WHT' | 'Corporate' | 'Payroll'

    [MaxLength(50)]
    public string? Period { get; set; } // 'Monthly' | 'Quarterly' | 'Annually'

    [MaxLength(10)]
    public string? FiscalYear { get; set; }

    public DateTime? FilingDate { get; set; }
    public DateTime? DueDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxableAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceDue { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Pending' | 'Filed' | 'Overdue' | 'Refunded'

    [MaxLength(200)]
    public string? FiledBy { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
}