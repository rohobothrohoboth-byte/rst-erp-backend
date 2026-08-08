// Cor.CRM/Models/Entities/SocialMediaPost.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.CRM.Models.Entities;

public enum SocialMediaPlatform
{
    Facebook = 1,
    Twitter = 2,
    Instagram = 3,
    LinkedIn = 4,
    YouTube = 5,
    TikTok = 6
}

public enum SocialMediaStatus
{
    Draft = 1,
    Scheduled = 2,
    Published = 3,
    Failed = 4,
    Pending = 5
}

public class SocialMediaPost : BaseEntity
{
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    public SocialMediaPlatform Platform { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? VideoUrl { get; set; }

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(500)]
    public string? Hashtags { get; set; } // Comma separated

    public SocialMediaStatus Status { get; set; } = SocialMediaStatus.Draft;

    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    public int EngagementCount { get; set; } = 0;
    public int ReachCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int ShareCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;

    [MaxLength(100)]
    public string? PostId { get; set; } // Social media platform's post ID

    [Column(TypeName = "jsonb")]
    public string? AnalyticsJson { get; set; }

    public Guid? CampaignId { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }
}