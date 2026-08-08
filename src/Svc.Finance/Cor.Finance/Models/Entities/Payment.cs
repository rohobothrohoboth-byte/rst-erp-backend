// Models/Entities/Payment.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Payment : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = default!;

    [Required]
    public DateTime PaymentDate { get; set; }

    [MaxLength(20)]
    public string PaymentType { get; set; } = default!; // "Purchase" (AP) or "Sales" (AR)

    [MaxLength(20)]
    public string PaymentMethod { get; set; } = default!; // 'Cash' | 'Bank' | 'Check' | 'Transfer'

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Reference { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = default!; // 'Draft' | 'Pending' | 'Approved' | 'Completed' | 'Cancelled'

    // AP - Paying vendors
    public Guid? VendorId { get; set; }

    // AR - Receiving from customers
    public Guid? CustomerId { get; set; }

    // Invoice linking
    public Guid? InvoiceId { get; set; }

    // Journal Entry linking
    public Guid? JournalEntryId { get; set; }

    // Bank Account
    public Guid? BankAccountId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? EmployeeId { get; set; }

    // ? PeriodId is inherited from BaseEntity
    // ? REMOVED duplicate: public Guid PeriodId { get; set; }
    // ? REMOVED duplicate: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey(nameof(JournalEntryId))]
    public virtual JournalEntry? JournalEntry { get; set; }

    [ForeignKey(nameof(BankAccountId))]
    public virtual BankAccount? BankAccount { get; set; }
}