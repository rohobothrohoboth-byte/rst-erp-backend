// Cor.CRM/Models/Entities/Company.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class Company : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LegalName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    [MaxLength(50)]
    public string? Size { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? FoundedYear { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Revenue { get; set; }

    public int? EmployeeCount { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }

    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    public int ContactCount { get; set; } = 0;
    public int LeadCount { get; set; } = 0;

    public bool IsActive { get; set; } = true;
   // public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}