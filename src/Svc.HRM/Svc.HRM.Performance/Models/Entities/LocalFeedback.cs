namespace Svc.HRM.Performance.Models.Entities;

public class LocalFeedback
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid EmployeeId { get; set; }
    public Guid? FromEmployeeId { get; set; }
    public Guid? ReviewId { get; set; }
    public string FeedbackType { get; set; } = "Peer";
    public string Content { get; set; } = default!;
    public int? Rating { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime DateAdd { get; set; } = DateTime.UtcNow;
    public DateTime? DateMod { get; set; }
    public bool IsDeleted { get; set; }
}
