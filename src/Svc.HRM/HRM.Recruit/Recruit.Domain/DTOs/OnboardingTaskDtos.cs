// Recruit.Domain/DTOs/OnboardingTaskDtos.cs
using System.ComponentModel.DataAnnotations;

namespace Recruit.Domain.DTOs;



public class OnboardingTaskListDto
{
    public Guid Id { get; set; }
    public string TaskName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int SequenceOrder { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public string DateAddAm { get; set; } = default!;
    public DateTime? DateMod { get; set; }
    public string? DateModAm { get; set; }
    public string RowVersion { get; set; } = default!; // ✅ Make it a regular property with setter
}

// But for ModDto, keep RowVersion as string (required for update)
public class OnboardingTaskModDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string TaskName { get; set; } = default!;

    [Required]
    public string Description { get; set; } = default!;

    public int SequenceOrder { get; set; }

    [Required]
    public string RowVersion { get; set; } = default!; // ✅ Keep as string for updates
}

public class OnboardingTaskAddDto
{
    [Required]
    public string TaskName { get; set; } = default!;

    [Required]
    public string Description { get; set; } = default!;

    public int SequenceOrder { get; set; }
}
public class OnboardingTaskRawDto
{
    public Guid Id { get; set; }
    public string TaskName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int SequenceOrder { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public uint xmin { get; set; } // ✅ This will properly map from database
}
