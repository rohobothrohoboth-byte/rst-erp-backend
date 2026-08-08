// Shared/Helpers/Events/LeadEventData.cs
namespace Shared.Helpers.Events;

public class LeadEventData
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Status { get; set; }
    public string? Source { get; set; }
    public string? Priority { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Budget { get; set; }
    public decimal? EstimatedValue { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public int? Score { get; set; }
    public string? Tags { get; set; }
    public bool? IsConverted { get; set; }
    public DateTime? ConvertedDate { get; set; }
}

public class CustomerEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
}

public class OpportunityEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public decimal? Amount { get; set; }
    public string? Stage { get; set; }
    public int? WinProbability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class ActivityEventData
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class TaskEventData
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class CampaignEventData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
}
