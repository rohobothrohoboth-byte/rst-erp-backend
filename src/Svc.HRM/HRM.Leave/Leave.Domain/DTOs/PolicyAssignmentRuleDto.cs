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
    public string LeavePolicy { get; set; } = default!; // LeavePolicy
    public string LeaveType { get; set; } = default!; // LeaveType
}

public class PolicyAssignmentRuleAddDto
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Priority { get; set; } // lower = higher priority
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public Guid LeaveTypeId { get; set; } // LeaveType
}

public class PolicyAssignmentRuleModDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Priority { get; set; } // lower = higher priority
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid LeavePolicyId { get; set; } // LeavePolicy
    public Guid LeaveTypeId { get; set; } // LeaveType
    public string RowVersion { get; set; } = default!;
}
