namespace Leave.Domain.DTOs;

public class LeavePolicyListDto : BaseDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string Name { get; set; } = default!;
    public bool RequiresAttachment { get; set; } = true;
    public double MinDurPerReq { get; set; } = default!;
    public double MaxDurPerReq { get; set; } = default!;
    public bool HolidaysAsLeave { get; set; } = false;
    public string LeaveType { get; set; } = default!;
    public string RequiresAttachmentStr { get; set; } = default!;
    public string MinDurPerReqStr { get; set; } = default!;
    public string MaxDurPerReqStr { get; set; } = default!;
    public string HolidaysAsLeaveStr { get; set; } = default!;
}

public class LeavePolicyAddDto
{
    public string Name { get; set; } = default!;
    public bool RequiresAttachment { get; set; } = true;
    public double MinDurPerReq { get; set; } = default!;
    public double MaxDurPerReq { get; set; } = default!;
    public bool HolidaysAsLeave { get; set; } = false;
    public Guid LeaveTypeId { get; set; } // LeaveType
}

public class LeavePolicyModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool RequiresAttachment { get; set; } = true;
    public double MinDurPerReq { get; set; } = default!;
    public double MaxDurPerReq { get; set; } = default!;
    public bool HolidaysAsLeave { get; set; } = false;
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string RowVersion { get; set; } = default!;
}