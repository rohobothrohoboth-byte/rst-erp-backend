using Svc.HRM.Payroll.Models.DTOs;

namespace Svc.HRM.Payroll.Services;

public interface ITaxCalculator
{
    Task<TaxCalculationDto> CalculateTaxAsync(decimal grossIncome, CancellationToken ct = default);
}