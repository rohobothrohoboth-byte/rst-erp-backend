namespace Svc.HRM.Payroll.Models.Entities;

public class LocalPayslip
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid PayrollEmployeeId { get; set; }
    public Guid EmployeeId { get; set; }
    public string PayslipNumber { get; set; } = default!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetPay { get; set; }
    public string? PaymentMethod { get; set; }
    public string? BankAccount { get; set; }
    public bool IsGenerated { get; set; }
    public DateTime? GeneratedAt { get; set; }
    public string? GeneratedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public LocalPayrollEmployee PayrollEmployee { get; set; } = null!;
}