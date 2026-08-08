using Profile.Domain.Entities.Local;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
namespace Profile.App.Services;

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
                await syncService.SyncCompanyAsync(company, cancellationToken);
                _logger.LogInformation("✅ Synced Company: {CompanyId} from Core Module", company.Id);
            }
            _logger.LogInformation("Synced {Count} companies from Core Module", companies.Count);

            var branches = await coreApi.GetAllBranchesAsync(cancellationToken);
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, cancellationToken);
                _logger.LogInformation("✅ Synced Branch: {BranchId} from Core Module", branch.Id);
            }
            _logger.LogInformation("Synced {Count} branches from Core Module", branches.Count);

            var departments = await coreApi.GetAllDepartmentsAsync(cancellationToken);
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, cancellationToken);
                _logger.LogInformation("✅ Synced Department: {DepartmentId} from Core Module", dept.Id);
            }
            _logger.LogInformation("Synced {Count} departments from Core Module", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            var positions = await hrmmApi.GetAllPositionsAsync(cancellationToken);
            foreach (var position in positions)
            {
                await syncService.SyncPositionAsync(position, cancellationToken);
                _logger.LogInformation("✅ Synced Position: {PositionId} from Core HRMM", position.Id);
            }
            _logger.LogInformation("Synced {Count} positions from Core HRMM", positions.Count);

            var jobGrades = await hrmmApi.GetAllJobGradesAsync(cancellationToken);
            foreach (var jobGrade in jobGrades)
            {
                await syncService.SyncJobGradeAsync(jobGrade, cancellationToken);
                _logger.LogInformation("✅ Synced JobGrade: {JobGradeId} from Core HRMM", jobGrade.Id);
            }
            _logger.LogInformation("Synced {Count} job grades from Core HRMM", jobGrades.Count);

            var jgSteps = await hrmmApi.GetAllJgStepsAsync(cancellationToken);
            foreach (var jgStep in jgSteps)
            {
                await syncService.SyncJgStepAsync(jgStep, cancellationToken);
                _logger.LogInformation("✅ Synced JgStep: {JgStepId} from Core HRMM", jgStep.Id);
            }
            _logger.LogInformation("Synced {Count} job grade steps from Core HRMM", jgSteps.Count);




            _logger.LogInformation("=========================================");
            _logger.LogInformation("✅ Initial data sync completed successfully!");
            _logger.LogInformation("📊 Total records synced:");
            _logger.LogInformation("  Companies:   {Companies}", companies.Count);
            _logger.LogInformation("  Branches:    {Branches}", branches.Count);
            _logger.LogInformation("  Departments: {Departments}", departments.Count);
            _logger.LogInformation("  Positions:   {Positions}", positions.Count);
            _logger.LogInformation("  JobGrades:   {JobGrades}", jobGrades.Count);
            _logger.LogInformation("  JgStep:     {JgStep}", jgSteps.Count);
            _logger.LogInformation("=========================================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Initial data sync failed. HRM Profile service will start with empty local data.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}