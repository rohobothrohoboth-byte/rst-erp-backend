namespace Profile.Domain.Entities;

public class EmpOffboardingTask : BaseEntity
{
    public Guid TerminationId { get; set; }
    public string Category { get; set; } = "Other"; // Asset | Access | Document | Other
    public string Title { get; set; } = default!;
    public string Status { get; set; } = "Pending"; // Pending | InProgress | Completed | Skipped | Blocked
    public Guid? AssignedToId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public int SortOrder { get; set; }

    public EmpTermination Termination { get; set; } = null!;
}
