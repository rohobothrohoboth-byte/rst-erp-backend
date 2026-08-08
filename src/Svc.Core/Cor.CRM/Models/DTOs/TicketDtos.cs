// Cor.CRM/Models/DTOs/TicketDtos.cs
using System;

namespace Cor.CRM.Models.DTOs;

public class TicketDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime? AssignedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public string? Resolution { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public bool IsEscalated { get; set; }
    public int EscalationLevel { get; set; }
    public int? SatisfactionScore { get; set; }
    public int CommentCount { get; set; }
    public int AttachmentCount { get; set; }
}

public class CreateTicketDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Source { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
}

public class UpdateTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Source { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public string? Resolution { get; set; }
}

public class TicketStatsResponse
{
    public int Total { get; set; }
    public int New { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public int Reopened { get; set; }
    public int OnHold { get; set; }
    public int Overdue { get; set; }
    public Dictionary<string, int> ByPriority { get; set; } = new();
    public Dictionary<string, int> BySource { get; set; } = new();
    public double? AverageResolutionHours { get; set; }
}

public class TicketCommentDto : BaseDto
{
    public Guid TicketId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
}

public class CreateTicketCommentDto
{
    public Guid TicketId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;
}

public class TicketAttachmentDto : BaseDto
{
    public Guid TicketId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public long FileSize { get; set; }
    public string? FileType { get; set; }
}

public class UpdateTicketStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class ResolveTicketDto
{
    public string? Resolution { get; set; }
}

public class CloseTicketDto
{
    public int? SatisfactionScore { get; set; }
}