using Cor.Procurement.Models.DTOs;
using Cor.Procurement.Models.Entities.Local;
using Cor.Procurement.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cor.Procurement.Services;

public class InitialSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InitialSyncService> _logger;
    private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(5); // ? Check every 5 minutes
    private static bool _hasRun = false;
   private static readonly object _lock = new object();
   private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(10);
    public InitialSyncService(IServiceProvider serviceProvider, ILogger<InitialSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

   // InitialSyncService.cs

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
       _logger.LogInformation("?? InitialSyncService starting...");
        await Task.Delay(_initialDelay, stoppingToken);

    // ? Only run once
    lock (_lock)
    {
        if (_hasRun)
        {
            _logger.LogInformation("?? Initial sync already completed, skipping...");
            return;
        }
        _hasRun = true;
    }
       // ? Wait 10 seconds for services to be ready
       await Task.Delay(10000, stoppingToken);




       try
       {
           using var scope = _serviceProvider.CreateScope();
           var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

           // ? Use Full Sync instead of Incremental Sync
           await syncService.FullSyncAsync(stoppingToken);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "? Initial sync failed");
       }

       // ? Run periodic full sync every hour (or as needed)
       while (!stoppingToken.IsCancellationRequested)
       {
           await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

           try
           {
               using var scope = _serviceProvider.CreateScope();
               var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
               await syncService.FullSyncAsync(stoppingToken);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "? Periodic sync failed");
           }
       }

       _logger.LogInformation("?? InitialSyncService stopping...");
   }

    // ============================================================
    // ? Legacy Full Sync Method (Only used if needed)
    // ============================================================

    public async Task FullSyncAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var coreApi = scope.ServiceProvider.GetRequiredService<ICoreModuleApiService>();
        var hrmmApi = scope.ServiceProvider.GetRequiredService<ICoreHrmmApiService>();
        var hrmProApi = scope.ServiceProvider.GetRequiredService<IHrmProApiService>();
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

        try
        {
            _logger.LogInformation("?? Starting full data sync from all services...");

            // ============ SYNC FROM CORE MODULE API ============
            // Companies, Branches, Departments

            // Sync Companies
            var companies = await coreApi.GetAllCompaniesAsync(cancellationToken);
            foreach (var company in companies)
            {
                await syncService.SyncCompanyAsync(company, cancellationToken);
            }
            _logger.LogInformation("? Synced {Count} companies from Core Module", companies.Count);

            // Sync Branches
            var branches = await coreApi.GetAllBranchesAsync(cancellationToken);
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, cancellationToken);
            }
            _logger.LogInformation("? Synced {Count} branches from Core Module", branches.Count);

            // Sync Departments
            var departments = await coreApi.GetAllDepartmentsAsync(cancellationToken);
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, cancellationToken);
            }
            _logger.LogInformation("? Synced {Count} departments from Core Module", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            // Positions, JobGrades

            // Sync Positions
            var positions = await hrmmApi.GetAllPositionsAsync(cancellationToken);
            foreach (var positionDto in positions)
            {
                if (string.IsNullOrEmpty(positionDto.Name) || string.IsNullOrEmpty(positionDto.NameAm))
                {
                    _logger.LogWarning("?? Skipping position {PositionId} - missing Name or NameAm", positionDto.Id);
                    continue;
                }

                var localPosition = MapToLocalPosition(positionDto);
                await syncService.SyncPositionAsync(localPosition, cancellationToken);
            }
            _logger.LogInformation("? Synced {Count} positions from Core HRMM", positions.Count);

            // Sync JobGrades
            var jobGrades = await hrmmApi.GetAllJobGradesAsync(cancellationToken);
            foreach (var jobGradeDto in jobGrades)
            {
                if (string.IsNullOrEmpty(jobGradeDto.Name))
                {
                    _logger.LogWarning("?? Skipping job grade {JobGradeId} - missing Name", jobGradeDto.Id);
                    continue;
                }

                var localJobGrade = MapToLocalJobGrade(jobGradeDto);
                await syncService.SyncJobGradeAsync(localJobGrade, cancellationToken);
            }
            _logger.LogInformation("? Synced {Count} job grades from Core HRMM", jobGrades.Count);

            // ============ SYNC FROM HRM PRO API ============
            // Employees

            var employees = await hrmProApi.GetAllEmployeesAsync(cancellationToken);
            foreach (var employeeDto in employees)
            {
                var localEmployee = MapToLocalEmployee(employeeDto);
                await syncService.SyncEmployeeAsync(localEmployee, cancellationToken);
            }
            _logger.LogInformation("? Synced {Count} employees from HRM Pro", employees.Count);

            _logger.LogInformation("=========================================");
            _logger.LogInformation("? Full data sync completed successfully!");
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
            _logger.LogError(ex, "? Full data sync failed");
            throw;
        }
    }

    // ============================================================
    // ? MAPPING METHODS
    // ============================================================

    private LocalPosition MapToLocalPosition(PositionDto dto)
    {
        return new LocalPosition
        {
            Id = dto.Id,
            Name = dto.Name ?? string.Empty,
            NameAm = dto.NameAm ?? string.Empty,
            NoOfPosition = dto.NoOfPosition,
            IsVacant = dto.IsVacant ?? "Unknown",
            DepartmentId = dto.DepartmentId,
            JobGradeId = dto.JobGradeId,
            Department = null!,
            JobGrade = null!
        };
    }

    private LocalJobGrade MapToLocalJobGrade(JobGradeDto dto)
    {
        return new LocalJobGrade
        {
            Id = dto.Id,
            Name = dto.Name ?? string.Empty,
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
            Code = dto.Code ?? string.Empty,
            FirstName = dto.FirstName ?? string.Empty,
            FirstNameAm = dto.FirstNameAm ?? string.Empty,
            MiddleName = dto.MiddleName ?? string.Empty,
            MiddleNameAm = dto.MiddleNameAm ?? string.Empty,
            LastName = dto.LastName ?? string.Empty,
            LastNameAm = dto.LastNameAm ?? string.Empty,
            Gender = dto.Gender ?? string.Empty,
            Nationality = dto.Nationality ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Phone = dto.Phone ?? string.Empty,
            PersonId = dto.PersonId,
            PositionId = dto.PositionId,
            DepartmentId = dto.DepartmentId,
            JobGradeId = dto.JobGradeId,
            AppUserId = dto.AppUserId,
            BranchId = dto.BranchId,
            EmpState = dto.EmpState ?? "Active",
            EmploymentType = dto.EmploymentType ?? "FullTime",
            EmploymentNature = dto.EmploymentNature ?? "Permanent",
            WorkArrangement = dto.WorkArrangement ?? "OnSite",
            EmploymentDate = dto.EmploymentDate,
            IsActive = dto.IsActive,
            DateAdd = DateTime.UtcNow,
            SyncedAt = DateTime.UtcNow,
            IsDeleted = false,
            Position = null!,
            Department = null!,
            JobGrade = null!
        };
    }

    private EmployeeDto MapToEmployeeDto(LocalEmployee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            Code = employee.Code,
            FirstName = employee.FirstName,
            FirstNameAm = employee.FirstNameAm,
            MiddleName = employee.MiddleName,
            MiddleNameAm = employee.MiddleNameAm,
            LastName = employee.LastName,
            LastNameAm = employee.LastNameAm,
            Gender = employee.Gender,
            Nationality = employee.Nationality,
            Email = employee.Email,
            Phone = employee.Phone,
            PersonId = employee.PersonId,
            PositionId = employee.PositionId,
            DepartmentId = employee.DepartmentId,
            JobGradeId = employee.JobGradeId,
            AppUserId = employee.AppUserId,
            BranchId = employee.BranchId,
            EmpState = employee.EmpState,
            EmploymentType = employee.EmploymentType,
            EmploymentNature = employee.EmploymentNature,
            WorkArrangement = employee.WorkArrangement,
            EmploymentDate = employee.EmploymentDate,
            IsActive = employee.IsActive?? true
        };
    }
}