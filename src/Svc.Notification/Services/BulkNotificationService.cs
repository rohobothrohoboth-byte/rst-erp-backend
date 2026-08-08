// Svc.Notification/Services/BulkNotificationService.cs
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;
using Microsoft.Extensions.Logging;  // Add this
namespace Svc.Notification.Services;

public class BulkNotificationService : IBulkNotificationService
{
    private readonly IDapperHelper _dapper;
    private readonly INotificationQueueService _queue;
    private readonly ILogger<BulkNotificationService> _logger;

    public BulkNotificationService(
        IDapperHelper dapper,
        INotificationQueueService queue,
        ILogger<BulkNotificationService> logger)
    {
        _dapper = dapper;
        _queue = queue;
        _logger = logger;
    }

    public async Task SendBulkNotificationsAsync(List<Guid> userIds, CreateNotificationDto notification)
    {
        // Implementation
        var batchSize = 100;
        for (int i = 0; i < userIds.Count; i += batchSize)
        {
            var batch = userIds.Skip(i).Take(batchSize);
            foreach (var userId in batch)
            {
                await _queue.QueueNotificationAsync(new NotificationMessage
                {
                    UserId = userId,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    Priority = notification.Priority,
                    ModuleName = notification.ModuleName,
                    ReferenceId = notification.ReferenceId,
                    Metadata = notification.Metadata
                });
            }
            await Task.Delay(100);
        }
        _logger.LogInformation("Queued {Count} notifications", userIds.Count);
    }

    public async Task SendToDepartmentAsync(Guid departmentId, CreateNotificationDto notification)
    {
        var sql = @"
            SELECT DISTINCT e.""UserId""
            FROM ""Employee"" e
            WHERE e.""DepartmentId"" = @DepartmentId
            AND e.""IsDeleted"" = false";

        var userIds = await _dapper.QueryAsync<Guid>(sql, new { DepartmentId = departmentId });
        await SendBulkNotificationsAsync(userIds.ToList(), notification);
    }

    public async Task SendToAllEmployeesAsync(CreateNotificationDto notification)
    {
        var sql = @"
            SELECT DISTINCT ""UserId""
            FROM ""Employee""
            WHERE ""IsDeleted"" = false";

        var userIds = await _dapper.QueryAsync<Guid>(sql);
        await SendBulkNotificationsAsync(userIds.ToList(), notification);
    }

    public async Task SendByEmploymentTypeAsync(string employmentType, CreateNotificationDto notification)
    {
        var sql = @"
            SELECT DISTINCT ""UserId""
            FROM ""Employee""
            WHERE ""EmploymentType"" = @EmploymentType
            AND ""IsDeleted"" = false";

        var userIds = await _dapper.QueryAsync<Guid>(sql, new { EmploymentType = employmentType });
        await SendBulkNotificationsAsync(userIds.ToList(), notification);
    }
}