// Models/Entities/PortalInvoice.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PortalInvoice : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public Guid VendorId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; } // 'Draft' | 'Submitted' | 'UnderReview' | 'Approved' | 'Rejected' | 'Paid' | 'Scheduled'

    [MaxLength(200)]
    public string? SubmittedBy { get; set; }

    public DateTime? SubmittedAt { get; set; }

    [MaxLength(200)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? PaymentDate { get; set; }

    [MaxLength(100)]
    public string? PaymentReference { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }
}