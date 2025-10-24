namespace Profile.Domain.Entities;

public class EmpBio: BaseEntity
{
    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public string MaritalStatus { get; set; } = default!; //eum.MaritalStatus (0/1)
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid EmployeeId { get; set; } = default!; //Employee

    //******************************************//

    public Employee Employee { get; set; } = null!;
}