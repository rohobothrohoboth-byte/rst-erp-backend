// Cor.CRM/Models/DTOs/InvoiceDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid? LeadId { get; set; }
    public string? LeadName { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? OpportunityId { get; set; }
    public string? OpportunityName { get; set; }
    public Guid? QuoteId { get; set; }
    public string? QuoteNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? AmountPaid { get; set; }
    public decimal? BalanceDue { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<InvoiceLineDto> InvoiceLines { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class InvoiceLineDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal TotalPrice { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Notes { get; set; }
}

public class CreateInvoiceDto
{
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? QuoteId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? Type { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? Currency { get; set; }
    public decimal? DiscountAmount { get; set; }
    public List<CreateInvoiceLineDto> InvoiceLines { get; set; } = new();
}

public class CreateInvoiceLineDto
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public Guid? ProductId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInvoiceDto
{
    public DateTime? InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Type { get; set; }
    public string? Terms { get; set; }
    public string? Notes { get; set; }
    public string? Currency { get; set; }
     public string Status { get; set; } = string.Empty;
    public decimal? DiscountAmount { get; set; }
    public List<CreateInvoiceLineDto> InvoiceLines { get; set; } = new();
}

public class InvoiceFilterDto
{
    public string? Status { get; set; }
    public string? Type { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
public class MarkPaidDto
{
    public decimal? AmountPaid { get; set; }
}

public class RefundDto
{
    public decimal Amount { get; set; }
}
public class InvoiceStatsDto
{
    public int TotalInvoices { get; set; }
    public int Draft { get; set; }
    public int Sent { get; set; }
    public int Paid { get; set; }
    public int Partial { get; set; }
    public int Overdue { get; set; }
    public int Cancelled { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public double PaymentRate { get; set; }
    public decimal AverageAmount { get; set; }
}
public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class CancelOrderDto
{
    public string? Reason { get; set; }
}
public class OrderStatsDto
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Draft { get; set; }
    public int Processing { get; set; }
    public int Shipped { get; set; }
    public int Delivered { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageOrderValue { get; set; }
}