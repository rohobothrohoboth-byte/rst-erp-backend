// Cor.CRM/Queries/EmailCampaignQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class EmailCampaignAllQry : IRequest<List<EmailCampaignDto>>
{
    public string? Status { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid? TemplateId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class EmailCampaignByIdQry : IRequest<EmailCampaignDto?>
{
    public Guid Id { get; set; }
}

public class EmailCampaignStatsQry : IRequest<EmailCampaignStatsDto>
{
    public Guid? CampaignId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class EmailCampaignAllHandler : IRequestHandler<EmailCampaignAllQry, List<EmailCampaignDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public EmailCampaignAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<EmailCampaignDto>> Handle(EmailCampaignAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    e.*
                FROM ""EmailCampaigns"" e
                WHERE e.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND e.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<EmailCampaignStatus>(request.Status));
            }

            if (request.CampaignId.HasValue)
            {
                sql += " AND e.\"CampaignId\" = @CampaignId";
                parameters.Add("@CampaignId", request.CampaignId.Value);
            }

            if (request.TemplateId.HasValue)
            {
                sql += " AND e.\"TemplateId\" = @TemplateId";
                parameters.Add("@TemplateId", request.TemplateId.Value);
            }

            if (request.FromDate.HasValue)
            {
                sql += " AND e.\"CreatedAt\" >= @FromDate";
                parameters.Add("@FromDate", request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                sql += " AND e.\"CreatedAt\" <= @ToDate";
                parameters.Add("@ToDate", request.ToDate.Value);
            }

            var sortBy = request.SortBy ?? "CreatedAt";
            var sortOrder = request.SortDescending ? "DESC" : "ASC";
            sql += $" ORDER BY e.\"{sortBy}\" {sortOrder}";

            var offset = (request.Page - 1) * request.PageSize;
            sql += $" OFFSET {offset} LIMIT {request.PageSize}";

            var campaigns = await _dapper.QueryAsync<EmailCampaignDto>(sql, parameters, ct);
            return campaigns.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get email campaigns");
            throw;
        }
    }
}

public class EmailCampaignByIdHandler : IRequestHandler<EmailCampaignByIdQry, EmailCampaignDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public EmailCampaignByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<EmailCampaignDto?> Handle(EmailCampaignByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    e.*
                FROM ""EmailCampaigns"" e
                WHERE e.""Id"" = @Id AND e.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            return await _dapper.QueryFirstOrDefaultAsync<EmailCampaignDto>(sql, parameters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get email campaign by ID: {CampaignId}", request.Id);
            throw;
        }
    }
}

public class EmailCampaignStatsHandler : IRequestHandler<EmailCampaignStatsQry, EmailCampaignStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public EmailCampaignStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<EmailCampaignStatsDto> Handle(EmailCampaignStatsQry request, CancellationToken ct)
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
                    COALESCE(SUM(""OpenCount""), 0) as TotalOpens,
                    COALESCE(SUM(""ClickCount""), 0) as TotalClicks,
                    COALESCE(AVG(""OpenRate""), 0) as AvgOpenRate,
                    COALESCE(AVG(""ClickRate""), 0) as AvgClickRate
                FROM ""EmailCampaigns""
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

            var stats = await _dapper.QueryFirstOrDefaultAsync<EmailCampaignStatsDto>(sql, parameters, ct);
            return stats ?? new EmailCampaignStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get email campaign stats");
            throw;
        }
    }
}