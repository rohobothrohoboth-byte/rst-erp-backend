// Cor.CRM/Models/DTOs/ContractDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

// ============================================================
// CONTRACT LINE DTOS
// ============================================================

public class ContractLineDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int SortOrder { get; set; }
    public string? Notes { get; set; }
}

public class CreateContractLineDto
{
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

public class UpdateContractLineDto
{
    public Guid? Id { get; set; }
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

// ============================================================
// CONTRACT DTOS
// ============================================================

public class ContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? OpportunityId { get; set; }
    public string? OpportunityName { get; set; }
    public Guid? QuoteId { get; set; }
    public string? QuoteNumber { get; set; }
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ContractLineDto> ContractLines { get; set; } = new();
}

public class CreateContractDto
{
    public Guid CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalValue { get; set; }
    public string? Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Notes { get; set; }
    public List<CreateContractLineDto>? ContractLines { get; set; }
}

public class UpdateContractDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? TotalValue { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Notes { get; set; }
    public List<UpdateContractLineDto>? ContractLines { get; set; }
}

// ============================================================
// CONTRACT FILTER & STATS DTOS
// ============================================================

public class ContractFilterDto
{
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}

public class ContractStatsDto
{
    public int TotalContracts { get; set; }
    public int Draft { get; set; }
    public int Pending { get; set; }
    public int Active { get; set; }
    public int Signed { get; set; }
    public int Expired { get; set; }
    public int Terminated { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageContractValue { get; set; }
}