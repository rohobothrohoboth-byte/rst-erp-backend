using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cor.Finance.Models.Entities;

public class BudgetCode : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string BudgetType { get; set; } = string.Empty; // Operating, Capital, Project, Departmental

    [Required]
    [MaxLength(10)]
    public string FiscalYear { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalAmount { get; set; }

    public bool IsActive { get; set; } = true;

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
    // No additional PeriodId needed
}