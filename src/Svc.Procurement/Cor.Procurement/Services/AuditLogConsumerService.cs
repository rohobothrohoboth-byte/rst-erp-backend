// Services/AuditLogConsumerService.cs

using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities;
using Cor.Procurement.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Cor.Procurement.Services;

public class AuditLogConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<AuditLogConsumerService> _logger;
    private readonly string _queueName = "audit-events";
    private bool _disposed;

    public AuditLogConsumerService(
        IServiceScopeFactory scopeFactory,
        IConnectionFactory connectionFactory,
        ILogger<AuditLogConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        try
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            // ✅ FIX: Remove x-max-priority for quorum queue
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    { "x-queue-type", "quorum" }
                });

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 20, global: false);

            _logger.LogInformation("✅ Audit Log Consumer Service initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize Audit Log Consumer Service: {Message}", ex.Message);
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var auditEvent = JsonSerializer.Deserialize<AuditLogEventDto>(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (auditEvent != null)
                {
                    await ProcessAuditLogAsync(auditEvent);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    _logger.LogDebug("✅ Audit log processed: {Action}", auditEvent.Action);
                }
                else
                {
                    _logger.LogWarning("⚠️ Failed to deserialize audit event");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing audit log message");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("🔄 Audit Log Consumer started, waiting for messages...");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessAuditLogAsync(AuditLogEventDto auditEvent)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProcurementDbContext>();

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = auditEvent.UserId,
                UserName = auditEvent.UserName,
                UserEmail = auditEvent.UserEmail,
                UserRole = auditEvent.UserRole,
                EntityType = auditEvent.EntityType ?? string.Empty,
                EntityId = auditEvent.EntityId,
                Action = auditEvent.Action ?? string.Empty,
                ActionDate = auditEvent.ActionDate,
                OldValues = auditEvent.OldValues,
                NewValues = auditEvent.NewValues,
                ChangesJson = auditEvent.ChangesJson,
                IpAddress = auditEvent.IpAddress,
                RequestId = auditEvent.RequestId,
                DurationMs = auditEvent.DurationMs,
                Status = auditEvent.Status,
                ErrorMessage = auditEvent.ErrorMessage,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await dbContext.AuditLogs.AddAsync(auditLog);
            await dbContext.SaveChangesAsync();

            _logger.LogDebug("💾 Audit log saved: {Action}", auditEvent.Action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to save audit log for {Action}", auditEvent.Action);
            throw;
        }
    }

    public override void Dispose()
    {
        if (_disposed) return;

        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error during disposal");
        }

        _disposed = true;
        base.Dispose();
    }
}