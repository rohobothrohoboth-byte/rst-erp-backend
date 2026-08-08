// Integration.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cor.CRM.Models.Entities;

public enum IntegrationType
{
    Webhook = 1,
    API = 2,
    Email = 3,
    SMS = 4,
    SocialMedia = 5,
    Analytics = 6,
    CRM = 7,
    Other = 8
}

public enum IntegrationStatus
{
    Active = 1,
    Inactive = 2,
    Pending = 3,
    Failed = 4
}

public class Integration : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public IntegrationType Type { get; set; }
    public IntegrationStatus Status { get; set; } = IntegrationStatus.Pending;

    [Required]
    [MaxLength(500)]
    public string Endpoint { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AuthKey { get; set; }

    [MaxLength(500)]
    public string? AuthSecret { get; set; }

    [MaxLength(50)]
    public string? AuthType { get; set; } // Bearer, APIKey, OAuth

    public string? Events { get; set; } // Comma separated list of events

    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;

    public DateTime? LastTriggeredAt { get; set; }
    public DateTime? LastSuccessAt { get; set; }
    public DateTime? LastFailedAt { get; set; }

    [MaxLength(500)]
    public string? LastError { get; set; }

    public int SuccessCount { get; set; } = 0;
    public int FailureCount { get; set; } = 0;

    public string? HeadersJson { get; set; }
    public string? ConfigJson { get; set; }

    public bool IsActive { get; set; } = true;
}