namespace Profile.Domain.Entities;

public class EmpGuarantor : BaseEntity
{
    public Guid AddressId { get; set; } = default!; //Cor.HRMM.Address
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid PersonId { get; set; } = default!; //Person

    //******************************************//

    public Employee Employee { get; set; } = null!;
    public Person Person { get; set; } = null!;
}
