namespace Recruit.Domain.DTOs;

public class OnboardingTaskListDto : BaseDto
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
}

public class OnboardingTaskAddDto
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
}

public class OnboardingTaskModDto
{
    public Guid Id { get; set; }
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
    public string RowVersion { get; set; } = default!;
}