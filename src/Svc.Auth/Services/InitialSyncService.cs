using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Svc.Auth.Services;

public class InitialSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InitialSyncService> _logger;
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(10);
    private static bool _hasRun = false;
    private static readonly object _lock = new object();

    public InitialSyncService(IServiceProvider serviceProvider, ILogger<InitialSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🔄 InitialSyncService starting...");

        // ✅ Wait for services to be ready
        await Task.Delay(_initialDelay, stoppingToken);

        // ✅ Only run once
        lock (_lock)
        {
            if (_hasRun)
            {
                _logger.LogInformation("ℹ️ Initial sync already completed, skipping...");
                return;
            }
            _hasRun = true;
        }

        try
        {
            using var scope = _serviceProvider.CreateScope();

            // ✅ Check if setup is complete before syncing
            var setupService = scope.ServiceProvider.GetService<ISetupService>();
            if (setupService != null)
            {
                var isSetupComplete = await setupService.IsSystemSetupCompleteAsync();
                if (!isSetupComplete)
                {
                    _logger.LogInformation("ℹ️ Setup not complete, skipping initial sync");
                    return;
                }
            }

            var coreApi = scope.ServiceProvider.GetRequiredService<ICoreModuleApiService>();
            var hrmmApi = scope.ServiceProvider.GetRequiredService<ICoreHrmmApiService>();
            var hrmProApi = scope.ServiceProvider.GetRequiredService<IHrmProApiService>();
            var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

            _logger.LogInformation("🔄 Starting initial data sync from all services...");

            // ✅ Parallel API calls
            var companiesTask = coreApi.GetAllCompaniesAsync(stoppingToken);
            var branchesTask = coreApi.GetAllBranchesAsync(stoppingToken);
            var departmentsTask = coreApi.GetAllDepartmentsAsync(stoppingToken);
            var positionsTask = hrmmApi.GetAllPositionsAsync(stoppingToken);
            var jobGradesTask = hrmmApi.GetAllJobGradesAsync(stoppingToken);
            var employeesTask = hrmProApi.GetAllEmployeesAsync(stoppingToken);

            await Task.WhenAll(companiesTask, branchesTask, departmentsTask,
                               positionsTask, jobGradesTask, employeesTask);

            var companies = await companiesTask;
            var branches = await branchesTask;
            var departments = await departmentsTask;
            var positions = await positionsTask;
            var jobGrades = await jobGradesTask;
            var employees = await employeesTask;

            // ============ SYNC FROM CORE MODULE API ============
            // Companies, Branches, Departments

            _logger.LogInformation("🔄 Syncing companies...");
            foreach (var company in companies)
            {
                await syncService.SyncCompanyAsync(company, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} companies from Core Module", companies.Count);

            _logger.LogInformation("🔄 Syncing branches...");
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} branches from Core Module", branches.Count);

            _logger.LogInformation("🔄 Syncing departments...");
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} departments from Core Module", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            // Positions, JobGrades

            _logger.LogInformation("🔄 Syncing positions...");
            foreach (var position in positions)
            {
                await syncService.SyncPositionAsync(position, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} positions from Core HRMM", positions.Count);

            _logger.LogInformation("🔄 Syncing job grades...");
            foreach (var jobGrade in jobGrades)
            {
                await syncService.SyncJobGradeAsync(jobGrade, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} job grades from Core HRMM", jobGrades.Count);

            // ============ SYNC FROM HRM PRO API ============
            // Employees, Persons

            _logger.LogInformation("🔄 Syncing employees...");
            foreach (var employee in employees)
            {
                await syncService.SyncEmployeeAsync(employee, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} employees from HRM Pro", employees.Count);

            _logger.LogInformation("=========================================");
            _logger.LogInformation("✅ Initial data sync completed successfully!");
            _logger.LogInformation("📊 Total records synced:");
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
            _logger.LogError(ex, "❌ Initial data sync failed. Auth service will start with empty local data.");
        }
    }
}