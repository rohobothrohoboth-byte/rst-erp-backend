// Models/DTOs/BankTransactionDtos.cs

using System.Text.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.DTOs;

public class BankTransactionDto
{
    public Guid Id { get; set; }
    public Guid BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankAccountNumber { get; set; }

    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public string? Reference { get; set; }
    public decimal? BalanceAfter { get; set; }
    public bool IsReconciled { get; set; }
    public DateTime? ReconciliationDate { get; set; }
    public string Status { get; set; } = default!;
    public string? ReconciledBy { get; set; }
    public string? CreatedByUserName { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CheckNumber { get; set; }

    public string? BankReference { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddBankTransactionDto
{
    [Required]
    public Guid BankAccountId { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    [Required]
    public string TransactionType { get; set; } = default!;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string Description { get; set; } = default!;

    public string? Reference { get; set; }

    public string? PaymentMethod { get; set; }

    public string? CheckNumber { get; set; }

    public string? BankReference { get; set; }

    [Required]
    public Guid PeriodId { get; set; }
}

public class EditBankTransactionDto
{
    public Guid Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public string? Reference { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CheckNumber { get; set; }
    public string? BankReference { get; set; }
    public Guid? PeriodId { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class ReconcileBankTransactionDto
{
    public Guid Id { get; set; }
    public bool IsReconciled { get; set; }
    public DateTime? ReconciliationDate { get; set; }
}

public class BulkReconcileDto
{
    public List<Guid> TransactionIds { get; set; } = new();
    public bool IsReconciled { get; set; }
    public DateTime? ReconciliationDate { get; set; }
}

public class BulkReconcileResultDto
{
    public int ReconciledCount { get; set; }
    public int FailedCount { get; set; }
    public List<BulkReconcileErrorDto> Errors { get; set; } = new();
}

public class BulkReconcileErrorDto
{
    public Guid TransactionId { get; set; }
    public string? Error { get; set; }
}

public class VoidTransactionDto
{
    public string? Reason { get; set; }
}

public class TransactionStatsDto
{
    public int TotalTransactions { get; set; }
    public int ReconciledCount { get; set; }
    public int UnreconciledCount { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal NetChange { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public decimal MinTransactionAmount { get; set; }
    public decimal MaxTransactionAmount { get; set; }
    public DateTime? FirstTransactionDate { get; set; }
    public DateTime? LastTransactionDate { get; set; }
    public Dictionary<string, int> TransactionsByType { get; set; } = new();
    public Dictionary<string, int> TransactionsByStatus { get; set; } = new();
    public Dictionary<string, decimal> AmountByType { get; set; } = new();
}

public class ReconciliationSummaryDto
{
    public Guid? BankAccountId { get; set; }
    public string BankAccountName { get; set; } = string.Empty;
    public Guid? PeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public int ReconciledCount { get; set; }
    public int UnreconciledCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ReconciledAmount { get; set; }
    public decimal UnreconciledAmount { get; set; }
    public decimal ReconciliationProgress { get; set; }
}