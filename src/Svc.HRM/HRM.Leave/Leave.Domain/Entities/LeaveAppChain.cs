namespace Leave.Domain.Entities;

public class LeaveAppChain : BaseEntity
{
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public bool IsFinal { get; set; }
    public Guid LeaveTypeId { get; set; } // LeaveType

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}