namespace Leave.Domain.Entities;

public class LeaveAppStep : BaseEntity
{
    public string StepName { get; set; } = default!;
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public string Role { get; set; } = default!;  // enum.ApprovalRole
    public Guid? EmployeeId { get; set; }   // HRM.Profile.Employee
    public bool IsFinal { get; set; } = false;
    public Guid LeaveAppChainId { get; set; } // LeaveAppChain
    public int? TimeoutHours { get; set; }
    public Guid LeaveAppStepId { get; set; } // LeaveAppStep
     public Guid LeavePolicyId { get; set; } // LeavePolicy
    //******************************************//
    public LeaveAppChain LeaveAppChain { get; set; } = null!;
}