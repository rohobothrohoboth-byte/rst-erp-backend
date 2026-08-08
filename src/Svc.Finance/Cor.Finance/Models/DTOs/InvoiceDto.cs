// Svc.Finance.Models.DTOs - InvoiceDtos.cs

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MessagePack;

namespace Cor.Finance.Models.DTOs;

// ============================================================
// CREATE DTOs
// ============================================================

[MessagePackObject]
public class AddInvoiceDto
{
    [Key(0)]
    public DateTime InvoiceDate { get; set; }

    [Key(1)]
    public DateTime? DueDate { get; set; }

    [Key(2)]
    public decimal SubTotal { get; set; }

    [Key(3)]
    public decimal TaxAmount { get; set; }

    [Key(4)]
    public decimal TotalAmount { get; set; }

    [Key(5)]
    public decimal DiscountAmount { get; set; }

    [Key(6)]
    public string? Notes { get; set; }

    [Key(7)]
    public Guid? BranchId { get; set; }

    [Key(8)]
    public Guid? DepartmentId { get; set; }

    [Key(9)]
    public Guid? EmployeeId { get; set; }

    [Key(10)]
    public string InvoiceType { get; set; } = "Purchase";

    // For AP (Purchase Invoices)
    [Key(11)]
    public Guid? VendorId { get; set; }

    [Key(12)]
    public Guid? PurchaseOrderId { get; set; }

    [Key(13)]
    public DateTime? ReceivedDate { get; set; }

    // For AR (Sales Invoices)
    [Key(14)]
    public Guid? CustomerId { get; set; }

    [Key(15)]
    public string? SalesRep { get; set; }

    [Key(16)]
    public DateTime? DeliveryDate { get; set; }

    [Key(17)]
    [JsonIgnore]
    public string? InvoiceNumber { get; set; }

    [Key(18)]
    public Guid? PeriodId { get; set; }

    [Key(19)]
    public List<AddInvoiceLineDto> Lines { get; set; } = new();
}

[MessagePackObject]
public class AddInvoiceLineDto
{
    [Key(0)]
    public string Description { get; set; } = default!;

    [Key(1)]
    public int Quantity { get; set; } = 1;

    [Key(2)]
    public decimal UnitPrice { get; set; }

    [Key(3)]
    public decimal TotalAmount { get; set; }

    [Key(4)]
    public decimal Discount { get; set; }

    [Key(5)]
    public decimal TaxRate { get; set; }

    [Key(6)]
    public Guid? PeriodId { get; set; }
}

// ============================================================
// RESPONSE DTOs
// ============================================================

[MessagePackObject]
public class InvoiceDto
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Key(2)]
    public DateTime InvoiceDate { get; set; }

    [Key(3)]
    public DateTime? DueDate { get; set; }

    [Key(4)]
    public decimal SubTotal { get; set; }

    [Key(5)]
    public decimal TaxAmount { get; set; }

    [Key(6)]
    public decimal DiscountAmount { get; set; }

    [Key(7)]
    public decimal TotalAmount { get; set; }

    [Key(8)]
    public decimal PaidAmount { get; set; }

    [Key(9)]
    public decimal BalanceDue { get; set; }

    [Key(10)]
    public string Status { get; set; } = default!;

    [Key(11)]
    public string? Notes { get; set; }

    [Key(12)]
    public string InvoiceType { get; set; } = "Purchase";

    // For AP
    [Key(13)]
    public Guid? VendorId { get; set; }

    [Key(14)]
    public string? VendorName { get; set; }

    [Key(15)]
    public Guid? PurchaseOrderId { get; set; }

    [Key(16)]
    public DateTime? ReceivedDate { get; set; }

    // For AR
    [Key(17)]
    public Guid? CustomerId { get; set; }

    [Key(18)]
    public string? CustomerName { get; set; }

    [Key(19)]
    public string? SalesRep { get; set; }

    [Key(20)]
    public DateTime? DeliveryDate { get; set; }

    // Common
    [Key(21)]
    public Guid? BranchId { get; set; }

    [Key(22)]
    public string? BranchName { get; set; }

    [Key(23)]
    public Guid? DepartmentId { get; set; }

    [Key(24)]
    public string? DepartmentName { get; set; }

    [Key(25)]
    public Guid? EmployeeId { get; set; }

    [Key(26)]
    public string? EmployeeName { get; set; }

    // Collections
    [Key(27)]
    public List<InvoiceLineDto> Lines { get; set; } = new();

    [Key(28)]
    public List<PaymentDto> Payments { get; set; } = new();

    // Audit
    [Key(29)]
    public DateTime DateAdd { get; set; }

    [Key(30)]
    public DateTime? DateMod { get; set; }

    [Key(31)]
    public Guid? PeriodId { get; set; }

    [Key(32)]
    public string? PeriodName { get; set; }

    [Key(33)]
    public string RowVersion { get; set; } = default!;
}

