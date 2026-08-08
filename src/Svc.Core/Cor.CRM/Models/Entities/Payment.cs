// Payment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5,
    Cancelled = 6
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    BankTransfer = 4,
    Check = 5,
    PayPal = 6,
    MobileMoney = 7,
    Other = 8
}

public class Payment : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;

    public Guid? InvoiceId { get; set; }
    public Guid? CustomerId { get; set; }

    public DateTime PaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod Method { get; set; }

    [MaxLength(100)]
    public string? ReferenceNumber { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(200)]
    public string? BankName { get; set; }

    [MaxLength(50)]
    public string? TransactionId { get; set; }

    public DateTime? ProcessedDate { get; set; }
    public DateTime? RefundedDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? RefundAmount { get; set; }

    public bool IsReconciled { get; set; } = false;
    public DateTime? ReconciledDate { get; set; }

    [MaxLength(50)]
    public string? Currency { get; set; }

    public decimal? ExchangeRate { get; set; }

    public string? MetadataJson { get; set; }

    // Navigation Properties
    [ForeignKey("InvoiceId")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
}