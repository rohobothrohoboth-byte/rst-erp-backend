namespace Leave.Domain.Entities;

public class EmpLeavePolicy : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public Guid LeavePolicyId { get; set; }

    //******************************************//

    public LeavePolicy LeavePolicy { get; set; } = null!;
}