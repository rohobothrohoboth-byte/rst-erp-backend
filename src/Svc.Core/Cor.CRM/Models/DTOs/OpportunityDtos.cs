// Cor.CRM/Models/DTOs/OpportunityDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class OpportunityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? LeadId { get; set; }
    public string? LeadName { get; set; }
    public decimal Amount { get; set; }
    public string Stage { get; set; } = string.Empty;
    public int WinProbability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public bool IsActive { get; set; }
    public int ActivityCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateOpportunityDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public decimal Amount { get; set; }
    public string? Stage { get; set; }
    public int? WinProbability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class UpdateOpportunityDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public string? Stage { get; set; }
    public int? WinProbability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public Guid? AssignedToUserId { get; set; }
}

public class OpportunityStatsDto
{
    public int TotalOpportunities { get; set; }
    public int Active { get; set; }
    public int ClosedWon { get; set; }
    public int ClosedLost { get; set; }
    public decimal TotalWonAmount { get; set; }
    public double ConversionRate { get; set; }
    public double AvgWinProbability { get; set; }
    public List<StageDistributionDto> StageDistribution { get; set; } = new();
}

public class StageDistributionDto
{
    public string Key { get; set; } = string.Empty;
    public int Value { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OpportunityPipelineDto
{
    public decimal TotalPipelineValue { get; set; }
    public int TotalOpportunities { get; set; }
    public List<PipelineStageDto> Stages { get; set; } = new();
}

public class PipelineStageDto
{
    public string StageName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public double AvgProbability { get; set; }
}
public class OpportunityFilterDto
{
    public string? SearchTerm { get; set; }
    public string? Stage { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}