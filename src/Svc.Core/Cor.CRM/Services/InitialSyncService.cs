using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Task = System.Threading.Tasks.Task;
using Cor.CRM.Models.Entities.Local;
namespace Cor.CRM.Services;

public class InitialSyncService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InitialSyncService> _logger;

    public InitialSyncService(IServiceProvider serviceProvider, ILogger<InitialSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var coreApi = scope.ServiceProvider.GetRequiredService<ICoreModuleApiService>();
        var hrmmApi = scope.ServiceProvider.GetRequiredService<ICoreHrmmApiService>();
        var hrmProApi = scope.ServiceProvider.GetRequiredService<IHrmProApiService>();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

        try
        {
            _logger.LogInformation("Starting initial data sync from all services...");

            // ============ SYNC FROM CORE MODULE API ============
            var companies = await coreApi.GetAllCompaniesAsync(cancellationToken);
            foreach (var company in companies)
            {
                await syncService.SyncCompanyAsync(company, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} companies", companies.Count);

            var branches = await coreApi.GetAllBranchesAsync(cancellationToken);
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} branches", branches.Count);

            var departments = await coreApi.GetAllDepartmentsAsync(cancellationToken);
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} departments", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            var positions = await hrmmApi.GetAllPositionsAsync(cancellationToken);
            foreach (var positionDto in positions)
            {
                if (string.IsNullOrEmpty(positionDto.Name) || string.IsNullOrEmpty(positionDto.NameAm))
                {
                    _logger.LogWarning("Skipping position {PositionId} - missing Name or NameAm", positionDto.Id);
                    continue;
                }
                var position = MapToPosition(positionDto);
                await syncService.SyncPositionAsync(position, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} positions", positions.Count);

            var jobGrades = await hrmmApi.GetAllJobGradesAsync(cancellationToken);
            foreach (var jobGradeDto in jobGrades)
            {
                if (string.IsNullOrEmpty(jobGradeDto.Name))
                {
                    _logger.LogWarning("Skipping job grade {JobGradeId} - missing Name", jobGradeDto.Id);
                    continue;
                }
                var jobGrade = MapToJobGrade(jobGradeDto);
                await syncService.SyncJobGradeAsync(jobGrade, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} job grades", jobGrades.Count);

            // ============ SYNC FROM HRM PRO API ============
            var employees = await hrmProApi.GetAllEmployeesAsync(cancellationToken);
            foreach (var employeeDto in employees)
            {
                var employee = MapToEmployee(employeeDto);
                await syncService.SyncEmployeeAsync(employee, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} employees", employees.Count);

            _logger.LogInformation("=========================================");
            _logger.LogInformation("Initial data sync completed successfully!");
            _logger.LogInformation("  Companies:   {Companies}", companies.Count);
            _logger.LogInformation("  Branches:    {Branches}", branches.Count);
            _logger.LogInformation("  Departments: {Departments}", departments.Count);
            _logger.LogInformation("  Positions:   {Positions}", positions.Count);
            _logger.LogInformation("  JobGrades:   {JobGrades}", jobGrades.Count);
            _logger.LogInformation("  Employees:   {Employees}", employees.Count);
            _logger.LogInformation("=========================================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initial data sync failed. CRM service will start with empty local data.");
        }
    }

    // ============ MAPPING METHODS ============
    private LocalPosition MapToPosition(PositionDto dto)
    {
        return new LocalPosition
        {
            Id = dto.Id,
            Name = dto.Name,
            NameAm = dto.NameAm,
            NoOfPosition = dto.NoOfPosition,
            IsVacant = dto.IsVacant ?? "Unknown",
            DepartmentId = dto.DepartmentId,
            JobGradeId = dto.JobGradeId
        };
    }

    private LocalJobGrade MapToJobGrade(JobGradeDto dto)
    {
        return new LocalJobGrade
        {
            Id = dto.Id,
            Name = dto.Name,
            StartSalary = dto.StartSalary,
            MaxSalary = dto.MaxSalary
        };
    }

    private LocalEmployee MapToEmployee(EmployeeDto dto)
    {
        return new LocalEmployee
        {
            Id = dto.Id,
            Code = dto.Code,
            FirstName = dto.FirstName,
            FirstNameAm = dto.FirstNameAm,
            MiddleName = dto.MiddleName,
            MiddleNameAm = dto.MiddleNameAm,
            LastName = dto.LastName,
            LastNameAm = dto.LastNameAm,
            Gender = dto.Gender,
            Nationality = dto.Nationality,
            Email = dto.Email,
            Phone = dto.Phone,
            PersonId = dto.PersonId,
            PositionId = dto.PositionId,
            DepartmentId = dto.DepartmentId,
            JobGradeId = dto.JobGradeId,
            AppUserId = dto.AppUserId,
            EmpState = dto.EmpState,
            EmploymentType = dto.EmploymentType,
            EmploymentNature = dto.EmploymentNature,
            WorkArrangement = dto.WorkArrangement,
            EmploymentDate = dto.EmploymentDate,
            DateAdd = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}