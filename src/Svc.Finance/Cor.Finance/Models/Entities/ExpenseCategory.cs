using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class ExpenseCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [MaxLength(100)]
    public string? NameAm { get; set; }

    [MaxLength(20)]
    public string CategoryType { get; set; } = default!; // 'Operating' | 'Capital' | 'Administrative' | 'Selling'

    public bool IsActive { get; set; } = true;

    // ? PeriodId is inherited from BaseEntity
    // ? Period navigation is inherited from BaseEntity
    // No additional PeriodId needed
}