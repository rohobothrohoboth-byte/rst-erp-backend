using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Payroll.Models.DTOs;
using Svc.HRM.Payroll.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Helpers;

namespace Svc.HRM.Payroll.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payslips")]
public class PayslipController : ControllerBase
{
    private readonly IPayrollService _payrollService;
    private readonly ILogger<PayslipController> _logger;

    public PayslipController(IPayrollService payrollService, ILogger<PayslipController> logger)
    {
        _payrollService = payrollService;
        _logger = logger;
    }

    /// <summary>
    /// Get all payslips with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 100, CancellationToken ct = default)
    {
        try
        {
            var payslips = await _payrollService.GetAllPayslipsAsync(ct);

            var pagedItems = payslips
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                total = payslips.Count,
                page = page,
                pageSize = pageSize,
                items = pagedItems
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all payslips");
            return StatusCode(500, new { error = "Error retrieving payslips" });
        }
    }

    /// <summary>
    /// Get payslip by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        try
        {
            var payslip = await _payrollService.GetPayslipAsync(id, ct);
            return Ok(payslip);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Payslip with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payslip {Id}", id);
            return StatusCode(500, new { error = "Error retrieving payslip" });
        }
    }

    /// <summary>
    /// Get payslips by employee ID
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100, CancellationToken ct = default)
    {
        try
        {
            var payslips = await _payrollService.GetPayslipsByEmployeeAsync(employeeId, ct);

            var pagedItems = payslips
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                employeeId = employeeId,
                total = payslips.Count,
                page = page,
                pageSize = pageSize,
                items = pagedItems
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payslips for employee {EmployeeId}", employeeId);
            return StatusCode(500, new { error = "Error retrieving employee payslips" });
        }
    }

    /// <summary>
    /// Get payslip by employee ID and payroll run ID
    /// </summary>
    [HttpGet("employee/{employeeId:guid}/payroll-run/{payrollRunId:guid}")]
    public async Task<IActionResult> GetByEmployeeAndPayrollRun(Guid employeeId, Guid payrollRunId, CancellationToken ct)
    {
        try
        {
            var payslip = await _payrollService.GetPayslipByEmployeeAsync(employeeId, payrollRunId, ct);
            return Ok(payslip);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Payslip not found for employee {employeeId} in payroll run {payrollRunId}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payslip for employee {EmployeeId} in payroll run {PayrollRunId}", employeeId, payrollRunId);
            return StatusCode(500, new { error = "Error retrieving payslip" });
        }
    }

    /// <summary>
    /// Generate payslip for a specific payroll employee
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GeneratePayslipRequest request, CancellationToken ct)
    {
        try
        {
            var payslip = await _payrollService.GeneratePayslipAsync(request.PayrollEmployeeId, ct);
            return Ok(payslip);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating payslip for {PayrollEmployeeId}", request.PayrollEmployeeId);
            return StatusCode(500, new { error = "Error generating payslip" });
        }
    }

    /// <summary>
    /// Generate payslips for a payroll run
    /// </summary>
    [HttpPost("generate/payroll-run/{payrollRunId:guid}")]
    public async Task<IActionResult> GenerateForPayrollRun(Guid payrollRunId, CancellationToken ct)
    {
        try
        {
            var payslips = await _payrollService.GeneratePayslipsAsync(payrollRunId, ct);
            return Ok(new
            {
                payrollRunId = payrollRunId,
                total = payslips.Count,
                items = payslips
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating payslips for payroll run {PayrollRunId}", payrollRunId);
            return StatusCode(500, new { error = "Error generating payslips" });
        }
    }
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        try
        {
            var payslip = await _payrollService.GetPayslipAsync(id, ct);
            var htmlData = await _payrollService.GeneratePayslipPdfAsync(id, ct);

            if (htmlData == null || htmlData.Length == 0)
            {
                return NotFound(new { error = "Payslip could not be generated" });
            }

            // Return as HTML
            return File(htmlData, "text/html", $"Payslip_{payslip.PayslipNumber}.html");
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Payslip with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading payslip {Id}", id);
            return StatusCode(500, new { error = "Error downloading payslip" });
        }
    }
}

public class GeneratePayslipRequest
{
    public Guid PayrollEmployeeId { get; set; }
}