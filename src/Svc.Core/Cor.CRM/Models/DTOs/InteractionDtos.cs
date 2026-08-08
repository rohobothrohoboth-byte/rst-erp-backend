// Cor.CRM/Models/DTOs/InteractionDtos.cs

using System;

namespace Cor.CRM.Models.DTOs;

public class InteractionDto
{
    public Guid Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty; // Will be "Call", "Email", etc.
    public string? Status { get; set; } // Will be "Scheduled", "Completed", etc.
    public string? Priority { get; set; } // Will be "Low", "Medium", etc.
    public Guid? LeadId { get; set; }
    public string? LeadName { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? ContactId { get; set; }
    public string? ContactName { get; set; }
    public Guid? OpportunityId { get; set; }
    public string? OpportunityName { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? Duration { get; set; }
    public string? Outcome { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateInteractionDto
{
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "Call"; // Will be converted to enum
    public string? Status { get; set; } // Will be converted to enum
    public string? Priority { get; set; } // Will be converted to enum
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? Duration { get; set; }
    public string? Outcome { get; set; }
    public string? Location { get; set; }
    public bool IsAllDay { get; set; }
}

public class UpdateInteractionDto
{
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public int? Duration { get; set; }
    public string? Outcome { get; set; }
    public string? Location { get; set; }
    public bool? IsAllDay { get; set; }
}

public class InteractionFilterDto
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class InteractionStatsDto
{
    public int Total { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> ByPriority { get; set; } = new();
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
}