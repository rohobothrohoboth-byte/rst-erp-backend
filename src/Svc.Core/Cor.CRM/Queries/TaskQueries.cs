// Cor.CRM/Queries/TaskQueries.cs
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class TaskByIdQry : IRequest<TaskDto?>
{
    public Guid Id { get; set; }
}

public class TaskAllQry : IRequest<List<TaskDto>>
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class TaskStatsQry : IRequest<TaskStatsResponse>
{
}

public class TaskStatsResponse
{
    public int Total { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
    public int Overdue { get; set; }
    public int OnHold { get; set; }
    public Dictionary<string, int> ByPriority { get; set; } = new();
}

// ============================================================
// TASK BY ID HANDLER
// ============================================================

public class TaskByIdHandler : IRequestHandler<TaskByIdQry, TaskDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TaskByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<TaskDto?> Handle(TaskByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    t.""Id"", t.""Title"", t.""Description"",
                    t.""Status"", t.""Priority"",
                    t.""DueDate"", t.""CompletedDate"", t.""StartedDate"",
                    t.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    t.""CustomerId"", c.""Name"" as CustomerName,
                    t.""OpportunityId"", o.""Name"" as OpportunityName,
                    t.""AssignedToUserId"", t.""AssignedToUserName"",
                    t.""IsRecurring"", t.""CompletionPercentage"",
                    t.""EstimatedHours"", t.""ActualHours"",
                    t.""CreatedAt"", t.""UpdatedAt"",
                    t.""CreatedByUserId"", t.""CreatedByUserName"",
                    t.""UpdatedByUserId"", t.""UpdatedByUserName""
                FROM ""Tasks"" t
                LEFT JOIN ""Leads"" l ON t.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON t.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON t.""OpportunityId"" = o.""Id""
                WHERE t.""Id"" = @Id AND t.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var task = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(sql, parameters, ct);
            return task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get task by ID: {TaskId}", request.Id);
            throw;
        }
    }
}

// ============================================================
// TASK ALL HANDLER
// ============================================================

public class TaskAllHandler : IRequestHandler<TaskAllQry, List<TaskDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TaskAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<TaskDto>> Handle(TaskAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    t.""Id"", t.""Title"", t.""Description"",
                    t.""Status"", t.""Priority"",
                    t.""DueDate"", t.""CompletedDate"", t.""StartedDate"",
                    t.""LeadId"", l.""FirstName"" || ' ' || l.""LastName"" as LeadName,
                    t.""CustomerId"", c.""Name"" as CustomerName,
                    t.""OpportunityId"", o.""Name"" as OpportunityName,
                    t.""AssignedToUserId"", t.""AssignedToUserName"",
                    t.""IsRecurring"", t.""CompletionPercentage"",
                    t.""EstimatedHours"", t.""ActualHours"",
                    t.""CreatedAt"", t.""UpdatedAt"",
                    t.""CreatedByUserId"", t.""CreatedByUserName"",
                    t.""UpdatedByUserId"", t.""UpdatedByUserName""
                FROM ""Tasks"" t
                LEFT JOIN ""Leads"" l ON t.""LeadId"" = l.""Id""
                LEFT JOIN ""Customers"" c ON t.""CustomerId"" = c.""Id""
                LEFT JOIN ""Opportunities"" o ON t.""OpportunityId"" = o.""Id""
                WHERE t.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND t.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<TaskState>(request.Status));
            }

            if (!string.IsNullOrEmpty(request.Priority))
            {
                sql += " AND t.\"Priority\" = @Priority";
                parameters.Add("@Priority", Enum.Parse<TaskPriority>(request.Priority));
            }

            if (request.LeadId.HasValue)
            {
                sql += " AND t.\"LeadId\" = @LeadId";
                parameters.Add("@LeadId", request.LeadId.Value);
            }

            if (request.CustomerId.HasValue)
            {
                sql += " AND t.\"CustomerId\" = @CustomerId";
                parameters.Add("@CustomerId", request.CustomerId.Value);
            }

            if (request.OpportunityId.HasValue)
            {
                sql += " AND t.\"OpportunityId\" = @OpportunityId";
                parameters.Add("@OpportunityId", request.OpportunityId.Value);
            }

            if (request.AssignedToUserId.HasValue)
            {
                sql += " AND t.\"AssignedToUserId\" = @AssignedToUserId";
                parameters.Add("@AssignedToUserId", request.AssignedToUserId.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND t.\"DueDate\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND t.\"DueDate\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY t.\"DueDate\" ASC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var tasks = await _dapper.QueryAsync<TaskDto>(sql, parameters, ct);
            return tasks.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all tasks");
            throw;
        }
    }
}

// ============================================================
// TASK STATS HANDLER
// ============================================================

public class TaskStatsHandler : IRequestHandler<TaskStatsQry, TaskStatsResponse>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public TaskStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<TaskStatsResponse> Handle(TaskStatsQry request, CancellationToken ct)
    {
        try
        {
            var result = new TaskStatsResponse();

            // Get total counts
            var countSql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Pending,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as InProgress,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Completed,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Cancelled,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Overdue,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as OnHold
                FROM ""Tasks""
                WHERE ""IsDeleted"" = false
            ";

            var counts = await _dapper.QueryFirstOrDefaultAsync<TaskStatsResponse>(countSql, null, ct);
            if (counts != null)
            {
                result.Total = counts.Total;
                result.Pending = counts.Pending;
                result.InProgress = counts.InProgress;
                result.Completed = counts.Completed;
                result.Cancelled = counts.Cancelled;
                result.Overdue = counts.Overdue;
                result.OnHold = counts.OnHold;
            }

            // Get counts by priority
            var byPrioritySql = @"
                SELECT
                    ""Priority"" as Key,
                    COUNT(*) as Value
                FROM ""Tasks""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Priority""
            ";

            var byPriority = await _dapper.QueryAsync<KeyValuePair<string, int>>(byPrioritySql, null, ct);
            result.ByPriority = byPriority.ToDictionary(x => x.Key, x => x.Value);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get task stats");
            throw;
        }
    }
}