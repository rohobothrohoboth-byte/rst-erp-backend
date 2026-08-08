// Models/Entities/Receipt.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Receipt : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string ReceiptNumber { get; set; } = string.Empty;

    [Required]
    public DateTime ReceiptDate { get; set; }

    [MaxLength(20)]
    public string ReceiptType { get; set; } = "Cash"; // Cash, Bank, Transfer

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public Guid? CustomerId { get; set; }
    public Guid? VendorId { get; set; }

    public Guid? InvoiceId { get; set; }
    public Guid? PaymentId { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Reference { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Void

    public Guid? BankAccountId { get; set; }

    // ✅ PeriodId inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(BankAccountId))]
    public virtual BankAccount? BankAccount { get; set; }
}