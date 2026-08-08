// Cor.CRM/Queries/CommissionQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

// ============================================================
// QUERIES
// ============================================================

public class CommissionByIdQry : IRequest<CommissionDto?>
{
    public Guid Id { get; set; }
}

public class CommissionAllQry : IRequest<List<CommissionDto>>
{
    public Guid? AgentId { get; set; }
    public Guid? TransactionId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// ✅ ADD THIS MISSING QUERY
public class CommissionByAgentQry : IRequest<List<CommissionDto>>
{
    public Guid AgentId { get; set; }
    public int? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CommissionStatsQry : IRequest<CommissionStatsDto>
{
    public Guid? AgentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class CommissionByIdHandler : IRequestHandler<CommissionByIdQry, CommissionDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CommissionByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<CommissionDto?> Handle(CommissionByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.*,
                    t.""TransactionNumber"",
                    e.""FirstName"" || ' ' || e.""LastName"" as AgentName
                FROM ""Commissions"" c
                LEFT JOIN ""RealEstateTransactions"" t ON c.""TransactionId"" = t.""Id""
                LEFT JOIN ""LocalEmployees"" e ON c.""AgentId"" = e.""AppUserId""
                WHERE c.""Id"" = @Id AND c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            return await _dapper.QueryFirstOrDefaultAsync<CommissionDto>(sql, parameters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get commission by ID: {CommissionId}", request.Id);
            throw;
        }
    }
}

public class CommissionAllHandler : IRequestHandler<CommissionAllQry, List<CommissionDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CommissionAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<CommissionDto>> Handle(CommissionAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.*,
                    t.""TransactionNumber"",
                    e.""FirstName"" || ' ' || e.""LastName"" as AgentName
                FROM ""Commissions"" c
                LEFT JOIN ""RealEstateTransactions"" t ON c.""TransactionId"" = t.""Id""
                LEFT JOIN ""LocalEmployees"" e ON c.""AgentId"" = e.""AppUserId""
                WHERE c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.AgentId.HasValue)
            {
                sql += " AND c.\"AgentId\" = @AgentId";
                parameters.Add("@AgentId", request.AgentId.Value);
            }

            if (request.TransactionId.HasValue)
            {
                sql += " AND c.\"TransactionId\" = @TransactionId";
                parameters.Add("@TransactionId", request.TransactionId.Value);
            }

            if (request.Status.HasValue)
            {
                sql += " AND c.\"Status\" = @Status";
                parameters.Add("@Status", request.Status.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND c.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND c.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY c.\"CreatedAt\" DESC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var commissions = await _dapper.QueryAsync<CommissionDto>(sql, parameters, ct);
            return commissions.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all commissions");
            throw;
        }
    }
}

// ✅ ADD THIS MISSING HANDLER
public class CommissionByAgentHandler : IRequestHandler<CommissionByAgentQry, List<CommissionDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CommissionByAgentHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<CommissionDto>> Handle(CommissionByAgentQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.*,
                    t.""TransactionNumber"",
                    e.""FirstName"" || ' ' || e.""LastName"" as AgentName
                FROM ""Commissions"" c
                LEFT JOIN ""RealEstateTransactions"" t ON c.""TransactionId"" = t.""Id""
                LEFT JOIN ""LocalEmployees"" e ON c.""AgentId"" = e.""AppUserId""
                WHERE c.""IsDeleted"" = false
                    AND c.""AgentId"" = @AgentId
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@AgentId", request.AgentId);

            if (request.Status.HasValue)
            {
                sql += " AND c.\"Status\" = @Status";
                parameters.Add("@Status", request.Status.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND c.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND c.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            sql += " ORDER BY c.\"CreatedAt\" DESC";
            sql += $" OFFSET {(request.Page - 1) * request.PageSize} LIMIT {request.PageSize}";

            var commissions = await _dapper.QueryAsync<CommissionDto>(sql, parameters, ct);
            return commissions.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get commissions for agent: {AgentId}", request.AgentId);
            throw;
        }
    }
}

// ✅ ADD STATS HANDLER IF NEEDED
public class CommissionStatsHandler : IRequestHandler<CommissionStatsQry, CommissionStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CommissionStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<CommissionStatsDto> Handle(CommissionStatsQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    COUNT(*) as TotalCommissions,
                    COALESCE(SUM(CASE WHEN ""Status"" = 1 THEN ""Amount"" ELSE 0 END), 0) as TotalEarned,
                    COALESCE(SUM(CASE WHEN ""Status"" = 2 THEN ""Amount"" ELSE 0 END), 0) as TotalPending,
                    COALESCE(SUM(CASE WHEN ""Status"" = 3 THEN ""Amount"" ELSE 0 END), 0) as TotalPaid,
                    COALESCE(AVG(""Amount""), 0) as AverageCommission
                FROM ""Commissions""
                WHERE ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.AgentId.HasValue)
            {
                sql += " AND \"AgentId\" = @AgentId";
                parameters.Add("@AgentId", request.AgentId.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND \"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND \"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            var stats = await _dapper.QueryFirstOrDefaultAsync<CommissionStatsDto>(sql, parameters, ct);
            return stats ?? new CommissionStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get commission stats");
            throw;
        }
    }
}