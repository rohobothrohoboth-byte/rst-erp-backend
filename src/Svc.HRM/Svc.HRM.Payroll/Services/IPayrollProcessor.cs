namespace Svc.HRM.Payroll.Services;

public interface IPayrollProcessor
{
    Task<PayrollProcessingResult> ProcessPayrollRunAsync(Guid payrollRunId, CancellationToken ct = default);
    Task<EmployeePayrollCalculation> CalculateEmployeePayrollAsync(Guid employeeId, PayrollPeriod period, CancellationToken ct = default);
    Task<decimal> CalculateOvertimePayAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<decimal> CalculateLeaveDeductionAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default);
}

public class PayrollProcessingResult
{
    public Guid PayrollRunId { get; set; }
    public int TotalEmployees { get; set; }
    public int ProcessedEmployees { get; set; }
    public int FailedEmployees { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalNetPay { get; set; }
    public decimal TotalTaxes { get; set; }
    public decimal TotalDeductions { get; set; }
    public List<EmployeePayrollResult> EmployeeResults { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public DateTime ProcessedAt { get; set; }
}

public class EmployeePayrollResult
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public bool Success { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Deductions { get; set; }
    public List<string>? Errors { get; set; }
    public EmployeePayrollCalculation Calculation { get; set; } = new();
}

public class EmployeePayrollCalculation
{
    public decimal BaseSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal BonusPay { get; set; }
    public decimal CommissionPay { get; set; }
    public decimal LeaveDeduction { get; set; }
    public decimal AbsentDeduction { get; set; }
    public decimal GrossPay { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal PensionContribution { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal NetPay { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public double OvertimeHours { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class PayrollPeriod
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalWorkingDays { get; set; }
}