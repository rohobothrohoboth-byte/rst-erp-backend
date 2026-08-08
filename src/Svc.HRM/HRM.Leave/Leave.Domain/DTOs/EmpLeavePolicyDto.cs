// Leave.Domain/DTOs/EmpLeavePolicyDto.cs
using System;

namespace Leave.Domain.DTOs;

public class EmpLeavePolicyListDto : BaseDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public Guid LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public Guid? LeavePolicyId { get; set; }
    public string LeavePolicyName { get; set; } = string.Empty;
    public decimal AssignedEntitlement { get; set; }
    public decimal UsedEntitlement { get; set; }
    public decimal RemainingEntitlement => AssignedEntitlement - UsedEntitlement;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string AssignmentReason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Active, Expired, Pending
}
public class EmpLeavePolicyAddDto
{
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePolicyId { get; set; }
    public decimal AssignedEntitlement { get; set; }

    private DateTime _effectiveFrom;
    public DateTime EffectiveFrom
    {
        get => _effectiveFrom;
        set => _effectiveFrom = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private DateTime? _effectiveTo;
    public DateTime? EffectiveTo
    {
        get => _effectiveTo;
        set => _effectiveTo = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    public string AssignmentReason { get; set; } = "Onboarding";
}

public class EmpLeavePolicyModDto
{
    public Guid Id { get; set; }
    public decimal AssignedEntitlement { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
    public string RowVersion { get; set; } = string.Empty;
}

public class BatchAssignDto
{
    public List<Guid> EmployeeIds { get; set; } = new();
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePolicyId { get; set; }
    public decimal AssignedEntitlement { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string AssignmentReason { get; set; } = "BulkAssignment";
}

// Add this missing class
public class DepartmentAssignDto
{
    public Guid DepartmentId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePolicyId { get; set; }
    public decimal AssignedEntitlement { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string AssignmentReason { get; set; } = "DepartmentAssignment";
}

public class EmpLeavePolicyHistoryDto
{
    public Guid Id { get; set; }
    public Guid OriginalId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeaveTypeId { get; set; }
    public Guid? LeavePolicyId { get; set; }
    public decimal AssignedEntitlement { get; set; }
    public decimal UsedEntitlement { get; set; }
    public decimal CarryForward { get; set; }


    public bool IsActive { get; set; }
    public string? AssignmentReason { get; set; }
    public string? Reason { get; set; }


    public bool IsDeleted { get; set; }

    public string ArchiveReason { get; set; } = string.Empty;

    public int? ProcessedYear { get; set; }

       public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateAdd { get; set; }
        public DateTime? DateMod { get; set; }
        public DateTime ArchivedDate { get; set; }
        public Guid? ProcessedBy { get; set; }
}