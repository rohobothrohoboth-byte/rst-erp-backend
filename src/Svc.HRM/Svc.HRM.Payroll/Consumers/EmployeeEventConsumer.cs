using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Svc.HRM.Payroll.Services;

namespace Svc.HRM.Payroll.Consumers;

public class EmployeeEventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EmployeeEventConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public EmployeeEventConsumer(IServiceScopeFactory serviceScopeFactory, ILogger<EmployeeEventConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);
        _channel.QueueDeclare("payroll.employee.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("payroll.employee.queue", "hrm.events", "hrm.employee.#");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmployeeEventConsumer started");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await ProcessEmployeeEventAsync(message, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing employee event");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume("payroll.employee.queue", autoAck: false, consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessEmployeeEventAsync(string message, CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var payrollService = scope.ServiceProvider.GetRequiredService<IPayrollService>();

        try
        {
            // Parse employee event
            var eventData = JsonSerializer.Deserialize<EmployeeEventData>(message);
            if (eventData == null) return;

            _logger.LogInformation("Processing employee event: {EventType} for {EmployeeId}",
                eventData.EventType, eventData.EmployeeId);

            // Handle different event types
            switch (eventData.EventType)
            {
                case "EMPLOYEE_CREATED":
                case "EMPLOYEE_UPDATED":
                    // Update employee information in payroll system
                    // This could include salary structure assignment
                    break;
                case "EMPLOYEE_DELETED":
                    // Handle employee deletion
                    break;
                case "EMPLOYEE_SALARY_UPDATED":
                    // Handle salary update
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing employee event");
            throw;
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

public class EmployeeEventData
{
    public string EventType { get; set; } = default!;
    public string EntityName { get; set; } = default!;
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DepartmentId { get; set; }
    public string? PositionId { get; set; }
}