// Models/Entities/InvoiceAmendment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

[Table("InvoiceAmendments")]
public class InvoiceAmendment : BaseEntity
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Reason { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalSubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalTaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OriginalTotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestedSubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestedTaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RequestedTotalAmount { get; set; }

    public string? Comment { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending_Approval";

    [MaxLength(100)]
    public string RequestedBy { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }
}