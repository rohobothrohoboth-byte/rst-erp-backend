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
using Leave.Domain.Entities.Local;

namespace Leave.App.Services;

public class EventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EventConsumer> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new object();
    private bool _isInitialized = false;
    private bool _isEnabled;

    public EventConsumer(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EventConsumer> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _configuration = configuration;

        // Check if RabbitMQ is disabled via environment variable
        _isEnabled = !Environment.GetEnvironmentVariable("DISABLE_RABBITMQ")?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? true;

        if (_isEnabled)
        {
            try
            {
                InitializeRabbitMQ();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Failed to initialize RabbitMQ. EventConsumer will be disabled.");
                _isEnabled = false;
            }
        }
        else
        {
            _logger.LogInformation("ℹ️ EventConsumer disabled via DISABLE_RABBITMQ environment variable");
        }
    }

    private void InitializeRabbitMQ()
    {
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

            _channel.ExchangeDeclare("core.events", ExchangeType.Topic, durable: true);
            _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);
            _channel.QueueDeclare("hrm.sync.queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("hrm.sync.queue", "core.events", "core.#");
            _channel.QueueBind("hrm.sync.queue", "hrm.events", "hrm.#");

            _isInitialized = true;
            _logger.LogInformation("✅ EventConsumer connected to RabbitMQ for HRM.Leave");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Failed to initialize RabbitMQ");
            _isEnabled = false;
            throw; // Re-throw to let caller handle
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isEnabled || _connection == null || _channel == null)
        {
            _logger.LogInformation("ℹ️ EventConsumer is disabled or RabbitMQ not available");
            // Keep the service running even if RabbitMQ is not available
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            return;
        }

        _logger.LogInformation("✅ EventConsumer started. Waiting for messages...");

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

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessEventAsync(string message, CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

        try
        {
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

                case "jgstep":
                    var jgStep = JsonSerializer.Deserialize<LocalJgStep>(JsonSerializer.Serialize(data));
                    if (jgStep != null)
                    {
                        await syncService.SyncJgStepAsync(jgStep, ct);
                        _logger.LogInformation("JgStep {EventType} synced: {JgStepId}", eventData.EventType, jgStep.Id);
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
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}