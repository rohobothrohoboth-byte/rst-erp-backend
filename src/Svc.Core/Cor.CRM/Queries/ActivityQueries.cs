// Cor.CRM/Queries/ActivityQueries.cs
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class ActivityByIdQry : IRequest<ActivityDto?>
{
    public Guid Id { get; set; }
}

public class ActivityAllQry : IRequest<List<ActivityDto>>
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ActivityStatsQry : IRequest<ActivityStatsResponse>
{
}

public class ActivityStatsResponse
{
    public int Total { get; set; }
    public int Scheduled { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int Overdue { get; set; }
    public int Postponed { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
}

// ============================================================
// ACTIVITY BY ID HANDLER
// ============================================================

public class ActivityByIdHandler : IRequestHandler<ActivityByIdQry, ActivityDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ActivityByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ActivityDto?> Handle(ActivityByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    a.""Id"", a.""Title"", a.""Description"",
                    a.""Type"", a.""Status"",
                    a.""StartDateTime"", a.""EndDateTime"", a.""DurationMinutes"",
                    a.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    a.""CustomerId"", c.""Name"" as CustomerName,
                    a.""OpportunityId"", o.""Name"" as OpportunityName,
                    a.""AssignedToUserId"", a.""AssignedToUserName"",
                    a.""Location"", a.""IsAllDay"", a.""Outcome"", a.""CompletedAt"",
                    a.""CreatedAt"", a.""UpdatedAt"",
                    a.""CreatedByUserId"", a.""CreatedByUserName"",
                    a.""UpdatedByUserId"", a.""UpdatedByUserName""
                FROM ""Activities"" a
                LEFT JOIN ""Leads"" l ON a.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON a.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON a.""OpportunityId"" = o.""Id""
                WHERE a.""Id"" = @Id AND a.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var activity = await _dapper.QueryFirstOrDefaultAsync<ActivityDto>(sql, parameters, ct);

            return activity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get activity by ID: {ActivityId}", request.Id);
            throw;
        }
    }
}

// ============================================================
// ACTIVITY ALL HANDLER
// ============================================================

public class ActivityAllHandler : IRequestHandler<ActivityAllQry, List<ActivityDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ActivityAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<ActivityDto>> Handle(ActivityAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    a.""Id"", a.""Title"", a.""Description"",
                    a.""Type"", a.""Status"",
                    a.""StartDateTime"", a.""EndDateTime"", a.""DurationMinutes"",
                    a.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    a.""CustomerId"", c.""Name"" as CustomerName,
                    a.""OpportunityId"", o.""Name"" as OpportunityName,
                    a.""AssignedToUserId"", a.""AssignedToUserName"",
                    a.""Location"", a.""IsAllDay"", a.""Outcome"", a.""CompletedAt"",
                    a.""CreatedAt"", a.""UpdatedAt"",
                    a.""CreatedByUserId"", a.""CreatedByUserName"",
                    a.""UpdatedByUserId"", a.""UpdatedByUserName""
                FROM ""Activities"" a
                LEFT JOIN ""Leads"" l ON a.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON a.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON a.""OpportunityId"" = o.""Id""
                WHERE a.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Type))
            {
                sql += " AND a.\"Type\" = @Type";
                parameters.Add("@Type", Enum.Parse<ActivityType>(request.Type));
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND a.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<ActivityStatus>(request.Status));
            }

            if (request.LeadId.HasValue)
            {
                sql += " AND a.\"LeadId\" = @LeadId";
                parameters.Add("@LeadId", request.LeadId.Value);
            }

            if (request.CustomerId.HasValue)
            {
                sql += " AND a.\"CustomerId\" = @CustomerId";
                parameters.Add("@CustomerId", request.CustomerId.Value);
            }

            if (request.OpportunityId.HasValue)
            {
                sql += " AND a.\"OpportunityId\" = @OpportunityId";
                parameters.Add("@OpportunityId", request.OpportunityId.Value);
            }

            if (request.AssignedToUserId.HasValue)
            {
                sql += " AND a.\"AssignedToUserId\" = @AssignedToUserId";
                parameters.Add("@AssignedToUserId", request.AssignedToUserId.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND a.\"StartDateTime\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND a.\"StartDateTime\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY a.\"StartDateTime\" DESC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var activities = await _dapper.QueryAsync<ActivityDto>(sql, parameters, ct);
            return activities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all activities");
            throw;
        }
    }
}

// ============================================================
// ACTIVITY STATS HANDLER
// ============================================================

public class ActivityStatsHandler : IRequestHandler<ActivityStatsQry, ActivityStatsResponse>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ActivityStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ActivityStatsResponse> Handle(ActivityStatsQry request, CancellationToken ct)
    {
        try
        {
            var result = new ActivityStatsResponse();

            // Get total counts
            var countSql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Scheduled,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as InProgress,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Completed,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Cancelled,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Overdue,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Postponed
                FROM ""Activities""
                WHERE ""IsDeleted"" = false
            ";

            var counts = await _dapper.QueryFirstOrDefaultAsync<ActivityStatsResponse>(countSql, null, ct);
            if (counts != null)
            {
                result.Total = counts.Total;
                result.Scheduled = counts.Scheduled;
                result.InProgress = counts.InProgress;
                result.Completed = counts.Completed;
                result.Cancelled = counts.Cancelled;
                result.Overdue = counts.Overdue;
                result.Postponed = counts.Postponed;
            }

            // Get counts by type
            var byTypeSql = @"
                SELECT
                    ""Type"" as Key,
                    COUNT(*) as Value
                FROM ""Activities""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Type""
            ";

            var byType = await _dapper.QueryAsync<KeyValuePair<string, int>>(byTypeSql, null, ct);
            result.ByType = byType.ToDictionary(x => x.Key, x => x.Value);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get activity stats");
            throw;
        }
    }
}