using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

// An encumbrance/commitment against a Budget: money reserved (not yet spent) when a
// workforce plan / requisition is approved. Available = Budget.TotalAmount - SpentAmount
// - sum(active reservations). On hire/payroll it is "Consumed" (moved to SpentAmount).
public class BudgetReservation : BaseEntity
{
    [Required]
    public Guid BudgetId { get; set; }

    // The thing that owns this reservation (e.g. a workforce plan id). Reservations are
    // idempotent per (BudgetId, ReferenceId, ReferenceType).
    [Required]
    public Guid ReferenceId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ReferenceType { get; set; } = "WorkforcePlan";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    // Active, Released, Consumed
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [MaxLength(500)]
    public string? Note { get; set; }

    [ForeignKey(nameof(BudgetId))]
    public virtual Budget? Budget { get; set; }
}
