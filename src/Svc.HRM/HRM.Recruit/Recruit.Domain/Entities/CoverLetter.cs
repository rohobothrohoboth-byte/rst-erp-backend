namespace Recruit.Domain.Entities;

public class CoverLetter : BaseEntity
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    public Guid CandidateId { get; set; } // Candidate

    //******************************************//

    public Candidate Candidate { get; set; } = null!;
}