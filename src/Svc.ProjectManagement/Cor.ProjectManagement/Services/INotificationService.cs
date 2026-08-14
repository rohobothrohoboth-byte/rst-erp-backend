// Services/INotificationService.cs
using Cor.ProjectManagement.Models.DTOs;

namespace Cor.ProjectManagement.Services
{
    public interface INotificationService
    {
        Task<ProjectNotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<bool> MarkNotificationReadAsync(MarkNotificationReadDto dto);
        Task<bool> DeleteNotificationAsync(Guid id, Guid userId);
        Task<bool> DeleteAllNotificationsAsync(Guid userId);
        Task<PaginatedResponse<ProjectNotificationDto>> GetUserNotificationsAsync(Guid userId, NotificationFilterDto filter);
        Task<int> GetUnreadCountAsync(Guid userId);
    }

    public class NotificationService : INotificationService
    {
        public Task<ProjectNotificationDto> CreateNotificationAsync(CreateNotificationDto dto) => throw new NotImplementedException();
        public Task<bool> MarkNotificationReadAsync(MarkNotificationReadDto dto) => throw new NotImplementedException();
        public Task<bool> DeleteNotificationAsync(Guid id, Guid userId) => throw new NotImplementedException();
        public Task<bool> DeleteAllNotificationsAsync(Guid userId) => throw new NotImplementedException();
        public Task<PaginatedResponse<ProjectNotificationDto>> GetUserNotificationsAsync(Guid userId, NotificationFilterDto filter) => throw new NotImplementedException();
        public Task<int> GetUnreadCountAsync(Guid userId) => throw new NotImplementedException();
    }
}