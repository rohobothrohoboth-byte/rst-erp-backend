namespace Profile.Domain.DTOs;

public class EmpCertRes
{
    public Guid? BiCertId { get; set; }
    public string BiCertName { get; set; } = default!;
    public string BiCertType { get; set; } = default!;
    public long BiCertSize { get; set; } = default!;
    public bool HasBiCert { get; set; }

    public Guid? MaCertId { get; set; }
    public string MaCertName { get; set; } = default!;
    public string MaCertType { get; set; } = default!;
    public long MaCertSize { get; set; } = default!;
    public bool HasMaCert { get; set; }
}

public class EmpCertJoin
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public string CertType { get; set; } = default!; // enum.CertType
}

public class ProBasicJoin
{
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Code { get; private set; } = default!;
    public string EmploymentType { get; set; } = default!; //enum.EmpType
    public string EmploymentNature { get; set; } = default!; //enum.EmpNature
    public string WorkArrangement { get; set; } = default!; //enum.WorkArrangement
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public string MaritalStatus { get; set; } = default!; //eum.MaritalStatus (0/1)
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;

    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    public Guid JgStepId { get; set; } = default!; //Cor.HRMM.JgStep
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public double BaseSalary { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public string SalaryPayFreq { get; set; } = default!;

    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public Guid AddressId { get; set; } = default!; //Address
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string Telephone { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Website { get; set; } = default!;
}

public class ProOverviewJoin
{
    public Guid Id { get; init; }
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
}

public class ProInfoJoin
{
    public string FirstName { get; init; } = default!;
    public string MiddleName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FirstNameAm { get; init; } = default!;
    public string MiddleNameAm { get; init; } = default!;
    public string LastNameAm { get; init; } = default!;
    public string EmpState { get; set; } = default!; //enum.EmpState
    public Guid PositionId { get; init; }
}
