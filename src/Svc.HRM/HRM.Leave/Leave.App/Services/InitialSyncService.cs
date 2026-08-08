// Leave.App/Services/InitialSyncService.cs

using Leave.Domain.Entities.Local;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Leave.Domain.DTOs;
namespace Leave.App.Services;

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
            _logger.LogInformation("🔄 Starting initial data sync from all services...");

            // ============ SYNC FROM CORE MODULE API ============
            var companies = await coreApi.GetAllCompaniesAsync(cancellationToken);
            foreach (var company in companies)
            {
                await syncService.SyncCompanyAsync(company, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} companies from Core Module", companies.Count);

            var branches = await coreApi.GetAllBranchesAsync(cancellationToken);
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} branches from Core Module", branches.Count);

            var departments = await coreApi.GetAllDepartmentsAsync(cancellationToken);
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} departments from Core Module", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            var positions = await hrmmApi.GetAllPositionsAsync(cancellationToken);
            foreach (var position in positions)
            {
                await syncService.SyncPositionAsync(position, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} positions from Core HRMM", positions.Count);

            var jobGrades = await hrmmApi.GetAllJobGradesAsync(cancellationToken);
            foreach (var jobGrade in jobGrades)
            {
                await syncService.SyncJobGradeAsync(jobGrade, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} job grades from Core HRMM", jobGrades.Count);

            var jgSteps = await hrmmApi.GetAllJgStepsAsync(cancellationToken);
            foreach (var jgStep in jgSteps)
            {
                await syncService.SyncJgStepAsync(jgStep, cancellationToken);
            }
            _logger.LogInformation("Synced {Count} job grade steps from Core HRMM", jgSteps.Count);

            // ============ SYNC POSITION REQUIREMENTS ============

                var positionRequirements = await hrmmApi.GetAllPositionRequirementsAsync(cancellationToken);

                foreach (var positionReq in positionRequirements)
                {
                    await syncService.SyncPositionRequirementAsync(positionReq, cancellationToken);
                }
                _logger.LogInformation("Synced {Count} position requirements from Core HRMM", positionRequirements.Count);


 // Sync Employees - Map from LocalEmployee to EmployeeDto
                      var employees = await hrmProApi.GetAllEmployeesAsync(cancellationToken);
                      foreach (var employee in employees)
                      {
                           var localEmployee = MapToLocalEmployee(employee);
                          await syncService.SyncEmployeeAsync(localEmployee, cancellationToken);
                      }
  _logger.LogInformation("Synced {Count} employees from HRM Pro", employees.Count);
            // ============ SUMMARY ============
            _logger.LogInformation("=========================================");
            _logger.LogInformation("✅ Initial data sync completed successfully!");
            _logger.LogInformation("📊 Total records synced:");
            _logger.LogInformation("  Companies:            {Companies}", companies.Count);
            _logger.LogInformation("  Branches:             {Branches}", branches.Count);
            _logger.LogInformation("  Departments:          {Departments}", departments.Count);
            _logger.LogInformation("  Positions:            {Positions}", positions.Count);
            _logger.LogInformation("  JobGrades:            {JobGrades}", jobGrades.Count);
            _logger.LogInformation("  JgSteps:              {JgSteps}", jgSteps.Count);
              _logger.LogInformation("  Employees:              {Employees}", employees.Count);
            _logger.LogInformation("  PositionRequirements: {PositionReqs}", positionRequirements.Count);
            _logger.LogInformation("=========================================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Initial data sync failed. HRM Leave service will start with empty local data.");
        }
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