namespace Leave.Domain.Entities;

public class LeavePolicy : BaseEntity
{
    public double Entitlement { get; set; } = default!; // e.g., 20.0 days per year
    public string Frequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; } = default!; // days per month or per frequency
    public int? MinServiceMonths { get; set; } = default!;
    public double? MaxCarryoverDays { get; set; } = default!; // cap
    public int? CarryoverExpiryDays { get; set; } = default; // days after FY to expire
    public bool RequiresAttachment { get; set; } = false;
    public double MinDurationPerRequest { get; set; } = default; // supports half-day
    public double MaxDurationPerRequest { get; set; } = default;
    public bool CountPublicHolidaysAsLeave { get; set; } = false;
    public Guid LeaveTypeId { get; set; }

    //******************************************//

    public LeaveType LeaveType { get; set; } = null!;
}