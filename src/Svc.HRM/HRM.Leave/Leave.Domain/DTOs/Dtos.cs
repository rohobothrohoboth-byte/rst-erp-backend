namespace Leave.Domain.DTOs;

public class StatChangeDto
{
    public Guid Id { get; set; }
    public bool Stat { get; set; } = true;
}

public class NameList
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class LedgerEntryDto
{
    public double Amount { get; set; } = default!; // +/-
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSource (0/1)
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid? LeavePolicyId { get; set; } // LeavePolicy
    public Guid? ReferenceId { get; set; } // LeaveRequestId, AccrualBatchId, etc.
}

public class EmpPolicyCtx
{
    public Guid EmployeeId { get; set; }
    public string Name { get; set; } = default!;
    public double SerYear { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string WorkAr { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Jg { get; set; } = default!;
}

public class PolicyCondCtx
{
    public Guid PolicyId { get; set; }
    public Guid PolAssignRuleId { get; set; }
    public Guid PolRuleCondId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public string Priority { get; set; } = default!;
    public string Field { get; set; } = default!; // enum.ConditionField
    public string Operator { get; set; } = default!; // enum.ConditionOperator
    public string Value { get; set; } = default!;
}

public class ResolvePolicy
{
    public DateTime EffectiveFrom { get; set; }
    public double AssignedEntitlement { get; set; }
    //public string Reason { get; set; } = default!; // enum.EmpLeavePolReason(0/1)
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid LeaveTypeId { get; set; } // LeaveType
    public Guid LeavePolicyId { get; set; } // LeavePolicy
}

//public class TempCtx
//{
//    public List<ResolvePolicy> ResolvePolicy { get; set; } = default!;
//    public List<EmpPolicyCtx> EmpPolicyCtx { get; set; } = default!;
//    public List<PolicyCondCtx> PolicyCondCtx { get; set; } = default!;
//}

