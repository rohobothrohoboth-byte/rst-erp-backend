namespace Cor.Finance.Models.DTOs;

public class CurrencyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!; // USD, EUR, ETB
    public string Name { get; set; } = default!;
    public string Symbol { get; set; } = default!;
    public decimal ExchangeRate { get; set; }
    public DateTime? RateDate { get; set; }
    public bool IsBaseCurrency { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddCurrencyDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Symbol { get; set; } = default!;
    public decimal ExchangeRate { get; set; }
    public bool IsBaseCurrency { get; set; }
}

public class ExchangeRateDto
{
    public Guid FromCurrencyId { get; set; }
    public Guid ToCurrencyId { get; set; }
    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; }
}