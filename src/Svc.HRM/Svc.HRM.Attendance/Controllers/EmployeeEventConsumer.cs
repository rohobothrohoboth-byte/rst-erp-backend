using Microsoft.AspNetCore.Mvc;
using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using MediatR;
using Microsoft.AspNetCore.Authorization;

using Asp.Versioning;
using Helpers;

namespace Svc.HRM.Attendance.Consumers;

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
        _channel.QueueDeclare("attendance.employee.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("attendance.employee.queue", "hrm.events", "hrm.employee.#");
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

        _channel.BasicConsume("attendance.employee.queue", autoAck: false, consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessEmployeeEventAsync(string message, CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var leaveService = scope.ServiceProvider.GetRequiredService<ILeaveService>();

        try
        {
            var eventData = JsonSerializer.Deserialize<EmployeeEventData>(message);
            if (eventData == null) return;

            _logger.LogInformation("Processing employee event: {EventType}", eventData.EventType);

            switch (eventData.EventType)
            {
                case "EMPLOYEE_CREATED":
                    // Initialize leave balance for new employee
                    var year = DateTime.UtcNow.Year;
                    await leaveService.InitializeLeaveBalanceAsync(eventData.EmployeeId, year, ct);
                    _logger.LogInformation("Initialized leave balance for employee {EmployeeId}", eventData.EmployeeId);
                    break;
                case "EMPLOYEE_DELETED":
                    // Handle employee deletion
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
    public Guid EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}