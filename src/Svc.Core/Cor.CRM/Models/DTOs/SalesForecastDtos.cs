// Cor.CRM/Models/DTOs/SalesForecastDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class ForecastByStageDto
{
    public string Stage { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
    public int Probability { get; set; }
}

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class RepPerformanceDto
{
    public string RepName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Deals { get; set; }
    public decimal Target { get; set; }
    public int Achievement { get; set; }
}

public class SalesForecastDto
{
    public decimal TotalForecast { get; set; }
    public int ConversionRate { get; set; }
    public decimal AverageDealSize { get; set; }
    public int PipelineVelocity { get; set; }
    public List<ForecastByStageDto> ByStage { get; set; } = new();
    public List<MonthlyTrendDto> MonthlyTrend { get; set; } = new();
    public List<RepPerformanceDto> ByRep { get; set; } = new();
}