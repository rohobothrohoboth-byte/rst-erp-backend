// Cor.CRM/Models/DTOs/EmailCampaignDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class EmailCampaignDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? HtmlContent { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? SentDate { get; set; }
    public int RecipientCount { get; set; }
    public int SentCount { get; set; }
    public int DeliveredCount { get; set; }
    public int OpenCount { get; set; }
    public int ClickCount { get; set; }
    public int BounceCount { get; set; }
    public int UnsubscribeCount { get; set; }
    public decimal? OpenRate { get; set; }
    public decimal? ClickRate { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? CampaignId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateEmailCampaignDto
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? HtmlContent { get; set; }
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? CampaignId { get; set; }
    public List<string>? Recipients { get; set; }
}

public class UpdateEmailCampaignDto
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public string? HtmlContent { get; set; }
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public Guid? TemplateId { get; set; }
}
public class EmailCampaignStatsDto
{
    // Count stats
    public int TotalCampaigns { get; set; }
    public int Sent { get; set; }
    public int Scheduled { get; set; }
    public int Draft { get; set; }
    public int Sending { get; set; }
    public int Paused { get; set; }
    public int Cancelled { get; set; }
    public int Failed { get; set; }

    // Delivery stats
    public int TotalRecipients { get; set; }
    public int TotalSent { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalBounces { get; set; }
    public int TotalUnsubscribes { get; set; }

    // Engagement stats
    public int TotalOpens { get; set; }
    public int TotalClicks { get; set; }
    public decimal AvgOpenRate { get; set; }
    public decimal AvgClickRate { get; set; }
    public decimal AvgBounceRate { get; set; }
    public decimal AvgUnsubscribeRate { get; set; }

    // Top campaigns
    public List<EmailCampaignDto>? TopPerforming { get; set; }

    // Status breakdown
    public Dictionary<string, int>? CampaignsByStatus { get; set; }

    // Time-based stats
    public int CreatedLast7Days { get; set; }
    public int CreatedLast30Days { get; set; }
    public int CreatedThisMonth { get; set; }
    public int CreatedThisYear { get; set; }
}