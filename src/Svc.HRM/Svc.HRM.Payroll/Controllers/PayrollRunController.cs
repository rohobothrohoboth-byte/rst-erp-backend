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
[Route("api/v{version:apiVersion}/payroll-runs")]
public class PayrollRunController : ControllerBase
{
    private readonly IPayrollService _payrollService;
    private readonly ILogger<PayrollRunController> _logger;

    public PayrollRunController(IPayrollService payrollService, ILogger<PayrollRunController> logger)
    {
        _payrollService = payrollService;
        _logger = logger;
    }

    [HttpGet]
    [PerAuth(PayPerm.RunView)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var runs = await _payrollService.GetAllPayrollRunsAsync(ct);
        return Ok(runs);
    }

    [HttpGet("{id}")]
    [PerAuth(PayPerm.RunView)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var run = await _payrollService.GetPayrollRunAsync(id, ct);
        return Ok(run);
    }

    [HttpPost]
    [PerAuth(PayPerm.RunCreate)]
    public async Task<IActionResult> Create([FromBody] PayrollRunCreateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.CreatePayrollRunAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPost("{id}/process")]
    [PerAuth(PayPerm.RunProcess)]
    public async Task<IActionResult> Process(Guid id, CancellationToken ct)
    {
        var result = await _payrollService.ProcessPayrollRunAsync(id, ct);
        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    [PerAuth(PayPerm.RunApprove)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovePayrollRequest request, CancellationToken ct)
    {
        var result = await _payrollService.ApprovePayrollRunAsync(id, request.ApprovedBy, ct);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    [PerAuth(PayPerm.RunApprove)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] PayrollRunStatusUpdateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.UpdatePayrollRunStatusAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpPost("{id}/payslips")]
    [PerAuth(PayPerm.PayslipGenerate)]
    public async Task<IActionResult> GeneratePayslips(Guid id, CancellationToken ct)
    {
        var result = await _payrollService.GeneratePayslipsAsync(id, ct);
        return Ok(result);
    }
}

public class ApprovePayrollRequest
{
    public string ApprovedBy { get; set; } = default!;
}