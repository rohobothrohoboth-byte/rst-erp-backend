namespace Svc.HRM.Payroll.Models.DTOs;

public class PayslipDto
{
    public Guid Id { get; set; }
    public string PayslipNumber { get; set; } = default!;
    public string EmployeeName { get; set; } = default!;
    public string EmployeeCode { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Position { get; set; } = default!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal GrossPay { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal BonusPay { get; set; }
    public decimal CommissionPay { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal PensionContribution { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public string? PaymentMethod { get; set; }
    public string? BankAccount { get; set; }
    public int DaysWorked { get; set; }
    public int DaysAbsent { get; set; }
    public int OvertimeHours { get; set; }
    public string? Notes { get; set; }
    public bool IsGenerated { get; set; }
    public DateTime? GeneratedAt { get; set; }
}

public class PayslipGenerateDto
{
    public Guid PayrollRunId { get; set; }
    public bool RegenerateAll { get; set; }
    public List<Guid>? EmployeeIds { get; set; }
}