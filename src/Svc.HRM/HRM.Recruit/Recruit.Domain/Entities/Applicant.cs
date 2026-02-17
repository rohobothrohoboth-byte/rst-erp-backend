namespace Recruit.Domain.Entities;

public class Applicant : BaseEntity
{
    public DateTime RegisteredDate { get; set; }
    public string RegisteredBy { get; set; } = default!;
    public Guid PersonId { get; set; } // ApplicantPerson
    public Guid ContactId { get; set; } // ApplicantContact
    public Guid AddressId { get; set; } // ApplicantAddress

    //******************************************//

    public virtual ApplicantAddress Address { get; set; } = null!;
    public virtual ApplicantContact Contact { get; set; } = null!;
    public virtual ApplicantPerson Person { get; set; } = null!;
}