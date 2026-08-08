// Cor.CRM/Models/DTOs/CommissionDtos.cs

using System;

namespace Cor.CRM.Models.DTOs;

public class CommissionDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string? TransactionNumber { get; set; }
    public Guid AgentId { get; set; }
    public string? AgentName { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusValue { get; set; } // ✅ Added for numeric status
    public DateTime? PaymentDate { get; set; }
    public string? Notes { get; set; }
    public bool IsBuyerAgent { get; set; }
    public bool IsSellerAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; } // ✅ Added
    public string? CreatedByUserName { get; set; } // ✅ Added
}

public class CreateCommissionDto
{
    public Guid TransactionId { get; set; }
    public Guid AgentId { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public int? Status { get; set; }
    public string? Notes { get; set; }
    public bool IsBuyerAgent { get; set; }
    public bool IsSellerAgent { get; set; }
    public Guid? CreatedByUserId { get; set; } // ✅ Added
    public string? CreatedByUserName { get; set; } // ✅ Added
}

public class UpdateCommissionDto
{
    public decimal? Amount { get; set; }
    public decimal? Percentage { get; set; }
    public int? Status { get; set; } // ✅ Added missing Status
    public string? Notes { get; set; }
    public DateTime? PaymentDate { get; set; } // ✅ Added missing PaymentDate
    public bool? IsBuyerAgent { get; set; }
    public bool? IsSellerAgent { get; set; }
}

public class CommissionStatsDto
{
    public int TotalCommissions { get; set; }
    public decimal TotalEarned { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal AverageCommission { get; set; }
    public int EarnedCount { get; set; }
    public int PendingCount { get; set; }
    public int PaidCount { get; set; }
}

public class CommissionFilterDto
{
    public Guid? AgentId { get; set; }
    public Guid? TransactionId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

