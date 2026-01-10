namespace Leave.Domain.Entities;

public class LeaveLedger: BaseEntity
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double Amount { get; set; } = default!; // +/-
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSource (0/1)
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid? LeavePolicyId { get; set; } // LeavePolicy
    public Guid? ReferenceId { get; set; } // LeaveRequestId, AccrualBatchId, etc.

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
}