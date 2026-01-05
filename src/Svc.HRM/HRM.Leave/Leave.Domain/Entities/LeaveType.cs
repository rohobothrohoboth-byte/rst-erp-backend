namespace Leave.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = default!;
    public bool IsPaid { get; set; } = false;
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = false;
}