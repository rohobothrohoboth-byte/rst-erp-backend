namespace Leave.Domain.Entities;

public class LeavePolicy : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool AllowEncashment { get; set; } = true;
    public decimal MaxEncashableDays { get; set; } = 5;     // Add this - Max days that can be encashed per year
    public decimal EncashmentRate { get; set; } = 100;      // Add this - Percentage of daily rate (100 = full pay)
    public bool RequiresAttachment { get; set; } = true;
    public string Status { get; set; } =  default!; // enum.PolicyStatus
    public Guid LeaveTypeId { get; set; } // LeaveType

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}