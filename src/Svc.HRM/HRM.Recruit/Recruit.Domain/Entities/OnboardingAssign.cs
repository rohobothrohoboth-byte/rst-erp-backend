namespace Recruit.Domain.Entities;

public class OnboardingAssign : BaseEntity
{
    public bool IsMandatory { get; set; }
    public string Status { get; set; } = default!; // enum.OnboardingStatus(0/1)
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Guid? VerifyById { get; set; } // HRM.Profile.Employee
    public Guid EmployeeId { get; set; } // HRM.Profile.Employee
    public Guid OnboardingTaskId { get; set; } // OnboardingTask
    //public bool IsOverdue => DateTime.UtcNow > ScheduledDate && Status != OnboardingStatus.Completed;

    //******************************************//

    public virtual OnboardingTask OnboardingTask { get; set; } = null!;
}