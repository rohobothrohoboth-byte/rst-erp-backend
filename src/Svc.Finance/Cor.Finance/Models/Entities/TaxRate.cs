// Models/Entities/TaxRate.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class TaxRate : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [MaxLength(20)]
    public string Code { get; set; } = default!;

    [Column(TypeName = "decimal(5,2)")]
    public decimal Rate { get; set; }

    public bool IsActive { get; set; } = true;

    // ? PeriodId is inherited from BaseEntity
    // ? Period navigation is inherited from BaseEntity

    // ? If you need period-specific tax rates, you can add:
    // public Guid? PeriodId { get; set; } // Already inherited from BaseEntity
}