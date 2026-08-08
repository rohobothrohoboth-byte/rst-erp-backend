namespace Cor.Finance.Models.DTOs;

public class TaxDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string TaxCode { get; set; } = default!;
    public decimal Rate { get; set; }
    public string Type { get; set; } = default!; // SalesTax, VAT, Withholding
    public bool IsActive { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? AccountId { get; set; }
    public string? AccountName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddTaxDto
{
    public string Name { get; set; } = default!;
    public string TaxCode { get; set; } = default!;
    public decimal Rate { get; set; }
    public string Type { get; set; } = default!;
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? AccountId { get; set; }
}

public class TaxCalculationDto
{
    public decimal Amount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}