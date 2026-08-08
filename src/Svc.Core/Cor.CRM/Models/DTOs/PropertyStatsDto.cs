// Cor.CRM/Models/DTOs/PropertyStatsDto.cs

namespace Cor.CRM.Models.DTOs;

public class PropertyStatsDto
{
    public int TotalProperties { get; set; }
    public int Active { get; set; }
    public int Pending { get; set; }
    public int Sold { get; set; }
    public int Rented { get; set; }
    public int OffMarket { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal TotalValue { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinPrice { get; set; }
}

public class TransactionStatsDto
{
    public int TotalTransactions { get; set; }
    public int Negotiation { get; set; }
    public int Accepted { get; set; }
    public int PendingInspection { get; set; }
    public int PendingFinancing { get; set; }
    public int PendingAppraisal { get; set; }
    public int Closing { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public decimal TotalSalesValue { get; set; }
    public decimal AverageSalePrice { get; set; }
    public decimal TotalCommission { get; set; }
}

