using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shared.Helpers.Events;

namespace Leave.App.Services;

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new object();
    private bool _isInitialized = false;
    private bool _isDisposed = false;

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private void EnsureConnected()
    {
        if (_isDisposed) throw new ObjectDisposedException(nameof(RabbitMQEventPublisher));
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest",
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = TimeSpan.FromSeconds(30)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchanges
                _channel.ExchangeDeclare("core.events", ExchangeType.Topic, durable: true);
                _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);
                _channel.ExchangeDeclare("auth.events", ExchangeType.Topic, durable: true);

                _isInitialized = true;
                _logger.LogInformation("✅ RabbitMQ connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Failed to connect to RabbitMQ. Events will not be published.");
                _isInitialized = true; // Mark as initialized to prevent retry spam
                throw; // Re-throw to let caller know
            }
        }
    }

    public async Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default)
    {
        try
        {
            EnsureConnected();

            if (_channel == null || _connection == null || !_connection.IsOpen)
            {
                _logger.LogWarning("⚠️ RabbitMQ not connected. Event not published: {EntityName} {EventType}", entityName, eventType);
                return;
            }

            var eventData = new EntityEvent<T>
            {
                EventType = eventType,
                EntityName = entityName,
                Data = data,
                Timestamp = DateTime.UtcNow,
                CorrelationId = Guid.NewGuid().ToString()
            };

            var message = JsonSerializer.Serialize(eventData);
            var body = Encoding.UTF8.GetBytes(message);

            var routingKey = $"core.{entityName.ToLower()}.{eventType.ToLower()}";

            _logger.LogInformation("📤 Publishing to exchange: core.events, routingKey: {RoutingKey}", routingKey);

            _channel.BasicPublish(
                exchange: "core.events",
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );

            _logger.LogInformation("✅ Published event: {EntityName} {EventType} with CorrelationId: {CorrelationId}",
                entityName, eventType, eventData.CorrelationId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to publish event: {EntityName} {EventType}", entityName, eventType);
            // Don't throw - let the application continue
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _isDisposed = true;
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}