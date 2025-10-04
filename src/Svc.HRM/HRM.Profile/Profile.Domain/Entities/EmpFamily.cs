namespace Profile.Domain.Entities;

public class EmpFamily : BaseEntity
{
    public Guid RelationId { get; set; } = default!; //lup.Relation
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid PersonId { get; set; } = default!; //Person

    //******************************************//

    public Employee Employee { get; set; } = null!;
    public Person Person { get; set; } = null!;
}
