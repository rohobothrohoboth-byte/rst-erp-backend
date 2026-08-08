using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cor.Finance.Models.DTOs;
public class JournalEntryDto
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsPosted { get; set; }
    public bool IsApproved { get; set; }
    public bool IsReversed { get; set; }
    public string? Status { get; set; }
    public DateTime? PostedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ReversedDate { get; set; }
    public string? ReversedBy { get; set; }
    public string? RejectionReason { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
     public string? RowVersion { get; set; }
       public Guid? CreatedByUserId { get; set; }
         public string? CreatedByUserName { get; set; }
         public Guid? UpdatedByUserId { get; set; }
         public string? UpdatedByUserName { get; set; }
    public List<JournalLineDto> Lines { get; set; } = new();
}
public class JournalLineDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCode { get; set; }
    public string Direction { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
     public Guid? PeriodId { get; set; }

}

public class AddJournalEntryDto
{
    public string Reference { get; set; } = default!;
    public DateTime EntryDate { get; set; }
    public string Description { get; set; } = default!;
    public string EntryType { get; set; } = default!;

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public List<AddJournalLineDto> Lines { get; set; } = new();
    public Guid? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public Guid? PeriodId { get; set; }

}

public class AddJournalLineDto
{
    public Guid AccountId { get; set; }
    public string Direction { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;

    public Guid? PeriodId { get; set; }
}

public class EditJournalEntryDto
{
    public Guid Id { get; set; }
    public string Reference { get; set; } = default!;
    public DateTime EntryDate { get; set; }
    public string Description { get; set; } = default!;
    public string EntryType { get; set; } = default!;

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
      public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    public List<EditJournalLineDto> Lines { get; set; } = new();
  public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
public string? RowVersion { get; set; }
           public Guid? PeriodId { get; set; }

}

public class EditJournalLineDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string Direction { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;

    public Guid? PeriodId { get; set; }
}

public class PostJournalEntryDto
{
    public Guid Id { get; set; }
      public Guid? PeriodId { get; set; }
}
// ============================================================
// DTOs
// ============================================================

public class RejectJournalEntryDto
{
    public string Reason { get; set; } = string.Empty;
}

public class ReverseJournalEntryDto
{
    public string? Reason { get; set; }
    public DateTime? ReverseDate { get; set; }
}

public class JournalEntrySummaryDto
{
    public int TotalEntries { get; set; }
    public int PostedEntries { get; set; }
    public int UnpostedEntries { get; set; }
    public int ApprovedEntries { get; set; }
    public int RejectedEntries { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public decimal NetBalance { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }

    // By entry type breakdown
    public Dictionary<string, int> EntriesByType { get; set; } = new();
    public Dictionary<string, decimal> AmountByType { get; set; } = new();

    // By date breakdown
    public Dictionary<string, int> EntriesByDate { get; set; } = new();
    public Dictionary<string, decimal> AmountByDate { get; set; } = new();
}
