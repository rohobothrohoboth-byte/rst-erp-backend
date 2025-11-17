namespace Leave.Domain.Entities;

public class LeavePolicyAccrual : BaseEntity
{
    public Guid LeavePolicyId { get; set; }
    public double Entitlement { get; set; }
    public string Frequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public int MinServiceMonths { get; set; }
    public double MaxCarryoverDays { get; set; }
    public int CarryoverExpiryDays { get; set; }

    //******************************************//

    public LeavePolicy LeavePolicy { get; set; } = null!;
}