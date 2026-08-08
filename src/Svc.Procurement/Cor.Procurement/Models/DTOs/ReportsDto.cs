using System;
using System.Collections.Generic;

namespace Cor.Procurement.Models.DTOs;

public class ReportsDashboardDto
{
    public ReportsStats Stats { get; set; } = new();
    public List<ReportDto> Reports { get; set; } = new();
    public SpendAnalysisDto SpendAnalysis { get; set; } = new();
    public List<VendorPerformanceDto> VendorPerformance { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class ReportsStats
{
    public int TotalReports { get; set; }
    public int ReadyReports { get; set; }
    public int GeneratingReports { get; set; }
    public int ScheduledReports { get; set; }
    public int TotalDownloads { get; set; }
    public int CategoriesCount { get; set; }
}



public class SpendAnalysisDto
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalSpend { get; set; }
    public List<SpendCategoryDto> Categories { get; set; } = new();
    public List<TopVendorDto> TopVendors { get; set; } = new();
    public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    public List<BudgetUtilizationDto> BudgetUtilization { get; set; } = new();
    public decimal MonthlyChange { get; set; }
    public decimal BudgetUtilizationPercentage { get; set; }
}

public class SpendCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Percentage { get; set; }
    public string Trend { get; set; } = "stable";
}

public class TopVendorDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Percentage { get; set; }
}

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class BudgetUtilizationDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Budgeted { get; set; }
    public decimal Actual { get; set; }
    public decimal Variance { get; set; }
    public decimal UtilizationPercentage { get; set; }
}

public class VendorPerformanceDto
{
    public Guid Id { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int OverallScore { get; set; }
    public VendorMetricsDto PerformanceMetrics { get; set; } = new();
    public VendorTrendsDto Trends { get; set; } = new();
    public int TotalOrders { get; set; }
    public decimal OnTimeDelivery { get; set; }
    public decimal QualityRate { get; set; }
    public decimal AverageResponseTime { get; set; }
    public string Status { get; set; } = "Average";
    public DateTime LastEvaluation { get; set; }
}

public class VendorMetricsDto
{
    public int Delivery { get; set; }
    public int Quality { get; set; }
    public int Price { get; set; }
    public int Communication { get; set; }
    public int Compliance { get; set; }
}

public class VendorTrendsDto
{
    public string Delivery { get; set; } = "stable";
    public string Quality { get; set; } = "stable";
    public string Price { get; set; } = "stable";
}


public class ReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
    public string Period { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string? Size { get; set; }
    public int Downloads { get; set; }
    public DateTime? LastViewed { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? ReportUrl { get; set; }
    public object? Data { get; set; }
}

public class CreateReportDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Format { get; set; } = "pdf";
    public List<string> Tags { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IncludeCharts { get; set; } = true;
    public bool IncludeSummary { get; set; } = true;
    public bool IncludeDetails { get; set; } = true;
}

public class GenerateReportResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; }
}