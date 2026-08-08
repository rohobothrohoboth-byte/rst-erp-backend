using System.ComponentModel.DataAnnotations;

namespace Cor.Finance.Models.Entities;

public class BudgetCategory : BaseEntity
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

    [MaxLength(10)]
    public string? Color { get; set; } // Hex color code

    [MaxLength(50)]
    public string? Icon { get; set; }

    public bool IsActive { get; set; } = true;

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
    // No additional PeriodId needed
}