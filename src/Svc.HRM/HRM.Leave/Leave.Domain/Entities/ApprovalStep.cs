namespace Leave.Domain.Entities;

public class ApprovalStep : BaseEntity
{
    public int StepOrder { get; set; }
    public Guid ApproverId { get; set; } // external user id for approver (Employee)
    public bool IsApproved { get; set; } = false;
    public DateTime? ActionedAt { get; set; }
    public string? Comments { get; set; } = default!;
    public Guid LeaveRequestId { get; set; }

    //******************************************//

    public LeaveRequest LeaveRequest { get; set; } = null!;
}