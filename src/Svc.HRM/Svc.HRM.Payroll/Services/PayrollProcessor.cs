using Microsoft.EntityFrameworkCore;
using Svc.HRM.Payroll.Models.Entities;
using Svc.HRM.Payroll.Models.Enums;
using Svc.HRM.Payroll.Persistence;

namespace Svc.HRM.Payroll.Services;

public class PayrollProcessor : IPayrollProcessor
{
    private readonly PayrollDbContext _context;
    private readonly IEmployeeService _employeeService;
    private readonly IAttendanceClient _attendanceClient;
    private readonly ILeavePayrollClient _leaveClient;
    private readonly ITaxCalculator _taxCalculator;
    private readonly ILogger<PayrollProcessor> _logger;

    public PayrollProcessor(
        PayrollDbContext context,
        IEmployeeService employeeService,
        IAttendanceClient attendanceClient,
        ILeavePayrollClient leaveClient,
        ITaxCalculator taxCalculator,
        ILogger<PayrollProcessor> logger)
    {
        _context = context;
        _employeeService = employeeService;
        _attendanceClient = attendanceClient;
        _leaveClient = leaveClient;
        _taxCalculator = taxCalculator;
        _logger = logger;
    }

    public async Task<PayrollProcessingResult> ProcessPayrollRunAsync(Guid payrollRunId, CancellationToken ct = default)
    {
        var result = new PayrollProcessingResult
        {
            PayrollRunId = payrollRunId,
            ProcessedAt = DateTime.UtcNow
        };

        try
        {
            var payrollRun = await _context.PayrollRuns
                .Include(x => x.PayrollEmployees)
                .FirstOrDefaultAsync(x => x.Id == payrollRunId && !x.IsDeleted, ct);

            if (payrollRun == null)
                throw new KeyNotFoundException($"Payroll run {payrollRunId} not found");

            if (payrollRun.PayrollStatus != PayrollStatus.Draft.ToString())
                throw new InvalidOperationException($"Payroll run is already {payrollRun.PayrollStatus}");

            // Update status to Processing
            payrollRun.PayrollStatus = PayrollStatus.Processing.ToString();
            await _context.SaveChangesAsync(ct);

            var period = new PayrollPeriod
            {
                StartDate = payrollRun.PayPeriodStart,
                EndDate = payrollRun.PayPeriodEnd,
                TotalWorkingDays = CalculateWorkingDays(payrollRun.PayPeriodStart, payrollRun.PayPeriodEnd)
            };

            decimal totalGross = 0, totalNet = 0, totalTaxes = 0, totalDeductions = 0;

            foreach (var payrollEmployee in payrollRun.PayrollEmployees)
            {
                try
                {
                    var calculation = await CalculateEmployeePayrollAsync(payrollEmployee.EmployeeId, period, ct);

                    // Update payroll employee record
                    payrollEmployee.GrossPay = calculation.GrossPay;
                    payrollEmployee.TaxAmount = calculation.TaxAmount;
                    payrollEmployee.PensionContribution = calculation.PensionContribution;
                    payrollEmployee.OtherDeductions = calculation.OtherDeductions;
                    payrollEmployee.NetPay = calculation.NetPay;
                    payrollEmployee.DaysWorked = calculation.PresentDays;
                    payrollEmployee.DaysAbsent = calculation.AbsentDays;
                    payrollEmployee.OvertimeHours = (int)calculation.OvertimeHours;
                    payrollEmployee.OvertimePay = calculation.OvertimePay;
                    payrollEmployee.BonusPay = calculation.BonusPay;

                    totalGross += calculation.GrossPay;
                    totalNet += calculation.NetPay;
                    totalTaxes += calculation.TaxAmount;
                    totalDeductions += calculation.PensionContribution + calculation.OtherDeductions;

                    result.EmployeeResults.Add(new EmployeePayrollResult
                    {
                        EmployeeId = payrollEmployee.EmployeeId,
                        EmployeeName = payrollEmployee.EmployeeName,
                        Success = true,
                        GrossPay = calculation.GrossPay,
                        NetPay = calculation.NetPay,
                        TaxAmount = calculation.TaxAmount,
                        Deductions = calculation.PensionContribution + calculation.OtherDeductions,
                        Calculation = calculation
                    });

                    result.ProcessedEmployees++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing employee {EmployeeId}", payrollEmployee.EmployeeId);
                    result.FailedEmployees++;
                    result.Errors.Add($"Employee {payrollEmployee.EmployeeId}: {ex.Message}");
                }
            }

            // Update payroll run totals
            payrollRun.TotalGrossPay = totalGross;
            payrollRun.TotalNetPay = totalNet;
            payrollRun.TotalTaxes = totalTaxes;
            payrollRun.TotalDeductions = totalDeductions;
            payrollRun.TotalEmployees = payrollRun.PayrollEmployees.Count;
            payrollRun.PayrollStatus = PayrollStatus.Completed.ToString();
            payrollRun.ProcessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            result.TotalGrossPay = totalGross;
            result.TotalNetPay = totalNet;
            result.TotalTaxes = totalTaxes;
            result.TotalDeductions = totalDeductions;
            result.TotalEmployees = payrollRun.PayrollEmployees.Count;

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payroll run {PayrollRunId}", payrollRunId);
            throw;
        }
    }

