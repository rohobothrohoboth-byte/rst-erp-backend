// Models/Entities/DebitNote.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class DebitNote : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string NoteNumber { get; set; } = string.Empty;

    [Required]
    public DateTime NoteDate { get; set; }

    public Guid? VendorId { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? InvoiceId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string Reason { get; set; } = string.Empty; // Return, Discount, Adjustment, Damage, Overcharge

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Approved, Posted, Cancelled

    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Guid? PostedBy { get; set; }
    public DateTime? PostedAt { get; set; }

    // ✅ PeriodId is inherited from BaseEntity

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<DebitNoteLine> Lines { get; set; } = new List<DebitNoteLine>();
}

public class DebitNoteLine : BaseEntity
{
    [Required]
    public Guid DebitNoteId { get; set; }

    public Guid? ProductId { get; set; }

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

    [ForeignKey(nameof(DebitNoteId))]
    public virtual DebitNote? DebitNote { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }
}