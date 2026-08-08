// Models/Entities/Vendor.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Vendor : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; } // Amharic name

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
    public string VendorType { get; set; } = "Supplier"; // Supplier, Contractor, Consultant, etc.

    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Pending, Suspended

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
    public string? ContactPerson { get; set; } // JSON: { "name": "John", "title": "Manager", "email": "john@..." }

    [Column(TypeName = "decimal(3,2)")]
    public decimal? Rating { get; set; } // 1-5 stars

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalSpent { get; set; }

    public int? TotalTransactions { get; set; }

    public bool IsActive { get; set; } = true;

    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity

    // Navigation properties
    public virtual ICollection<VendorPortalUser> PortalUsers { get; set; } = new List<VendorPortalUser>();
    public virtual ICollection<PortalInvoice> Invoices { get; set; } = new List<PortalInvoice>();
    public virtual ICollection<PortalPayment> Payments { get; set; } = new List<PortalPayment>();
}