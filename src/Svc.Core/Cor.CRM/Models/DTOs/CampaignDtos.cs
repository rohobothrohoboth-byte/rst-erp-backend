using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs
{




public class CampaignAnalyticsDto
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public int TotalReach { get; set; }
    public int TotalEngagement { get; set; }
    public int TotalConversions { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal ROI { get; set; }
    public decimal CostPerLead { get; set; }
    public decimal CostPerConversion { get; set; }
    public List<DailyAnalyticsDto> DailyStats { get; set; } = new();
    public List<ChannelAnalyticsDto> ChannelStats { get; set; } = new();
}

public class DailyAnalyticsDto
{
    public DateTime Date { get; set; }
    public int Reach { get; set; }
    public int Engagement { get; set; }
    public int Conversions { get; set; }
}

public class ChannelAnalyticsDto
{
    public string Channel { get; set; } = string.Empty;
    public int Reach { get; set; }
    public int Engagement { get; set; }
    public int Conversions { get; set; }
}
public class CampaignPerformanceDto
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public decimal OpenRate { get; set; }
    public decimal ClickRate { get; set; }
    public decimal BounceRate { get; set; }
    public decimal UnsubscribeRate { get; set; }
    public int TotalOpens { get; set; }
    public int TotalClicks { get; set; }
    public int TotalBounces { get; set; }
    public int TotalUnsubscribes { get; set; }
    public Dictionary<string, int> OpensByDevice { get; set; } = new();
    public Dictionary<string, int> OpensByLocation { get; set; } = new();
    public List<PerformanceTimelineDto> Timeline { get; set; } = new();
}

public class PerformanceTimelineDto
{
    public DateTime Date { get; set; }
    public string Event { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CampaignROIDto
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; } = string.Empty;
    public decimal Invested { get; set; }
    public decimal Revenue { get; set; }
    public decimal ROI { get; set; }
    public decimal Profit { get; set; }
    public decimal CostPerLead { get; set; }
    public decimal CostPerConversion { get; set; }
    public decimal RevenuePerLead { get; set; }
    public decimal RevenuePerConversion { get; set; }
}


    public class CampaignDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public decimal? ActualRevenue { get; set; }
        public string? TargetAudience { get; set; }
        public string? TargetIndustry { get; set; }
        public string? TargetLocation { get; set; }
        public int TargetCount { get; set; }
        public int ReachCount { get; set; }
        public int EngagementCount { get; set; }
        public int ConversionCount { get; set; }
        public decimal? ConversionRate { get; set; }
        public decimal? EngagementRate { get; set; }
        public string? Channel { get; set; }
        public string? MetricsJson { get; set; }
        public string? ContentJson { get; set; }
        public bool IsActive { get; set; }
        public int LeadCount { get; set; }
        public int CustomerCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCampaignDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Email";
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public string? TargetAudience { get; set; }
        public string? TargetIndustry { get; set; }
        public string? TargetLocation { get; set; }
        public int TargetCount { get; set; }
        public string? Channel { get; set; }
        public string? MetricsJson { get; set; }
        public string? ContentJson { get; set; }
    }

    public class UpdateCampaignDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public decimal? ActualRevenue { get; set; }
        public string? TargetAudience { get; set; }
        public string? TargetIndustry { get; set; }
        public string? TargetLocation { get; set; }
        public int? TargetCount { get; set; }
        public int? ReachCount { get; set; }
        public int? EngagementCount { get; set; }
        public int? ConversionCount { get; set; }
        public decimal? ConversionRate { get; set; }
        public decimal? EngagementRate { get; set; }
        public string? Channel { get; set; }
        public string? MetricsJson { get; set; }
        public string? ContentJson { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CampaignStatsDto
    {
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int CompletedCampaigns { get; set; }
        public int DraftCampaigns { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalActualCost { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageConversionRate { get; set; }
        public decimal AverageEngagementRate { get; set; }
        public Dictionary<string, int> CampaignsByType { get; set; } = new();
        public Dictionary<string, int> CampaignsByStatus { get; set; } = new();
    }
}
