// Models/Entities/BudgetControl.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class BudgetControl : BaseEntity
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
    public decimal AvailableAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PercentageUsed { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Exceeded, Frozen, Closed

    [MaxLength(500)]
    public string? Notes { get; set; }

    // ✅ PeriodId inherited from BaseEntity

    [ForeignKey(nameof(BudgetId))]
    public virtual Budget? Budget { get; set; }

    [ForeignKey(nameof(AccountId))]
    public virtual ChartOfAccounts? Account { get; set; }
}