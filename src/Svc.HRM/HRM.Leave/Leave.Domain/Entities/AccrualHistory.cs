namespace Leave.Domain.Entities;

public class AccrualHistory : BaseEntity
{
    public string Frequency { get; set; } = default!; // enum.AccrualFrequency(0/1)
    public decimal AccruedAmount { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public string Source { get; set; } = default!; // enum.AccuralSource(0/1)
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public Guid LeaveLedgerId { get; set; } // LeaveLedger

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
    public LeaveLedger LeaveLedger { get; set; } = null!;
}