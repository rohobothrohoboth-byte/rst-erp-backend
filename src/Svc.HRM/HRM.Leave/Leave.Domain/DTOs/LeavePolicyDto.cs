namespace Leave.Domain.DTOs;

public class LeavePolicyListDto : BaseDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } = default!; // enum.PolicyStatus
    public string LeaveType { get; set; } = default!;
    public string StatusStr { get; set; } = default!; // LeaveType
    public string AllowEncashmentStr { get; set; } = default!;
    public string RequiresAttachmentStr { get; set; } = default!;
}

public class LeavePolicyAddDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } = default!; // enum.PolicyStatus
    public Guid LeaveTypeId { get; set; } // LeaveType
}

public class LeavePolicyModDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } = default!; // enum.PolicyStatus
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string RowVersion { get; set; } = default!;
}