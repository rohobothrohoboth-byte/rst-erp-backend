namespace Leave.Domain.DTOs;

public class PolicyAssignmentRuleListDto : BaseDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Priority { get; set; } = default!; // enum.Priority
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public string EffectiveFromStr => $"{EffectiveFrom:MMMM dd, yyyy}";
    public string EffectiveToStr => EffectiveTo.HasValue ? $"{EffectiveTo:MMMM dd, yyyy}" : "";
    public string PriorityStr { get; set; } = default!;
    public string IsActiveStr { get; set; } = default!;
}

public class PolicyAssignmentRuleAddDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Priority { get; set; } = default!; // enum.Priority
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
}

public class PolicyAssignmentRuleModDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Priority { get; set; } = default!; // enum.Priority
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string RowVersion { get; set; } = default!;
}
