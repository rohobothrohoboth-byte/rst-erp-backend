// Models/Entities/Aggregates/PaymentAggregate.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Models.Entities.Aggregates;

[Table("PaymentAggregates")]
[Index(nameof(AggregateDate))]
[Index(nameof(Period))]
[Index(nameof(AggregateType))]
public class PaymentAggregate
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime AggregateDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Period { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string AggregateType { get; set; } = string.Empty;

    public int TotalPayments { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal VendorPayments { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CustomerPayments { get; set; }

    public int ProcessedCount { get; set; }
    public int PendingCount { get; set; }

    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
}