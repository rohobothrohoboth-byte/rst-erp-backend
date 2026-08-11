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
[Route("api/v{version:apiVersion}/employee-salaries")]
public class EmployeeSalaryController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public EmployeeSalaryController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpGet]
    [PerAuth(PayPerm.SalaryView)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var salaries = await _payrollService.GetAllEmployeeSalariesAsync(ct);
        return Ok(salaries);
    }

    [HttpGet("employee/{employeeId}")]
    [PerAuth(PayPerm.SalaryView)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
    {
        var salaries = await _payrollService.GetEmployeeSalariesAsync(employeeId, ct);
        return Ok(salaries);
    }

    [HttpGet("{id}")]
    [PerAuth(PayPerm.SalaryView)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var salary = await _payrollService.GetEmployeeSalaryAsync(id, ct);
        return Ok(salary);
    }

    [HttpPost]
    [PerAuth(PayPerm.SalaryManage)]
    public async Task<IActionResult> Create([FromBody] EmployeeSalaryCreateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.AssignSalaryToEmployeeAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [PerAuth(PayPerm.SalaryManage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeSalaryCreateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.UpdateEmployeeSalaryAsync(id, dto, ct);
        return Ok(result);
    }
}