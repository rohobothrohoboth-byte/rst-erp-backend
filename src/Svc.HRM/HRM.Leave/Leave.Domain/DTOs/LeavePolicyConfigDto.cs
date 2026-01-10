using EthiopianCalendar;

namespace Leave.Domain.DTOs;

public class LeavePolicyConfigListDto : BaseDto
{
    public double AnnualEntitlement { get; set; }
    public string AccrualFrequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public double MaxDaysPerReq { get; set; }
    public double MaxCarryOverDays { get; set; }
    public int MinServiceMonths { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }

    public string AnnualEntitlementStr { get; set; } = default!;
    public string AccrualFrequencyStr { get; set; } = default!;
    public string AccrualRateStr { get; set; } = default!;
    public string MaxDaysPerReqStr { get; set; } = default!;
    public string MaxCarryOverDaysStr { get; set; } = default!;
    public string MinServiceMonthsStr { get; set; } = default!;
    public string IsActiveStr { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!;
    public string EffectiveFromStr => $"{EffectiveFrom:MMMM dd, yyyy}";
    public string EffectiveFromStrAm => EffectiveFrom.ToEthiopianDateString("MMMM dd, yyyy");
    public string EffectiveToStr => EffectiveTo.HasValue ? $"{EffectiveTo:MMMM dd, yyyy}" : "";
    public string EffectiveToStrAm => EffectiveTo.HasValue ? EffectiveTo.Value.ToEthiopianDateString("MMMM dd, yyyy") : "";
}

public class LeavePolicyConfigAddDto
{
    public double AnnualEntitlement { get; set; }
    public string AccrualFrequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public double MaxDaysPerReq { get; set; }
    public double MaxCarryOverDays { get; set; }
    public int MinServiceMonths { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
}

public class LeavePolicyConfigModDto
{
    public Guid Id { get; set; }
    public double AnnualEntitlement { get; set; }
    public string AccrualFrequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public double MaxDaysPerReq { get; set; }
    public double MaxCarryOverDays { get; set; }
    public int MinServiceMonths { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public string RowVersion { get; set; } = default!;
}