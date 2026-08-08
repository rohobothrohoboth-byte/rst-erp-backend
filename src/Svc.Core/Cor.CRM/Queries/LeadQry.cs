using Common;
using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class LeadAllQry : IRequest<List<LeadDto>>
{
    public LeadFilterDto? Filter { get; set; }
}

public class LeadByIdQry : IRequest<LeadDto?>
{
    public Guid Id { get; set; }
}

public class LeadStatsQry : IRequest<LeadStatsDto> { }

public class LeadByStatusQry : IRequest<List<LeadDto>>
{
    public string Status { get; set; } = string.Empty;
}

public class LeadByAssignedUserQry : IRequest<List<LeadDto>>
{
    public Guid UserId { get; set; }
}

public class LeadForRoutingQry : IRequest<List<LeadDto>> { }

public class LeadCountQry : IRequest<int>
{
    public string? Status { get; set; }
}

// ==================== GET ALL LEADS ====================
public class LeadAllHandler : IRequestHandler<LeadAllQry, List<LeadDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly IUserClient _userClient;
    private readonly ILogService _logger;

    public LeadAllHandler(IDapperHelper dapper, IUserClient userClient, ILogService logger)
    {
        _dapper = dapper;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<List<LeadDto>> Handle(LeadAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"", ""FirstName"", ""LastName"", ""CompanyName"", ""Email"",
                    ""Phone"", ""Mobile"", ""Address"", ""City"", ""State"", ""Country"",
                    ""Status"", ""Source"", ""Priority"", ""Industry"", ""Title"", ""Description"",
                    ""Budget"", ""EstimatedValue"", ""ExpectedCloseDate"", ""AssignedToUserId"",
                    ""IsConverted"", ""ConvertedDate"", ""Score"", ""EngagementScore"", ""Tags"",
                    ""CreatedAt"", ""UpdatedAt"", ""LastContactDate"", ""ContactCount"",
                    ""PropertyType"", ""PropertyPrice"", ""PropertyLocation"", ""PropertySize"",
                    ""ProductCategory"", ""OrderQuantity"", ""RequiredDeliveryDate"",
                    ""TenderNumber"", ""TenderDeadline"", ""Department""
                FROM ""Leads""
                WHERE ""IsDeleted"" = false";

            var parameters = new DynamicParameters();

            // Apply filters
            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.SearchTerm))
                {
                    sql += @" AND (
                        LOWER(""FirstName"") LIKE @Search OR
                        LOWER(""LastName"") LIKE @Search OR
                        LOWER(""Email"") LIKE @Search OR
                        LOWER(""CompanyName"") LIKE @Search)";
                    parameters.Add("@Search", $"%{request.Filter.SearchTerm.ToLower()}%");
                }

                if (!string.IsNullOrEmpty(request.Filter.Status))
                {
                    var status = (int)Enum.Parse<LeadStatus>(request.Filter.Status);
                    sql += @" AND ""Status"" = @Status";
                    parameters.Add("@Status", status);
                }

                if (!string.IsNullOrEmpty(request.Filter.Source))
                {
                    var source = (int)Enum.Parse<LeadSource>(request.Filter.Source);
                    sql += @" AND ""Source"" = @Source";
                    parameters.Add("@Source", source);
                }

                if (!string.IsNullOrEmpty(request.Filter.Priority))
                {
                    var priority = (int)Enum.Parse<LeadPriority>(request.Filter.Priority);
                    sql += @" AND ""Priority"" = @Priority";
                    parameters.Add("@Priority", priority);
                }

                if (!string.IsNullOrEmpty(request.Filter.Industry))
                {
                    var industry = (int)Enum.Parse<Industry>(request.Filter.Industry);
                    sql += @" AND ""Industry"" = @Industry";
                    parameters.Add("@Industry", industry);
                }

                if (request.Filter.AssignedToUserId.HasValue)
                {
                    sql += @" AND ""AssignedToUserId"" = @AssignedToUserId";
                    parameters.Add("@AssignedToUserId", request.Filter.AssignedToUserId.Value);
                }

                if (request.Filter.FromDate.HasValue)
                {
                    sql += @" AND ""CreatedAt"" >= @FromDate";
                    parameters.Add("@FromDate", request.Filter.FromDate.Value);
                }

                if (request.Filter.ToDate.HasValue)
                {
                    sql += @" AND ""CreatedAt"" <= @ToDate";
                    parameters.Add("@ToDate", request.Filter.ToDate.Value);
                }

                if (request.Filter.IsConverted.HasValue)
                {
                    sql += @" AND ""IsConverted"" = @IsConverted";
                    parameters.Add("@IsConverted", request.Filter.IsConverted.Value);
                }

                if (request.Filter.MinScore.HasValue)
                {
                    sql += @" AND ""Score"" >= @MinScore";
                    parameters.Add("@MinScore", request.Filter.MinScore.Value);
                }

                if (request.Filter.MaxScore.HasValue)
                {
                    sql += @" AND ""Score"" <= @MaxScore";
                    parameters.Add("@MaxScore", request.Filter.MaxScore.Value);
                }
            }

            // Sorting
            var sortBy = request.Filter?.SortBy?.ToLower() ?? "createdat";
            var desc = request.Filter?.SortDescending ?? true;

            var orderByField = sortBy switch
            {
                "firstname" => "FirstName",
                "lastname" => "LastName",
                "email" => "Email",
                "score" => "Score",
                "estimatedvalue" => "EstimatedValue",
                _ => "CreatedAt"
            };

            sql += $" ORDER BY \"{orderByField}\" {(desc ? "DESC" : "ASC")}";

            // Pagination
            var page = request.Filter?.Page ?? 1;
            var pageSize = request.Filter?.PageSize ?? 20;
            sql += $" LIMIT {pageSize} OFFSET {(page - 1) * pageSize}";

            var data = await _dapper.QueryAsync<LeadDto>(sql, parameters, ct);

            var leadList = data.ToList();
            foreach (var lead in leadList)
            {
                lead.FullName = $"{lead.FirstName} {lead.LastName}";

                if (lead.AssignedToUserId.HasValue)
                {
                    try
                    {
                        var user = await _userClient.GetUser(lead.AssignedToUserId.Value.ToString(), ct);
                        lead.AssignedToUserName = user?.Res?.Name ?? "";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to get user name for {UserId}: {Error}",
                            lead.AssignedToUserId, ex.Message);
                        lead.AssignedToUserName = "";
                    }
                }
            }

            return leadList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all leads");
            throw;
        }
    }
}

