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

public class EmployeePolicyContext
{
    public Guid EmployeeId { get; set; }
    public double SerYear { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string EmpNat { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Disable { get; set; } = default!;
}
