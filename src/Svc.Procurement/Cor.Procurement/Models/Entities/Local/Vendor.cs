// src/Svc.Procurement/Cor.Procurement/Models/Entities/Vendor.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Procurement.Models.Entities.Local;

public class Vendor : BaseEntity
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

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(50)]
    public string? Mobile { get; set; }

    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? TaxId { get; set; }

    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    [MaxLength(50)]
    public string VendorType { get; set; } = "Supplier";

    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [MaxLength(20)]
    public string? PaymentTerms { get; set; } = "Net 30";

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    [MaxLength(200)]
    public string? BankName { get; set; }

    [MaxLength(100)]
    public string? BankAccount { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ContactPerson { get; set; }

    [Column(TypeName = "decimal(3,2)")]
    public decimal? Rating { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalSpent { get; set; }

    public int? TotalTransactions { get; set; }

    public bool IsActive { get; set; } = true;

    // Local copy tracking
    public DateTime? SyncedAt { get; set; }
    public Guid? SourceId { get; set; } // ID from Finance module

    public bool IsLocalOnly { get; set; } = false;
}