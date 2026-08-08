namespace Svc.HRM.Payroll.Models.Entities;

public class LocalPayrollEmployee
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid PayrollRunId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = default!;
    public string EmployeeName { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Position { get; set; } = default!;
    public decimal BaseSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal BonusPay { get; set; }
    public decimal CommissionPay { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal PensionContribution { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal NetPay { get; set; }
    public int DaysWorked { get; set; }
    public int DaysAbsent { get; set; }
    public int OvertimeHours { get; set; }
    public string? Notes { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public LocalPayrollRun PayrollRun { get; set; } = null!;
}