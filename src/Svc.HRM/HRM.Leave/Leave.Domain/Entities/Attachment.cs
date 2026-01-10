namespace Leave.Domain.Entities;

public class Attachment : BaseEntity
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    public Guid LeaveRequestId { get; set; } // 

    //******************************************//

    public LeaveRequest LeaveRequest { get; set; } = null!;
}