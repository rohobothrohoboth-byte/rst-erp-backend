using System.ComponentModel.DataAnnotations;

namespace Recruit.Domain.DTOs;

public class OnboardingAssignmentListDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = "";
    public string EmployeeEmail { get; set; } = "";
    public string? EmployeePhone { get; set; }
    public string Position { get; set; } = "";
    public string Department { get; set; } = "";
    public Guid TaskId { get; set; }
    public string TaskName { get; set; } = "";
    public string TaskDescription { get; set; } = "";
    public string Status { get; set; } = "";
    public bool IsMandatory { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string RowVersion { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OnboardingAssignmentAddDto
{
    [Required]
    public Guid EmployeeId { get; set; }
    [Required]
    public Guid TaskId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public bool IsMandatory { get; set; }
}

public class OnboardingAssignmentModDto
{
    [Required]
    public Guid Id { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Status { get; set; }
    public bool? IsMandatory { get; set; }
    [Required]
    public string RowVersion { get; set; } = default!;
}

public class OnboardingAssignmentStatusDto
{
    [Required]
    public string Status { get; set; } = default!;
}

// Raw Dapper mapping (assignment joined to its task).
public class OnboardingAssignRawDto
{
    public Guid Id { get; set; }
    public bool IsMandatory { get; set; }
    public string Status { get; set; } = "";
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid OnboardingTaskId { get; set; }
    public string TaskName { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public uint xmin { get; set; }
}
