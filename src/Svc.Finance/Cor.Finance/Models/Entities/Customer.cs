// Models/Entities/Customer.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class Customer : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameAm { get; set; }

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
    public string CustomerType { get; set; } = "Company";

    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [MaxLength(20)]
    public string? PaymentTerms { get; set; } = "Net 30";

    [MaxLength(10)]
    public string? Currency { get; set; } = "USD";

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CreditLimit { get; set; }

    [MaxLength(50)]
    public string? SalesRep { get; set; }

    [Column(TypeName = "jsonb")]
    public string? ContactPerson { get; set; }

    public bool IsActive { get; set; } = true;
    public int? CustomerKey { get; set; }
    // ✅ PeriodId is inherited from BaseEntity
    // ✅ Period navigation is inherited from BaseEntity
}