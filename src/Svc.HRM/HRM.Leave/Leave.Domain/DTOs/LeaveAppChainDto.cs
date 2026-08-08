namespace Leave.Domain.DTOs;

public class LeaveAppChainListDto : BaseDto
{
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string EffectiveFromStr => $"{EffectiveFrom:MMMM dd, yyyy}";
    public string EffectiveToStr => EffectiveTo.HasValue ? $"{EffectiveTo:MMMM dd, yyyy}" : "";
    public string IsActiveStr { get; set; } = default!;
    public int AddedSteps { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!; // LeavePolicy
}

public class LeaveAppChainAddDto
{
    public Guid? LeavePolicyId { get; set; }
    public Guid? LeaveTypeId { get; set; }  // Add this property
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public List<LeaveAppStepAddDto>? Steps { get; set; }

}

public class LeaveAppChainModDto
{
    public Guid Id { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string RowVersion { get; set; } = default!;
}