namespace Profile.Domain.Entities;

public class EmpCertBirth : BaseEntity
{
    public byte[] Data { get; set; } = default!; // For BYTEA only
    public Guid EmpCertId { get; set; } = default!; //EmpCert

    //******************************************//

    public EmpCert EmpCert { get; set; } = null!;
}