// ==================== GET LEAD BY ID ====================
public class LeadByIdHandler : IRequestHandler<LeadByIdQry, LeadDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly IUserClient _userClient;
    private readonly ILogService _logger;

    public LeadByIdHandler(IDapperHelper dapper, IUserClient userClient, ILogService logger)
    {
        _dapper = dapper;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<LeadDto?> Handle(LeadByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"", ""FirstName"", ""LastName"", ""CompanyName"", ""Email"",
                    ""Phone"", ""Mobile"", ""Address"", ""City"", ""State"", ""Country"",
                    ""Status"", ""Source"", ""Priority"", ""Industry"", ""Title"", ""Description"",
                    ""Budget"", ""EstimatedValue"", ""ExpectedCloseDate"", ""AssignedToUserId"",
                    ""IsConverted"", ""ConvertedDate"", ""Score"", ""EngagementScore"", ""Tags"",
                    ""CreatedAt"", ""UpdatedAt"", ""LastContactDate"", ""ContactCount"",
                    ""PropertyType"", ""PropertyPrice"", ""PropertyLocation"", ""PropertySize"",
                    ""ProductCategory"", ""OrderQuantity"", ""RequiredDeliveryDate"",
                    ""TenderNumber"", ""TenderDeadline"", ""Department""
                FROM ""Leads""
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<LeadDto>(sql, parameters, ct);

            if (data == null) return null;

            data.FullName = $"{data.FirstName} {data.LastName}";

            if (data.AssignedToUserId.HasValue)
            {
                try
                {
                    var user = await _userClient.GetUser(data.AssignedToUserId.Value.ToString(), ct);
                    data.AssignedToUserName = user?.Res?.Name ?? "";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to get user name for {UserId}: {Error}",
                        data.AssignedToUserId, ex.Message);
                    data.AssignedToUserName = "";
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get lead by ID: {LeadId}", request.Id);
            throw;
        }
    }
}

