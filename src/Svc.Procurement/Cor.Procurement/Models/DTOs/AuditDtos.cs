using Cor.Procurement.Models.Enums;
namespace Cor.Procurement.Models.DTOs;


public class AuditFilterDto
{
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Action { get; set; }
    public string? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
public class AuditLogListResponse
{
    public List<AuditLogDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
public class AuditLogEventDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // User Information
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }

    // Entity Information
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Action { get; set; }
    public DateTime ActionDate { get; set; }

    // Change Tracking
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangesJson { get; set; }
    public string? Changes { get; set; } // Alias for ChangesJson

    // Request Information
    public string? IpAddress { get; set; }
    public string? ClientIP { get; set; }
    public string? RequestId { get; set; }
    public string? UserAgent { get; set; }
    public string? QueryString { get; set; }
    public string? RequestHeaders { get; set; }

    // Performance
    public long? DurationMs { get; set; }
    public long? Duration { get; set; } // Alias for DurationMs

    // Status
    public AuditStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AuditSummaryDto
{
    // ============================================================
    // BASIC COUNTS
    // ============================================================

    /// <summary>
    /// Total number of audit logs
    /// </summary>
    public int TotalLogs { get; set; }

    /// <summary>
    /// Number of logs in the last 24 hours
    /// </summary>
    public int Last24hCount { get; set; }

    /// <summary>
    /// Number of logs in the last 7 days
    /// </summary>
    public int Last7DaysCount { get; set; }

    /// <summary>
    /// Number of logs in the last 30 days
    /// </summary>
    public int Last30DaysCount { get; set; }

    // ============================================================
    // STATUS COUNTS (ADD THESE MISSING PROPERTIES)
    // ============================================================

    /// <summary>
    /// Number of successful operations
    /// </summary>
    public int SuccessLogs { get; set; }

    /// <summary>
    /// Number of failed operations
    /// </summary>
    public int FailureLogs { get; set; }
     public int PendingLogs { get; set; }

    /// <summary>
    /// Number of warning operations
    /// </summary>
    public int WarningLogs { get; set; }

    // ============================================================
    // USER STATISTICS
    // ============================================================

    /// <summary>
    /// Number of unique users
    /// </summary>
    public int UniqueUsers { get; set; }

    /// <summary>
    /// Number of successful operations (alternative name)
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed operations (alternative name)
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// Number of pending operations
    /// </summary>
    public int PendingCount { get; set; }

    // ============================================================
    // PERFORMANCE METRICS
    // ============================================================

    /// <summary>
    /// Average response time in milliseconds
    /// </summary>
    public double AverageDurationMs { get; set; }

    /// <summary>
    /// Maximum response time in milliseconds
    /// </summary>
    public long? MaxDurationMs { get; set; }

    /// <summary>
    /// Minimum response time in milliseconds
    /// </summary>
    public long? MinDurationMs { get; set; }

    // ============================================================
    // GROUPED STATISTICS
    // ============================================================

    /// <summary>
    /// Top 10 most frequent actions
    /// </summary>
    public Dictionary<string, int> ActionsByType { get; set; } = new();

    /// <summary>
    /// Top 10 most frequent entities
    /// </summary>
    public Dictionary<string, int> ActionsByEntity { get; set; } = new();

    /// <summary>
    /// Top 10 most active users
    /// </summary>
    public Dictionary<string, int> ActionsByUser { get; set; } = new();

    /// <summary>
    /// Actions grouped by day for the last 7 days (trend)
    /// </summary>
    public Dictionary<DateTime, int> DailyTrend { get; set; } = new();

    // ============================================================
    // RECENT LOGS
    // ============================================================

    /// <summary>
    /// Recent audit logs (last 10)
    /// </summary>
    public List<AuditLogDto> RecentLogs { get; set; } = new();

    // ============================================================
    // ERROR STATISTICS
    // ============================================================

    /// <summary>
    /// Top 5 error messages (if any)
    /// </summary>
    public Dictionary<string, int> TopErrors { get; set; } = new();

    // ============================================================
    // TIME-BASED STATISTICS
    // ============================================================

    /// <summary>
    /// Most active hour of the day
    /// </summary>
    public int MostActiveHour { get; set; }

    /// <summary>
    /// Peak hour count
    /// </summary>
    public int PeakHourCount { get; set; }

    /// <summary>
    /// Total logs by HTTP method
    /// </summary>
    public Dictionary<string, int> ActionsByMethod { get; set; } = new();

    /// <summary>
    /// Status code distribution
    /// </summary>
    public Dictionary<int, int> StatusCodeDistribution { get; set; } = new();

    // ============================================================
    // ADDITIONAL METRICS (Optional)
    // ============================================================

    /// <summary>
    /// Total logs by user role
    /// </summary>
    public Dictionary<string, int> ActionsByUserRole { get; set; } = new();

    /// <summary>
    /// Average logs per day
    /// </summary>
    public double AverageLogsPerDay { get; set; }

    /// <summary>
    /// Success rate percentage
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Failure rate percentage
    /// </summary>
    public double FailureRate { get; set; }
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;  // String for API
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;  // String for API
    public string Action { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangesJson { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestId { get; set; }
    public int? DurationMs { get; set; }
    public string? Status { get; set; }  // String for API
    public string? ErrorMessage { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}