[MessagePackObject]
public class InvoiceLineDto
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public string Description { get; set; } = default!;

    [Key(2)]
    public int Quantity { get; set; }

    [Key(3)]
    public decimal UnitPrice { get; set; }

    [Key(4)]
    public decimal Discount { get; set; }

    [Key(5)]
    public decimal TaxRate { get; set; }

    [Key(6)]
    public decimal TotalAmount { get; set; }

    [Key(7)]
    public Guid? PeriodId { get; set; }

    [Key(8)]
    public string? PeriodName { get; set; }

    [Key(9)]
    public DateTime DateAdd { get; set; }

    [Key(10)]
    public DateTime? DateMod { get; set; }
}

// ============================================================
// UPDATE DTOs
// ============================================================

[MessagePackObject]
public class EditInvoiceDto
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public DateTime InvoiceDate { get; set; }

    [Key(2)]
    public DateTime? DueDate { get; set; }

    [Key(3)]
    public decimal SubTotal { get; set; }

    [Key(4)]
    public decimal TaxAmount { get; set; }

    [Key(5)]
    public decimal DiscountAmount { get; set; }

    [Key(6)]
    public string? Notes { get; set; }

    [Key(7)]
    public Guid? BranchId { get; set; }

    [Key(8)]
    public Guid? DepartmentId { get; set; }

    [Key(9)]
    public Guid? EmployeeId { get; set; }

    [Key(10)]
    public List<InvoiceLineEditDto> Lines { get; set; } = new();

    [Key(11)]
    public string RowVersion { get; set; } = default!;

    // For AP (Vendor)
    [Key(12)]
    public Guid? VendorId { get; set; }

    [Key(13)]
    public Guid? PurchaseOrderId { get; set; }

    [Key(14)]
    public DateTime? ReceivedDate { get; set; }

    // For AR (Customer)
    [Key(15)]
    public Guid? CustomerId { get; set; }

    [Key(16)]
    public string? SalesRep { get; set; }

    [Key(17)]
    public DateTime? DeliveryDate { get; set; }

    [Key(18)]
    public string InvoiceType { get; set; } = "Purchase";

    [Key(19)]
    public Guid PeriodId { get; set; }
}

[MessagePackObject]
public class InvoiceLineEditDto
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public string Description { get; set; } = default!;

    [Key(2)]
    public int Quantity { get; set; }

    [Key(3)]
    public decimal UnitPrice { get; set; }

    [Key(4)]
    public decimal Discount { get; set; }

    [Key(5)]
    public decimal TaxRate { get; set; }

    [Key(6)]
    public Guid? PeriodId { get; set; }
}

// ============================================================
// STATUS UPDATE DTO
// ============================================================

[MessagePackObject]
public class InvoiceStatusUpdateDto
{
    [Key(0)]
    public Guid Id { get; set; }

    [Key(1)]
    public string Status { get; set; } = default!;

    [Key(2)]
    public string? Comment { get; set; }

    [Key(3)]
    public Guid? PeriodId { get; set; }
}

// ============================================================
// FILTER / QUERY DTOs
// ============================================================

[MessagePackObject]
public class InvoiceFilterDto
{
    [Key(0)]
    public string? InvoiceNumber { get; set; }

