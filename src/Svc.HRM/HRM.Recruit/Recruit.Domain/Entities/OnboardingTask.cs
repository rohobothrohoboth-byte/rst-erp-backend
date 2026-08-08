namespace Recruit.Domain.Entities;

public class OnboardingTask : BaseEntity
{
    public string TaskName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int SequenceOrder { get; set; }
}