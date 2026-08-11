using Microsoft.EntityFrameworkCore;
using Svc.Auth.Persistence;
using Svc.Auth.Models.Entities;
using MediatR;
using Svc.Auth.Models.Dtos;
using Common;
using Svc.Auth.Interfaces;
using Shared.Helpers.Services;

namespace Svc.Auth.Services;

public interface ISetupService
{
    Task<bool> IsSystemSetupCompleteAsync();
    Task<bool> IsFirstRunAsync();
    Task<SetupStatusDto> GetSetupStatusAsync();
    Task TriggerSyncAsync();
    Task InvalidateStatusCacheAsync();
}

public class SetupService : ISetupService
{
    private readonly AuthDbContext _context;
    private readonly ICoreModuleApiService _coreApi;
    private readonly ICoreHrmmApiService _hrmmApi;
    private readonly IHrmProApiService _hrmProApi;
    private readonly ISyncService _syncService;
    private readonly ILogger<SetupService> _logger;
    private readonly ICacheService _cache;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    // ✅ Cache setup status to avoid repeated checks
    private static bool? _cachedSetupStatus = null;
    private static DateTime _lastStatusCheck = DateTime.MinValue;
    private static readonly object _statusLock = new object();

    public SetupService(
        AuthDbContext context,
        ICoreModuleApiService coreApi,
        ICoreHrmmApiService hrmmApi,
        IHrmProApiService hrmProApi,
        ISyncService syncService,
        ILogger<SetupService> logger,
        ICacheService cache)
    {
        _context = context;
        _coreApi = coreApi;
        _hrmmApi = hrmmApi;
        _hrmProApi = hrmProApi;
        _syncService = syncService;
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> IsSystemSetupCompleteAsync()
    {
        // ✅ Check cache first (5 minute cache)
        lock (_statusLock)
        {
            if (_cachedSetupStatus.HasValue && (DateTime.UtcNow - _lastStatusCheck).TotalMinutes < 5)
            {
                Console.WriteLine($"✅ Using cached setup status: {_cachedSetupStatus.Value}");
                return _cachedSetupStatus.Value;
            }
        }

        try
        {
            Console.WriteLine("🔍 Checking if system setup is complete...");

            // ✅ Check with timeout
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var adminExists = await _context.Users
                .AnyAsync(u => u.UserName == "Admin" ||
                    _context.UserRoles
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                        .Any(x => x.ur.UserId == u.Id && x.r.Name == "admin"), cts.Token);

            Console.WriteLine($"✅ Admin exists: {adminExists}");

            // ✅ Cache the result
            lock (_statusLock)
            {
                _cachedSetupStatus = adminExists;
                _lastStatusCheck = DateTime.UtcNow;
            }

            return adminExists;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("⚠️ Setup check timed out after 10 seconds");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error checking setup completion: {ex.Message}");
            _logger.LogError(ex, "Error checking if system setup is complete");
            return false;
        }
    }

    public async Task<bool> IsFirstRunAsync()
    {
        try
        {
            Console.WriteLine("🔍 Checking if this is first run...");

            // ✅ Check with timeout
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var userCount = await _context.Users.CountAsync(cts.Token);
            var isFirstRun = userCount == 0;

            Console.WriteLine($"✅ User count: {userCount}, Is first run: {isFirstRun}");
            return isFirstRun;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("⚠️ First run check timed out after 10 seconds");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error checking first run: {ex.Message}");
            _logger.LogError(ex, "Error checking first run");
            return true;
        }
    }

    public async Task<SetupStatusDto> GetSetupStatusAsync()
    {
        // ✅ Check cache first
        var cacheKey = "setup_status";
        var cached = await _cache.GetAsync<SetupStatusDto>(cacheKey);
        if (cached != null)
        {
            Console.WriteLine("✅ Using cached setup status from Redis");
            return cached;
        }

        try
        {
            Console.WriteLine("🔍 Getting setup status...");

            // ✅ Get all counts with timeout
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            var userCount = await _context.Users.CountAsync(cts.Token);
            var moduleCount = await _context.PerModule.CountAsync(cts.Token);
            var roleCount = await _context.Roles.CountAsync(cts.Token);
            // Setup is "complete" when a user in the admin role exists. The admin
            // username is chosen during setup, so we must not hard-code "Admin".
            var adminExists = await _context.Users.AnyAsync(u =>
                _context.UserRoles
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                    .Any(x => x.ur.UserId == u.Id && x.r.Name == "admin"), cts.Token);

            Console.WriteLine($"📊 User count: {userCount}");
            Console.WriteLine($"📊 Module count: {moduleCount}");
            Console.WriteLine($"📊 Role count: {roleCount}");
            Console.WriteLine($"📊 Admin exists: {adminExists}");

            var result = new SetupStatusDto
            {
                IsFirstRun = userCount == 0,
                IsSetupComplete = adminExists,
                UserCount = userCount,
                ModuleCount = moduleCount,
                RoleCount = roleCount
            };

            // ✅ Cache for 5 minutes
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            Console.WriteLine($"✅ Setup status: {System.Text.Json.JsonSerializer.Serialize(result)}");
            return result;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("⚠️ Setup status check timed out after 10 seconds");
            return new SetupStatusDto
            {
                IsFirstRun = true,
                IsSetupComplete = false,
                UserCount = 0,
                ModuleCount = 0,
                RoleCount = 0
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting setup status: {ex.Message}");
            _logger.LogError(ex, "Error getting setup status");

            return new SetupStatusDto
            {
                IsFirstRun = true,
                IsSetupComplete = false,
                UserCount = 0,
                ModuleCount = 0,
                RoleCount = 0
            };
        }
    }

    // Clear the cached setup status (static + Redis) so the very next status
    // check reflects reality — called right after setup completes.
    public async Task InvalidateStatusCacheAsync()
    {
        lock (_statusLock)
        {
            _cachedSetupStatus = null;
            _lastStatusCheck = DateTime.MinValue;
        }
        await _cache.RemoveAsync("setup_status");
    }

    public async Task TriggerSyncAsync()
    {
        try
        {
            Console.WriteLine("🔄 Manual sync triggered...");
            _logger.LogInformation("Manual sync triggered...");

            // ✅ Check if sync is already running
            if (!await _lock.WaitAsync(TimeSpan.FromSeconds(30)))
            {
                Console.WriteLine("⏰ Sync already in progress, skipping...");
                _logger.LogWarning("Sync already in progress, skipping...");
                return;
            }

            try
            {
                // ✅ Only sync if setup is complete
                if (!await IsSystemSetupCompleteAsync())
                {
                    Console.WriteLine("⚠️ Setup not complete, skipping sync...");
                    return;
                }

                // ✅ Sync Companies
                Console.WriteLine("🏢 Fetching companies...");
                var companies = await _coreApi.GetAllCompaniesAsync();
                Console.WriteLine($"✅ Found {companies?.Count() ?? 0} companies");

                if (companies != null && companies.Any())
                {
                    foreach (var company in companies)
                    {
                        Console.WriteLine($"   Syncing company: {company.Name} (ID: {company.Id})");
                        await _syncService.SyncCompanyAsync(company);
                    }
                }

                // ✅ Sync Branches
                Console.WriteLine("🏪 Fetching branches...");
                var branches = await _coreApi.GetAllBranchesAsync();
                Console.WriteLine($"✅ Found {branches?.Count() ?? 0} branches");

                if (branches != null && branches.Any())
                {
                    foreach (var branch in branches)
                    {
                        Console.WriteLine($"   Syncing branch: {branch.Name} (ID: {branch.Id})");
                        await _syncService.SyncBranchAsync(branch);
                    }
                }

                // ✅ Sync Departments
                Console.WriteLine("🏢 Fetching departments...");
                var departments = await _coreApi.GetAllDepartmentsAsync();
                Console.WriteLine($"✅ Found {departments?.Count() ?? 0} departments");

                if (departments != null && departments.Any())
                {
                    foreach (var dept in departments)
                    {
                        Console.WriteLine($"   Syncing department: {dept.Name} (ID: {dept.Id})");
                        await _syncService.SyncDepartmentAsync(dept);
                    }
                }

                // ✅ Sync Positions
                Console.WriteLine("💼 Fetching positions...");
                var positions = await _hrmmApi.GetAllPositionsAsync();
                Console.WriteLine($"✅ Found {positions?.Count() ?? 0} positions");

                if (positions != null && positions.Any())
                {
                    foreach (var position in positions)
                    {
                        Console.WriteLine($"   Syncing position: {position.Name} (ID: {position.Id})");
                        await _syncService.SyncPositionAsync(position);
                    }
                }

                // ✅ Sync JobGrades
                Console.WriteLine("📊 Fetching job grades...");
                var jobGrades = await _hrmmApi.GetAllJobGradesAsync();
                Console.WriteLine($"✅ Found {jobGrades?.Count() ?? 0} job grades");

                if (jobGrades != null && jobGrades.Any())
                {
                    foreach (var jobGrade in jobGrades)
                    {
                        Console.WriteLine($"   Syncing job grade: {jobGrade.Name} (ID: {jobGrade.Id})");
                        await _syncService.SyncJobGradeAsync(jobGrade);
                    }
                }

                // ✅ Sync Employees
                Console.WriteLine("👤 Fetching employees...");
                var employees = await _hrmProApi.GetAllEmployeesAsync();
                Console.WriteLine($"✅ Found {employees?.Count() ?? 0} employees");

                if (employees != null && employees.Any())
                {
                    foreach (var employee in employees)
                    {
                        Console.WriteLine($"   Syncing employee: {employee.FirstName} {employee.LastName} (ID: {employee.Id})");
                        await _syncService.SyncEmployeeAsync(employee);
                    }
                }

                Console.WriteLine("✅ Manual sync completed successfully!");
                _logger.LogInformation("Manual sync completed successfully.");
            }
            finally
            {
                _lock.Release();
                Console.WriteLine("🔓 Lock released");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Manual sync failed!");
            Console.WriteLine($"📋 Error: {ex.Message}");
            Console.WriteLine($"📋 Stack trace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                Console.WriteLine($"📋 Inner error: {ex.InnerException.Message}");
                Console.WriteLine($"📋 Inner stack trace: {ex.InnerException.StackTrace}");
            }

            _logger.LogError(ex, "Manual sync failed.");
            throw;
        }
    }
}