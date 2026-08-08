// Models/Entities/PortalPayment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class PortalPayment : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;

    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    public Guid VendorId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty; // BankTransfer, CreditCard, Check, Cash, DigitalWallet

    [MaxLength(100)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed, Cancelled, Refunded

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime? ProcessedDate { get; set; }

    [MaxLength(100)]
    public string? ProcessedBy { get; set; }

    [MaxLength(100)]
    public string? TransactionId { get; set; }

    [MaxLength(100)]
    public string? BankName { get; set; }

    [MaxLength(50)]
    public string? AccountNumber { get; set; }

    public DateTime? CompletedDate { get; set; }

    [MaxLength(500)]
    public string? FailureReason { get; set; }

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(InvoiceId))]
    public virtual PortalInvoice? Invoice { get; set; }

    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }
}