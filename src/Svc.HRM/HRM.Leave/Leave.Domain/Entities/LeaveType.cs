namespace Leave.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!; // enum.LeaveTypeCode (0/1)
    public bool IsPaid { get; set; } = false;
}