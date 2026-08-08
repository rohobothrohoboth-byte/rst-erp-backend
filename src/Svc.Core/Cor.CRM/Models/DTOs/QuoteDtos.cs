// Cor.CRM/Models/DTOs/QuoteDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class QuoteDto
{
    public Guid Id { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public Guid? LeadId { get; set; }
    public string? LeadName { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public Guid? OpportunityId { get; set; }
    public string? OpportunityName { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }  // Made nullable
    public decimal? DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? ShippingCost { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? TermsAndConditions { get; set; }
    public string? Notes { get; set; }
    public int ViewCount { get; set; }
    public DateTime? SentDate { get; set; }
    public DateTime? AcceptedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<QuoteLineDto> QuoteLines { get; set; } = new();

}

public class QuoteLineDto
{
    public Guid Id { get; set; }
     public Guid QuoteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal TotalPrice { get; set; }
     public int SortOrder { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Notes { get; set; }
}

public class CreateQuoteDto
{
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Notes { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal? DiscountAmount { get; set; }
    public List<CreateQuoteLineDto> QuoteLines { get; set; } = new();
}


public class CreateQuoteLineDto
{
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TotalPrice { get; set; }  // ? Make sure this is TotalPrice (not totalPrice)
    public string? Notes { get; set; }
}

public class UpdateQuoteDto
{
    public DateTime? ValidUntil { get; set; }
    public string? Status { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Notes { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal? DiscountAmount { get; set; }
    public List<CreateQuoteLineDto> QuoteLines { get; set; } = new();
}

public class QuoteFilterDto
{
    public string? Status { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class QuoteStatsDto
{
public int TotalQuotes { get; set; }
    public int Total { get; set; }
    public int Draft { get; set; }
    public int Sent { get; set; }
    public int Viewed { get; set; }
    public int Negotiating { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public int Expired { get; set; }
    public int Converted { get; set; }

    // Financial stats
    public decimal TotalValue { get; set; }
    public decimal AcceptedValue { get; set; }
    public decimal AverageValue { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }

    // Conversion metrics
    public double ConversionRate { get; set; } // (Accepted / Total) * 100

    // Time-based stats
    public int CreatedLast7Days { get; set; }
    public int CreatedLast30Days { get; set; }
    public int CreatedThisMonth { get; set; }
    public int CreatedThisYear { get; set; }

    // Optional: Dictionary for dynamic status grouping
    public Dictionary<string, int>? StatusCounts { get; set; }
    public Dictionary<string, decimal>? StatusValues { get; set; }
}