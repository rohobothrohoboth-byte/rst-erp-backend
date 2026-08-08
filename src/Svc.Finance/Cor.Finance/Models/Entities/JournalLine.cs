using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class JournalLine : BaseEntity
{
    [Required]
    public Guid JournalEntryId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [MaxLength(10)]
    public string Direction { get; set; } = default!; // 'Debit' | 'Credit'

    [Column(TypeName = "decimal(18,2)")]
    public decimal Debit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Credit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = default!;

    // ? PeriodId is inherited from BaseEntity
    // ? REMOVED duplicate: public Guid PeriodId { get; set; }
    // ? REMOVED duplicate: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    [ForeignKey(nameof(JournalEntryId))]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccounts Account { get; set; } = null!;
}