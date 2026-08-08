using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class JournalEntry : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Reference { get; set; } = default!;

    [Required]
    public DateTime EntryDate { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = default!;

    [MaxLength(20)]
    public string EntryType { get; set; } = default!; // 'Manual' | 'Auto' | 'Reversal' | 'Adjustment'

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDebit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCredit { get; set; }

    public bool IsPosted { get; set; } = false;
    public DateTime? PostedDate { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }

public bool IsApproved { get; set; } = false;
public DateTime? ApprovedDate { get; set; }
public string? ApprovedBy { get; set; }
public bool IsReversed { get; set; } = false;
public DateTime? ReversedDate { get; set; }
public string? ReversedBy { get; set; }
public string? RejectionReason { get; set; }

    // ? PeriodId is inherited from BaseEntity
    // ? REMOVED duplicate: public Guid PeriodId { get; set; }
    // ? REMOVED duplicate: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    public virtual ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
}