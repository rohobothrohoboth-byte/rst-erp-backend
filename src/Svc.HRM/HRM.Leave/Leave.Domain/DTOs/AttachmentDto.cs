using EthiopianCalendar;
using Microsoft.AspNetCore.Http;

namespace Leave.Domain.DTOs;

public class AttachmentListDto : BaseDto
{
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string FileSize { get; set; } = default!;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    public string DateAt => $"{DateUpload:MMMM dd, yyyy}";
    public string DateAtAm => DateUpload.ToEthiopianDateString("MMMM dd, yyyy");
}

public class AttachmentAddDto
{
    public IFormFile? File { get; set; } = default!;
    public Guid LeaveRequestId { get; set; } // LeaveRequest
}

public class AttachmentModDto
{
    public Guid Id { get; set; }
    public IFormFile? File { get; set; } = default!;
    public Guid LeaveRequestId { get; set; } // LeaveRequest
    public string RowVersion { get; set; } = default!;
}