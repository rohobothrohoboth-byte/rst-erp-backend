// Svc.Finance.Models.DTOs - PaymentDtos.cs

using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace Cor.Finance.Models.DTOs;

// ============================================================
// PAYMENT DTO (Response)
// ============================================================

[MessagePackObject]
public class PaymentDto
{
    [MessagePack.Key(0)]
    public Guid Id { get; set; }

    [MessagePack.Key(1)]
    public string PaymentNumber { get; set; } = default!;

    [MessagePack.Key(2)]
    public DateTime PaymentDate { get; set; }

    [MessagePack.Key(3)]
    public string PaymentType { get; set; } = default!;

    [MessagePack.Key(4)]
    public string PaymentMethod { get; set; } = default!;

    [MessagePack.Key(5)]
    public decimal Amount { get; set; }

    [MessagePack.Key(6)]
    public string? Description { get; set; }

    [MessagePack.Key(7)]
    public string Status { get; set; } = default!;

    [MessagePack.Key(8)]
    public string? Reference { get; set; }

    [MessagePack.Key(9)]
    public Guid? VendorId { get; set; }

    [MessagePack.Key(10)]
    public string? VendorName { get; set; }

    [MessagePack.Key(11)]
    public Guid? CustomerId { get; set; }

    [MessagePack.Key(12)]
    public string? CustomerName { get; set; }

    [MessagePack.Key(13)]
    public Guid? InvoiceId { get; set; }

    [MessagePack.Key(14)]
    public string? InvoiceNumber { get; set; }

    [MessagePack.Key(15)]
    public Guid? JournalEntryId { get; set; }

    [MessagePack.Key(16)]
    public Guid? BankAccountId { get; set; }

    [MessagePack.Key(17)]
    public string? BankAccountName { get; set; }

    [MessagePack.Key(18)]
    public Guid? BranchId { get; set; }

    [MessagePack.Key(19)]
    public string? BranchName { get; set; }

    [MessagePack.Key(20)]
    public Guid? EmployeeId { get; set; }

    [MessagePack.Key(21)]
    public string? EmployeeName { get; set; }

    [MessagePack.Key(22)]
    public string? PeriodName { get; set; }

    [MessagePack.Key(23)]
    public DateTime DateAdd { get; set; }

    [MessagePack.Key(24)]
    public DateTime? DateMod { get; set; }

    [MessagePack.Key(25)]
    public Guid? PeriodId { get; set; }

    [MessagePack.Key(26)]
    public string RowVersion { get; set; } = default!;
}

// ============================================================
// CREATE PAYMENT DTOs
// ============================================================

[MessagePackObject]
public class BasePaymentCreateDto
{
    [MessagePack.Key(0)]
    [Required(ErrorMessage = "Payment date is required")]
    public DateTime PaymentDate { get; set; }

    [MessagePack.Key(1)]
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [MessagePack.Key(2)]
    [Required(ErrorMessage = "Payment method is required")]
    public string PaymentMethod { get; set; } = default!;

    [MessagePack.Key(3)]
    public string? Reference { get; set; }

    [MessagePack.Key(4)]
    public string? Description { get; set; }

    [MessagePack.Key(5)]
    public Guid? InvoiceId { get; set; }

    [MessagePack.Key(6)]
    public Guid? BankAccountId { get; set; }

    [MessagePack.Key(7)]
    public string Currency { get; set; } = "USD";

    [MessagePack.Key(8)]
    public Guid? BranchId { get; set; }

    [MessagePack.Key(9)]
    public Guid? EmployeeId { get; set; }

    [MessagePack.Key(10)]
    public Guid? PeriodId { get; set; }
}

[MessagePackObject]
public class VendorPaymentCreateDto : BasePaymentCreateDto
{
    [MessagePack.Key(11)]  // ✅ Start from 11 (after BasePaymentCreateDto keys)
    [Required(ErrorMessage = "Vendor is required")]
    public Guid VendorId { get; set; }

    [MessagePack.Key(12)]
    public string PaymentType { get; set; } = "Purchase";

    [MessagePack.Key(13)]
    public string PaymentStatus { get; set; } = "Pending";
}

[MessagePackObject]
public class CustomerPaymentCreateDto : BasePaymentCreateDto
{
    [MessagePack.Key(11)]  // ✅ Start from 11
    [Required(ErrorMessage = "Customer is required")]
    public Guid CustomerId { get; set; }

