
// Models/Entities/Aggregates/InvoiceAggregate.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Cor.Finance.Models.DTOs;

public class DashboardSummaryDto
{
    public List<InvoiceStatusSummary> InvoiceStats { get; set; } = new();
    public List<PaymentStatusSummary> PaymentStats { get; set; } = new();
    public decimal TotalExpenses { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal BudgetUtilization { get; set; }
}

public class InvoiceStatusSummary
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Total { get; set; }
}

public class PaymentStatusSummary
{
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
}