// ==================== GET LEAD STATISTICS ====================
public class LeadStatsHandler : IRequestHandler<LeadStatsQry, LeadStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public LeadStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<LeadStatsDto> Handle(LeadStatsQry request, CancellationToken ct)
    {
        try
        {
            const string sql = @"
                SELECT
                    COUNT(*) as TotalLeads,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as NewLeads,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as ContactedLeads,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as QualifiedLeads,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as ConvertedLeads,
                    COUNT(CASE WHEN ""Status"" = 7 THEN 1 END) as LostLeads,
                    COALESCE(AVG(""Score""), 0) as AverageLeadScore
                FROM ""Leads""
                WHERE ""IsDeleted"" = false";

            var stats = await _dapper.QueryFirstOrDefaultAsync<LeadStatsDto>(sql, new { }, ct);
            stats ??= new LeadStatsDto();

            // Get source breakdown
            const string sourceSql = @"
                SELECT ""Source"" as Key, COUNT(*) as Value
                FROM ""Leads""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Source""";

            var sources = await _dapper.QueryAsync<KeyValuePair<string, int>>(sourceSql, new { }, ct);
            stats.LeadsBySource = sources.ToDictionary(x => x.Key.ToString(), x => x.Value);

            // Get industry breakdown
            const string industrySql = @"
                SELECT ""Industry"" as Key, COUNT(*) as Value
                FROM ""Leads""
                WHERE ""IsDeleted"" = false AND ""Industry"" IS NOT NULL
                GROUP BY ""Industry""";

            var industries = await _dapper.QueryAsync<KeyValuePair<string, int>>(industrySql, new { }, ct);
            stats.LeadsByIndustry = industries.ToDictionary(x => x.Key.ToString(), x => x.Value);

            // Get priority breakdown
            const string prioritySql = @"
                SELECT ""Priority"" as Key, COUNT(*) as Value
                FROM ""Leads""
                WHERE ""IsDeleted"" = false
                GROUP BY ""Priority""";

            var priorities = await _dapper.QueryAsync<KeyValuePair<string, int>>(prioritySql, new { }, ct);
            stats.LeadsByPriority = priorities.ToDictionary(x => x.Key.ToString(), x => x.Value);

            stats.ConversionRate = stats.TotalLeads > 0
                ? (decimal)stats.ConvertedLeads / stats.TotalLeads * 100
                : 0;

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get lead statistics");
            throw;
        }
    }
}

// ==================== GET LEADS BY STATUS ====================
public class LeadByStatusHandler : IRequestHandler<LeadByStatusQry, List<LeadDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public LeadByStatusHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<LeadDto>> Handle(LeadByStatusQry request, CancellationToken ct)
    {
        try
        {
            var status = (int)Enum.Parse<LeadStatus>(request.Status);

            var sql = @"
                SELECT
                    ""Id"", ""FirstName"", ""LastName"", ""CompanyName"", ""Email"",
                    ""Status"", ""Score"", ""CreatedAt""
                FROM ""Leads""
                WHERE ""Status"" = @Status AND ""IsDeleted"" = false
                ORDER BY ""CreatedAt"" DESC";

            var parameters = new DynamicParameters();
            parameters.Add("@Status", status);

            var data = await _dapper.QueryAsync<LeadDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get leads by status: {Status}", request.Status);
            throw;
        }
    }
}

// ==================== GET LEADS BY ASSIGNED USER ====================
public class LeadByAssignedUserHandler : IRequestHandler<LeadByAssignedUserQry, List<LeadDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public LeadByAssignedUserHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<LeadDto>> Handle(LeadByAssignedUserQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"", ""FirstName"", ""LastName"", ""CompanyName"", ""Email"",
                    ""Status"", ""Priority"", ""Score"", ""CreatedAt""
                FROM ""Leads""
                WHERE ""AssignedToUserId"" = @UserId AND ""IsDeleted"" = false
                ORDER BY ""Priority"" DESC, ""Score"" DESC";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", request.UserId);

            var data = await _dapper.QueryAsync<LeadDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get leads by assigned user: {UserId}", request.UserId);
            throw;
        }
    }
}

// ==================== GET LEADS FOR ROUTING ====================
public class LeadForRoutingHandler : IRequestHandler<LeadForRoutingQry, List<LeadDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public LeadForRoutingHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<LeadDto>> Handle(LeadForRoutingQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    ""Id"", ""FirstName"", ""LastName"", ""CompanyName"", ""Email"",
                    ""Status"", ""Priority"", ""Score"", ""CreatedAt""
                FROM ""Leads""
                WHERE ""AssignedToUserId"" IS NULL
                    AND ""IsDeleted"" = false
                    AND (""Priority"" = 3 OR ""Priority"" = 4)  -- High or Urgent
                ORDER BY ""Priority"" DESC, ""Score"" DESC
                LIMIT 50";

            var data = await _dapper.QueryAsync<LeadDto>(sql, new { }, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get leads for routing");
            throw;
        }
    }
}

// ==================== GET LEAD COUNT ====================
public class LeadCountHandler : IRequestHandler<LeadCountQry, int>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public LeadCountHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<int> Handle(LeadCountQry request, CancellationToken ct)
    {
        try
        {
            var sql = "SELECT COUNT(*) FROM \"Leads\" WHERE \"IsDeleted\" = false";

            if (!string.IsNullOrEmpty(request.Status))
            {
                var status = (int)Enum.Parse<LeadStatus>(request.Status);
                sql += $" AND \"Status\" = {status}";
            }

            var count = await _dapper.ExecuteScalarAsync<int>(sql, new { }, ct);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get lead count");
            throw;
        }
    }
}