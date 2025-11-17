namespace Leave.Domain.Entities;

public class LeaveLedger: BaseEntity
{
    public Guid LeaveBalanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeavePolicyId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double Amount { get; set; } = default!; // positive (accrual) or negative (debit)
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSourceType (0/1)
    public double BalanceAfter { get; set; } = default!;

    //******************************************//

    public LeavePolicy LeavePolicy { get; set; } = null!;
    public LeaveBalance LeaveBalance { get; set; } = null!;
}