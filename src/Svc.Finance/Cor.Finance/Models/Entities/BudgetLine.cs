using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class BudgetLine : BaseEntity
{
    [Required]
    public Guid BudgetId { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AllocatedAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SpentAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingAmount => AllocatedAmount - SpentAmount;

    [MaxLength(500)]
    public string? Description { get; set; }

    // ? PeriodId is inherited from BaseEntity
    // ? Period navigation is inherited from BaseEntity
    // ? REMOVED: public Guid PeriodId { get; set; }
    // ? REMOVED: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    [ForeignKey(nameof(BudgetId))]
    public virtual Budget Budget { get; set; } = null!;

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccounts Account { get; set; } = null!;
}