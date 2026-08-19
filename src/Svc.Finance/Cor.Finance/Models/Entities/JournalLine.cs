using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class JournalLine : BaseEntity
{
    private string _direction = string.Empty;
    private decimal _amount;

    [Required]
    public Guid JournalEntryId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [MaxLength(10)]
    public string Direction
    {
        get => _direction;
        set
        {
            _direction = value ?? string.Empty;
            SynchronizeDebitCredit();
        }
    }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Debit { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Credit { get; private set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Journal line amount cannot be negative.");

            _amount = value;
            SynchronizeDebitCredit();
        }
    }

    [MaxLength(500)]
    public string Description { get; set; } = default!;

    // PeriodId is inherited from BaseEntity.

    [ForeignKey(nameof(JournalEntryId))]
    public virtual JournalEntry JournalEntry { get; set; } = null!;

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccounts Account { get; set; } = null!;

    private void SynchronizeDebitCredit()
    {
        if (string.Equals(_direction, "Debit", StringComparison.OrdinalIgnoreCase))
        {
            Debit = _amount;
            Credit = 0m;
        }
        else if (string.Equals(_direction, "Credit", StringComparison.OrdinalIgnoreCase))
        {
            Debit = 0m;
            Credit = _amount;
        }
        else
        {
            Debit = 0m;
            Credit = 0m;
        }
    }
}
