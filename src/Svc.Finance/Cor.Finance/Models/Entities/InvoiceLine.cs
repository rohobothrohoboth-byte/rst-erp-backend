using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class InvoiceLine : BaseEntity
{
    [Required]
    public Guid InvoiceId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = default!;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxRate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    // ? PeriodId is inherited from BaseEntity
    // ? REMOVED duplicate: public Guid PeriodId { get; set; }
    // ? REMOVED duplicate: public virtual FinancialPeriod? FinancialPeriod { get; set; }

    // Navigation
    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice Invoice { get; set; } = null!;
}