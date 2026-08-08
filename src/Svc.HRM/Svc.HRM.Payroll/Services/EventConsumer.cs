using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Shared.Helpers.Events;
using Microsoft.Extensions.Hosting;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Svc.HRM.Payroll.Models.Entities.Local;

namespace Svc.HRM.Payroll.Services;

public class EventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EventConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public EventConsumer(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EventConsumer> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

       var factory = new ConnectionFactory
       {
           HostName = configuration["RabbitMQ:Host"] ?? "rabbitmq",  // ✅ Use service name
           Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
           UserName = configuration["RabbitMQ:Username"] ?? "guest",
           Password = configuration["RabbitMQ:Password"] ?? "guest"
       };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("core.events", ExchangeType.Topic, durable: true);
        _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);
        _channel.QueueDeclare("hrm.sync.queue", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind("hrm.sync.queue", "core.events", "core.#");
        _channel.QueueBind("hrm.sync.queue", "hrm.events", "hrm.#");

        _logger.LogInformation("EventConsumer connected to RabbitMQ for HRM.Profile");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventConsumer started. Waiting for messages...");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                await ProcessEventAsync(message, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process event");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume("hrm.sync.queue", autoAck: false, consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ProcessEventAsync(string message, CancellationToken ct)
    {
        // ✅ Use IServiceScopeFactory to create scope
        using var scope = _serviceScopeFactory.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

        try
        {
            // ✅ Deserialize to a concrete type instead of dynamic
            var eventData = JsonSerializer.Deserialize<EntityEvent<object>>(message);
            if (eventData == null)
            {
                _logger.LogWarning("Failed to deserialize event message");
                return;
            }

            string entityName = eventData.EntityName;
            var data = eventData.Data;

            _logger.LogInformation("Processing event: {EntityName} {EventType} with CorrelationId: {CorrelationId}",
                entityName, eventData.EventType, eventData.CorrelationId);

            switch (entityName?.ToLower())
            {
                case "company":
                    var company = JsonSerializer.Deserialize<LocalCompany>(JsonSerializer.Serialize(data));
                    if (company != null)
                    {
                        await syncService.SyncCompanyAsync(company, ct);
                        _logger.LogInformation("Company {EventType} synced: {CompanyId}", eventData.EventType, company.Id);
                    }
                    break;

                case "branch":
                    var branch = JsonSerializer.Deserialize<LocalBranch>(JsonSerializer.Serialize(data));
                    if (branch != null)
                    {
                        await syncService.SyncBranchAsync(branch, ct);
                        _logger.LogInformation("Branch {EventType} synced: {BranchId}", eventData.EventType, branch.Id);
                    }
                    break;

                case "department":
                    var dept = JsonSerializer.Deserialize<LocalDepartment>(JsonSerializer.Serialize(data));
                    if (dept != null)
                    {
                        await syncService.SyncDepartmentAsync(dept, ct);
                        _logger.LogInformation("Department {EventType} synced: {DepartmentId}", eventData.EventType, dept.Id);
                    }
                    break;

                case "position":
                    var position = JsonSerializer.Deserialize<LocalPosition>(JsonSerializer.Serialize(data));
                    if (position != null)
                    {
                        await syncService.SyncPositionAsync(position, ct);
                        _logger.LogInformation("Position {EventType} synced: {PositionId}", eventData.EventType, position.Id);
                    }
                    break;

                case "jobgrade":
                    var jobGrade = JsonSerializer.Deserialize<LocalJobGrade>(JsonSerializer.Serialize(data));
                    if (jobGrade != null)
                    {
                        await syncService.SyncJobGradeAsync(jobGrade, ct);
                        _logger.LogInformation("JobGrade {EventType} synced: {JobGradeId}", eventData.EventType, jobGrade.Id);
                    }
                    break;



                default:
                    _logger.LogWarning("Unknown entity type: {EntityName}", entityName ?? "null");
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize event message: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event: {Message}", message);
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