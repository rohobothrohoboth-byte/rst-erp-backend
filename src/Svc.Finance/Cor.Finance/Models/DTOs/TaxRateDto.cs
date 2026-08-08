namespace Cor.Finance.Models.DTOs;

public class TaxRateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public decimal Rate { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddTaxRateDto
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public decimal Rate { get; set; }
}

public class EditTaxRateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public decimal Rate { get; set; }
    public string RowVersion { get; set; } = default!;
}
