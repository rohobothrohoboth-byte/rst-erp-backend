// Cor.CRM/Models/Entities/RealEstateTransaction.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Models.Entities;

public enum TransactionStatus
{
    Negotiation = 1,
    Accepted = 2,
    PendingInspection = 3,
    PendingFinancing = 4,
    PendingAppraisal = 5,
    Closing = 6,
    Completed = 7,
    Cancelled = 8
}

public class RealEstateTransaction : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string TransactionNumber { get; set; } = string.Empty;

    // Property
    public Guid PropertyId { get; set; }
    [ForeignKey(nameof(PropertyId))]
    public virtual Property? Property { get; set; }

    // Parties
    public Guid BuyerId { get; set; }
    [ForeignKey(nameof(BuyerId))]
    public virtual Customer? Buyer { get; set; }

    public Guid SellerId { get; set; }
    [ForeignKey(nameof(SellerId))]
    public virtual Customer? Seller { get; set; }

    // Agents
    public Guid? BuyerAgentId { get; set; }
    [ForeignKey(nameof(BuyerAgentId))]
    public virtual LocalEmployee? BuyerAgent { get; set; }

    public Guid? SellerAgentId { get; set; }
    [ForeignKey(nameof(SellerAgentId))]
    public virtual LocalEmployee? SellerAgent { get; set; }

    // Financial
    [Column(TypeName = "decimal(18,2)")]
    public decimal SalePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DepositAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CommissionAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? BuyerCommission { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? SellerCommission { get; set; }

    // Dates
    public DateTime? OfferDate { get; set; }
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? PossessionDate { get; set; }

    // Status
    public TransactionStatus Status { get; set; } = TransactionStatus.Negotiation;

    // Documents
    public string? DocumentsJson { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Reference to Quote/Order
    public Guid? QuoteId { get; set; }
    [ForeignKey(nameof(QuoteId))]
    public virtual Quote? Quote { get; set; }

    public Guid? OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public virtual SalesOrder? Order { get; set; }

    public Guid? ContractId { get; set; }
    [ForeignKey(nameof(ContractId))]
    public virtual Contract? Contract { get; set; }
}