using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;
using System.Data;
using System.Text;

namespace Cor.CRM.Queries;

// ============================================================
// QUERIES
// ============================================================

public class InteractionAllQry : IRequest<List<InteractionDto>>
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class InteractionByIdQry : IRequest<InteractionDto?>
{
    public Guid Id { get; set; }
}

public class InteractionStatsQry : IRequest<InteractionStatsDto> { }

// ============================================================
// HANDLERS
// ============================================================

public class InteractionAllHandler : IRequestHandler<InteractionAllQry, List<InteractionDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InteractionAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<InteractionDto>> Handle(InteractionAllQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("=== STARTING InteractionAllHandler ===");
            _logger.LogInformation($"Request parameters: Type={request.Type}, Status={request.Status}, Priority={request.Priority}");
            _logger.LogInformation($"LeadId={request.LeadId}, CustomerId={request.CustomerId}, ContactId={request.ContactId}");
            _logger.LogInformation($"OpportunityId={request.OpportunityId}, AssignedToUserId={request.AssignedToUserId}");
            _logger.LogInformation($"FromDate={request.FromDate}, ToDate={request.ToDate}");
            _logger.LogInformation($"Page={request.Page}, PageSize={request.PageSize}, SortBy={request.SortBy}, SortOrder={request.SortOrder}");

            // Let's use a simpler query first to test if the issue is with the complex joins
            var sqlBuilder = new StringBuilder(@"
                SELECT
                    i.""Id"", i.""Subject"", i.""Description"", i.""Type"", i.""Status"",
                    i.""Priority"", i.""LeadId"", i.""CustomerId"", i.""ContactId"",
                    i.""OpportunityId"", i.""AssignedToUserId"",
                    i.""ScheduledDate"", i.""CompletedDate"", i.""Duration"",
                    i.""Outcome"", i.""Location"", i.""IsAllDay"",
                    i.""CreatedAt"", i.""UpdatedAt""
                FROM ""Interactions"" i
                WHERE i.""IsDeleted"" = false
            ");

            var parameters = new DynamicParameters();

            // String parameters
            if (!string.IsNullOrEmpty(request.Type))
            {
                sqlBuilder.Append(" AND i.\"Type\" = @Type");
                parameters.Add("@Type", request.Type);
                _logger.LogInformation($"Added Type filter: {request.Type}");
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sqlBuilder.Append(" AND i.\"Status\" = @Status");
                parameters.Add("@Status", request.Status);
                _logger.LogInformation($"Added Status filter: {request.Status}");
            }

            if (!string.IsNullOrEmpty(request.Priority))
            {
                sqlBuilder.Append(" AND i.\"Priority\" = @Priority");
                parameters.Add("@Priority", request.Priority);
                _logger.LogInformation($"Added Priority filter: {request.Priority}");
            }

            // GUID parameters
            if (request.LeadId.HasValue)
            {
                sqlBuilder.Append(" AND i.\"LeadId\" = @LeadId");
                parameters.Add("@LeadId", request.LeadId.Value, DbType.Guid);
                _logger.LogInformation($"Added LeadId filter: {request.LeadId.Value}");
            }

            if (request.CustomerId.HasValue)
            {
                sqlBuilder.Append(" AND i.\"CustomerId\" = @CustomerId");
                parameters.Add("@CustomerId", request.CustomerId.Value, DbType.Guid);
                _logger.LogInformation($"Added CustomerId filter: {request.CustomerId.Value}");
            }

            if (request.ContactId.HasValue)
            {
                sqlBuilder.Append(" AND i.\"ContactId\" = @ContactId");
                parameters.Add("@ContactId", request.ContactId.Value, DbType.Guid);
                _logger.LogInformation($"Added ContactId filter: {request.ContactId.Value}");
            }

            if (request.OpportunityId.HasValue)
            {
                sqlBuilder.Append(" AND i.\"OpportunityId\" = @OpportunityId");
                parameters.Add("@OpportunityId", request.OpportunityId.Value, DbType.Guid);
                _logger.LogInformation($"Added OpportunityId filter: {request.OpportunityId.Value}");
            }

            if (request.AssignedToUserId.HasValue)
            {
                sqlBuilder.Append(" AND i.\"AssignedToUserId\" = @AssignedToUserId");
                parameters.Add("@AssignedToUserId", request.AssignedToUserId.Value, DbType.Guid);
                _logger.LogInformation($"Added AssignedToUserId filter: {request.AssignedToUserId.Value}");
            }

            // Date parameters
            if (request.FromDate.HasValue)
            {
                sqlBuilder.Append(" AND i.\"CreatedAt\" >= @FromDate");
                parameters.Add("@FromDate", request.FromDate.Value);
                _logger.LogInformation($"Added FromDate filter: {request.FromDate.Value}");
            }

            if (request.ToDate.HasValue)
            {
                sqlBuilder.Append(" AND i.\"CreatedAt\" <= @ToDate");
                parameters.Add("@ToDate", request.ToDate.Value);
                _logger.LogInformation($"Added ToDate filter: {request.ToDate.Value}");
            }

            // Sorting and pagination
            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortOrder?.ToUpper() == "ASC" ? "ASC" : "DESC";
            sqlBuilder.Append($" ORDER BY i.\"{sortBy}\" {sortOrder}");
            _logger.LogInformation($"Sorting: {sortBy} {sortOrder}");

            if (request.Page.HasValue && request.PageSize.HasValue)
            {
                var offset = (request.Page.Value - 1) * request.PageSize.Value;
                sqlBuilder.Append($" OFFSET {offset} LIMIT {request.PageSize.Value}");
                _logger.LogInformation($"Pagination: Offset={offset}, Limit={request.PageSize.Value}");
            }

            var finalSql = sqlBuilder.ToString();

            // Log the final SQL and parameters
            _logger.LogInformation("=== FINAL SQL QUERY ===");
            _logger.LogInformation(finalSql);

            _logger.LogInformation("=== PARAMETERS ===");
            foreach (var paramName in parameters.ParameterNames)
            {
                var value = parameters.Get<object>(paramName);
                var type = value?.GetType().Name ?? "null";
                _logger.LogInformation($"  {paramName} = {value} (Type: {type})");
            }

            // Try to execute the query
            var data = await _dapper.QueryAsync<InteractionDto>(finalSql, parameters, ct);

            _logger.LogInformation($"=== QUERY RESULTS ===");
            _logger.LogInformation($"Returned {data.Count()} interactions");

            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all interactions");
            _logger.LogError(ex, $"Exception Type: {ex.GetType().FullName}");
            _logger.LogError(ex, $"Exception Message: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, $"Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}

public class InteractionByIdHandler : IRequestHandler<InteractionByIdQry, InteractionDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InteractionByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<InteractionDto?> Handle(InteractionByIdQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation($"=== STARTING InteractionByIdHandler with Id: {request.Id} ===");

            var sql = @"
                SELECT
                    i.""Id"", i.""Subject"", i.""Description"", i.""Type"", i.""Status"",
                    i.""Priority"", i.""LeadId"", i.""CustomerId"", i.""ContactId"",
                    i.""OpportunityId"", i.""AssignedToUserId"",
                    i.""ScheduledDate"", i.""CompletedDate"", i.""Duration"",
                    i.""Outcome"", i.""Location"", i.""IsAllDay"",
                    i.""CreatedAt"", i.""UpdatedAt""
                FROM ""Interactions"" i
                WHERE i.""Id"" = @Id AND i.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id, DbType.Guid);

            _logger.LogInformation($"SQL Query: {sql}");
            _logger.LogInformation($"Parameter @Id = {request.Id} (Type: Guid)");

            var data = await _dapper.QueryFirstOrDefaultAsync<InteractionDto>(sql, parameters, ct);

            if (data != null)
            {
                _logger.LogInformation($"Found interaction with Id: {data.Id}");
            }
            else
            {
                _logger.LogInformation($"No interaction found with Id: {request.Id}");
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get interaction by ID: {request.Id}");
            _logger.LogError(ex, $"Exception Type: {ex.GetType().FullName}");
            _logger.LogError(ex, $"Exception Message: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, $"Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}

public class InteractionStatsHandler : IRequestHandler<InteractionStatsQry, InteractionStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public InteractionStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<InteractionStatsDto> Handle(InteractionStatsQry request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("=== STARTING InteractionStatsHandler ===");
            var stats = new InteractionStatsDto();

            var countSql = @"
                SELECT
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Completed,
                    COUNT(CASE WHEN ""Status"" IS NOT NULL AND ""Status"" != 3 AND ""Status"" != 4 THEN 1 END) as Pending,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Cancelled
                FROM ""Interactions""
                WHERE ""IsDeleted"" = false
            ";

            _logger.LogInformation($"Count SQL: {countSql}");

            var counts = await _dapper.QueryFirstOrDefaultAsync<dynamic>(countSql, new { }, ct);
            if (counts != null)
            {
                stats.Total = counts.Total ?? 0;
                stats.Completed = counts.Completed ?? 0;
                stats.Pending = counts.Pending ?? 0;
                stats.Cancelled = counts.Cancelled ?? 0;
                _logger.LogInformation($"Stats: Total={stats.Total}, Completed={stats.Completed}, Pending={stats.Pending}, Cancelled={stats.Cancelled}");
            }

            var statusSql = @"
                SELECT
                    CAST(""Status"" AS TEXT) as Key,
                    COUNT(*) as Value
                FROM ""Interactions""
                WHERE ""IsDeleted"" = false AND ""Status"" IS NOT NULL
                GROUP BY ""Status""
            ";

            var statusData = await _dapper.QueryAsync<KeyValuePair<string, int>>(statusSql, new { }, ct);
            stats.ByStatus = statusData.ToDictionary(x => x.Key, x => x.Value);
            _logger.LogInformation($"ByStatus: {string.Join(", ", stats.ByStatus.Select(kv => $"{kv.Key}={kv.Value}"))}");

            var typeSql = @"
                SELECT
                    CAST(""Type"" AS TEXT) as Key,
                    COUNT(*) as Value
                FROM ""Interactions""
                WHERE ""IsDeleted"" = false AND ""Type"" IS NOT NULL
                GROUP BY ""Type""
            ";

            var typeData = await _dapper.QueryAsync<KeyValuePair<string, int>>(typeSql, new { }, ct);
            stats.ByType = typeData.ToDictionary(x => x.Key, x => x.Value);
            _logger.LogInformation($"ByType: {string.Join(", ", stats.ByType.Select(kv => $"{kv.Key}={kv.Value}"))}");

            var prioritySql = @"
                SELECT
                    CAST(""Priority"" AS TEXT) as Key,
                    COUNT(*) as Value
                FROM ""Interactions""
                WHERE ""IsDeleted"" = false AND ""Priority"" IS NOT NULL
                GROUP BY ""Priority""
            ";

            var priorityData = await _dapper.QueryAsync<KeyValuePair<string, int>>(prioritySql, new { }, ct);
            stats.ByPriority = priorityData.ToDictionary(x => x.Key, x => x.Value);
            _logger.LogInformation($"ByPriority: {string.Join(", ", stats.ByPriority.Select(kv => $"{kv.Key}={kv.Value}"))}");

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get interaction stats");
            _logger.LogError(ex, $"Exception Type: {ex.GetType().FullName}");
            _logger.LogError(ex, $"Exception Message: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogError(ex.InnerException, $"Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}