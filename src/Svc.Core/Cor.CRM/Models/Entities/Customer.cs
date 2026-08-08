using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum CustomerStatus
{
    Active = 1,
    Inactive = 2,
    Lead = 3,
    Prospect = 4,
    VIP = 5,
    Archived = 6
}

public enum CustomerType
{
    Individual = 1,
    Company = 2,
    Government = 3,
    NonProfit = 4,
    Partnership = 5
}

public class Customer : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(20)]
    public string? Mobile { get; set; }

    [MaxLength(20)]
    public string? Fax { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public CustomerType Type { get; set; } = CustomerType.Individual;
    public Industry? Industry { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? AnnualRevenue { get; set; }

    public int EmployeeCount { get; set; } = 0;

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }

    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    public DateTime? FirstPurchaseDate { get; set; }
    public DateTime? LastPurchaseDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? LifetimeValue { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; }

    public bool IsActive { get; set; } = true;
    public int TotalOrders { get; set; } = 0;
    public decimal? AverageOrderValue { get; set; }

    // Navigation Properties
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}