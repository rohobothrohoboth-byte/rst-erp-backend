// Models/DTOs/BankAccountDtos.cs
using System.Text.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cor.Finance.Models.DTOs;

public class BankAccountDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = default!;
    public string AccountNumber { get; set; } = default!;
    public string BankName { get; set; } = default!;
    public string AccountType { get; set; } = default!;
    public string? GLCode { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal AvailableBalance { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? AccountId { get; set; }
    public string? AccountCode { get; set; }
    public string? Description { get; set; }
    public string? IBAN { get; set; }
    public string? SwiftCode { get; set; }
    public string? BankAddress { get; set; }
    public DateTime? LastReconciledDate { get; set; }
    public decimal? OverdraftLimit { get; set; }
    public bool IsReconciled { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public DateTime? SyncedAt { get; set; }
}

public class AddBankAccountDto
{
    [Required]
    public string AccountName { get; set; } = default!;

    [Required]
    public string AccountNumber { get; set; } = default!;

    public string BankName { get; set; } = default!;

    public string AccountType { get; set; } = default!;

    public string? GLCode { get; set; }

    public decimal OpeningBalance { get; set; }

    public string Currency { get; set; } = "USD";

    public bool IsDefault { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? AccountId { get; set; }

    public string? Description { get; set; }

    public string? IBAN { get; set; }

    public string? SwiftCode { get; set; }

    public string? BankAddress { get; set; }

    public decimal? OverdraftLimit { get; set; }

    public Guid? PeriodId { get; set; }
}

public class EditBankAccountDto
{
    public Guid Id { get; set; }

    [Required]
    public string AccountName { get; set; } = default!;

    [Required]
    public string AccountNumber { get; set; } = default!;

    public string BankName { get; set; } = default!;

    public string AccountType { get; set; } = default!;

    public string? GLCode { get; set; }

    public string Currency { get; set; } = "USD";

    public bool IsActive { get; set; }

    public bool IsDefault { get; set; }

    public Guid? BranchId { get; set; }

    public Guid? AccountId { get; set; }

    public string? Description { get; set; }

    public string? IBAN { get; set; }

    public string? SwiftCode { get; set; }

    public string? BankAddress { get; set; }

    public decimal? OverdraftLimit { get; set; }

    public Guid? PeriodId { get; set; }

    [JsonIgnore]
    public string RowVersion { get; set; } = default!;
}

public class UpdateBankAccountBalanceDto
{
    public Guid Id { get; set; }
    public decimal NewBalance { get; set; }
    public Guid? PeriodId { get; set; }
    public string? Reason { get; set; }
}

public class BankAccountBalanceDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal NetChange { get; set; }
    public DateTime? AsOfDate { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
      public decimal AvailableBalance { get; set; }
}

public class BankAccountSummaryDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal AverageBalance { get; set; }
    public decimal MinBalance { get; set; }
    public decimal MaxBalance { get; set; }
}

public class BankAccountListDto
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = default!;
    public string AccountNumber { get; set; } = default!;
    public string BankName { get; set; } = default!;
    public string AccountType { get; set; } = default!;
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public string? BranchName { get; set; }
}

