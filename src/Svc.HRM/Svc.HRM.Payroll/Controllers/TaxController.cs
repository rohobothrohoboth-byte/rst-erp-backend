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
[Route("api/v{version:apiVersion}/tax")]
public class TaxController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public TaxController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    [HttpPost("calculate")]
    [PerAuth(PayPerm.TaxCalculate)]
    public async Task<IActionResult> Calculate([FromBody] TaxCalculationRequest request, CancellationToken ct)
    {
        var result = await _payrollService.CalculateTaxAsync(request.GrossIncome, ct);
        return Ok(result);
    }

    [HttpGet("rates")]
    [PerAuth(PayPerm.TaxView)]
    public async Task<IActionResult> GetRates([FromQuery] string? taxYear, CancellationToken ct)
    {
        var rates = await _payrollService.GetTaxRatesAsync(taxYear, ct);
        return Ok(rates);
    }

    [HttpPost("rates")]
    [PerAuth(PayPerm.TaxManage)]
    public async Task<IActionResult> CreateRate([FromBody] TaxRateCreateDto dto, CancellationToken ct)
    {
        var result = await _payrollService.CreateTaxRateAsync(dto, ct);
        return CreatedAtAction(nameof(GetRates), new { taxYear = result.TaxYear }, result);
    }
}

public class TaxCalculationRequest
{
    public decimal GrossIncome { get; set; }
}