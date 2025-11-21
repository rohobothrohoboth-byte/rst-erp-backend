using EthiopianCalendar;

namespace Leave.Domain.DTOs;

public class ApprovalStepListDto : BaseDto
{
    public Guid ApprovedById { get; set; } //HRM.Profile.Employee
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public int StepOrder { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime? Date { get; set; }
    public string Comments { get; set; } = default!;
    public string IsApprovedStr { get; set; } = default!;
    public string DateAt => Date.HasValue ? $"{Date:MMMM dd, yyyy}" : "";
    public string DateAtAm => Date.HasValue ? Date.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
    public string ApprovedBy { get; set; } = default!; // Approver (Employee)
}

public class ApprovalStepAddDto
{
    //public Guid ApprovedById { get; set; } //HRM.Profile.Employee
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public int StepOrder { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime? Date { get; set; }
    public string Comments { get; set; } = default!;
}

public class ApprovalStepModDto
{
    public Guid Id { get; set; }
    public Guid ApprovedById { get; set; } //HRM.Profile.Employee
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public int StepOrder { get; set; }
    public bool IsApproved { get; set; } = false;
    public DateTime? Date { get; set; }
    public string Comments { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}