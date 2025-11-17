using EthiopianCalendar;

namespace Leave.Domain.DTOs;

public class LeaveLedgerListDto : BaseDto
{
    public Guid EmployeeId { get; set; } //HRM.Profile.Employee
    public Guid LeavePolicyId { get; set; } //LeavePolicy
    public Guid LeaveBalanceId { get; set; } //LeaveBalance
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double Amount { get; set; } = default!; // positive (accrual) or negative (debit)
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSourceType (0/1)
    public string AmountStr { get; set; } = default!;
    public string EntryTypeStr { get; set; } = default!;
    public string SourceTypeStr { get; set; } = default!;
    public string BalanceAfter { get; set; } = default!;
    public string DateAt => $"{Date:MMMM dd, yyyy}";
    public string DateAtAm => Date.ToEthiopianDateString("MMMM dd, yyyy");
    public string EmployeeName { get; set; } = default!;
    public string LeavePolicy { get; set; } = default!;
    public string LeaveBalance { get; set; } = default!;
}

public class LeaveLedgerAddDto
{
    public Guid LeaveBalanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeavePolicyId { get; set; }
    public double Amount { get; set; } = default!; // positive (accrual) or negative (debit)
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSourceType (0/1)
}

public class LeaveLedgerModDto
{
    public Guid Id { get; set; }
    public Guid LeaveBalanceId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid LeavePolicyId { get; set; }
    public double Amount { get; set; } = default!; // positive (accrual) or negative (debit)
    public string EntryType { get; set; } = default!; // enum.LedgerEntryType (0/1)
    public string SourceType { get; set; } = default!; // enum.LedgerSourceType (0/1)
    public string RowVersion { get; set; } = default!;
}