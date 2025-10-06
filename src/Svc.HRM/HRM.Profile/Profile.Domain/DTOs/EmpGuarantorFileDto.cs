using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpGuarantorFileDto : BaseDto
{
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileSize { get; set; } = default!;
    public Guid EmpGuarantorFileBlobId { get; set; } // EmpGuarantorFileBlob
    public Guid EmpGuarantorId { get; set; } = default!; //EmpGuarantor
    public string EmpGuarantorName { get; set; } = default!;
    public string EmpGuarantorNameAm { get; set; } = default!;
}

public class EmpGuarantorFileAddDto
{
    public IFormFile File { get; set; } = default!;
    public Guid EmpGuarantorId { get; set; } = default!; //EmpGuarantor
}

public class EmpGuarantorFileModDto
{
    public Guid Id { get; set; }
    public IFormFile File { get; set; } = default!;
    public Guid EmpGuarantorId { get; set; } = default!; //EmpGuarantor
    public string RowVersion { get; set; } = default!;
}