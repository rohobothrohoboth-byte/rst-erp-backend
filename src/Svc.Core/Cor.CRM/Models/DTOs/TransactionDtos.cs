// Cor.CRM/Models/DTOs/TransactionDtos.cs

using System;
using System.Collections.Generic;

namespace Cor.CRM.Models.DTOs;

public class RealEstateTransactionDto
{
    public Guid Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public Guid PropertyId { get; set; }
    public string? PropertyTitle { get; set; }
    public string? PropertyAddress { get; set; }
    public Guid BuyerId { get; set; }
    public string? BuyerName { get; set; }
    public Guid SellerId { get; set; }
    public string? SellerName { get; set; }
    public Guid? BuyerAgentId { get; set; }
    public string? BuyerAgentName { get; set; }
    public Guid? SellerAgentId { get; set; }
    public string? SellerAgentName { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? CommissionAmount { get; set; }
    public DateTime? OfferDate { get; set; }
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? PossessionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? QuoteId { get; set; }
    public string? QuoteNumber { get; set; }
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTransactionDto
{
    public Guid PropertyId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }
    public Guid? BuyerAgentId { get; set; }
    public Guid? SellerAgentId { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? CommissionAmount { get; set; }
    public int? Status { get; set; }
    public DateTime? OfferDate { get; set; }
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? PossessionDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateTransactionDto
{
    public decimal? SalePrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? CommissionAmount { get; set; }
    public int? Status { get; set; }
    public DateTime? OfferDate { get; set; }
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public DateTime? PossessionDate { get; set; }
    public string? Notes { get; set; }
}