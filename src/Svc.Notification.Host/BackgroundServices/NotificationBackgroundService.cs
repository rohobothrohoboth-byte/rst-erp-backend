using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Svc.Notification.Services;
using Dapper;
using Npgsql;

namespace Svc.Notification.Host.BackgroundServices;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly string _connectionString;

    public NotificationBackgroundService(
        IServiceProvider services,
        ILogger<NotificationBackgroundService> logger,
        IConfiguration configuration)
    {
        _services = services;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("NotificationDb")
            ?? throw new InvalidOperationException("Connection string not found");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Use _logger.LogInformation instead of LogInformation
        _logger.LogInformation("Notification Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                // Run checks
                await CheckOverdueTasks(notificationService);

                _logger.LogInformation("Background notification checks completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in notification background service");
            }

            // Wait 24 hours before next run
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation("Notification Background Service stopped");
    }

    private async Task CheckOverdueTasks(INotificationService notificationService)
    {
        try
        {
            // Note: This requires that Svc.Task database is accessible
            // You may need to add a reference to Svc.Task or use HTTP call
            _logger.LogInformation("Checking overdue tasks...");

            // For now, just log that we're checking
            // You'll need to implement this based on your database setup
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking overdue tasks");
        }
    }
}