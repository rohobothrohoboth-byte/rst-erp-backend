namespace Leave.Domain.DTOs;

public class LeaveAppStepListDto : BaseDto
{
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public string Role { get; set; } = default!;  // enum.ApprovalRole
    public bool IsFinal { get; set; } = false;

    public string RoleStr { get; set; } = default!;
    public string IsFinalStr { get; set; } = default!;
    public string Employee { get; set; } = default; // HRM.Profile.Employee
    public string LeaveAppChain { get; set; } = default!; // LeaveAppChain
}

public class LeaveAppStepAddDto
{
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public string Role { get; set; } = default!;  // enum.ApprovalRole
    public Guid? EmployeeId { get; set; }   // HRM.Profile.Employee
    public bool IsFinal { get; set; } = false;
    public Guid LeavePolicyId { get; set; } // LeavePolicy
}

public class LeaveAppStepModDto
{
    public Guid Id { get; set; }
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public string Role { get; set; } = default!;  // enum.ApprovalRole
    public Guid? EmployeeId { get; set; }   // HRM.Profile.Employee
    public bool IsFinal { get; set; } = false;
    public string RowVersion { get; set; } = default!;
}