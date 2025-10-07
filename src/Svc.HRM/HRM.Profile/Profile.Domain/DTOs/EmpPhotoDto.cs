using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpPhotoDto: BaseDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData
    public Guid PhotoBlobId { get; set; } // EmpPhotoBlob
    public Guid PhotoThumbnailId { get; set; } // EmpPhotoThumbnail
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileSize { get; set; } = default!;
    public string EmpFullName { get; set; } = default!;
    public string EmpFullNameAm { get; set; } = default!;
}

public class EmpPhotoAddDto
{
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpPhotoModDto
{
    public Guid Id { get; set; }
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}