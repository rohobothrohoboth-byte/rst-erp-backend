using Svc.HRM.Attendance.Models.DTOs;
using Svc.HRM.Attendance.Models.Entities.Local;
namespace Svc.HRM.Attendance.Services;

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
            // Companies, Branches, Departments

            // Sync Companies
            var companies = await coreApi.GetAllCompaniesAsync(cancellationToken);
            foreach (var company in companies)
            {
                await syncService.SyncCompanyAsync(company, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} companies from Core Module", companies.Count);

            // Sync Branches
            var branches = await coreApi.GetAllBranchesAsync(cancellationToken);
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} branches from Core Module", branches.Count);

            // Sync Departments
            var departments = await coreApi.GetAllDepartmentsAsync(cancellationToken);
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} departments from Core Module", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            // Positions, JobGrades

            // Sync Positions - Map from LocalPosition to PositionDto
           // In InitialSyncService.cs - Update the position and job grade sync

           // Sync Positions - Map from PositionDto to LocalPosition
           var positions = await hrmmApi.GetAllPositionsAsync(cancellationToken);
           foreach (var positionDto in positions)
           {
               // Skip if required fields are missing
               if (string.IsNullOrEmpty(positionDto.Name) || string.IsNullOrEmpty(positionDto.NameAm))
               {
                   _logger.LogWarning("Skipping position {PositionId} - missing Name or NameAm", positionDto.Id);
                   continue;
               }

               var localPosition = MapToLocalPosition(positionDto);
               await syncService.SyncPositionAsync(localPosition, cancellationToken);
           }
           _logger.LogInformation("Synced {Count} positions from Core HRMM", positions.Count);

           // Sync JobGrades - Map from JobGradeDto to LocalJobGrade
           var jobGrades = await hrmmApi.GetAllJobGradesAsync(cancellationToken);
           foreach (var jobGradeDto in jobGrades)
           {
               // Skip if required fields are missing
               if (string.IsNullOrEmpty(jobGradeDto.Name))
               {
                   _logger.LogWarning("Skipping job grade {JobGradeId} - missing Name", jobGradeDto.Id);
                   continue;
               }

               var localJobGrade = MapToLocalJobGrade(jobGradeDto);
               await syncService.SyncJobGradeAsync(localJobGrade, cancellationToken);
           }
           _logger.LogInformation("Synced {Count} job grades from Core HRMM", jobGrades.Count);

            // ============ SYNC FROM HRM PRO API ============
            // Employees, Persons

            // Sync Employees - Map from LocalEmployee to EmployeeDto
            var employees = await hrmProApi.GetAllEmployeesAsync(cancellationToken);
            foreach (var employee in employees)
            {
                 var localEmployee = MapToLocalEmployee(employee);
                await syncService.SyncEmployeeAsync(localEmployee, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} employees from HRM Pro", employees.Count);

            _logger.LogInformation("=========================================");
            _logger.LogInformation("Initial data sync completed successfully!");
            _logger.LogInformation("Total records synced:");
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
            _logger.LogError(ex, "Initial data sync failed. Finance service will start with empty local data.");
        }
    }

    // ============ MAPPING METHODS ============
  private LocalPosition MapToLocalPosition(PositionDto dto)
    {
        return new LocalPosition
        {
            Id = dto.Id,
            Name = dto.Name,
            NameAm = dto.NameAm,
            NoOfPosition = dto.NoOfPosition,
            IsVacant = dto.IsVacant ?? "Unknown",
            DepartmentId = dto.DepartmentId,
            JobGradeId = dto.JobGradeId,
            // Navigation properties will be set by the sync service
            Department = null!,
            JobGrade = null!
        };
    }

    private LocalJobGrade MapToLocalJobGrade(JobGradeDto dto)
    {
        return new LocalJobGrade
        {
            Id = dto.Id,
            Name = dto.Name,
            StartSalary = dto.StartSalary,
            MaxSalary = dto.MaxSalary
        };
    }
    private PositionDto MapToPositionDto(LocalPosition position)
    {
        return new PositionDto
        {
            Id = position.Id,
            Name = position.Name,
            NameAm = position.NameAm,
            NoOfPosition = position.NoOfPosition,
            IsVacant = position.IsVacant,
            DepartmentId = position.DepartmentId,
            JobGradeId = position.JobGradeId
        };
    }

    private JobGradeDto MapToJobGradeDto(LocalJobGrade jobGrade)
    {
        return new JobGradeDto
        {
            Id = jobGrade.Id,
            Name = jobGrade.Name,
            StartSalary = jobGrade.StartSalary,
            MaxSalary = jobGrade.MaxSalary
        };
    }

    private LocalEmployee MapToLocalEmployee(EmployeeDto dto)
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
                BranchId = dto.BranchId,
                EmpState = dto.EmpState,
                EmploymentType = dto.EmploymentType,
                EmploymentNature = dto.EmploymentNature,
                WorkArrangement = dto.WorkArrangement,
                EmploymentDate = dto.EmploymentDate,
             //   IsActive = dto.IsActive,  // ? Add this if not already there
                DateAdd = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow,
                IsDeleted = false,
                // Navigation properties will be set by the sync service
                Position = null!,
                Department = null!,
                JobGrade = null!
            };

    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}