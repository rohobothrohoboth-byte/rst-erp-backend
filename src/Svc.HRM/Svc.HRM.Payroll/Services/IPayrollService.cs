using Svc.HRM.Payroll.Models.DTOs;

namespace Svc.HRM.Payroll.Services;
public interface IPayrollService
{
    // Salary Structure
    Task<SalaryStructureDto> CreateSalaryStructureAsync(SalaryStructureCreateDto dto, CancellationToken ct = default);
    Task<SalaryStructureDto> UpdateSalaryStructureAsync(Guid id, SalaryStructureCreateDto dto, CancellationToken ct = default);
    Task<SalaryStructureDto> GetSalaryStructureAsync(Guid id, CancellationToken ct = default);
    Task<List<SalaryStructureDto>> GetAllSalaryStructuresAsync(CancellationToken ct = default);
    Task DeleteSalaryStructureAsync(Guid id, CancellationToken ct = default);

    // Employee Salary
    Task<EmployeeSalaryDto> AssignSalaryToEmployeeAsync(EmployeeSalaryCreateDto dto, CancellationToken ct = default);
    Task<EmployeeSalaryDto> UpdateEmployeeSalaryAsync(Guid id, EmployeeSalaryCreateDto dto, CancellationToken ct = default);
    Task<EmployeeSalaryDto> GetEmployeeSalaryAsync(Guid id, CancellationToken ct = default);
    Task<List<EmployeeSalaryDto>> GetEmployeeSalariesAsync(Guid employeeId, CancellationToken ct = default);
    Task<List<EmployeeSalaryDto>> GetAllEmployeeSalariesAsync(CancellationToken ct = default);

    // Payroll Run
    Task<PayrollRunDto> CreatePayrollRunAsync(PayrollRunCreateDto dto, CancellationToken ct = default);

    Task<PayrollRunDto> ApprovePayrollRunAsync(Guid id, string approvedBy, CancellationToken ct = default);
    Task<PayrollRunDto> GetPayrollRunAsync(Guid id, CancellationToken ct = default);
    Task<List<PayrollRunDto>> GetAllPayrollRunsAsync(CancellationToken ct = default);
    Task<PayrollRunDto> UpdatePayrollRunStatusAsync(Guid id, PayrollRunStatusUpdateDto dto, CancellationToken ct = default);

    // Tax
    Task<TaxCalculationDto> CalculateTaxAsync(decimal grossIncome, CancellationToken ct = default);
    Task<TaxRateDto> CreateTaxRateAsync(TaxRateCreateDto dto, CancellationToken ct = default);
    Task<List<TaxRateDto>> GetTaxRatesAsync(string? taxYear = null, CancellationToken ct = default);

    // Payslip
    Task<PayslipDto> GeneratePayslipAsync(Guid payrollEmployeeId, CancellationToken ct = default);
    Task<List<PayslipDto>> GeneratePayslipsAsync(Guid payrollRunId, CancellationToken ct = default);
    Task<PayslipDto> GetPayslipAsync(Guid id, CancellationToken ct = default);
    Task<PayslipDto> GetPayslipByEmployeeAsync(Guid employeeId, Guid payrollRunId, CancellationToken ct = default);
    Task<List<PayslipDto>> GetPayslipsByEmployeeAsync(Guid employeeId, CancellationToken ct = default);
    Task<List<PayslipDto>> GetAllPayslipsAsync(CancellationToken ct = default);
    Task<byte[]> GeneratePayslipPdfAsync(Guid id, CancellationToken ct = default);
     Task<PayrollProcessingResult> ProcessPayrollRunAsync(Guid id, CancellationToken ct = default);


      Task<PayrollSummaryReport> GetPayrollSummaryReportAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
         Task<BankExportFile> GenerateBankExportAsync(Guid payrollRunId, CancellationToken ct = default);
         Task<byte[]> GenerateBankExportCsvAsync(Guid payrollRunId, CancellationToken ct = default);
         Task SendPayslipEmailAsync(Guid payslipId, CancellationToken ct = default);
         Task SendBulkPayslipEmailsAsync(Guid payrollRunId, CancellationToken ct = default);
         Task<List<PayslipHistoryDto>> GetEmployeePayslipHistoryAsync(Guid employeeId, int year, CancellationToken ct = default);
}