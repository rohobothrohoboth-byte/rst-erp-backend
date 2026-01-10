namespace Leave.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!; // enum.LeaveCategory
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = false;
    public bool HolidaysAsLeave { get; set; } = false;
    public bool IsActive { get; set; } = true;
}