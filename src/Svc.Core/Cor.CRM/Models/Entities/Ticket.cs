// Cor.CRM/Models/Entities/Ticket.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum TicketStatus
{
    New = 1,
    Open = 2,
    InProgress = 3,
    Resolved = 4,
    Closed = 5,
    Reopened = 6,
    OnHold = 7
}

public enum TicketPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

public enum TicketSource
{
    Email = 1,
    Phone = 2,
    Chat = 3,
    Portal = 4,
    SocialMedia = 5,
    Api = 6
}

public class Ticket : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.New;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketSource Source { get; set; } = TicketSource.Email;

    public Guid? CustomerId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }

    public new Guid? CreatedByUserId { get; set; }
    public new string? CreatedByUserName { get; set; }

    public DateTime? AssignedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public DateTime? DueDate { get; set; }

    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }

    [MaxLength(500)]
    public string? Resolution { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(100)]
    public string? SubCategory { get; set; }

    public bool IsEscalated { get; set; } = false;
    public int EscalationLevel { get; set; } = 0;

    public int? SatisfactionScore { get; set; } // 1-5

    [Column(TypeName = "jsonb")]
    public string? MetadataJson { get; set; }

    // Navigation Properties
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }

    public virtual ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public virtual ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
}

public class TicketComment : BaseEntity
{
    [Required]
    public Guid TicketId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsInternal { get; set; } = false;

    public Guid? UserId { get; set; }
    public string? UserName { get; set; }

    [ForeignKey("TicketId")]
    public virtual Ticket? Ticket { get; set; }
}

public class TicketAttachment : BaseEntity
{
    [Required]
    public Guid TicketId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FilePath { get; set; }

    public long FileSize { get; set; }

    [MaxLength(100)]
    public string? FileType { get; set; }

    [ForeignKey("TicketId")]
    public virtual Ticket? Ticket { get; set; }
}