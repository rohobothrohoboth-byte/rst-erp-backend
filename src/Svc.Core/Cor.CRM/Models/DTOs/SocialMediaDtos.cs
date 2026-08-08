// Cor.CRM/Models/DTOs/SocialMediaDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class SocialMediaPostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? Location { get; set; }
    public string? Hashtags { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }
    public int EngagementCount { get; set; }
    public int ReachCount { get; set; }
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public int CommentCount { get; set; }
    public string? PostId { get; set; }
    public Guid? CampaignId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSocialMediaPostDto
{
    public string Content { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? Location { get; set; }
    public string? Hashtags { get; set; }
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public Guid? CampaignId { get; set; }
}

public class UpdateSocialMediaPostDto
{
    public string? Content { get; set; }
    public string? Platform { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? Location { get; set; }
    public string? Hashtags { get; set; }
    public string? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
}

public class SocialMediaPostFilterDto
{
    public string? Platform { get; set; }
    public string? Status { get; set; }
    public Guid? CampaignId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SocialMediaStatsDto
{
    // Count stats
    public int TotalPosts { get; set; }
    public int Published { get; set; }
    public int Scheduled { get; set; }
    public int Draft { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }

    // Engagement stats
    public int TotalEngagement { get; set; }
    public int TotalReach { get; set; }
    public int TotalLikes { get; set; }
    public int TotalShares { get; set; }
    public int TotalComments { get; set; }

    // Average stats
    public decimal AvgEngagement { get; set; }
    public decimal AvgReach { get; set; }
    public decimal AvgLikes { get; set; }
    public decimal AvgShares { get; set; }
    public decimal AvgComments { get; set; }

    // Platform breakdown
    public Dictionary<string, int>? PostsByPlatform { get; set; }
    public Dictionary<string, int>? EngagementByPlatform { get; set; }

    // Status breakdown
    public Dictionary<string, int>? PostsByStatus { get; set; }

    // Time-based stats
    public int CreatedLast7Days { get; set; }
    public int CreatedLast30Days { get; set; }
    public int CreatedThisMonth { get; set; }
    public int CreatedThisYear { get; set; }
}