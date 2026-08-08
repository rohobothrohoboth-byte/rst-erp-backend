// Models/Entities/PaymentApprovalChain.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PaymentApprovalChain : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string PaymentType { get; set; } = string.Empty; // All, Petty Cash, Supplier, Employee, Contractor

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxAmount { get; set; }

    public bool IsActive { get; set; } = true;

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    public virtual ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
}