using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Shared.Helpers.Events;
using Microsoft.Extensions.Hosting;
using Cor.Procurement.Models.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Cor.Procurement.Models.Entities.Local;
namespace Cor.Procurement.Services;

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

            _channel.ExchangeDeclare("core.events", ExchangeType.Topic, durable: true);
            _channel.ExchangeDeclare("hrm.events", ExchangeType.Topic, durable: true);

            _channel.QueueDeclare("finance.sync.queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("finance.sync.queue", "core.events", "core.#");
            _channel.QueueBind("finance.sync.queue", "hrm.events", "hrm.#");

            _logger.LogInformation("? EventConsumer connected to RabbitMQ successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Failed to connect to RabbitMQ");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("?? EventConsumer started. Waiting for messages...");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("?? Received message: {Message}", message);

            try
            {
                if (!IsValidJson(message))
                {
                    _logger.LogWarning("?? Received invalid JSON message: {Message}", message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                await ProcessEventAsync(message, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to process event: {Message}", message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume("finance.sync.queue", autoAck: false, consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private bool IsValidJson(string message)
    {
        if (string.IsNullOrEmpty(message))
            return false;

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

            _logger.LogInformation("Processing event: {EntityName} {EventType} with CorrelationId: {CorrelationId}",
                entityName, eventType, eventData.CorrelationId);

            switch (entityName?.ToLower())
            {
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
                    _logger.LogWarning("Unknown entity type: {EntityName}", entityName);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event");
            throw;
        }
    }

    private async Task HandleBranchEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
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
            OpenDate = branchData.OpenDate,
            BranchType = branchData.BranchType,
            BranchStat = branchData.BranchStat,
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

    private async Task HandleDepartmentEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        var deptData = JsonSerializer.Deserialize<DepartmentEventData>(JsonSerializer.Serialize(data));
        if (deptData == null) return;

        var dto = new DepartmentDto
        {
            Id = deptData.Id,
            Name = deptData.Name,
            NameAm = deptData.NameAm,
            DeptStat = deptData.DeptStat,
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

    private async Task HandleCompanyEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        var companyData = JsonSerializer.Deserialize<CompanyEventData>(JsonSerializer.Serialize(data));
        if (companyData == null) return;

        var dto = new CompanyDto
        {
            Id = companyData.Id,
            Name = companyData.Name,
            NameAm = companyData.NameAm,
            TaxId = companyData.TaxId,
            Phone = companyData.Phone,
            Email = companyData.Email,
            Address = companyData.Address,
            LogoUrl = companyData.LogoUrl
        };

        switch (eventType)
        {
            case "CREATED":
            case "UPDATED":
                await syncService.SyncCompanyAsync(dto, ct);
                _logger.LogInformation("Company {EventType} synced: {CompanyId}", eventType, companyData.Id);
                break;
            case "DELETED":
                break;
        }
    }

   private async Task HandlePositionEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
   {
       var positionData = JsonSerializer.Deserialize<Shared.Helpers.Events.PositionEventData>(JsonSerializer.Serialize(data));
       if (positionData == null) return;

       // Map to LocalPosition entity
       var localPosition = new LocalPosition
       {
           Id = positionData.Id,
           Name = positionData.Name,
           NameAm = positionData.NameAm,
           NoOfPosition = positionData.NoOfPosition,
           IsVacant = positionData.IsVacant ?? "Unknown",
           DepartmentId = positionData.DepartmentId,
           JobGradeId = positionData.JobGradeId,
           Department = null!,
           JobGrade = null!
       };

       switch (eventType)
       {
           case "CREATED":
           case "UPDATED":
               await syncService.SyncPositionAsync(localPosition, ct);
               _logger.LogInformation("Position {EventType} synced: {PositionId}", eventType, positionData.Id);
               break;
           case "DELETED":
               // Handle soft delete if needed
               break;
       }
   }

   private async Task HandleJobGradeEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
   {
       var jobGradeData = JsonSerializer.Deserialize<Shared.Helpers.Events.JobGradeEventData>(JsonSerializer.Serialize(data));
       if (jobGradeData == null) return;

       // Map to LocalJobGrade entity
       var localJobGrade = new LocalJobGrade
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
               await syncService.SyncJobGradeAsync(localJobGrade, ct);
               _logger.LogInformation("JobGrade {EventType} synced: {JobGradeId}", eventType, jobGradeData.Id);
               break;
           case "DELETED":
               // Handle soft delete if needed
               break;
       }
   }


    private async Task HandleEmployeeEvent(ISyncService syncService, string eventType, object data, CancellationToken ct)
    {
        var employeeData = JsonSerializer.Deserialize<EmployeeEventData>(JsonSerializer.Serialize(data));
        if (employeeData == null) return;

        var dto = new LocalEmployee
        {
            Id = employeeData.Id,
            Code = employeeData.Code,
            PersonId = employeeData.PersonId,
            JobGradeId = employeeData.JobGradeId,
            PositionId = employeeData.PositionId,
            DepartmentId = employeeData.DepartmentId,
            AppUserId = employeeData.AppUserId,
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
            EmploymentType = employeeData.EmploymentType,
            EmploymentNature = employeeData.EmploymentNature,
            WorkArrangement = employeeData.WorkArrangement,
            EmpState = employeeData.EmpState,
            EmploymentDate = employeeData.EmploymentDate
        };

        switch (eventType)
        {
            case "CREATED":
            case "UPDATED":
                await syncService.SyncEmployeeAsync(dto, ct);
                _logger.LogInformation("Employee {EventType} synced: {EmployeeId}", eventType, employeeData.Id);
                break;
            case "DELETED":
                break;
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}