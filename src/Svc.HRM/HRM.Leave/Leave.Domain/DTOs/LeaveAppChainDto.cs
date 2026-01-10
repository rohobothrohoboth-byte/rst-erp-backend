namespace Leave.Domain.DTOs;

public class LeaveAppChainDto : BaseDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public bool IsFinal { get; set; }

    public string RoleStr { get; set; } = default!;
    public string IIsFinalStr { get; set; } = default!;
}

public class LeaveAppChainAddDto
{
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public bool IsFinal { get; set; }
    public Guid LeaveTypeId { get; set; } // LeaveType
}

public class LeaveAppChainModDto
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public bool IsFinal { get; set; }
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string RowVersion { get; set; } = default!;
}