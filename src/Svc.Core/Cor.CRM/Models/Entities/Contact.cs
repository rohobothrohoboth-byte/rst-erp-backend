using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public class Contact : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(20)]
    public string? Mobile { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    public Guid? CustomerId { get; set; }

    public bool IsPrimary { get; set; } = false;
    public bool IsDecisionMaker { get; set; } = false;
    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Communication preferences
    public bool AcceptsEmail { get; set; } = true;
    public bool AcceptsSMS { get; set; } = true;
    public bool AcceptsCalls { get; set; } = true;
    public bool AcceptsMarketing { get; set; } = true;

    [MaxLength(100)]
    public string? PreferredContactMethod { get; set; }

    public DateTime? LastContactDate { get; set; }
    public int ContactCount { get; set; } = 0;

    // Social media
    [MaxLength(200)]
    public string? LinkedIn { get; set; }

    [MaxLength(200)]
    public string? Twitter { get; set; }
    
    [MaxLength(200)]
    public string? Facebook { get; set; }

    // Navigation Property
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
}