using System.Text.Json.Serialization;

namespace Leave.Domain.DTOs;

public class LeavePolicyConfigListDto : BaseDto
{
    [JsonIgnore]
    public Guid FiscalYearId { get; set; } // Cor.Module.FiscalYear
    public double AnnualEntitlement { get; set; }
    public string AccrualFrequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public double MaxDaysPerReq { get; set; }
    public double MaxCarryOverDays { get; set; }
    public int MinServiceMonths { get; set; }
    public bool IsActive { get; set; } = true;
    public string AnnualEntitlementStr { get; set; } = default!;
    public string AccrualFrequencyStr { get; set; } = default!;
    public string AccrualRateStr { get; set; } = default!;
    public string MaxDaysPerReqStr { get; set; } = default!;
    public string MaxCarryOverDaysStr { get; set; } = default!;
    public string MinServiceMonthsStr { get; set; } = default!;
    public string IsActiveStr { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!; // LeavePolicy
    public string FiscalYear { get; set; } = default!; // Cor.Module.FiscalYear
}

public class LeavePolicyConfigAddDto
{
    public double AnnualEntitlement { get; set; }
    public string AccrualFrequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public double MaxDaysPerReq { get; set; }
    public double MaxCarryOverDays { get; set; }
    public int MinServiceMonths { get; set; }
    public Guid FiscalYearId { get; set; } // Cor.Module.FiscalYear
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
    public bool IsActive { get; set; } = true;
    public Guid FiscalYearId { get; set; } // Cor.Module.FiscalYear
    public string RowVersion { get; set; } = default!;
}