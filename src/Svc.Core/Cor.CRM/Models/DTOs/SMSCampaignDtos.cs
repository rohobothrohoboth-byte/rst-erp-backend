// Cor.CRM/Models/DTOs/SMSCampaignDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class SMSCampaignDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? SentDate { get; set; }
    public int RecipientCount { get; set; }
    public int SentCount { get; set; }
    public int DeliveredCount { get; set; }
    public int FailedCount { get; set; }
    public int CharacterCount { get; set; }
    public int MessageParts { get; set; }
    public string? FromNumber { get; set; }
    public Guid? CampaignId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSMSCampaignDto
{
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? FromNumber { get; set; }
    public Guid? CampaignId { get; set; }
    public List<string>? Recipients { get; set; }
}

public class UpdateSMSCampaignDto
{
    public string? Name { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? FromNumber { get; set; }
}

public class SMSCampaignStatsDto
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
    public int TotalFailed { get; set; }

    // SMS specific stats
    public int TotalCharacters { get; set; }
    public int TotalMessageParts { get; set; }
    public decimal AvgCharactersPerCampaign { get; set; }
    public decimal AvgMessagePartsPerCampaign { get; set; }
    public decimal DeliveryRate { get; set; } // (Delivered / Sent) * 100
    public decimal FailureRate { get; set; }  // (Failed / Sent) * 100

    // Top campaigns
    public List<SMSCampaignDto>? TopPerforming { get; set; }

    // Status breakdown
    public Dictionary<string, int>? CampaignsByStatus { get; set; }

    // Time-based stats
    public int CreatedLast7Days { get; set; }
    public int CreatedLast30Days { get; set; }
    public int CreatedThisMonth { get; set; }
    public int CreatedThisYear { get; set; }
}