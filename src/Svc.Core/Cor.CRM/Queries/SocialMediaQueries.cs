// Cor.CRM/Queries/SocialMediaQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.CRM.Queries;

public class SocialMediaAllQry : IRequest<List<SocialMediaPostDto>>
{
    public string? Platform { get; set; }
    public string? Status { get; set; }
    public Guid? CampaignId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class SocialMediaByIdQry : IRequest<SocialMediaPostDto?>
{
    public Guid Id { get; set; }
}

public class SocialMediaStatsQry : IRequest<SocialMediaStatsDto>
{
    public Guid? CampaignId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class SocialMediaAllHandler : IRequestHandler<SocialMediaAllQry, List<SocialMediaPostDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public SocialMediaAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<SocialMediaPostDto>> Handle(SocialMediaAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    s.*
                FROM ""SocialMediaPosts"" s
                WHERE s.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.Platform))
            {
                sql += " AND s.\"Platform\" = @Platform";
                parameters.Add("@Platform", Enum.Parse<SocialMediaPlatform>(request.Platform));
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sql += " AND s.\"Status\" = @Status";
                parameters.Add("@Status", Enum.Parse<SocialMediaStatus>(request.Status));
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

            var posts = await _dapper.QueryAsync<SocialMediaPostDto>(sql, parameters, ct);
            return posts.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get social media posts");
            throw;
        }
    }
}

public class SocialMediaByIdHandler : IRequestHandler<SocialMediaByIdQry, SocialMediaPostDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public SocialMediaByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<SocialMediaPostDto?> Handle(SocialMediaByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    s.*
                FROM ""SocialMediaPosts"" s
                WHERE s.""Id"" = @Id AND s.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            return await _dapper.QueryFirstOrDefaultAsync<SocialMediaPostDto>(sql, parameters, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get social media post by ID: {PostId}", request.Id);
            throw;
        }
    }
}

public class SocialMediaStatsHandler : IRequestHandler<SocialMediaStatsQry, SocialMediaStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public SocialMediaStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<SocialMediaStatsDto> Handle(SocialMediaStatsQry request, CancellationToken ct)
    {
        try
               {
                   var sql = @"
                       SELECT
                           COUNT(*) as TotalPosts,
                           COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Published,
                           COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Scheduled,
                           COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Draft,
                           COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Failed,
                           COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as Pending,
                           COALESCE(SUM(""EngagementCount""), 0) as TotalEngagement,
                           COALESCE(SUM(""ReachCount""), 0) as TotalReach,
                           COALESCE(SUM(""LikeCount""), 0) as TotalLikes,
                           COALESCE(SUM(""ShareCount""), 0) as TotalShares,
                           COALESCE(SUM(""CommentCount""), 0) as TotalComments,
                           COALESCE(AVG(""EngagementCount""), 0) as AvgEngagement,
                           COALESCE(AVG(""ReachCount""), 0) as AvgReach,
                           COALESCE(AVG(""LikeCount""), 0) as AvgLikes,
                           COALESCE(AVG(""ShareCount""), 0) as AvgShares,
                           COALESCE(AVG(""CommentCount""), 0) as AvgComments
                       FROM ""SocialMediaPosts""
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

            var stats = await _dapper.QueryFirstOrDefaultAsync<SocialMediaStatsDto>(sql, parameters, ct);
            return stats ?? new SocialMediaStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get social media stats");
            throw;
        }
    }
}