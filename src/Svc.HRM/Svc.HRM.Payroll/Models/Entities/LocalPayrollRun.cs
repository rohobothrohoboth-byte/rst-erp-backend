namespace Svc.HRM.Payroll.Models.Entities;

public class LocalPayrollRun
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = default!;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PayrollStatus { get; set; } = "Draft"; // Draft, Processing, Completed, Approved
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
    public string? FinancePostingStatus { get; set; } // Created, Posted, Failed
    public DateTime? FinancePostedAt { get; set; }
    public string? FinancePostingError { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<LocalPayrollEmployee> PayrollEmployees { get; set; } = new List<LocalPayrollEmployee>();
}