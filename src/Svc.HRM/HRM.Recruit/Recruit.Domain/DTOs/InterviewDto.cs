// Recruit.Domain/DTOs/InterviewDto.cs

using System.Text.Json.Serialization;

namespace Recruit.Domain.DTOs;

public class InterviewAddDto
{
    public Guid ApplicantId { get; set; }
    public Guid JobPostingId { get; set; }
    public string InterviewType { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
    public Guid? InterviewerId { get; set; }
}

public class InterviewModDto
{
    public Guid Id { get; set; }
    public string InterviewType { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
    public Guid? InterviewerId { get; set; }
    public string Status { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

public class InterviewStatusUpdateDto
{
    public string Status { get; set; } = default!;
}

public class InterviewListDto : BaseDto
{
    public Guid ApplicantId { get; set; }
    public Guid JobPostingId { get; set; }
    public string InterviewType { get; set; } = default!;
    public string InterviewTypeStr { get; set; } = default!;
    public DateTime ScheduledDate { get; set; }
    public string ScheduledDateStr => $"{ScheduledDate:MMMM dd, yyyy HH:mm}";
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string? Notes { get; set; }
    public Guid? InterviewerId { get; set; }
    public string? InterviewerName { get; set; }
    public string Status { get; set; } = default!;
    public string StatusStr { get; set; } = default!;
    public string ApplicantName { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
 public Guid PositionId { get; set; }

}

public class InterviewDetailDto : InterviewListDto
{
    public string ApplicantEmail { get; set; } = default!;
    public string ApplicantPhone { get; set; } = default!;
    public string JobPostingTitle { get; set; } = default!;
    public string JobPostingNumber { get; set; } = default!;
    public List<InterviewFeedbackDto>? Feedbacks { get; set; }
}

public class InterviewFeedbackDto
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = default!;
    public int? Rating { get; set; }
    public string? Comments { get; set; }
    public DateTime ReviewedDate { get; set; }
    public string Status { get; set; } = default!;
}