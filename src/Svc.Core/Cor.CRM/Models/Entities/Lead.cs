using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum LeadStatus
{
    New = 1,
    Contacted = 2,
    Qualified = 3,
    Proposal = 4,
    Negotiation = 5,
    Converted = 6,
    Lost = 7,
    Archived = 8
}

public enum LeadSource
{
    Website = 1,
    Referral = 2,
    SocialMedia = 3,
    Email = 4,
    ColdCall = 5,
    Event = 6,
    Partner = 7,
    Advertisement = 8,
    Other = 9
}

public enum LeadPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum Industry
{
    RealEstate = 1,
    Manufacturing = 2,
    Technology = 3,
    Healthcare = 4,
    Finance = 5,
    Education = 6,
    Government = 7,
    Retail = 8,
    Consulting = 9,
    Construction = 10,
    Energy = 11,
    Transportation = 12,
    Hospitality = 13,
    Other = 14
}

public class Lead : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

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

    public LeadStatus Status { get; set; } = LeadStatus.New;
    public LeadSource Source { get; set; } = LeadSource.Website;
    public LeadPriority Priority { get; set; } = LeadPriority.Medium;
    public Industry? Industry { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedValue { get; set; }

    public DateTime? ExpectedCloseDate { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public Guid? AssignedToGroupId { get; set; }

    [MaxLength(200)]
    public string? AssignedToUserName { get; set; }

    public bool IsConverted { get; set; } = false;
    public DateTime? ConvertedDate { get; set; }
    public Guid? ConvertedCustomerId { get; set; }

    public int Score { get; set; } = 0;
    public int EngagementScore { get; set; } = 0;

    [MaxLength(500)]
    public string? Tags { get; set; }

    // Custom Fields for different industries
    [Column(TypeName = "jsonb")]
    public string? CustomFieldsJson { get; set; }

    // Real Estate Specific
    [MaxLength(100)]
    public string? PropertyType { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PropertyPrice { get; set; }

    [MaxLength(200)]
    public string? PropertyLocation { get; set; }

    public int? PropertySize { get; set; }

    // Manufacturing Specific
    [MaxLength(100)]
    public string? ProductCategory { get; set; }

    public int? OrderQuantity { get; set; }

    public DateTime? RequiredDeliveryDate { get; set; }

    // Government Specific
    [MaxLength(50)]
    public string? TenderNumber { get; set; }

    public DateTime? TenderDeadline { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? LastContactDate { get; set; }
    public int ContactCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public DateTime? LastViewedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
    public virtual ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}