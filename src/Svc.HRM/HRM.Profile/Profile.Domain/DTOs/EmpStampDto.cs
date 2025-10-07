using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpStampDto : BaseDto
{
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid EmpStampBlobId { get; set; } // EmpStampBlob
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileSize { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
}

public class EmpStampAddDto
{
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpStampModDto
{
    public Guid Id { get; set; }
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}