    [MessagePack.Key(12)]
    public string PaymentType { get; set; } = "Sales";

    [MessagePack.Key(13)]
    public string PaymentStatus { get; set; } = "Pending";
}

[MessagePackObject]
public class AddPaymentDto : BasePaymentCreateDto
{
    [MessagePack.Key(11)]  // ✅ Start from 11
    public Guid? VendorId { get; set; }

    [MessagePack.Key(12)]
    public Guid? CustomerId { get; set; }

    [MessagePack.Key(13)]
    public string PaymentType { get; set; } = "Purchase";

    [MessagePack.Key(14)]
    public string PaymentStatus { get; set; } = "Pending";

    [MessagePack.Key(15)]
    public Guid? FromAccountId { get; set; }

    [MessagePack.Key(16)]
    public Guid? ToAccountId { get; set; }

    [MessagePack.Key(17)]
    public Guid? JournalEntryId { get; set; }

    [MessagePack.Key(18)]
    [JsonIgnore]
    public string? BankAccountIdString { get; set; }
}

// ============================================================
// UPDATE PAYMENT DTOs
// ============================================================

[MessagePackObject]
public class EditPaymentDto
{
    [MessagePack.Key(0)]
    public Guid Id { get; set; }

    [MessagePack.Key(1)]
    public DateTime PaymentDate { get; set; }

    [MessagePack.Key(2)]
    public string PaymentType { get; set; } = default!;

    [MessagePack.Key(3)]
    public string PaymentMethod { get; set; } = default!;

    [MessagePack.Key(4)]
    public decimal Amount { get; set; }

    [MessagePack.Key(5)]
    public string? Description { get; set; }

    [MessagePack.Key(6)]
    public string? Reference { get; set; }

    [MessagePack.Key(7)]
    public Guid? InvoiceId { get; set; }

    [MessagePack.Key(8)]
    public Guid? JournalEntryId { get; set; }

    [MessagePack.Key(9)]
    public Guid? BankAccountId { get; set; }

    [MessagePack.Key(10)]
    public Guid? BranchId { get; set; }

    [MessagePack.Key(11)]
    public Guid? EmployeeId { get; set; }

    [MessagePack.Key(12)]
    public Guid? VendorId { get; set; }

    [MessagePack.Key(13)]
    public Guid? CustomerId { get; set; }

    [MessagePack.Key(14)]
    public string RowVersion { get; set; } = default!;

    [MessagePack.Key(15)]
    public Guid? PeriodId { get; set; }
}

// ============================================================
// PAYMENT SUMMARY DTOs
// ============================================================

[MessagePackObject]
public class PaymentSummaryDto
{
    [MessagePack.Key(0)]
    public decimal TotalPayments { get; set; }

    [MessagePack.Key(1)]
    public decimal TotalProcessed { get; set; }

    [MessagePack.Key(2)]
    public decimal TotalPending { get; set; }

    [MessagePack.Key(3)]
    public decimal TotalCancelled { get; set; }

    [MessagePack.Key(4)]
    public int PaymentCount { get; set; }

    [MessagePack.Key(5)]
    public Guid? PeriodId { get; set; }

    [MessagePack.Key(6)]
    public Dictionary<string, decimal> PaymentsByMethod { get; set; } = new();

    [MessagePack.Key(7)]
    public Dictionary<string, int> PaymentsByStatus { get; set; } = new();

    [MessagePack.Key(8)]
    public string? PeriodName { get; set; }

    [MessagePack.Key(9)]
    public decimal TotalVendorPayments { get; set; }

    [MessagePack.Key(10)]
    public decimal TotalCustomerPayments { get; set; }
}

// ============================================================
// PAYMENT FILTER DTO
// ============================================================

[MessagePackObject]
public class PaymentFilterDto
{
    [MessagePack.Key(0)]
    public string? PaymentNumber { get; set; }

    [MessagePack.Key(1)]
    public string? Status { get; set; }

    [MessagePack.Key(2)]
    public string? PaymentType { get; set; }

    [MessagePack.Key(3)]
    public string? PaymentMethod { get; set; }

    [MessagePack.Key(4)]
    public Guid? PeriodId { get; set; }

