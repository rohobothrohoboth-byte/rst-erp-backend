using System;

namespace Cor.CRM.Models.DTOs
{
    public class TaskDto : BaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Guid? LeadId { get; set; }
        public string? LeadName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? OpportunityId { get; set; }
        public string? OpportunityName { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public bool IsRecurring { get; set; }
        public decimal? CompletionPercentage { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OpportunityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public bool IsRecurring { get; set; }
        public int? EstimatedHours { get; set; }
    }

    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OpportunityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public int? EstimatedHours { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}
