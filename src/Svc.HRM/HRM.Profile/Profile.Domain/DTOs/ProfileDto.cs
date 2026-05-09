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

public class ProInfo
{
    public string FullName { get; set; } = default!;
    public string FullNameAm { get; set; } = default!;
    public string Position { get; set; } = default!;
    public string EmpState { get; set; } = default!;
}

public class ProOverview
{
    public string Tenure { get; set; } = default!;
    public string PerStr { get; set; } = default!;
    public string Training { get; set; } = default!;
    public double AttendPer { get; set; } = default!;
    public string AttendMonth { get; set; } = default!;
    public string RepToName { get; set; } = default!;
    public string RepToPos { get; set; } = default!;
}

public class ProBasic
{
    [JsonIgnore]
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;

    // Personal Information
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string BirthDate { get; set; } = default!;
    public string MaritalStatus { get; set; } = default!;

    // Employment Details
    public string EmpDate { get; set; } = default!;
    public string Department { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public string EmpType { get; set; } = default!;
    public string EmpNature { get; set; } = default!;
    public string WorkArr { get; set; } = default!;

    // Salary Information
    public string Salary { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public string SalaryPayFreq { get; set; } = default!;
    public string EffectiveFromStr => $"{EffectiveFrom:MMMM dd, yyyy}";
    public string JgStep { get; set; } = default!;
    public string JobGrade { get; set; } = default!;

    // Address & Contact
    public string AddressType { get; set; } = default!;
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

public class ProBio
{
    public Guid Id { get; set; } = default!; //Employee
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
    // Files
    public Guid? BiCertId { get; set; } = default!;
    public string BiCertName { get; set; } = default!;
    public string BiCertType { get; set; } = default!;
    public string BiCertSize { get; set; } = default!;
    public Guid? MaCertId { get; set; } = default!;
    public string MaCertName { get; set; } = default!;
    public string MaCertType { get; set; } = default!;
    public string MaCertSize { get; set; } = default!;
}

public class ProContact
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public bool HasContact { get; set; } = true;
    public ProContactList Contact { get; set; } = default!;
}

public class ProContactList
{
    public Guid Id { get; set; } = default!; //EmergencyContact
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    public string Gender { get; set; } = default!;
    public string Nationality { get; set; } = default!;
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
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

public class ProFamily
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public List<ProFamilyList> Family { get; set; } = [];
}

public class ProFamilyList
{
    public Guid Id { get; set; } = default!; //EmpFamily
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    public string Gender { get; set; } = default!; //enum.Gender
    public string Nationality { get; set; } = default!;
}

public class EmpGuaranty
{
    [JsonIgnore]
    public string FirstName { get; set; } = default!;
    [JsonIgnore]
    public string MiddleName { get; set; } = default!;
    [JsonIgnore]
    public string LastName { get; set; } = default!;
    [JsonIgnore]
    public long FileSize { get; init; } = default!;
    // Details
    public string FullName { get; set; } = default!;
    public string Gender { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    public string Nationality { get; set; } = default!;
    public string Telephone { get; set; } = default!;
    public string Email { get; set; } = default!;
    // Address
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string Country { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Subcity { get; set; } = default!;
    public string Zone { get; set; } = default!;
    public string Woreda { get; set; } = default!;
    public string Kebele { get; set; } = default!;
    public string HouseNo { get; set; } = default!;
    public string PoBox { get; set; } = default!;
    public string Fax { get; set; } = default!;
    public string Website { get; set; } = default!;
    // File
    public Guid FileId { get; set; } = default!; //EmergencyContact
    public string FileName { get; set; } = default!;
    public string ContentType { get; init; } = default!;
    public string FileSizeStr { get; set; } = default!;
}
