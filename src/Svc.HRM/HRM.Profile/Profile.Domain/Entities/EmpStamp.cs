namespace Profile.Domain.Entities;

public class EmpStamp : BaseEntity
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData

    //******************************************//

    public Employee Employee { get; set; } = null!;
    public FileMetaData FileMetaData { get; set; } = null!;
}