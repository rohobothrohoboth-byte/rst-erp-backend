// ActivityLog.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum ActivityLogType
{
    Create = 1,
    Update = 2,
    Delete = 3,
    View = 4,
    Export = 5,
    Import = 6,
    Login = 7,
    Logout = 8,
    Assign = 9,
    Convert = 10,
    StatusChange = 11,
    Other = 12
}

public class ActivityLog : BaseEntity
{
    [Required]
    public ActivityLogType Type { get; set; }

    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty; // Lead, Customer, Opportunity, etc.

    [Required]
    public string EntityId { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public string? ChangesJson { get; set; } // JSON diff of changes
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }

    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserIpAddress { get; set; }
    public string? UserAgent { get; set; }

    [MaxLength(50)]
    public string? ClientType { get; set; } // Web, Mobile, API

    public Guid? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }

  //  [ForeignKey("UserId")]
   // public virtual User? User { get; set; }
}