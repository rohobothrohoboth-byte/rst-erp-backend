namespace Leave.Domain.Entities;

public class LeaveLedger: BaseEntity
{
    public Guid EmployeeId { get; set; }
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double Amount { get; set; } = default!; // positive (accrual) or negative (debit)
    public string SourceType { get; set; } = default!; // enum.LedgerSourceType (0/1)
    public double? BalanceAfter { get; set; } = default!;
    public Guid LeaveTypeId { get; set; }
    //public Guid? ReferenceId { get; set; } // e.g., LeaveRequest.Id

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}