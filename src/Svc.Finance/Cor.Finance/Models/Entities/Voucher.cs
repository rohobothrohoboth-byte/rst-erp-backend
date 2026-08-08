// Models/Entities/Voucher.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Voucher : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string VoucherNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string VoucherType { get; set; } = "Journal"; // Payment, Receipt, Journal, Contra, Transfer

    public Guid? VendorId { get; set; }

    [Required]
    public DateTime VoucherDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDebit { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCredit { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Draft"; // Draft, Pending, Approved, Posted, Rejected, Void

    // ✅ PeriodId is inherited from BaseEntity
    // ❌ REMOVED duplicate: public Guid? PeriodId { get; set; }
    // ❌ REMOVED duplicate: public virtual FinancialPeriod? Period { get; set; }

    [MaxLength(200)]
    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    [MaxLength(200)]
    public string? PostedBy { get; set; }

    public DateTime? PostedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }

    public virtual ICollection<VoucherLine> Lines { get; set; } = new List<VoucherLine>();
}