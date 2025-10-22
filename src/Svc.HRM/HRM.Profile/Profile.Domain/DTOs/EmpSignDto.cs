using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpSignDto : BaseDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public Guid FileMetaDataId { get; set; } = default!; //FileMetaData
    public Guid EmpSignBlobId { get; set; } // EmpSignBlob
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileSize { get; set; } = default!;
}

public class EmpSignAddDto
{
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
}

public class EmpSignModDto
{
    public Guid Id { get; set; }
    public IFormFile File { get; set; } = default!;
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string RowVersion { get; set; } = default!;
}