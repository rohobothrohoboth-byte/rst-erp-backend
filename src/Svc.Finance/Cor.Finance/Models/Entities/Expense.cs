using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Expense : BaseEntity
{
    [Required]
    public DateTime ExpenseDate { get; set; }

    public Guid ExpenseCategoryId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = default!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = default!;

    [MaxLength(20)]
    public string Status { get; set; } = default!; // 'Pending' | 'Approved' | 'Rejected' | 'Paid'

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
public Guid? VendorId { get; set; }

[ForeignKey(nameof(VendorId))]
public virtual Vendor? Vendor { get; set; }
    // ? PeriodId is inherited from BaseEntity
    // ? Period navigation is inherited from BaseEntity
    // ? REMOVED: public Guid PeriodId { get; set; }
    // ? REMOVED: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    [ForeignKey(nameof(ExpenseCategoryId))]
    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;
}