// Svc.Notification/Services/NotificationMessage.cs
namespace Svc.Notification.Services;

public class NotificationMessage
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Priority { get; set; } = "Normal";
    public string? ModuleName { get; set; }
    public Guid? ReferenceId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}