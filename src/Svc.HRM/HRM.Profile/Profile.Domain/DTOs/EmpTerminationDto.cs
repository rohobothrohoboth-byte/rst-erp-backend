using System.ComponentModel.DataAnnotations;

namespace Profile.Domain.DTOs;

public class EmpTerminationListDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public string TerminationType { get; set; } = default!;
    public DateTime LastWorkingDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    public string Reason { get; set; } = default!;
    public string? Comments { get; set; }
    public string? ExitInterviewNotes { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? AppliedDate { get; set; }
    public bool RequestFinalPay { get; set; }
    public bool RequestLeaveSettlement { get; set; }
    public Guid? SettlementPayrollRunId { get; set; }
    public string? SettlementStatus { get; set; }
    public string? SettlementNotes { get; set; }
    public decimal? LeaveUnpaidDaysSnapshot { get; set; }
    public Guid EmployeeId { get; set; }
    public int OffboardingTotal { get; set; }
    public int OffboardingCompleted { get; set; }
    public string RowVersion { get; set; } = default!;
    public List<EmpOffboardingTaskDto>? Tasks { get; set; }
}

public class EmpTerminationAddDto
{
    [Required] public Guid EmployeeId { get; set; }
    [Required] public DateTime LastWorkingDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    [Required] public string Reason { get; set; } = default!;
    public string TerminationType { get; set; } = "Voluntary";
    public string? Comments { get; set; }
    public string? ExitInterviewNotes { get; set; }
    public bool RequestFinalPay { get; set; } = true;
    public bool RequestLeaveSettlement { get; set; } = true;
    public bool SeedDefaultChecklist { get; set; } = true;
}

public class EmpTerminationDecisionDto
{
    [Required] public Guid Id { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? Comments { get; set; }
    public string? ExitInterviewNotes { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}

public class EmpTerminationModDto
{
    [Required] public Guid Id { get; set; }
    [Required] public DateTime LastWorkingDate { get; set; }
    public DateTime? NoticeDate { get; set; }
    [Required] public string Reason { get; set; } = default!;
    public string TerminationType { get; set; } = "Voluntary";
    public string? Comments { get; set; }
    public string? ExitInterviewNotes { get; set; }
    public bool RequestFinalPay { get; set; } = true;
    public bool RequestLeaveSettlement { get; set; } = true;
    [Required] public string RowVersion { get; set; } = default!;
}

public class EmpOffboardingTaskDto
{
    public Guid Id { get; set; }
    public Guid TerminationId { get; set; }
    public string Category { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Status { get; set; } = default!;
    public Guid? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public int SortOrder { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class EmpOffboardingTaskAddDto
{
    [Required] public Guid TerminationId { get; set; }
    [Required] public string Category { get; set; } = "Other";
    [Required] public string Title { get; set; } = default!;
    public Guid? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public int SortOrder { get; set; }
}

public class EmpOffboardingTaskUpdateDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string Status { get; set; } = default!;
    public Guid? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    [Required] public string RowVersion { get; set; } = default!;
}
