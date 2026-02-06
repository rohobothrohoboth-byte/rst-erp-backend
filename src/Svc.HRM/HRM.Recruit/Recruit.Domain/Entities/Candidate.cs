namespace Recruit.Domain.Entities;

public class Candidate : BaseEntity
{
    //public string? CurrentCompany { get; set; } = default!;
    //public string? CurrentJobTitle { get; set; } = default!;
    //public string? ExperienceYears { get; set; } = default!;
    //public string? SkillsAndExpertise { get; set; } = default!;
    public DateTime RegisteredDate { get; set; }
    public string RegisteredBy { get; set; } = default!;
    public string? ModifiedBy { get; set; } = default!;
    public DateTime? ModifiedAt { get; set; }
    public Guid PersonId { get; set; } // CandidatePerson
    public Guid ContactId { get; set; } // CandidateContact
    public Guid AddressId { get; set; } // CandidateAddress

    //******************************************//

    public CandidatePerson Person { get; set; } = null!;
    public CandidateContact Contact { get; set; } = null!;
    public CandidateAddress Address { get; set; } = null!;
    public List<CandidateApplication> Applications { get; set; } = new();
}