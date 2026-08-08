using System.ComponentModel.DataAnnotations;

namespace Recruit.Domain.DTOs;

public class OnboardingAssignListDto
{
    public Guid Id { get; set; }
    public bool IsMandatory { get; set; }
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string ScheduledDateAm { get; set; } = default!;
    public DateTime? CompletedDate { get; set; }
    public string? CompletedDateAm { get; set; }
    public Guid? VerifyById { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid OnboardingTaskId { get; set; }
    public string? TaskName { get; set; }
    public int? SequenceOrder { get; set; }
    public DateTime DateAdd { get; set; }
    public string DateAddAm { get; set; } = default!;
    public DateTime? DateMod { get; set; }
    public string? DateModAm { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class OnboardingAssignAddDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid OnboardingTaskId { get; set; }

    public bool IsMandatory { get; set; } = true;

    [Required]
    public DateTime ScheduledDate { get; set; }
}

public class OnboardingAssignBulkAddDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public int DaysOffset { get; set; } = 7;

    public bool IsMandatory { get; set; } = true;
}

public class OnboardingAssignModDto
{
    [Required]
    public Guid Id { get; set; }

    public bool IsMandatory { get; set; }

    [Required]
    public string Status { get; set; } = default!;

    [Required]
    public DateTime ScheduledDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public Guid? VerifyById { get; set; }

    [Required]
    public string RowVersion { get; set; } = default!;
}

public class OnboardingAssignCompleteDto
{
    [Required]
    public Guid Id { get; set; }

    public Guid? VerifyById { get; set; }

    [Required]
    public string RowVersion { get; set; } = default!;
}

public class OnboardingAssignRawDto
{
    public Guid Id { get; set; }
    public bool IsMandatory { get; set; }
    public string Status { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Guid? VerifyById { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid OnboardingTaskId { get; set; }
    public string? TaskName { get; set; }
    public int? SequenceOrder { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public uint xmin { get; set; }
}
