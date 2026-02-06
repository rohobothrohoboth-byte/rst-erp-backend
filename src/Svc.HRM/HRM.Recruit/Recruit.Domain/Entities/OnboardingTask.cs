namespace Recruit.Domain.Entities;

public class OnboardingTask : BaseEntity
{
    public string TaskName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Category { get; set; } = default!;
    public string AssignedTo { get; set; } = default!;
    public string Status { get; set; } = default!; // enum.OnboardingStatus(0/1)
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int SequenceOrder { get; set; }
    public bool IsMandatory { get; set; }
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    //public bool IsOverdue => DateTime.UtcNow > ScheduledDate && Status != OnboardingStatus.Completed;
}