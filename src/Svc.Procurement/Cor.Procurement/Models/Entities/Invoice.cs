using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities;

public class Invoice : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public Guid PurchaseOrderId { get; set; }

    [MaxLength(50)]
    public string? PurchaseOrderNumber { get; set; }

    [Required]
    public Guid VendorId { get; set; }

    [MaxLength(200)]
    public string? VendorName { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public DateTime? ReceivedDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal NetAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Draft"; // Draft, Sent, Verified, Approved, Rejected, Paid

    [MaxLength(50)]
    public string? PaymentTerms { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    [MaxLength(100)]
    public string? PaidBy { get; set; }

    public DateTime? PaidDate { get; set; }

    public int AttachmentCount { get; set; }

    [Column(TypeName = "jsonb")]
    public string? LineItemsJson { get; set; }

    // Navigation
    [ForeignKey(nameof(PurchaseOrderId))]
    public virtual PurchaseOrder? PurchaseOrder { get; set; }
     public virtual ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}