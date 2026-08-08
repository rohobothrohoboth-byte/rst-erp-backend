namespace Svc.HRM.Payroll.Models.DTOs;

public class PayrollRunDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PayrollStatus { get; set; } = default!;
    public decimal TotalGrossPay { get; set; }
    public decimal TotalNetPay { get; set; }
    public decimal TotalTaxes { get; set; }
    public decimal TotalDeductions { get; set; }
    public int TotalEmployees { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
    public Guid? FinanceJournalEntryId { get; set; }
    public string? FinancePostingStatus { get; set; }
    public DateTime? FinancePostedAt { get; set; }
    public string? FinancePostingError { get; set; }
    public List<PayrollEmployeeDto> Employees { get; set; } = new();
}

public class PayrollRunCreateDto
{
    public string Name { get; set; } = default!;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public List<Guid> EmployeeIds { get; set; } = new();
    public string? Notes { get; set; }
}

public class PayrollRunUpdateDto
{
    public string? Name { get; set; }
    public DateTime? PayPeriodStart { get; set; }
    public DateTime? PayPeriodEnd { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Notes { get; set; }
}

public class PayrollRunStatusUpdateDto
{
    public string Status { get; set; } = default!; // Draft, Processing, Completed, Approved
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
}