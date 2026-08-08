// Models/Entities/CurrencyRate.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Entities;

public class CurrencyRate : BaseEntity
{
    [Required]
    [MaxLength(10)]
    public string FromCurrency { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string ToCurrency { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,6)")]
    public decimal Rate { get; set; }

    [Required]
    public DateTime RateDate { get; set; }

    [MaxLength(50)]
    public string? RateType { get; set; } // Spot, Forward, Average, Closing, Opening

    [MaxLength(500)]
    public string? Source { get; set; } // CentralBank, MarketRate, Custom

    [MaxLength(100)]
    public string? Provider { get; set; } // Name of the exchange rate provider

    [Column(TypeName = "jsonb")]
    public string? Metadata { get; set; } // Additional information

    // ✅ PeriodId is inherited from BaseEntity
}