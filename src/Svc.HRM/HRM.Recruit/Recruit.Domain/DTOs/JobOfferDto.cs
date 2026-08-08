using System.ComponentModel.DataAnnotations;

namespace Recruit.Domain.DTOs;

public class JobOfferListDto
{
    public Guid Id { get; set; }
    public string OfferNumber { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public DateTime OfferDate { get; set; }
    public string OfferDateAm { get; set; } = default!;
    public DateTime ExpirationDate { get; set; }
    public string ExpirationDateAm { get; set; } = default!;
    public string OfferDocument { get; set; } = default!;
    public Guid JobApplicationId { get; set; }
    public Guid JobPostingId { get; set; }
    public string? ApplicantName { get; set; }
    public string? PostNumber { get; set; }
    public Guid? HiredEmployeeId { get; set; }
    public DateTime DateAdd { get; set; }
    public string DateAddAm { get; set; } = default!;
    public DateTime? DateMod { get; set; }
    public string? DateModAm { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class JobOfferViewDto : JobOfferListDto
{
    public List<JobOfferApprovalDto> Approvals { get; set; } = [];
    public JobOfferReviewDto? Review { get; set; }
}

public class JobOfferAddDto
{
    [Required]
    public Guid JobApplicationId { get; set; }

    [Required]
    public DateTime OfferDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public string OfferDocument { get; set; } = default!;

    /// <summary>Optional approval steps (Dept/HR/Finance). If empty, offer starts as Approved.</summary>
    public List<JobOfferApprovalStepDto>? ApprovalSteps { get; set; }
}

public class JobOfferApprovalStepDto
{
    [Required]
    public int StepOrder { get; set; }

    [Required]
    public string Role { get; set; } = default!;
}

public class JobOfferModDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public DateTime OfferDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public string OfferDocument { get; set; } = default!;

    [Required]
    public string RowVersion { get; set; } = default!;
}

public class JobOfferApproveDto
{
    [Required]
    public Guid OfferId { get; set; }

    [Required]
    public Guid ApprovedById { get; set; }

    public string? Comments { get; set; }
}

public class JobOfferRejectDto
{
    [Required]
    public Guid OfferId { get; set; }

    [Required]
    public Guid RejectedById { get; set; }

    [Required]
    public string Reason { get; set; } = default!;
}

public class JobOfferRespondDto
{
    [Required]
    public Guid OfferId { get; set; }

    public string? Comments { get; set; }

    public string? RejectionReason { get; set; }
}

public class HireFromOfferDto
{
    [Required]
    public Guid OfferId { get; set; }

    /// <summary>Required. Job grade for Profile hire (not stored on JobRequisition).</summary>
    [Required]
    public Guid JobGradeId { get; set; }

    /// <summary>Optional overrides; defaults come from JobRequisition / WorkforcePlan.</summary>
    public Guid? JgStepId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? DepartmentId { get; set; }

    public string EmploymentType { get; set; } = "0"; // EmpType
    public string EmploymentNature { get; set; } = "0"; // EmpNature Permanent
    public string WorkArrangement { get; set; } = "0"; // WorkArrangement OnSite
    public string MaritalStatus { get; set; } = "0"; // MaritalStatus
    public DateTime? EmploymentDate { get; set; }
    public DateTime? BirthDate { get; set; }

    /// <summary>When true, create OnboardingAssign rows for all active onboarding tasks.</summary>
    public bool AssignOnboardingTasks { get; set; } = true;

    public int OnboardingDaysOffset { get; set; } = 7;
}

public class HireFromOfferResultDto
{
    public Guid OfferId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid JobApplicationId { get; set; }
    public int OnboardingAssignmentsCreated { get; set; }
}

public class JobOfferApprovalDto
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; }
    public string Role { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string StatusName { get; set; } = default!;
    public DateTime? ApprovedDate { get; set; }
    public Guid? ApprovedById { get; set; }
}

public class JobOfferReviewDto
{
    public Guid Id { get; set; }
    public DateTime? AcceptanceDate { get; set; }
    public DateTime? RejectionDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovalComments { get; set; }
}

public class JobOfferRawDto
{
    public Guid Id { get; set; }
    public string OfferNumber { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime OfferDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string OfferDocument { get; set; } = default!;
    public Guid JobApplicationId { get; set; }
    public Guid JobPostingId { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public uint xmin { get; set; }
}
