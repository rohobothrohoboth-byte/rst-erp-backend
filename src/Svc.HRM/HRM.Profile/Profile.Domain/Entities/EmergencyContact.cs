namespace Profile.Domain.Entities;

public class EmergencyContact: BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid AddressId { get; set; } = default!; //Address

    //******************************************//

    public Address Address { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}