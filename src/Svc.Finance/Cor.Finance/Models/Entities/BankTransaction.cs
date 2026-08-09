// Models/Entities/BankTransaction.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Cancelled = 3,
    Reconciled = 4,
    Failed = 5
}

public enum TransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    Transfer = 3,
    Expense = 4,
    Replenishment = 5,
    Adjustment = 6,
    OpeningBalance = 7,
    Interest = 8,
    Fee = 9
}

public class BankTransaction : BaseEntity
{
    [Required]
    public Guid BankAccountId { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = default!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = default!;

    [MaxLength(100)]
    public string? Reference { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? BalanceAfter { get; set; }

    public bool IsReconciled { get; set; } = false;

    public DateTime? ReconciliationDate { get; set; }

    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    [MaxLength(100)]
    public string? ReconciledBy { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    [MaxLength(100)]
    public string? CheckNumber { get; set; }

    [MaxLength(50)]
    public string? BankReference { get; set; }

    [Column(TypeName = "jsonb")]
    public string? MetadataJson { get; set; }

    // ✅ PeriodId - IMPORTANT for tracking which period the transaction belongs to
    public new Guid? PeriodId { get; set; }

    // Navigation
    [ForeignKey(nameof(BankAccountId))]
    public virtual BankAccount? BankAccount { get; set; }

    [ForeignKey(nameof(PeriodId))]
    public new virtual FinancialPeriod? Period { get; set; }
}