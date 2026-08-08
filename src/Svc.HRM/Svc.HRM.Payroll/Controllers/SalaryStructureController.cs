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
[Route("api/v{version:apiVersion}/salary-structures")]
public class SalaryStructureController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public SalaryStructureController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var structures = await _payrollService.GetAllSalaryStructuresAsync(ct);
        return Ok(structures);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var structure = await _payrollService.GetSalaryStructureAsync(id, ct);
        return Ok(structure);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SalaryStructureCreateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.CreateSalaryStructureAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SalaryStructureCreateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.UpdateSalaryStructureAsync(id, dto, ct);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _payrollService.DeleteSalaryStructureAsync(id, ct);
        return NoContent();
    }
}