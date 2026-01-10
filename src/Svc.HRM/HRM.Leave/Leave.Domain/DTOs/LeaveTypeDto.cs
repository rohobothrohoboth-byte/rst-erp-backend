namespace Leave.Domain.DTOs;

public class LeaveTypeListDto : BaseDto
{

    public string Name { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!; // enum.LeaveCategory
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool HolidaysAsLeave { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string LeaveCategoryStr { get; set; } = default!;
    public string RequiresApprovalStr { get; set; } = default!;
    public string AllowHalfDayStr { get; set; } = default!;
    public string IsActiveStr { get; set; } = default!;
    public string HolidaysAsLeaveStr { get; set; } = default!;
}

public class LeaveTypeAddDto
{
    public string Name { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!; // enum.LeaveCategory
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool HolidaysAsLeave { get; set; } = false;
}

public class LeaveTypeModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string LeaveCategory { get; set; } = default!; // enum.LeaveCategory
    public bool RequiresApproval { get; set; } = true;
    public bool AllowHalfDay { get; set; } = true;
    public bool HolidaysAsLeave { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}