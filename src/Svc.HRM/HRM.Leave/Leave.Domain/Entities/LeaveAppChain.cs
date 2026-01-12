namespace Leave.Domain.Entities;

public class LeaveAppChain : BaseEntity
{
    public Guid LeavePolicyId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;

    //******************************************//
    public LeavePolicy LeavePolicy { get; set; } = null!;
    public ICollection<LeaveAppStep> Steps { get; set; } = [];
}