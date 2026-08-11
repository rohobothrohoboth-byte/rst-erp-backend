using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Payroll.Models.DTOs;
using Svc.HRM.Payroll.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Helpers;
using Common;


namespace Svc.HRM.Payroll.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reports")]
public class ReportController : ControllerBase
{
    private readonly IPayrollService _payrollService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IPayrollService payrollService, ILogger<ReportController> logger)
    {
        _payrollService = payrollService;
        _logger = logger;
    }

    /// <summary>
    /// Get payroll summary report
    /// </summary>
    [HttpGet("payroll-summary")]
    [PerAuth(PayPerm.ReportView)]
    public async Task<IActionResult> GetPayrollSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var report = await _payrollService.GetPayrollSummaryReportAsync(from, to, ct);
        return Ok(report);
    }

    /// <summary>
    /// Get employee payslip history
    /// </summary>
    [HttpGet("employee/{employeeId}/history")]
    [PerAuth(PayPerm.ReportView)]
    public async Task<IActionResult> GetEmployeeHistory(Guid employeeId, [FromQuery] int year, CancellationToken ct)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        var history = await _payrollService.GetEmployeePayslipHistoryAsync(employeeId, year, ct);
        return Ok(history);
    }

    /// <summary>
    /// Export bank file
    /// </summary>
    [HttpGet("payroll-run/{payrollRunId}/bank-export")]
    [PerAuth(PayPerm.ReportView)]
    public async Task<IActionResult> ExportBankFile(Guid payrollRunId, CancellationToken ct, [FromQuery] string? format = null)
    {
        // Default to CSV if not specified
        var exportFormat = format?.ToLower() ?? "csv";

        if (exportFormat == "csv")
        {
            var csvData = await _payrollService.GenerateBankExportCsvAsync(payrollRunId, ct);
            return File(csvData, "text/csv", $"BankExport_{payrollRunId}.csv");
        }

        var bankFile = await _payrollService.GenerateBankExportAsync(payrollRunId, ct);
        return Ok(bankFile);
    }
}