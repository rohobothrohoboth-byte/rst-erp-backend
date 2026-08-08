 using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Cor.PlanDev.Services;

public class RabbitMQEventPublisher : IEventPublisher
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly object _lock = new object();
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMQEventPublisher(
        IConnectionFactory connectionFactory,
        ILogger<RabbitMQEventPublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        try
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "plandev_events",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("✅ RabbitMQ connection established for PlanDev events");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize RabbitMQ connection");
        }
    }

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (_channel == null || _channel.IsClosed)
            {
                _logger.LogWarning("RabbitMQ channel is closed, reinitializing...");
                InitializeRabbitMQ();

                if (_channel == null)
                {
                    _logger.LogError("Failed to reinitialize RabbitMQ channel");
                    return Task.CompletedTask;
                }
            }

            lock (_lock)
            {
                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(
                    exchange: "",
                    routingKey: "plandev_events",
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("Event published: {EventType}", typeof(T).Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event");
            // Don't throw - just log the error
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}