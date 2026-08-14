// Services/RabbitMQAuditPublisher.cs

using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Models.Enums;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Cor.ProjectManagement.Services;

public class RabbitMQAuditPublisher : IAuditLogPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQAuditPublisher> _logger;
    private readonly string _queueName = "audit-events";
    private bool _disposed;
    private bool _isAvailable;

    public bool IsAvailable => _isAvailable && !_disposed;

    public RabbitMQAuditPublisher(
        IConnectionFactory connectionFactory,
        ILogger<RabbitMQAuditPublisher> logger)
    {
        _logger = logger;
        _isAvailable = false;

        try
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            // ✅ FIX: Remove x-max-priority - Quorum queues don't support it
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    { "x-queue-type", "quorum" } // Quorum queue for HA (no priority support)
                });

            _isAvailable = true;
            _logger.LogInformation("✅ RabbitMQ Audit Publisher initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize RabbitMQ Audit Publisher: {Message}", ex.Message);
            _isAvailable = false;
        }
    }

   public async Task PublishAsync(AuditLogEventDto auditEvent)
   {
       if (_disposed || !_isAvailable)
       {
           _logger.LogWarning("⚠️ RabbitMQ unavailable, audit event not published");
           return;
       }

       try
       {
           // ✅ Ensure we have user info
           _logger.LogDebug("📤 Publishing audit - User: {User}, Entity: {Entity}, Action: {Action}",
               auditEvent.UserName ?? "Anonymous",
               auditEvent.EntityType ?? "Unknown",
               auditEvent.Action ?? "Unknown");

           var json = JsonSerializer.Serialize(auditEvent, new JsonSerializerOptions
           {
               PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
               WriteIndented = false,
               DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
           });

           var body = Encoding.UTF8.GetBytes(json);

           var properties = _channel.CreateBasicProperties();
           properties.Persistent = true;
           properties.DeliveryMode = 2;
           properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
           properties.ContentType = "application/json";
           properties.MessageId = Guid.NewGuid().ToString();

           _channel.BasicPublish(
               exchange: "",
               routingKey: _queueName,
               basicProperties: properties,
               body: body);

           _logger.LogDebug("📤 Audit event published: {Action}", auditEvent.Action);
           await Task.CompletedTask;
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "❌ Failed to publish audit event for {Action}", auditEvent.Action);
           _isAvailable = false;
       }
   }

    public void Dispose()
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
        _isAvailable = false;
        _logger.LogInformation("🧹 RabbitMQ Audit Publisher disposed");
    }
}