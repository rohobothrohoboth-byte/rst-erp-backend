namespace Recruit.Domain.DTOs;

public class CoverLetterListDto : BaseDto
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    public Guid CandidateId { get; set; } // Candidate
}

public class CoverLetterAddDto
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    public Guid CandidateId { get; set; } // Candidate
}

public class CoverLetterModDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    public Guid CandidateId { get; set; } // Candidate
    public string RowVersion { get; set; } = default!;
}