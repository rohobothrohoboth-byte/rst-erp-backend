namespace Profile.Domain.Entities;

public class EmpPhoto: BaseEntity
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData
    public Guid ThumbnailId { get; set; } = default!; //FileMetaData

    //******************************************//

    public Employee Employee { get; set; } = null!;
    public FileMetaData FileMetaData { get; set; } = null!;
    public FileMetaData Thumbnail { get; set; } = null!;
}