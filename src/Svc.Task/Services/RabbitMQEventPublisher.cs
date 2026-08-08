using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Shared.Helpers.Events;

namespace Svc.Task.Services;

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly IConfiguration _configuration;

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger;
        _configuration = configuration;

        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("core.events", ExchangeType.Topic, durable: true);
        _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);
        _channel.ExchangeDeclare("auth.events", ExchangeType.Topic, durable: true);
    }

    public async System.Threading.Tasks.Task PublishAsync<T>(string entityName, string eventType, T data, CancellationToken ct = default)
    {
        try
        {
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

            _logger.LogInformation("📤 Publishing to exchange: core.events, routingKey: {RoutingKey}, message: {Message}",
                routingKey, message);

            _channel.BasicPublish(
                exchange: "core.events",
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );

            _logger.LogInformation("Published event: {EntityName} {EventType} with CorrelationId: {CorrelationId}",
                entityName, eventType, eventData.CorrelationId);

            await System.Threading.Tasks.Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event: {EntityName} {EventType}", entityName, eventType);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}