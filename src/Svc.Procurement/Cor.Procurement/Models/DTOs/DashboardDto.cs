using System;
using System.Collections.Generic;

namespace Cor.Procurement.Models.DTOs;

public class DashboardDto
{
    public DashboardStats Stats { get; set; } = new();
    public List<DashboardVendorDto> Vendors { get; set; } = new();
    public List<DashboardPurchaseOrderDto> ActiveOrders { get; set; } = new();
    public List<DashboardSpendCategoryDto> SpendByCategory { get; set; } = new();
    public List<DashboardActivityDto> RecentActivities { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class DashboardStats
{
    public decimal MonthlySpend { get; set; }
    public decimal CostSavings { get; set; }
    public int ActiveVendors { get; set; }
    public int OpenPOs { get; set; }
    public int TotalRequisitions { get; set; }
    public int PendingInvoices { get; set; }
    public int ActiveContracts { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal MonthlyChange { get; set; }
    public decimal SavingsChange { get; set; }
    public decimal VendorsChange { get; set; }
    public decimal OrdersChange { get; set; }
}

public class DashboardVendorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public string Performance { get; set; } = string.Empty;
    public int Orders { get; set; }
    public decimal Spend { get; set; }
}

public class DashboardPurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
}

public class DashboardSpendCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Percentage { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class DashboardActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? ReferenceId { get; set; }
}