namespace Svc.Auth.Models.Dtos;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalBranches { get; set; }
    public int TotalDepartments { get; set; }
    public int TotalPositions { get; set; }
    public int TotalJobGrades { get; set; }
    public int TotalCompanies { get; set; }
    public DateTime LastSyncTime { get; set; }
    public bool SyncHealthy { get; set; }
    public Dictionary<string, int> UsersByDepartment { get; set; } = new();
    public Dictionary<string, int> UsersByRole { get; set; } = new();
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    public Dictionary<string, int> EmployeesByBranch { get; set; } = new();
}

public class SyncStatusDto
{
    public bool IsRunning { get; set; }
    public DateTime? LastSyncStart { get; set; }
    public DateTime? LastSyncEnd { get; set; }
    public int RecordsSynced { get; set; }
    public string? Status { get; set; }
    public List<SyncLogDto> RecentLogs { get; set; } = new();
    public Dictionary<string, int> SyncCounts { get; set; } = new();
}

public class SyncLogDto
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Entity { get; set; }
    public int? RecordsAffected { get; set; }
}

public class SystemHealthDto
{
    public string Status { get; set; } = "Unknown";
    public List<ServiceHealthDto> Services { get; set; } = new();
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}

public class ServiceHealthDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
    public string? Message { get; set; }
    public TimeSpan Duration { get; set; }
}