using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum TaskState
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Overdue = 5,
    OnHold = 6
}

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public class Task : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

   public TaskState Status { get; set; } = TaskState.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? StartedDate { get; set; }

    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid? ActivityId { get; set; }

    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }

    public bool IsRecurring { get; set; } = false;
    
    [MaxLength(100)]
    public string? RecurrenceRule { get; set; }

    public int? RecurrenceCount { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }

    public int EstimatedHours { get; set; } = 0;
    public int ActualHours { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal? CompletionPercentage { get; set; }

    [MaxLength(500)]
    public string? Outcome { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }

    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("OpportunityId")]
    public virtual Opportunity? Opportunity { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }

    [ForeignKey("ActivityId")]
    public virtual Activity? Activity { get; set; }
   // public virtual ICollection<Lead> LeadList { get; set; } = new List<Lead>();
}