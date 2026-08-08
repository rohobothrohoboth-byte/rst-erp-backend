// Svc.Notification.Models/Dtos/NotificationDto.cs
namespace Svc.Notification.Models.Dtos;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? Metadata { get; set; }
    public string? ModuleName { get; set; }
    public Guid? ReferenceId { get; set; }
}

public class CreateNotificationDto
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

public class NotificationAddDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info";
    public string Priority { get; set; } = "medium";
    public string? Metadata { get; set; }
}

public class NotificationStatsDto
{
    public int Total { get; set; }
    public int Unread { get; set; }
    public int Read { get; set; }
    public int Urgent { get; set; }
}


public class BulkNotificationRequest
{
    public List<Guid> UserIds { get; set; } = new();
    public CreateNotificationDto Notification { get; set; } = default!;
}

public class EmploymentTypeNotificationRequest
{
    public string EmploymentType { get; set; } = default!;
    public CreateNotificationDto Notification { get; set; } = default!;
}

public class BatchReadRequest
{
    public List<Guid> NotificationIds { get; set; } = new();
}