// Models/DTOs/FinancialPeriodDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using Cor.Finance.Models.Enums;
using Cor.Finance.Models.Entities;

namespace Cor.Finance.Models.DTOs;

// ==================== REQUEST DTOs ====================

public class AddFinancialPeriodDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string PeriodType { get; set; } = "MONTHLY"; // String for API input

    public void Validate()
    {
        if (StartDate > EndDate)
            throw new ValidationException("Start date must be before end date");

        if (!Enum.TryParse<PeriodType>(PeriodType, true, out _))
            throw new ValidationException($"Invalid PeriodType: {PeriodType}");
    }
}

public class EditFinancialPeriodDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public string PeriodType { get; set; } = "MONTHLY";

    public string? RowVersion { get; set; }

    public void Validate()
    {
        if (StartDate > EndDate)
            throw new ValidationException("Start date must be before end date");

        if (!Enum.TryParse<PeriodType>(PeriodType, true, out _))
            throw new ValidationException($"Invalid PeriodType: {PeriodType}");
    }
}

public class ClosePeriodDto
{
    public bool ForceClose { get; set; } = false;
    public string? Notes { get; set; }
    public string? Reason { get; set; }
}

public class PeriodFilterDto
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public bool? IsClosed { get; set; }
    public string? PeriodType { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "DESC";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdatePeriodDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public PeriodType? PeriodType { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class CreatePeriodDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public PeriodType PeriodType { get; set; } = PeriodType.MONTHLY;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public void Validate()
    {
        if (StartDate > EndDate)
            throw new ValidationException("Start date must be before end date");
    }
}

// ==================== RESPONSE DTOs ====================

// Cor.Finance/Models/DTOs/FinancialPeriodDto.cs

public class FinancialPeriodDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PeriodType { get; set; }
    public bool IsClosed { get; set; }
    public string? Status { get; set; }  // ? String, not PeriodStatus

    public DateTime? ClosedDate { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string? Notes { get; set; }
    public int TotalEntries { get; set; }
    public int PostedEntries { get; set; }
    public int UnpostedEntries { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public int DaysRemaining { get; set; }
    public double CompletionPercentage { get; set; }
    public bool CanBeClosed { get; set; }
    public string? ClosingReason { get; set; }

      public Guid? ClosedBy { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public string? UpdatedByUserName { get; set; }
}

public class PeriodResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PeriodType PeriodType { get; set; }
    public PeriodStatus Status { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedDate { get; set; }
    public Guid? ClosedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Notes { get; set; }
    public int TotalEntries { get; set; }
    public int PostedEntries { get; set; }
    public int UnpostedEntries { get; set; }

    public static PeriodResponseDto FromEntity(FinancialPeriod period)
    {
        return new PeriodResponseDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PeriodType = period.PeriodType,
            Status = period.Status,
            IsClosed = period.IsClosed,
            ClosedDate = period.ClosedDate,
            ClosedBy = period.ClosedBy,
            CreatedAt = period.DateAdd,
            UpdatedAt = period.DateMod,
            Notes = period.Notes,
            TotalEntries = period.TotalEntries,
            PostedEntries = period.PostedEntries,
            UnpostedEntries = period.UnpostedEntries
        };
    }
}

public class PeriodStatsDto
{
    public int TotalJournalEntries { get; set; }
    public int PostedEntries { get; set; }
    public int UnpostedEntries { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int DaysRemaining { get; set; }
    public double CompletionPercentage { get; set; }
    public bool CanBeClosed { get; set; }
    public string? ClosingReason { get; set; }
  public decimal NetBalance { get; set; }
     public int TotalAccountsUsed { get; set; }
            public int TotalUniqueAccounts { get; set; }
            public DateTime? LastEntryDate { get; set; }
            public DateTime? FirstEntryDate { get; set; }
}

// ==================== AUDIT DTOs ====================




 public class ActivePeriodWithStatsDto
    {
        /// <summary>
        /// The active financial period details
        /// </summary>
        public FinancialPeriodDto Period { get; set; } = new();

        /// <summary>
        /// Statistics for the active period
        /// </summary>
        public PeriodStatsDto Stats { get; set; } = new();

        /// <summary>
        /// Indicates if there are unposted entries in this period
        /// </summary>
        public bool HasUnpostedEntries { get; set; }

        /// <summary>
        /// Number of days remaining in the period
        /// </summary>
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Completion percentage based on posted entries
        /// </summary>
        public double CompletionPercentage { get; set; }

        /// <summary>
        /// Indicates if the period can be closed
        /// </summary>
        public bool CanBeClosed { get; set; }

        /// <summary>
        /// Reason why the period cannot be closed (if applicable)
        /// </summary>
        public string? ClosingReason { get; set; }

        /// <summary>
        /// Total number of days in the period
        /// </summary>
        public int TotalDays { get; set; }

        /// <summary>
        /// Number of days elapsed in the period
        /// </summary>
        public int DaysElapsed { get; set; }

        /// <summary>
        /// Progress percentage based on days elapsed
        /// </summary>
        public double TimeProgressPercentage { get; set; }
    }




public class YearEndCloseResultDto
{
    public int Year { get; set; }
    public int TotalPeriods { get; set; }
    public int ClosedPeriods { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool Success { get; set; }
}

public class PeriodTransferResultDto
{
    public Guid FromPeriodId { get; set; }
    public Guid ToPeriodId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransferDate { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}















public class YearPeriodSummaryDto
{
    public int Year { get; set; }
    public List<FinancialPeriodDto> Periods { get; set; } = new();
    public int TotalPeriods { get; set; }
    public int ClosedPeriods { get; set; }
    public int OpenPeriods { get; set; }
    public DateTime? FirstPeriodStart { get; set; }
    public DateTime? LastPeriodEnd { get; set; }

    // ? Add financial summary
    public YearFinancialSummaryDto? FinancialSummary { get; set; }
}

public class YearFinancialSummaryDto
{
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal NetBalance { get; set; }
    public int TotalTransactions { get; set; }
}

    public class PeriodComparisonDto
    {
        public PeriodComparisonItem Period1 { get; set; } = new();
        public PeriodComparisonItem Period2 { get; set; } = new();
        public PeriodComparisonResult Comparison { get; set; } = new();
    }

    public class PeriodComparisonItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal NetBalance { get; set; }
        public int JournalEntryCount { get; set; }
        public int VoucherCount { get; set; }
    }

    public class PeriodComparisonResult
    {
        public decimal BalanceDifference { get; set; }
        public decimal PercentageChange { get; set; }
        public string? Status { get; set; }
    }

    public class YearEndCloseDto
    {
        public int Year { get; set; }
        public string? NewYearName { get; set; }
        public bool AutoCreateNextYearPeriods { get; set; } = true;
        public bool CloseAllYearPeriods { get; set; } = true;
        public bool TransferRetainedEarnings { get; set; } = true;
        public string? Notes { get; set; }
    }

    public class AutoCloseConfigDto
    {
        public bool Enabled { get; set; }
        public int DaysAfterPeriodEnd { get; set; } = 15;
        public bool AutoCreateNextPeriod { get; set; } = true;
        public List<string>? ExcludedModules { get; set; }
        public string? NotificationEmails { get; set; }
    }

    public class BulkPeriodCloseDto
    {
        public List<Guid> PeriodIds { get; set; } = new();
        public bool ForceClose { get; set; } = false;
        public string? Notes { get; set; }
    }

    public class BulkPeriodOpenDto
    {
        public List<Guid> PeriodIds { get; set; } = new();
        public string? Notes { get; set; }
    }

    public class PeriodTransferDto
    {
        public Guid FromPeriodId { get; set; }
        public Guid ToPeriodId { get; set; }
        public decimal Amount { get; set; }
        public string? AccountCode { get; set; }
        public string? Description { get; set; }
        public bool TransferAllBalances { get; set; } = false;
    }

    public class BulkCloseResultDto
    {
        public int ClosedCount { get; set; }
        public int FailedCount { get; set; }
        public List<BulkCloseErrorDto> Errors { get; set; } = new();
    }

    public class BulkCloseErrorDto
    {
        public Guid PeriodId { get; set; }
        public string? PeriodName { get; set; }
        public string? Error { get; set; }
    }

    public class BulkOpenResultDto
    {
        public int OpenedCount { get; set; }
        public int FailedCount { get; set; }
        public List<BulkOpenErrorDto> Errors { get; set; } = new();
    }

    public class BulkOpenErrorDto
    {
        public Guid PeriodId { get; set; }
        public string? PeriodName { get; set; }
        public string? Error { get; set; }
    }

