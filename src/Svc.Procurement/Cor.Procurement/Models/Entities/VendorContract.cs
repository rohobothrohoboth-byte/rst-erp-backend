using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.Procurement.Models.Entities.Local;
namespace Cor.Procurement.Models.Entities;

public class VendorContract : BaseEntity
{
    [Required]
    public Guid VendorId { get; set; }

    [MaxLength(100)]
    public string? VendorName { get; set; }

    [MaxLength(50)]
    public string? VendorCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Type { get; set; } = "Supply"; // Service, Supply, Maintenance, Consulting

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Value { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Active, Expired, Pending, Terminated, Renewal

    public bool AutoRenew { get; set; }

    public DateTime? RenewalDate { get; set; }

    public DateTime? SignedDate { get; set; }

    public int AttachmentCount { get; set; }

    [Column(TypeName = "jsonb")]
    public string? TermsJson { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation
    [ForeignKey(nameof(VendorId))]
    public virtual Vendor? Vendor { get; set; }
}