    [Key(1)]
    public string? Status { get; set; }

    [Key(2)]
    public string? InvoiceType { get; set; }

    [Key(3)]
    public Guid? VendorId { get; set; }

    [Key(4)]
    public Guid? CustomerId { get; set; }

    [Key(5)]
    public DateTime? FromDate { get; set; }

    [Key(6)]
    public DateTime? ToDate { get; set; }

    [Key(7)]
    public decimal? MinAmount { get; set; }

    [Key(8)]
    public decimal? MaxAmount { get; set; }

    [Key(9)]
    public int Page { get; set; } = 1;

    [Key(10)]
    public int PageSize { get; set; } = 20;

    [Key(11)]
    public Guid? PeriodId { get; set; }
}

// ============================================================
// SUMMARY DTOs
// ============================================================

[MessagePackObject]
public class InvoiceSummaryDto
{
    [Key(0)]
    public int TotalInvoices { get; set; }

    [Key(1)]
    public int DraftCount { get; set; }

    [Key(2)]
    public int PostedCount { get; set; }

    [Key(3)]
    public int PaidCount { get; set; }

    [Key(4)]
    public int OverdueCount { get; set; }

    [Key(5)]
    public decimal TotalAmount { get; set; }

    [Key(6)]
    public decimal TotalPaid { get; set; }

    [Key(7)]
    public decimal TotalBalance { get; set; }

    [Key(8)]
    public decimal AverageInvoiceAmount { get; set; }

    [Key(9)]
    public Guid? PeriodId { get; set; }
}

// ============================================================
// AP - SPECIFIC DTOs
// ============================================================

[MessagePackObject]
public class VendorInvoiceSummaryDto
{
    [Key(0)]
    public Guid VendorId { get; set; }

    [Key(1)]
    public string VendorName { get; set; } = default!;

    [Key(2)]
    public int InvoiceCount { get; set; }

    [Key(3)]
    public decimal TotalAmount { get; set; }

    [Key(4)]
    public decimal TotalPaid { get; set; }

    [Key(5)]
    public decimal TotalBalance { get; set; }

    [Key(6)]
    public decimal AveragePaymentDays { get; set; }
}

// ============================================================
// AR - SPECIFIC DTOs
// ============================================================

[MessagePackObject]
public class CustomerInvoiceSummaryDto
{
    [Key(0)]
    public Guid CustomerId { get; set; }

    [Key(1)]
    public string CustomerName { get; set; } = default!;

    [Key(2)]
    public int InvoiceCount { get; set; }

    [Key(3)]
    public decimal TotalAmount { get; set; }

    [Key(4)]
    public decimal TotalPaid { get; set; }

    [Key(5)]
    public decimal TotalBalance { get; set; }

    [Key(6)]
    public decimal AveragePaymentDays { get; set; }
}

// ============================================================
// AGING REPORT DTOs
// ============================================================

[MessagePackObject]
public class AgingReportDto
{
    [Key(0)]
    public string Period { get; set; } = default!;

    [Key(1)]
    public decimal Current { get; set; }

    [Key(2)]
    public decimal Days30_60 { get; set; }

    [Key(3)]
    public decimal Days60_90 { get; set; }

    [Key(4)]
    public decimal Days90Plus { get; set; }

    [Key(5)]
    public decimal Total { get; set; }

    [Key(6)]
    public List<AgingDetailDto> Details { get; set; } = new();
}

[MessagePackObject]
public class AgingDetailDto
{
    [Key(0)]
    public Guid InvoiceId { get; set; }

    [Key(1)]
    public string InvoiceNumber { get; set; } = default!;

    [Key(2)]
    public string PartyName { get; set; } = default!;

    [Key(3)]
    public DateTime InvoiceDate { get; set; }

    [Key(4)]
    public DateTime DueDate { get; set; }

    [Key(5)]
    public decimal Amount { get; set; }

    [Key(6)]
    public int DaysOverdue { get; set; }

    [Key(7)]
    public string AgingBucket { get; set; } = default!;
}