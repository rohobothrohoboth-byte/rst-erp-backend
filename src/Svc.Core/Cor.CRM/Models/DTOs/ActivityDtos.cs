using System;

namespace Cor.CRM.Models.DTOs
{
    public class ActivityDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public Guid? LeadId { get; set; }
        public string? LeadName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? OpportunityId { get; set; }
        public string? OpportunityName { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; }
        public string? Outcome { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CreateActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OpportunityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? Location { get; set; }
        public bool IsAllDay { get; set; }
        public int? ReminderMinutes { get; set; }
    }

    public class UpdateActivityDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OpportunityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? Location { get; set; }
        public bool? IsAllDay { get; set; }
    }
}
