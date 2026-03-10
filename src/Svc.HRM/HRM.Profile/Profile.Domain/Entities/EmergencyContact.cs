namespace Profile.Domain.Entities;

public class EmergencyContact: BaseEntity
{
    public Guid AddressId { get; set; } = default!; //Address
    public string Relation { get; set; } = default!; //enum.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid PersonId { get; set; } = default!; //Person

    //******************************************//

    public Address Address { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
    public Person Person { get; set; } = null!;
}