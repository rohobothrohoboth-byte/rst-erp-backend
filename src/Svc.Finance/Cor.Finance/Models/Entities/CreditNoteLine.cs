// Models/Entities/CreditNoteLine.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class CreditNoteLine : BaseEntity
{
    [Required]
    public Guid CreditNoteId { get; set; }

    public Guid? ProductId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Discount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TaxRate { get; set; }

    // ✅ PeriodId is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(CreditNoteId))]
    public virtual CreditNote? CreditNote { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }
}