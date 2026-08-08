// Svc.Notification/Services/NotificationQueueService.cs
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;

namespace Svc.Notification.Services;

// NotificationMessage is already defined in separate file, so remove it from here
// Just keep the queue service implementation

public interface INotificationQueueService
{
    ValueTask QueueNotificationAsync(NotificationMessage notification);
    IAsyncEnumerable<NotificationMessage> DequeueAsync(CancellationToken cancellationToken);
}

public class NotificationQueueService : INotificationQueueService
{
    private readonly Channel<NotificationMessage> _channel;
    private readonly ILogger<NotificationQueueService> _logger;

    public NotificationQueueService(ILogger<NotificationQueueService> logger)
    {
        var options = new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = false,
            SingleReader = true
        };
        _channel = Channel.CreateBounded<NotificationMessage>(options);
        _logger = logger;
    }

    public async ValueTask QueueNotificationAsync(NotificationMessage notification)
    {
        await _channel.Writer.WriteAsync(notification);
        _logger.LogDebug("Queued notification for user {UserId}: {Title}", notification.UserId, notification.Title);
    }

    public IAsyncEnumerable<NotificationMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}

// Background service to process notifications
public class NotificationProcessingService : BackgroundService
{
    private readonly INotificationQueueService _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationProcessingService> _logger;
    private readonly int _batchSize = 100;
    //private readonly int _batchDelayMs = 1000;

    public NotificationProcessingService(
        INotificationQueueService queue,
        IServiceProvider serviceProvider,
        ILogger<NotificationProcessingService> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Processing Service started");

        var batch = new List<NotificationMessage>();

        await foreach (var notification in _queue.DequeueAsync(stoppingToken))
        {
            batch.Add(notification);

            if (batch.Count >= _batchSize)
            {
                await ProcessBatchAsync(batch, stoppingToken);
                batch.Clear();
            }
        }

        // Process remaining batch
        if (batch.Any())
        {
            await ProcessBatchAsync(batch, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(List<NotificationMessage> batch, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dapper = scope.ServiceProvider.GetRequiredService<IDapperHelper>();

            var sql = @"
                INSERT INTO ""Notifications""
                (""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                 ""ModuleName"", ""ReferenceId"", ""Metadata"", ""CreatedAt"", ""IsDeleted"")
                VALUES (@Id, @UserId, @Title, @Message, @Type, @Priority,
                        @ModuleName, @ReferenceId, @Metadata::jsonb, @CreatedAt, false)";

            var notifications = batch.Select(n => new
            {
                Id = Guid.NewGuid(),
                n.UserId,
                n.Title,
                n.Message,
                n.Type,
                n.Priority,
                n.ModuleName,
                n.ReferenceId,
                Metadata = n.Metadata != null ? System.Text.Json.JsonSerializer.Serialize(n.Metadata) : null,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await dapper.ExecuteAsync(sql, notifications, stoppingToken);

            _logger.LogInformation("Processed {Count} notifications in batch", batch.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification batch");
        }
    }
}