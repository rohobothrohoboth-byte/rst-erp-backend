using Microsoft.EntityFrameworkCore;
using Svc.HRM.Payroll.Models.DTOs;
using Svc.HRM.Payroll.Persistence;

namespace Svc.HRM.Payroll.Services;

public class TaxCalculator : ITaxCalculator
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<TaxCalculator> _logger;

    public TaxCalculator(PayrollDbContext context, ILogger<TaxCalculator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaxCalculationDto> CalculateTaxAsync(decimal grossIncome, CancellationToken ct = default)
    {
        var taxBrackets = await _context.TaxRates
            .Where(x => x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.MinIncome)
            .ToListAsync(ct);

        var result = new TaxCalculationDto
        {
            GrossIncome = grossIncome,
            TaxBrackets = new List<TaxBracketDto>()
        };

        // Assume pension contribution is 7% of gross income
        result.PensionContribution = grossIncome * 0.07m;
        var taxableIncome = grossIncome - result.PensionContribution;

        decimal totalTax = 0;
        decimal remainingIncome = taxableIncome;

        foreach (var bracket in taxBrackets)
        {
            if (remainingIncome <= 0) break;

            decimal taxableAmount;
            if (bracket.MaxIncome.HasValue)
            {
                var bracketMax = Math.Min(remainingIncome, bracket.MaxIncome.Value - bracket.MinIncome);
                taxableAmount = Math.Max(0, bracketMax);
            }
            else
            {
                taxableAmount = remainingIncome;
            }

            if (taxableAmount > 0)
            {
                var taxAmount = taxableAmount * (bracket.TaxRate / 100);
                totalTax += taxAmount;

                result.TaxBrackets.Add(new TaxBracketDto
                {
                    MinIncome = bracket.MinIncome,
                    MaxIncome = bracket.MaxIncome,
                    Rate = bracket.TaxRate,
                    Amount = taxAmount
                });

                remainingIncome -= taxableAmount;
            }
        }

        result.TaxAmount = totalTax;
        // Remove this line - NetIncome is computed automatically
        // result.NetIncome = grossIncome - result.PensionContribution - totalTax;

        return result;
    }
}