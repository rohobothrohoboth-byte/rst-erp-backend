using Cor.HRMM.Models.Entities.Local;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cor.HRMM.Services;

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

            var coreApi = scope.ServiceProvider.GetRequiredService<ICoreModuleApiService>();

            var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

            _logger.LogInformation("🔄 Starting initial data sync from all services...");

            // ============ SYNC FROM CORE MODULE API ============
            // Companies, Branches, Departments

            _logger.LogInformation("📡 Fetching companies from Core Module...");
            var companies = await coreApi.GetAllCompaniesAsync(stoppingToken);
            _logger.LogInformation("✅ Retrieved {Count} companies", companies.Count);

            _logger.LogInformation("📡 Fetching branches from Core Module...");
            var branches = await coreApi.GetAllBranchesAsync(stoppingToken);
            _logger.LogInformation("✅ Retrieved {Count} branches", branches.Count);

            _logger.LogInformation("📡 Fetching departments from Core Module...");
            var departments = await coreApi.GetAllDepartmentsAsync(stoppingToken);
            _logger.LogInformation("✅ Retrieved {Count} departments", departments.Count);

            // ============ SYNC FROM CORE HRMM API ============
            // Positions, JobGrades





            // ============ SYNC COMPANIES ============
            _logger.LogInformation("🔄 Syncing companies...");
            foreach (var company in companies)
            {
                await syncService.SyncCompanyAsync(company, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} companies", companies.Count);

            // ============ SYNC BRANCHES ============
            _logger.LogInformation("🔄 Syncing branches...");
            foreach (var branch in branches)
            {
                await syncService.SyncBranchAsync(branch, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} branches", branches.Count);

            // ============ SYNC DEPARTMENTS ============
            _logger.LogInformation("🔄 Syncing departments...");
            foreach (var dept in departments)
            {
                await syncService.SyncDepartmentAsync(dept, stoppingToken);
            }
            _logger.LogInformation("✅ Synced {Count} departments", departments.Count);




            _logger.LogInformation("=========================================");
            _logger.LogInformation("✅ Initial data sync completed successfully!");
            _logger.LogInformation("📊 Total records synced:");
            _logger.LogInformation("  Companies:   {Companies}", companies.Count);
            _logger.LogInformation("  Branches:    {Branches}", branches.Count);
            _logger.LogInformation("  Departments: {Departments}", departments.Count);

            _logger.LogInformation("=========================================");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Initial data sync failed.");
        }
    }
}