namespace Leave.Domain.Enums;

// Leave.Domain/Enums.cs
public enum LeaveRequestStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    ApprovedPartial = 3,
    Rejected = 4,
    Cancelled = 5,
    Withdrawn = 6,
    AutoApproved = 7
}

public enum LedgerSourceType
{
    Accrual = 0,
    Request = 1,
    Adjustment = 2,
    Encashment = 3,
    Reconciliation = 4
}

public enum AccrualFrequency
{
    Monthly = 0,
    Yearly = 1,
    Daily = 2,
    None = 3
}

public enum LedgerEntryType
{
    Accrual = 1,
    Request = 2,
    Approval = 3,
    Deduction = 4
}