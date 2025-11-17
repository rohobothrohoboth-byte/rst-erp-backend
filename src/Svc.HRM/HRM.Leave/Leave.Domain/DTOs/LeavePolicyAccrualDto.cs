namespace Leave.Domain.DTOs;

public class LeavePolicyAccrualListDto : BaseDto
{
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public double Entitlement { get; set; }
    public string Frequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public int MinServiceMonths { get; set; }
    public double MaxCarryoverDays { get; set; }
    public int CarryoverExpiryDays { get; set; }
    public string FrequencyStr { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!;
}

public class LeavePolicyAccrualAddDto
{
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public double Entitlement { get; set; }
    public string Frequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public int MinServiceMonths { get; set; }
    public double MaxCarryoverDays { get; set; }
    public int CarryoverExpiryDays { get; set; }
}

public class LeavePolicyAccrualModDto
{
    public Guid Id { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public double Entitlement { get; set; }
    public string Frequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public int MinServiceMonths { get; set; }
    public double MaxCarryoverDays { get; set; }
    public int CarryoverExpiryDays { get; set; }
    public string RowVersion { get; set; } = default!;
}