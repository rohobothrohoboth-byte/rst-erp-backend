namespace Leave.Domain.Entities;

public class LeaveAppAction : BaseEntity
{
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public string Action { get; set; } = default!; // enum.Status (0/1)
    public string? Comment { get; set; }
    public DateTime ActionAt { get; set; }
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public Guid ApprovedById { get; set; } // HRM.Profile.Employee
    public Guid LeaveAppStepId { get; set; } // LeaveAppStep
    public DateTime DateApp { get; set; } = DateTime.UtcNow;
    //******************************************//

    public LeaveRequest LeaveRequest { get; set; } = null!;
}