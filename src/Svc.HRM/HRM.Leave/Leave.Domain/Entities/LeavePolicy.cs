namespace Leave.Domain.Entities;

public class LeavePolicy : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } =  default!; // enum.PolicyStatus
    public Guid LeaveTypeId { get; set; } // LeaveType

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}