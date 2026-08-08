// Cor.CRM/Models/Entities/Interaction.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum InteractionType
{
    Call = 1,
    Email = 2,
    Meeting = 3,
    Note = 4,
    Task = 5,
    Chat = 6,
    SMS = 7,
    Letter = 8
}

public enum InteractionStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Postponed = 5
}

public enum InteractionPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public class Interaction : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public InteractionType Type { get; set; }
    public InteractionStatus? Status { get; set; }
    public InteractionPriority? Priority { get; set; }

    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? OpportunityId { get; set; }

    [ForeignKey(nameof(LeadId))]
    public virtual Lead? Lead { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }

    [ForeignKey(nameof(ContactId))]
    public virtual Contact? Contact { get; set; }

    [ForeignKey(nameof(OpportunityId))]
    public virtual Opportunity? Opportunity { get; set; }

    public Guid? AssignedToUserId { get; set; }
   // public Guid? CreatedByUserId { get; set; }

    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? Duration { get; set; }

    [MaxLength(500)]
    public string? Outcome { get; set; }

    [MaxLength(500)]
    public string? Location { get; set; }

    public bool IsAllDay { get; set; } = false;

    [MaxLength(50)]
    public string? ReminderMinutes { get; set; }

    public bool IsActive { get; set; } = true;
   // public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}