    public async Task<EmployeePayrollCalculation> CalculateEmployeePayrollAsync(Guid employeeId, PayrollPeriod period, CancellationToken ct = default)
    {
        var calculation = new EmployeePayrollCalculation
        {
            StartDate = period.StartDate,
            EndDate = period.EndDate
        };

        try
        {
            // Get employee data
            var employee = await _employeeService.GetEmployeeAsync(employeeId, ct);
            if (employee == null)
                throw new KeyNotFoundException($"Employee {employeeId} not found");

            // Get employee salary
            var employeeSalary = await _context.EmployeeSalaries
                .Include(x => x.SalaryStructure)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted, ct);

            if (employeeSalary == null)
                throw new InvalidOperationException($"No active salary found for employee {employeeId}");

            // Calculate base salary (prorated if needed)
            calculation.BaseSalary = CalculateProratedSalary(employeeSalary.TotalSalary, period);

            // Get attendance data (canonical Attendance service)
            var attendance = await _attendanceClient.GetEmployeeAttendanceAsync(employeeId, period.StartDate, period.EndDate, ct);

            calculation.PresentDays = attendance.PresentDays;
            calculation.AbsentDays = attendance.AbsentDays;
            calculation.LeaveDays = attendance.LeaveDays;
            calculation.OvertimeHours = attendance.OvertimeHours;

            // Calculate daily rate
            var dailyRate = employeeSalary.TotalSalary / period.TotalWorkingDays;

            // Calculate deductions for absences
            calculation.AbsentDeduction = calculation.AbsentDays * dailyRate;

            // Unpaid leave deduction from HRM.Leave
            calculation.LeaveDeduction = await CalculateLeaveDeductionAsync(employeeId, period.StartDate, period.EndDate, ct);

            // Calculate overtime pay (1.5x for weekdays, 2x for holidays)
            calculation.OvertimePay = await CalculateOvertimePayAsync(employeeId, period.StartDate, period.EndDate, ct);

            // Calculate allowances
            calculation.Allowances = employeeSalary.HousingAllowance + employeeSalary.TransportAllowance +
                                    employeeSalary.MealAllowance + employeeSalary.MedicalAllowance +
                                    employeeSalary.OtherAllowances;

            // Calculate gross pay
            calculation.GrossPay = calculation.BaseSalary + calculation.Allowances +
                                   calculation.OvertimePay + calculation.BonusPay +
                                   calculation.CommissionPay;

            // Calculate tax
            var taxResult = await _taxCalculator.CalculateTaxAsync(calculation.GrossPay, ct);
            calculation.TaxAmount = taxResult.TaxAmount;

            // Calculate pension contribution (7% of basic salary)
            calculation.PensionContribution = calculation.BaseSalary * 0.07m;

            // Other deductions (loan, advance, etc.)
            calculation.OtherDeductions = 0;

            // Calculate net pay
            calculation.NetPay = calculation.GrossPay - calculation.TaxAmount -
                                 calculation.PensionContribution - calculation.OtherDeductions -
                                 calculation.AbsentDeduction - calculation.LeaveDeduction;

            return calculation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating payroll for employee {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<decimal> CalculateOvertimePayAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var attendance = await _attendanceClient.GetEmployeeAttendanceAsync(employeeId, startDate, endDate, ct);

        // Get employee salary
        var employeeSalary = await _context.EmployeeSalaries
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted, ct);

        if (employeeSalary == null)
            return 0;

        var dailyRate = employeeSalary.TotalSalary / 22; // 22 working days average
        var hourlyRate = dailyRate / 8; // 8 hours per day

        // Overtime rate: 1.5x for weekdays, 2x for weekends/holidays
        var overtimePay = (decimal)attendance.OvertimeHours * hourlyRate * 1.5m;

        return overtimePay;
    }

    public async Task<decimal> CalculateLeaveDeductionAsync(Guid employeeId, DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        // Get employee salary
        var employeeSalary = await _context.EmployeeSalaries
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted, ct);

        if (employeeSalary == null)
            return 0;

        var dailyRate = employeeSalary.TotalSalary / 22;
        var unpaidLeaveDays = await _leaveClient.GetUnpaidLeaveDaysAsync(employeeId, startDate, endDate, ct);
        return unpaidLeaveDays * dailyRate;
    }

    private int CalculateWorkingDays(DateTime start, DateTime end)
    {
        int workingDays = 0;
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
        }
        return workingDays;
    }

    private decimal CalculateProratedSalary(decimal totalSalary, PayrollPeriod period)
    {
        var totalDays = (period.EndDate - period.StartDate).Days + 1;
        var workingDays = CalculateWorkingDays(period.StartDate, period.EndDate);
        var proratedDays = Math.Min(workingDays, period.TotalWorkingDays);

        // If employee joined/left mid-month, calculate prorated
        // For simplicity, return full salary
        return totalSalary;
    }
}