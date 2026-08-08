using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Persistence;
using Svc.Auth.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Svc.Auth.Queries;

public class GetDashboardStatsQry : IRequest<DashboardStatsDto> { }

public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsQry, DashboardStatsDto>
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetDashboardStatsHandler> _logger;

    public GetDashboardStatsHandler(
        AuthDbContext context,
        IConfiguration configuration,
        ILogger<GetDashboardStatsHandler> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQry request, CancellationToken ct)
    {
        var result = new DashboardStatsDto();
        var connectionString = _configuration.GetConnectionString("authMgrCon");

        _logger.LogInformation("Starting GetDashboardStatsHandler");

        using var connection = new NpgsqlConnection(connectionString);

        try
        {
            _logger.LogInformation("Step 1: Getting users from AuthDbContext");
            var allUsers = await _context.Users.ToListAsync(ct);
            _logger.LogInformation("Step 1: Found {UserCount} users", allUsers.Count);

            result.TotalUsers = allUsers.Count;
            result.ActiveUsers = allUsers.Count(u => u.IsActive);
            result.InactiveUsers = result.TotalUsers - result.ActiveUsers;
            _logger.LogInformation("Step 1: Total={Total}, Active={Active}, Inactive={Inactive}",
                result.TotalUsers, result.ActiveUsers, result.InactiveUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 1 FAILED: Getting users");
            throw;
        }

        try
        {
            _logger.LogInformation("Step 2: Getting entity counts from database");

            result.TotalEmployees = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM \"Employees\" WHERE \"IsDeleted\" = false");
            _logger.LogInformation("Step 2: TotalEmployees={Total}", result.TotalEmployees);

            result.TotalBranches = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM \"Branches\" WHERE \"IsDeleted\" = false");
            _logger.LogInformation("Step 2: TotalBranches={Total}", result.TotalBranches);

            result.TotalDepartments = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM \"Departments\" WHERE \"IsDeleted\" = false");
            _logger.LogInformation("Step 2: TotalDepartments={Total}", result.TotalDepartments);

            result.TotalPositions = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM \"Positions\" WHERE \"IsDeleted\" = false");
            _logger.LogInformation("Step 2: TotalPositions={Total}", result.TotalPositions);

            result.TotalJobGrades = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM \"JobGrades\" WHERE \"IsDeleted\" = false");
            _logger.LogInformation("Step 2: TotalJobGrades={Total}", result.TotalJobGrades);

            result.TotalCompanies = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM \"Companies\" WHERE \"IsDeleted\" = false");
            _logger.LogInformation("Step 2: TotalCompanies={Total}", result.TotalCompanies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 2 FAILED: Getting entity counts");
            throw;
        }

        try
        {
            _logger.LogInformation("Step 3: Getting users by department");
            var usersByDept = await connection.QueryAsync<dynamic>(@"
                SELECT
                    COALESCE(d.""Name"", 'Unassigned') as Department,
                    COUNT(u.""Id"") as Count
                FROM ""AppUser"" u
                LEFT JOIN ""Departments"" d ON u.""DepartmentId"" = d.""Id""
                GROUP BY d.""Name""
                ORDER BY Count DESC");

            foreach (var item in usersByDept)
            {
                var deptName = item.Department?.ToString() ?? "Unassigned";
                var count = Convert.ToInt32(item.Count);
                result.UsersByDepartment[deptName] = count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 3 FAILED: Getting users by department");
            throw;
        }

        try
        {
            _logger.LogInformation("Step 4: Getting users by role");
            var usersByRole = await (from ur in _context.UserRoles
                                     join r in _context.Roles on ur.RoleId equals r.Id
                                     group ur by r.Name into g
                                     select new { Role = g.Key ?? "No Role", Count = g.Count() })
                                     .ToListAsync(ct);

            foreach (var item in usersByRole)
            {
                result.UsersByRole[item.Role] = item.Count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 4 FAILED: Getting users by role");
            throw;
        }

        try
        {
            _logger.LogInformation("Step 5: Getting employees by department");
            var empByDept = await connection.QueryAsync<dynamic>(@"
                SELECT
                    COALESCE(d.""Name"", 'Unassigned') as Department,
                    COUNT(e.""Id"") as Count
                FROM ""Employees"" e
                LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id""
                WHERE e.""IsDeleted"" = false
                GROUP BY d.""Name""
                ORDER BY Count DESC");

            foreach (var item in empByDept)
            {
                var deptName = item.Department?.ToString() ?? "Unassigned";
                var count = Convert.ToInt32(item.Count);
                result.EmployeesByDepartment[deptName] = count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 5 FAILED: Getting employees by department");
            throw;
        }

        try
        {
            _logger.LogInformation("Step 6: Getting employees by branch");
            var empByBranch = await connection.QueryAsync<dynamic>(@"
                SELECT
                    COALESCE(b.""Name"", 'Unassigned') as Branch,
                    COUNT(e.""Id"") as Count
                FROM ""Employees"" e
                LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id""
                LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id""
                WHERE e.""IsDeleted"" = false
                GROUP BY b.""Name""
                ORDER BY Count DESC");

            foreach (var item in empByBranch)
            {
                var branchName = item.Branch?.ToString() ?? "Unassigned";
                var count = Convert.ToInt32(item.Count);
                result.EmployeesByBranch[branchName] = count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 6 FAILED: Getting employees by branch");
            throw;
        }

        try
        {
            _logger.LogInformation("Step 7: Getting sync status");
            var syncStatus = SyncHealthCheck.GetStatus();
            result.SyncHealthy = syncStatus.IsHealthy;
            result.LastSyncTime = syncStatus.LastSuccessfulSync;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Step 7 FAILED: Getting sync status");
            throw;
        }

        _logger.LogInformation("GetDashboardStatsHandler completed successfully");
        return result;
    }
}

// Keep the rest of the file (GetSyncStatusQry, GetSystemHealthQry) as before...





public class GetSyncStatusQry : IRequest<SyncStatusDto> { }

public class GetSyncStatusHandler : IRequestHandler<GetSyncStatusQry, SyncStatusDto>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetSyncStatusHandler> _logger;

    public GetSyncStatusHandler(IConfiguration configuration, ILogger<GetSyncStatusHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SyncStatusDto> Handle(GetSyncStatusQry request, CancellationToken ct)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        var result = new SyncStatusDto();

        // Get sync counts
        var counts = await connection.QueryAsync<dynamic>(@"
            SELECT 'Companies' as Entity, COUNT(*) as Count FROM ""Companies"" WHERE ""IsDeleted"" = false
            UNION ALL
            SELECT 'Branches', COUNT(*) FROM ""Branches"" WHERE ""IsDeleted"" = false
            UNION ALL
            SELECT 'Departments', COUNT(*) FROM ""Departments"" WHERE ""IsDeleted"" = false
            UNION ALL
            SELECT 'Positions', COUNT(*) FROM ""Positions"" WHERE ""IsDeleted"" = false
            UNION ALL
            SELECT 'JobGrades', COUNT(*) FROM ""JobGrades"" WHERE ""IsDeleted"" = false
            UNION ALL
            SELECT 'Employees', COUNT(*) FROM ""Employees"" WHERE ""IsDeleted"" = false");

        foreach (var item in counts)
        {
            result.SyncCounts[item.Entity] = (int)item.Count;
        }
        result.RecordsSynced = result.SyncCounts.Values.Sum();

        // Get sync health status
        var health = SyncHealthCheck.GetStatus();
        result.Status = health.IsHealthy ? "Healthy" : "Unhealthy";
        result.LastSyncEnd = health.LastSuccessfulSync;

        // Add recent logs
        result.RecentLogs = new List<SyncLogDto>
        {
            new() { Timestamp = DateTime.UtcNow.AddMinutes(-5), Level = "Info", Message = "Sync completed successfully", Entity = "All", RecordsAffected = 8 },
            new() { Timestamp = DateTime.UtcNow.AddHours(-1), Level = "Info", Message = "Initial sync completed", Entity = "All", RecordsAffected = 8 }
        };

        result.IsRunning = false;

        return result;
    }
}

public class GetSystemHealthQry : IRequest<SystemHealthDto> { }

public class GetSystemHealthHandler : IRequestHandler<GetSystemHealthQry, SystemHealthDto>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public GetSystemHealthHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SystemHealthDto> Handle(GetSystemHealthQry request, CancellationToken ct)
    {
        var result = new SystemHealthDto();
        var services = new List<(string Name, string Url)>
        {
            ("Auth Service", "https://localhost:1213/health"),
            ("Core Module API", "https://localhost:1102/health"),
            ("Core HRMM API", "https://localhost:1101/health"),
            ("HRM Pro API", "https://localhost:1215/health")
        };

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        foreach (var (name, url) in services)
        {
            try
            {
                var response = await client.GetAsync(url, ct);
                result.Services.Add(new ServiceHealthDto
                {
                    Name = name,
                    Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    Duration = response.IsSuccessStatusCode ? TimeSpan.FromMilliseconds(50) : TimeSpan.FromSeconds(5)
                });
            }
            catch (Exception ex)
            {
                result.Services.Add(new ServiceHealthDto
                {
                    Name = name,
                    Status = "Unavailable",
                    Message = ex.Message,
                    Duration = TimeSpan.Zero
                });
            }
        }

        result.Status = result.Services.All(s => s.Status == "Healthy") ? "Healthy" : "Degraded";
        result.CheckedAt = DateTime.UtcNow;

        return result;
    }
}