namespace Profile.Domain.Entities;

public class EmpBio: BaseEntity
{
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public Guid MaritalStatusId { get; set; } = default!; //lup.MaritalStatus
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid EmployeeId { get; set; } = default!; //Employee

    //******************************************//

    public Employee Employee { get; set; } = null!;
}