using System.Text.Json.Serialization;

namespace Profile.Domain.DTOs;

public class EmpPhotoRes
{
    [JsonIgnore]
    public byte[]? PhotoBinary { get; init; }
    [JsonIgnore]
    public long FileSize { get; set; } = default!;

    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string PhotoSize { get; set; } = default!;
    public string Photo { get; set; } = default!;
}

public class ProfileInfoJoin
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

public class ProfileInfo
{
    public string FullName { get; set; } = default!;
    public string FullNameAm { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string EmpState { get; set; } = default!;
}

public class ProfileCardJoin
{
    public Guid Id { get; init; }
    public DateTime EmploymentDate { get; set; } = DateTime.UtcNow;
}

public class ProfileCard
{
    public string Tenure { get; set; } = default!;
    public string Performance { get; set; } = default!;
    public string Training { get; set; } = default!;
    public string Attendance { get; set; } = default!;
    public string RepToName { get; set; } = default!;
    public string RepToPos { get; set; } = default!;
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
    public Guid PositionId { get; set; } = default!; //Cor.HRMM.Position
    public Guid DepartmentId { get; set; } = default!; //Cor.Module.Department
    public string MaritalStatus { get; set; } = default!; //eum.MaritalStatus (0/1)
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
}

public class ProBasicInfo
{
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string BirthDate { get; set; } = default!;
    public string MaritalStatus { get; set; } = default!;
    public string EmpDate { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string EmpNature { get; set; } = default!;
    public string WorkArr { get; set; } = default!;
}

public class ProSalary
{
    [JsonIgnore]
    public Guid JobGradeId { get; set; } = default!; //Cor.HRMM.JobGrade
    [JsonIgnore]
    public Guid JgStepId { get; set; } = default!; //Cor.HRMM.JgStep
    [JsonIgnore]
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public double BaseSalary { get; set; } = default!;

    public string Salary { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public string SalaryPayFreq { get; set; } = default!;
    public string EffectiveFromStr => $"{EffectiveFrom:MMMM dd, yyyy}";
    public string JgStep { get; set; } = default!;
    public string JobGrade { get; set; } = default!;
}

public class ProBasicAddress
{
    [JsonIgnore]
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    [JsonIgnore]
    public Guid AddressId { get; set; } = default!; //Address

    public string AddressTypeStr { get; set; } = default!;
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

public class ProBasicBio
{
    [JsonIgnore]
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    [JsonIgnore]
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo

    public Guid Id { get; set; } = default!; //Employee
    public Guid? MarriageCertId { get; set; } = default!;
    public Guid? BirthCertId { get; set; } = default!;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCertStr { get; set; } = default!;
    public string HasMarriageCertStr { get; set; } = default!;
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
}
