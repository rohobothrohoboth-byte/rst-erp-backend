using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Shared.Helpers.Events;
using Microsoft.Extensions.Hosting;
using Cor.CRM.Models.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Cor.CRM.Models.Entities.Local;
using Task = System.Threading.Tasks.Task;

namespace Cor.CRM.Services;

public class EventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EventConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;

    public EventConsumer(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EventConsumer> logger,
        IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _configuration = configuration;

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

            // Declare exchanges for external services ONLY
            _channel.ExchangeDeclare("core.events", ExchangeType.Topic, durable: true);
            _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);

            // Declare queue for syncing external data
            _channel.QueueDeclare("crm.sync.queue", durable: true, exclusive: false, autoDelete: false);

            // Bind to external exchanges ONLY
            _channel.QueueBind("crm.sync.queue", "core.events", "core.#");
            _channel.QueueBind("crm.sync.queue", "hrm.events", "hrm.#");

            _logger.LogInformation("✅ EventConsumer connected to RabbitMQ successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to connect to RabbitMQ");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 EventConsumer started. Waiting for messages from external services...");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("📩 Received message: {Message}", message);

            try
            {
                if (!IsValidJson(message))
                {
                    _logger.LogWarning("⚠️ Received invalid JSON message: {Message}", message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                await ProcessEventAsync(message, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to process event: {Message}", message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        // Start consuming
        _channel.BasicConsume("crm.sync.queue", autoAck: false, consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private bool IsValidJson(string message)
    {
        if (string.IsNullOrEmpty(message)) return false;
        try
        {
            JsonDocument.Parse(message);
            return true;
        }
        catch (JsonException)
        {
            return false;
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

            string eventType = eventData.EventType;
            string entityName = eventData.EntityName;
            var data = eventData.Data;

            _logger.LogInformation("Processing event: {EntityName} {EventType}", entityName, eventType);

            switch (entityName?.ToLower())
            {
                // External services only
                case "branch":
                    await HandleBranchEvent(syncService, eventType, data, ct);
                    break;
                case "department":
                    await HandleDepartmentEvent(syncService, eventType, data, ct);
                    break;
                case "company":
                    await HandleCompanyEvent(syncService, eventType, data, ct);
                    break;
                case "position":
                    await HandlePositionEvent(syncService, eventType, data, ct);
                    break;
                case "jobgrade":
                    await HandleJobGradeEvent(syncService, eventType, data, ct);
                    break;
                case "employee":
                    await HandleEmployeeEvent(syncService, eventType, data, ct);
                    break;
                default:
                    // Ignore CRM internal events (Lead, Customer, Opportunity, etc.)
                    _logger.LogDebug("Ignoring event for entity: {EntityName} (CRM internal)", entityName);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event");
            throw;
        }
    }

    // ==================== EXTERNAL SYNC HANDLERS ====================

    private async Task HandleBranchEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        try
        {
            var branchData = JsonSerializer.Deserialize<BranchEventData>(JsonSerializer.Serialize(data));
            if (branchData == null) return;

            var dto = new BranchDto
            {
                Id = branchData.Id,
                Name = branchData.Name,
                NameAm = branchData.NameAm,
                Code = branchData.Code,
                Location = branchData.Location,
                CompId = branchData.CompId
            };

            switch (eventType)
            {
                case "CREATED":
                case "UPDATED":
                    await syncService.SyncBranchAsync(dto, ct);
                    _logger.LogInformation("Branch {EventType} synced: {BranchId}", eventType, branchData.Id);
                    break;
                case "DELETED":
                    await syncService.SoftDeleteBranchAsync(branchData.Id, ct);
                    _logger.LogInformation("Branch DELETED synced: {BranchId}", branchData.Id);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Branch event");
        }
    }

    private async Task HandleDepartmentEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        try
        {
            var deptData = JsonSerializer.Deserialize<DepartmentEventData>(JsonSerializer.Serialize(data));
            if (deptData == null) return;

            var dto = new DepartmentDto
            {
                Id = deptData.Id,
                Name = deptData.Name,
                NameAm = deptData.NameAm,
                BranchId = deptData.BranchId
            };

            switch (eventType)
            {
                case "CREATED":
                case "UPDATED":
                    await syncService.SyncDepartmentAsync(dto, ct);
                    _logger.LogInformation("Department {EventType} synced: {DepartmentId}", eventType, deptData.Id);
                    break;
                case "DELETED":
                    await syncService.SoftDeleteDepartmentAsync(deptData.Id, ct);
                    _logger.LogInformation("Department DELETED synced: {DepartmentId}", deptData.Id);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Department event");
        }
    }

    private async Task HandleCompanyEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        try
        {
            var companyData = JsonSerializer.Deserialize<CompanyEventData>(JsonSerializer.Serialize(data));
            if (companyData == null) return;

            var dto = new LocalCompanyDto
            {
                Id = companyData.Id,
                Name = companyData.Name,
                NameAm = companyData.NameAm,
                TaxId = companyData.TaxId,
                Phone = companyData.Phone,
                Email = companyData.Email,
                Address = companyData.Address
            };

            switch (eventType)
            {
                case "CREATED":
                case "UPDATED":
                    await syncService.SyncCompanyAsync(dto, ct);
                    _logger.LogInformation("Company {EventType} synced: {CompanyId}", eventType, companyData.Id);
                    break;
                case "DELETED":
                    await syncService.SoftDeleteCompanyAsync(companyData.Id, ct);
                    _logger.LogInformation("Company DELETED synced: {CompanyId}", companyData.Id);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Company event");
        }
    }

    private async Task HandlePositionEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        try
        {
            var positionData = JsonSerializer.Deserialize<PositionEventData>(JsonSerializer.Serialize(data));
            if (positionData == null) return;

            var position = new LocalPosition
            {
                Id = positionData.Id,
                Name = positionData.Name,
                NameAm = positionData.NameAm,
                NoOfPosition = positionData.NoOfPosition,
                IsVacant = positionData.IsVacant ?? "Unknown",
                DepartmentId = positionData.DepartmentId,
                JobGradeId = positionData.JobGradeId
            };

            switch (eventType)
            {
                case "CREATED":
                case "UPDATED":
                    await syncService.SyncPositionAsync(position, ct);
                    _logger.LogInformation("Position {EventType} synced: {PositionId}", eventType, positionData.Id);
                    break;
                case "DELETED":
                    await syncService.SoftDeletePositionAsync(positionData.Id, ct);
                    _logger.LogInformation("Position DELETED synced: {PositionId}", positionData.Id);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Position event");
        }
    }

    private async Task HandleJobGradeEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        try
        {
            var jobGradeData = JsonSerializer.Deserialize<JobGradeEventData>(JsonSerializer.Serialize(data));
            if (jobGradeData == null) return;

            var jobGrade = new LocalJobGrade
            {
                Id = jobGradeData.Id,
                Name = jobGradeData.Name,
                StartSalary = jobGradeData.StartSalary,
                MaxSalary = jobGradeData.MaxSalary
            };

            switch (eventType)
            {
                case "CREATED":
                case "UPDATED":
                    await syncService.SyncJobGradeAsync(jobGrade, ct);
                    _logger.LogInformation("JobGrade {EventType} synced: {JobGradeId}", eventType, jobGradeData.Id);
                    break;
                case "DELETED":
                    await syncService.SoftDeleteJobGradeAsync(jobGradeData.Id, ct);
                    _logger.LogInformation("JobGrade DELETED synced: {JobGradeId}", jobGradeData.Id);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling JobGrade event");
        }
    }

    private async Task HandleEmployeeEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        try
        {
            var employeeData = JsonSerializer.Deserialize<EmployeeEventData>(JsonSerializer.Serialize(data));
            if (employeeData == null) return;

            var employee = new LocalEmployee
            {
                Id = employeeData.Id,
                Code = employeeData.Code,
                FirstName = employeeData.FirstName,
                FirstNameAm = employeeData.FirstNameAm,
                MiddleName = employeeData.MiddleName,
                MiddleNameAm = employeeData.MiddleNameAm,
                LastName = employeeData.LastName,
                LastNameAm = employeeData.LastNameAm,
                Gender = employeeData.Gender,
                Nationality = employeeData.Nationality,
                Email = employeeData.Email,
                Phone = employeeData.Phone,
                PersonId = employeeData.PersonId,
                PositionId = employeeData.PositionId,
                DepartmentId = employeeData.DepartmentId,
                JobGradeId = employeeData.JobGradeId,
                AppUserId = employeeData.AppUserId,
                EmpState = employeeData.EmpState,
                EmploymentType = employeeData.EmploymentType,
                EmploymentNature = employeeData.EmploymentNature,
                WorkArrangement = employeeData.WorkArrangement,
                EmploymentDate = employeeData.EmploymentDate,
                SyncedAt = DateTime.UtcNow
            };

            switch (eventType)
            {
                case "CREATED":
                case "UPDATED":
                    await syncService.SyncEmployeeAsync(employee, ct);
                    _logger.LogInformation("Employee {EventType} synced: {EmployeeId}", eventType, employeeData.Id);
                    break;
                case "DELETED":
                    await syncService.SoftDeleteEmployeeAsync(employeeData.Id, ct);
                    _logger.LogInformation("Employee DELETED synced: {EmployeeId}", employeeData.Id);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Employee event");
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}