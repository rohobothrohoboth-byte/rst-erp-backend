namespace Profile.Domain.Entities;

public class EmpGuarantorFile : BaseEntity
{
    public Guid EmpGuarantorId { get; set; } = default!; //EmpGuarantor
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData

    //******************************************//

    public EmpGuarantor EmpGuarantor { get; set; } = null!;
    public FileMetaData FileMetaData { get; set; } = null!;
}