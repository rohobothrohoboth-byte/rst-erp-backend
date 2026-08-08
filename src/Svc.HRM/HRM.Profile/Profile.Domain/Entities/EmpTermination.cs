namespace Profile.Domain.Entities;

public class EmpTermination : BaseEntity
{
    public string Status { get; set; } = default!; // HrChangeStatus
    public string TerminationType { get; set; } = "Voluntary"; // Voluntary | Involuntary | Retirement | EndOfContract | Other
    public DateTime LastWorkingDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    public string Reason { get; set; } = default!;
    public string? Comments { get; set; }
    public string? ExitInterviewNotes { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }

    public bool RequestFinalPay { get; set; } = true;
    public bool RequestLeaveSettlement { get; set; } = true;
    public Guid? SettlementPayrollRunId { get; set; }
    public string? SettlementStatus { get; set; } // Pending | Requested | Failed | Skipped
    public string? SettlementNotes { get; set; }
    public decimal? LeaveUnpaidDaysSnapshot { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public ICollection<EmpOffboardingTask> OffboardingTasks { get; set; } = new List<EmpOffboardingTask>();
}
