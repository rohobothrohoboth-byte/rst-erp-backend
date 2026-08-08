// Svc.Notification/Services/IBulkNotificationService.cs
using Svc.Notification.Models.Dtos;

namespace Svc.Notification.Services;

public interface IBulkNotificationService
{
    Task SendBulkNotificationsAsync(List<Guid> userIds, CreateNotificationDto notification);
    Task SendToDepartmentAsync(Guid departmentId, CreateNotificationDto notification);
    Task SendToAllEmployeesAsync(CreateNotificationDto notification);
    Task SendByEmploymentTypeAsync(string employmentType, CreateNotificationDto notification);
}