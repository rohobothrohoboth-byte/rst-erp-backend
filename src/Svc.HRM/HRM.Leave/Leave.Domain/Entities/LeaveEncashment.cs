namespace Leave.Domain.Entities;

public class LeaveEncashment : BaseEntity
{
    public double DaysEncashed { get; set; }
       public double RatePerDay { get; set; }
       public double TotalAmount { get; set; }
       public string Status { get; set; } = default!;
       public int CurrentAppStep { get; set; } = 0;
       public Guid EmployeeId { get; set; }
       public Guid LeaveTypeId { get; set; }
       public Guid? LeavePolicyId { get; set; }
       public Guid? LeaveAppChainId { get; set; }  // Add this - links to approval chain
       public string? Reason { get; set; }  // Add this for request reason
    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
    public LeavePolicy LeavePolicy { get; set; } = null!;
}