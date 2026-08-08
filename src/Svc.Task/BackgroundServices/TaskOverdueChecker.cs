using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Svc.Task.Interfaces;
using Svc.Task.Services;
using Svc.Task.Models.Dtos;
// ✅ Use an alias for your Task entity
using TaskEntity = Svc.Task.Models.Entities.Task;

namespace Svc.Task.BackgroundServices;

public class TaskOverdueChecker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskOverdueChecker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public TaskOverdueChecker(IServiceProvider serviceProvider, ILogger<TaskOverdueChecker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckOverdueTasks(stoppingToken);
                await System.Threading.Tasks.Task.Delay(_checkInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking overdue tasks");
            }
        }
    }

    private async System.Threading.Tasks.Task CheckOverdueTasks(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dapper = scope.ServiceProvider.GetRequiredService<IDapperHelper>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var sql = @"
            SELECT * FROM ""Tasks""
            WHERE ""DueDate"" < NOW()
            AND ""Status"" != 'completed'
            AND ""IsDeleted"" = false
            AND (""LastOverdueNotified"" IS NULL OR ""LastOverdueNotified"" < NOW() - INTERVAL '1 day')";

        // ✅ Use TaskEntity instead of Task
        var overdueTasks = await dapper.QueryAsync<TaskEntity>(sql, null, stoppingToken);

        foreach (var task in overdueTasks)
        {
            await notificationService.NotifyTaskOverdue(
                task.AssignedTo.ToString(),
                task.Title,
                task.Id.ToString(),
                task.DueDate
            );

            var updateSql = @"UPDATE ""Tasks"" SET ""LastOverdueNotified"" = NOW() WHERE ""Id"" = @Id";
            await dapper.ExecuteAsync(updateSql, new { task.Id }, stoppingToken);
        }

        if (overdueTasks.Any())
        {
            _logger.LogInformation("Sent overdue notifications for {Count} tasks", overdueTasks.Count());
        }
    }
}