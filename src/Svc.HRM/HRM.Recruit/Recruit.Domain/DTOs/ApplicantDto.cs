namespace Recruit.Domain.DTOs;

public class CandidateListDto : BaseDto
{
    public DateTime RegisteredDate { get; set; }
    public string RegisteredBy { get; set; } = default!;
    public string? ModifiedBy { get; set; } = default!;
    public Guid PersonId { get; set; } // CandidatePerson
    public Guid ContactId { get; set; } // CandidateContact
    public Guid AddressId { get; set; } // CandidateAddress
}

public class CandidateAddDto
{
    public DateTime RegisteredDate { get; set; }
    public string RegisteredBy { get; set; } = default!;
    public string? ModifiedBy { get; set; } = default!;
    public Guid PersonId { get; set; } // CandidatePerson
    public Guid ContactId { get; set; } // CandidateContact
    public Guid AddressId { get; set; } // CandidateAddress
}

public class CandidateModDto
{
    public Guid Id { get; set; }
    public DateTime RegisteredDate { get; set; }
    public string RegisteredBy { get; set; } = default!;
    public string? ModifiedBy { get; set; } = default!;
    public Guid PersonId { get; set; } // CandidatePerson
    public Guid ContactId { get; set; } // CandidateContact
    public Guid AddressId { get; set; } // CandidateAddress
    public string RowVersion { get; set; } = default!;
}