namespace Leave.Domain.Entities;

public class EncashmentAppAction : BaseEntity
{
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public string Action { get; set; } = default!; // enum.Status (0/1)
    public string? Comment { get; set; }
    public DateTime ActionAt { get; set; }
    public Guid LeaveEncashmentId { get; set; } // LeaveEncashment
    public Guid ApprovedById { get; set; } // HRM.Profile.Employee

    //******************************************//

    public LeaveEncashment LeaveEncashment { get; set; } = null!;

}