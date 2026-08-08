// Cor.CRM/Models/Entities/Commission.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Models.Entities;

public enum CommissionStatus
{
    Pending = 1,
    Approved = 2,
    Paid = 3,
    Disputed = 4
}

public class Commission : BaseEntity
{
    public Guid TransactionId { get; set; }
    [ForeignKey(nameof(TransactionId))]
    public virtual RealEstateTransaction? Transaction { get; set; }

    public Guid AgentId { get; set; }
    [ForeignKey(nameof(AgentId))]
    public virtual LocalEmployee? Agent { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal Percentage { get; set; }

    public CommissionStatus Status { get; set; } = CommissionStatus.Pending;

    public DateTime? PaymentDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsBuyerAgent { get; set; }
    public bool IsSellerAgent { get; set; }
}