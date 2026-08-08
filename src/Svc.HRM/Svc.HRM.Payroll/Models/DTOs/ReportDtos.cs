// Models/DTOs/ReportDtos.cs
namespace Svc.HRM.Payroll.Models.DTOs;

public class PayrollSummaryReport
{
    public int TotalPayrollRuns { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalNetPay { get; set; }
    public decimal TotalTaxes { get; set; }
    public decimal TotalDeductions { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<PayrollRunSummaryDto> PayrollRuns { get; set; } = new();
    public List<DepartmentPayrollSummaryDto> DepartmentBreakdown { get; set; } = new();
}

public class PayrollRunSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public string PayrollStatus { get; set; } = default!;
    public int TotalEmployees { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalNetPay { get; set; }
}

public class DepartmentPayrollSummaryDto
{
    public string DepartmentName { get; set; } = default!;
    public int EmployeeCount { get; set; }
    public decimal TotalGrossPay { get; set; }
    public decimal TotalNetPay { get; set; }
}

public class BankExportFile
{
    public Guid PayrollRunId { get; set; }
    public string PayrollRunName { get; set; } = default!;
    public DateTime PaymentDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalEmployees { get; set; }
    public List<BankTransactionDto> Transactions { get; set; } = new();
}

public class BankTransactionDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = default!;
    public string EmployeeName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string AccountNumber { get; set; } = default!;
    public string BankCode { get; set; } = default!;
}

public class PayslipHistoryDto
{
    public Guid Id { get; set; }
    public string PayslipNumber { get; set; } = default!;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal GrossPay { get; set; }
    public decimal NetPay { get; set; }
    public bool IsGenerated { get; set; }
    public DateTime? GeneratedAt { get; set; }
}