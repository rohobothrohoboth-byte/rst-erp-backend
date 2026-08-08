namespace Svc.HRM.Payroll.Models.DTOs;

public class TaxCalculationDto
{
    public decimal GrossIncome { get; set; }
    public decimal PensionContribution { get; set; }
    public decimal TaxableIncome => GrossIncome - PensionContribution;
    public decimal TaxAmount { get; set; }
    public decimal NetIncome => GrossIncome - PensionContribution - TaxAmount; // Computed property
    public List<TaxBracketDto> TaxBrackets { get; set; } = new();
}

public class TaxBracketDto
{
    public decimal MinIncome { get; set; }
    public decimal? MaxIncome { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}

public class TaxRateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal MinIncome { get; set; }
    public decimal? MaxIncome { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DeductibleAmount { get; set; }
    public string TaxYear { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class TaxRateCreateDto
{
    public string Name { get; set; } = default!;
    public decimal MinIncome { get; set; }
    public decimal? MaxIncome { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DeductibleAmount { get; set; }
    public string TaxYear { get; set; } = default!;
}