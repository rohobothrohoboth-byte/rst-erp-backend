// Models/Entities/ApprovalStep.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ApprovalStep : BaseEntity
{
    [Required]
    public int Order { get; set; }

    [Required]
    [MaxLength(100)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ApproverName { get; set; }

    public Guid? ApproverId { get; set; }

    [Required]
    public Guid PaymentApprovalChainId { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(PaymentApprovalChainId))]
    public virtual PaymentApprovalChain PaymentApprovalChain { get; set; } = null!;
}