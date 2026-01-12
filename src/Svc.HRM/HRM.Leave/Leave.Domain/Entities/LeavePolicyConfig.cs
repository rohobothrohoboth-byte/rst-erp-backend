namespace Leave.Domain.Entities;

public class LeavePolicyConfig : BaseEntity
{
    public double AnnualEntitlement { get; set; }
    public string AccrualFrequency { get; set; } = default!; // enum.AccrualFrequency (0/1)
    public double AccrualRate { get; set; }
    public double MaxDaysPerReq { get; set; }
    public double MaxCarryOverDays { get; set; }
    public int MinServiceMonths { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid FiscalYearId { get; set; } // Cor.Module.FiscalYear
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public Guid LeaveAppChainId { get; set; } // LeaveAppChain

    //******************************************//

    public LeavePolicy LeavePolicy { get; set; } = null!;
    public LeaveAppChain LeaveAppChain { get; set; } = null!;
}