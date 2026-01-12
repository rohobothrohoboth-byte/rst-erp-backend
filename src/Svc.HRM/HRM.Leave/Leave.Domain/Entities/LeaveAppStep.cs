namespace Leave.Domain.Entities;

public class LeaveAppStep : BaseEntity
{
    public int StepOrder { get; set; }    // 1, 2, 3 ...
    public string Role { get; set; } = default!;  // enum.ApprovalRole
    public string StepName { get; set; } = default!;
    public Guid? EmployeeId { get; set; }   // HRM.Profile.Employee
    public bool IsFinal { get; set; } = false;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid LeaveAppChainId { get; set; } // LeaveAppChain

    //******************************************//
    public LeaveAppChain LeaveAppChain { get; set; } = null!;
}