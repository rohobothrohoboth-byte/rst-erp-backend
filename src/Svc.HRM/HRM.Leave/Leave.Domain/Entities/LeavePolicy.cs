namespace Leave.Domain.Entities;

public class LeavePolicy : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.PolicyGender
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public Guid LeaveTypeId { get; set; } // LeaveType

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}