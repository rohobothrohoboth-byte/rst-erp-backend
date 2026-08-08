// Cor.CRM/Queries/SMSCampaignQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class SMSCampaignAllQry : IRequest<List<SMSCampaignDto>>
{
    public string? Status { get; set; }
    public Guid? CampaignId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class SMSCampaignByIdQry : IRequest<SMSCampaignDto?>
{
    public Guid Id { get; set; }
}

public class SMSCampaignStatsQry : IRequest<SMSCampaignStatsDto>
{
    public Guid? CampaignId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class SMSCampaignAllHandler : IRequestHandler<SMSCampaignAllQry, List<SMSCampaignDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public SMSCampaignAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<SMSCampaignDto>> Handle(SMSCampaignAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    s.*
                FROM ""SMSCampaigns"" s
                WHERE s.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND s.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<SMSCampaignStatus>(request.Status));
            }

            if (request.CampaignId.HasValue)
            {
                sql += " AND s.\"CampaignId\" = @CampaignId";
                parameters.Add("@CampaignId", request.CampaignId.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND s.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND s.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortDescending ? "DESC" : "ASC";
            sql += $" ORDER BY s.\"{sortBy}\" {sortOrder}";

            var offset = (request.Page - 1) * request.PageSize;
            sql += $" OFFSET {offset} LIMIT {request.PageSize}";

            var campaigns = await _dapper.QueryAsync<SMSCampaignDto>(sql, parameters, ct);
            return campaigns.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get SMS campaigns");
            throw;
        }
    }
}

public class SMSCampaignByIdHandler : IRequestHandler<SMSCampaignByIdQry, SMSCampaignDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public SMSCampaignByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<SMSCampaignDto?> Handle(SMSCampaignByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    s.*
                FROM ""SMSCampaigns"" s
                WHERE s.""Id"" = @Id AND s.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            return await _dapper.QueryFirstOrDefaultAsync<SMSCampaignDto>(sql, parameters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get SMS campaign by ID: {CampaignId}", request.Id);
            throw;
        }
    }
}

public class SMSCampaignStatsHandler : IRequestHandler<SMSCampaignStatsQry, SMSCampaignStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public SMSCampaignStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<SMSCampaignStatsDto> Handle(SMSCampaignStatsQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    COUNT(*) as TotalCampaigns,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Sent,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Scheduled,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Draft,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Sending,
                    COUNT(CASE WHEN ""Status"" = 6 THEN 1 END) as Paused,
                    COALESCE(SUM(""SentCount""), 0) as TotalSent,
                    COALESCE(SUM(""DeliveredCount""), 0) as TotalDelivered,
                    COALESCE(SUM(""FailedCount""), 0) as TotalFailed
                FROM ""SMSCampaigns""
                WHERE ""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.CampaignId.HasValue)
            {
                sql += " AND \"CampaignId\" = @CampaignId";
                parameters.Add("@CampaignId", request.CampaignId.Value);
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

            var stats = await _dapper.QueryFirstOrDefaultAsync<SMSCampaignStatsDto>(sql, parameters, ct);
            return stats ?? new SMSCampaignStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get SMS campaign stats");
            throw;
        }
    }
}