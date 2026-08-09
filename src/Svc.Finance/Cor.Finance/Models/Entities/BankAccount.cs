// Models/Entities/BankAccount.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Finance.Models.Entities.Local;
namespace Cor.Finance.Models.Entities;

public class BankAccount : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string AccountName { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = default!;

    [MaxLength(50)]
    public string BankName { get; set; } = default!;

    [MaxLength(20)]
    public string AccountType { get; set; } = default!;

    [MaxLength(50)]
    public string? GLCode { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CurrentBalance { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableBalance { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = "USD";

    public bool IsActive { get; set; } = true;

    public bool IsDefault { get; set; } = false;

    public Guid? BranchId { get; set; }

    public Guid? AccountId { get; set; }  // Chart of Accounts reference

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? IBAN { get; set; }

    [MaxLength(100)]
    public string? SwiftCode { get; set; }

    [MaxLength(100)]
    public string? BankAddress { get; set; }

    public DateTime? LastReconciledDate { get; set; }

    public decimal? OverdraftLimit { get; set; }

    public bool IsReconciled { get; set; }

    // ✅ PeriodId for tracking which period the account was created/active
    public new Guid? PeriodId { get; set; }

    // ✅ Timestamps (BaseEntity already has DateAdd, DateMod)
    public DateTime? SyncedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public virtual LocalBranch? Branch { get; set; }

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccounts? Account { get; set; }

    public virtual ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
}