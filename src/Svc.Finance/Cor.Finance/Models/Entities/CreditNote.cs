// Models/Entities/CreditNote.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class CreditNote : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string NoteNumber { get; set; } = string.Empty;

    [Required]
    public DateTime NoteDate { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? VendorId { get; set; }

    public Guid? InvoiceId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string Reason { get; set; } = string.Empty; // Return, Discount, Adjustment, Cancellation

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
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<CreditNoteLine> Lines { get; set; } = new List<CreditNoteLine>();
}