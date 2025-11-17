namespace Leave.Domain.DTOs;

public class LeaveTypeListDto : BaseDto
{

    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!; // enum.LeaveTypeCode (0/1)
    public bool IsPaid { get; set; } = false;
    public string CodeStr { get; set; } = default!;
    public string IsPaidStr { get; set; } = default!;
}

public class LeaveTypeAddDto
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!; // enum.LeaveTypeCode (0/1)
    public bool IsPaid { get; set; } = false;
}

public class LeaveTypeModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!; // enum.LeaveTypeCode (0/1)
    public bool IsPaid { get; set; } = false;
    public string RowVersion { get; set; } = default!;
}