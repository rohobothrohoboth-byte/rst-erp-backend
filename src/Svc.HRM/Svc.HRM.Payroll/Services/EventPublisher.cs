using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Svc.HRM.Payroll.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IConfiguration configuration, ILogger<EventPublisher> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("payroll.events", ExchangeType.Topic, durable: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ");
            throw;
        }
    }

    public async Task PublishPayrollProcessedAsync(Guid payrollRunId, string payrollName, CancellationToken ct = default)
    {
        var message = new
        {
            EventType = "PAYROLL_PROCESSED",
            PayrollRunId = payrollRunId,
            PayrollName = payrollName,
            Timestamp = DateTime.UtcNow
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish("payroll.events", "payroll.processed", null, body);
        _logger.LogInformation("Published PAYROLL_PROCESSED event for {PayrollName}", payrollName);
        await Task.CompletedTask;
    }
}