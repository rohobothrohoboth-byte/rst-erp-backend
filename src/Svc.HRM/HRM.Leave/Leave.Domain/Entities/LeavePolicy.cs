namespace Leave.Domain.Entities;

public class LeavePolicy : BaseEntity
{
    public string Name { get; set; } = default!;
    public bool RequiresAttachment { get; set; } = true;
    public double MinDurPerReq { get; set; } = default!;
    public double MaxDurPerReq { get; set; } = default!;
    public bool HolidaysAsLeave { get; set; } = false;
    public string Gender { get; set; } = default!; // enum.PolicyGender
    public Guid LeaveTypeId { get; set; }

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}