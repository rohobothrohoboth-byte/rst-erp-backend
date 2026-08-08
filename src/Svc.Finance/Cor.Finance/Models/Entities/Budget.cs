using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Budget : BaseEntity
{
     public Guid BudgetCodeId { get; set; }
     public virtual BudgetCode? BudgetCode { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
     public decimal SpentAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = default!; // Draft, Active, Approved, Closed

    [MaxLength(500)]
    public string? Description { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }



    // Navigation
    public virtual ICollection<BudgetLine> Lines { get; set; } = new List<BudgetLine>();
}