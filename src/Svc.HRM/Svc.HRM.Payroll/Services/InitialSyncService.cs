using Svc.HRM.Payroll.Models.DTOs;
using Svc.HRM.Payroll.Models.Entities.Local;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Svc.HRM.Payroll.Services;

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
        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

        try
        {
            _logger.LogInformation("🔄 Starting initial data sync from all services...");

            // ============ SYNC FROM CORE MODULE API ============
            var companies = await coreApi.GetAllCompaniesAsync(cancellationToken);
            foreach (var company in companies)
            {
                var localCompany = new LocalCompany
                {
                    Id = company.Id,
                    Name = company.Name,
                    NameAm = company.NameAm,
                    TaxId = company.TaxId,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    IsDeleted = false,
                    SyncedAt = DateTime.UtcNow,
                    DateAdd = DateTime.UtcNow
                };
                await syncService.SyncCompanyAsync(localCompany, cancellationToken);
                _logger.LogInformation("✅ Synced Company: {CompanyId} from Core Module", company.Id);
            }
            _logger.LogInformation("Synced {Count} companies from Core Module", companies.Count);

            var branches = await coreApi.GetAllBranchesAsync(cancellationToken);
            foreach (var branch in branches)
            {
                var localBranch = new LocalBranch
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    NameAm = branch.NameAm,
                    Code = branch.Code,
                    Location = branch.Location,
                    CompId = branch.CompId,
                    IsDeleted = false,
                    SyncedAt = DateTime.UtcNow,
                    DateAdd = DateTime.UtcNow
                };
                await syncService.SyncBranchAsync(localBranch, cancellationToken);
                _logger.LogInformation("✅ Synced Branch: {BranchId} from Core Module", branch.Id);
            }
            _logger.LogInformation("Synced {Count} branches from Core Module", branches.Count);

            var departments = await coreApi.GetAllDepartmentsAsync(cancellationToken);
            foreach (var dept in departments)
            {
                var localDept = new LocalDepartment
                {
                    Id = dept.Id,
                    Name = dept.Name,
                    NameAm = dept.NameAm,
                    BranchId = dept.BranchId,
                    IsDeleted = false,
                    SyncedAt = DateTime.UtcNow,
                    DateAdd = DateTime.UtcNow
                };
                await syncService.SyncDepartmentAsync(localDept, cancellationToken);
                _logger.LogInformation("✅ Synced Department: {DepartmentId} from Core Module", dept.Id);
            }
            _logger.LogInformation("Synced {Count} departments from Core Module", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            var positions = await hrmmApi.GetAllPositionsAsync(cancellationToken);
            foreach (var position in positions)
            {
                var localPosition = new LocalPosition
                {
                    Id = position.Id,
                    Name = position.Name,
                    NameAm = position.NameAm,
                    NoOfPosition = position.NoOfPosition,
                    IsVacant = position.IsVacant,
                    DepartmentId = position.DepartmentId,
                    JobGradeId = position.JobGradeId,
                    DateAdd = DateTime.UtcNow,
                    SyncedAt = DateTime.UtcNow
                };
                await syncService.SyncPositionAsync(localPosition, cancellationToken);
                _logger.LogInformation("✅ Synced Position: {PositionId} from Core HRMM", position.Id);
            }
            _logger.LogInformation("Synced {Count} positions from Core HRMM", positions.Count);

            var jobGrades = await hrmmApi.GetAllJobGradesAsync(cancellationToken);
            foreach (var jobGrade in jobGrades)
            {
                var localJobGrade = new LocalJobGrade
                {
                    Id = jobGrade.Id,
                    Name = jobGrade.Name,
                    StartSalary = jobGrade.StartSalary,
                    MaxSalary = jobGrade.MaxSalary,
                    DateAdd = DateTime.UtcNow,
                    SyncedAt = DateTime.UtcNow
                };
                await syncService.SyncJobGradeAsync(localJobGrade, cancellationToken);
                _logger.LogInformation("✅ Synced JobGrade: {JobGradeId} from Core HRMM", jobGrade.Id);
            }
            _logger.LogInformation("Synced {Count} job grades from Core HRMM", jobGrades.Count);

            _logger.LogInformation("=========================================");
            _logger.LogInformation("✅ Initial data sync completed successfully!");
            _logger.LogInformation("📊 Total records synced:");
            _logger.LogInformation("  Companies:   {Companies}", companies.Count);
            _logger.LogInformation("  Branches:    {Branches}", branches.Count);
            _logger.LogInformation("  Departments: {Departments}", departments.Count);
            _logger.LogInformation("  Positions:   {Positions}", positions.Count);
            _logger.LogInformation("  JobGrades:   {JobGrades}", jobGrades.Count);
            _logger.LogInformation("=========================================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Initial data sync failed. HRM Payroll service will start with empty local data.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}