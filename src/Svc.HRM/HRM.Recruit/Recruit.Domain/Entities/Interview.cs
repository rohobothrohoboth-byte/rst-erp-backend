// Recruit.Domain/Entities/Interview.cs

using System.ComponentModel.DataAnnotations.Schema;

namespace Recruit.Domain.Entities;

public class Interview : BaseEntity
{
    public Guid ApplicantId { get; set; }
    public Guid JobPostingId { get; set; }
    public string InterviewType { get; set; } = default!; // enum.InterviewType
    public DateTime ScheduledDate { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
    public Guid? InterviewerId { get; set; }
    public string Status { get; set; } = default!; // enum.InterviewStatus

    [ForeignKey(nameof(ApplicantId))]
    public virtual JobApplication Applicant { get; set; } = null!;

    [ForeignKey(nameof(JobPostingId))]
    public virtual JobPosting JobPosting { get; set; } = null!;
}