    [MessagePack.Key(5)]
    public Guid? VendorId { get; set; }

    [MessagePack.Key(6)]
    public Guid? CustomerId { get; set; }

    [MessagePack.Key(7)]
    public Guid? InvoiceId { get; set; }

    [MessagePack.Key(8)]
    public DateTime? FromDate { get; set; }

    [MessagePack.Key(9)]
    public DateTime? ToDate { get; set; }

    [MessagePack.Key(10)]
    public decimal? MinAmount { get; set; }

    [MessagePack.Key(11)]
    public decimal? MaxAmount { get; set; }

    [MessagePack.Key(12)]
    public int Page { get; set; } = 1;

    [MessagePack.Key(13)]
    public int PageSize { get; set; } = 20;
}

// ============================================================
// PAYMENT RECONCILIATION DTO
// ============================================================

[MessagePackObject]
public class PaymentReconciliationDto
{
    [MessagePack.Key(0)]
    public Guid PaymentId { get; set; }

    [MessagePack.Key(1)]
    public string PaymentNumber { get; set; } = default!;

    [MessagePack.Key(2)]
    public decimal Amount { get; set; }

    [MessagePack.Key(3)]
    public DateTime PaymentDate { get; set; }

    [MessagePack.Key(4)]
    public string? BankReference { get; set; }

    [MessagePack.Key(5)]
    public bool IsReconciled { get; set; }

    [MessagePack.Key(6)]
    public DateTime? ReconciledAt { get; set; }

    [MessagePack.Key(7)]
    public string? ReconciledBy { get; set; }

    [MessagePack.Key(8)]
    public string? Notes { get; set; }
}

// ============================================================
// BULK PAYMENT DTO
// ============================================================

[MessagePackObject]
public class BulkPaymentCreateDto
{
    [MessagePack.Key(0)]
    [Required(ErrorMessage = "Payments list is required")]
    public List<AddPaymentDto> Payments { get; set; } = new();

    [MessagePack.Key(1)]
    public string? BatchReference { get; set; }

    [MessagePack.Key(2)]
    public DateTime? ProcessDate { get; set; }
}

// ============================================================
// PAYMENT STATISTICS DTOs
// ============================================================

[MessagePackObject]
public class PaymentStatisticsDto
{
    [MessagePack.Key(0)]
    public int TotalPayments { get; set; }

    [MessagePack.Key(1)]
    public decimal TotalAmount { get; set; }

    [MessagePack.Key(2)]
    public decimal AveragePayment { get; set; }

    [MessagePack.Key(3)]
    public decimal MaxPayment { get; set; }

    [MessagePack.Key(4)]
    public decimal MinPayment { get; set; }

    [MessagePack.Key(5)]
    public Dictionary<string, PaymentTypeStatsDto> ByType { get; set; } = new();

    [MessagePack.Key(6)]
    public Dictionary<string, PaymentStatusStatsDto> ByStatus { get; set; } = new();

    [MessagePack.Key(7)]
    public List<MonthlyPaymentStatsDto> MonthlyTrend { get; set; } = new();
}

[MessagePackObject]
public class PaymentTypeStatsDto
{
    [MessagePack.Key(0)]
    public string PaymentType { get; set; } = default!;

    [MessagePack.Key(1)]
    public int Count { get; set; }

    [MessagePack.Key(2)]
    public decimal TotalAmount { get; set; }

    [MessagePack.Key(3)]
    public decimal Percentage { get; set; }
}

[MessagePackObject]
public class PaymentStatusStatsDto
{
    [MessagePack.Key(0)]
    public string Status { get; set; } = default!;

    [MessagePack.Key(1)]
    public int Count { get; set; }

    [MessagePack.Key(2)]
    public decimal TotalAmount { get; set; }

    [MessagePack.Key(3)]
    public decimal Percentage { get; set; }
}

[MessagePackObject]
public class MonthlyPaymentStatsDto
{
    [MessagePack.Key(0)]
    public string Month { get; set; } = default!;

    [MessagePack.Key(1)]
    public int Count { get; set; }

    [MessagePack.Key(2)]
    public decimal TotalAmount { get; set; }

    [MessagePack.Key(3)]
    public decimal VendorPayments { get; set; }

    [MessagePack.Key(4)]
    public decimal CustomerPayments { get; set; }
}