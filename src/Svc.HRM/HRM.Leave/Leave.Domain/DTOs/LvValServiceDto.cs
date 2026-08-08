namespace Leave.Domain.DTOs;





public class AppReqDto
{
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public double DaysRequested { get; set; } = default!;
}



public class LvAppActionDto
{
    public int StepOrder { get; set; } = 1;
    public string Role { get; set; } = default!; // enum.ApprovalRole (0/1)
    public string Action { get; set; } = default!; // enum.Status (0/1)
    public string? Comment { get; set; }
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public Guid ApprovedById { get; set; } // HRM.Profile.Employee
    public Guid LeaveAppStepId { get; set; } // LeaveAppStep
}

public class ProcessAppDto
{
    public Guid Id { get; set; }
    public Guid ApprovedById { get; set; } // HRM.Profile.Employee
    public string Action { get; set; } = default!; // enum.Status (0/1)
    public string? Comment { get; set; }
}









