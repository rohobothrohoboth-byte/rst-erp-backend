namespace Leave.Domain.DTOs;

public class LeavePolicyListDto : BaseDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.PolicyGender
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public string LeaveType { get; set; } = default!;
    public string GenderStr { get; set; } = default!;
    public string AllowEncashmentStr { get; set; } = default!;
    public string RequiresAttachmentStr { get; set; } = default!;
}

public class LeavePolicyAddDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.PolicyGender
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public Guid LeaveTypeId { get; set; } // LeaveType
}

public class LeavePolicyModDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.PolicyGender
    public bool AllowEncashment { get; set; } = true;
    public bool RequiresAttachment { get; set; } = true;
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string RowVersion { get; set; } = default!;
}