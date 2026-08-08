using Dapper;
using Helpers;
using MediatR;
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Models.Entities;

namespace Svc.Notification.Queries;

// ==================== QUERY DEFINITIONS ====================

public class NotificationAllQry : IRequest<List<NotificationDto>>
{
    public Guid? UserId { get; set; }
    public bool? IsRead { get; set; }
    public string? Type { get; set; }
    public int? Limit { get; set; }
}

public class NotificationByIdQry : IRequest<NotificationDto?>
{
    public Guid Id { get; set; }
}

public class NotificationUnreadCountQry : IRequest<int>
{
    public Guid UserId { get; set; }
}

public class NotificationStatsQry : IRequest<NotificationStatsDto>
{
    public Guid UserId { get; set; }
}

// ==================== HANDLERS ====================

public class NotificationAllQryHandler : IRequestHandler<NotificationAllQry, List<NotificationDto>>
{
    private readonly IDapperHelper _dapper;

    public NotificationAllQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<NotificationDto>> Handle(NotificationAllQry request, CancellationToken ct)
    {
        var sql = @"
            SELECT ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                   ""IsRead"", ""CreatedAt"", ""ReadAt"", ""Metadata""
            FROM ""Notifications""
            WHERE ""IsDeleted"" = false";

        var parameters = new DynamicParameters();

        if (request.UserId.HasValue)
        {
            sql += " AND \"UserId\" = @UserId";
            parameters.Add("@UserId", request.UserId.Value);
        }

        if (request.IsRead.HasValue)
        {
            sql += " AND \"IsRead\" = @IsRead";
            parameters.Add("@IsRead", request.IsRead.Value);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            sql += " AND \"Type\" = @Type";
            parameters.Add("@Type", request.Type);
        }

        sql += " ORDER BY \"CreatedAt\" DESC";

        if (request.Limit.HasValue)
        {
            sql += " LIMIT @Limit";
            parameters.Add("@Limit", request.Limit.Value);
        }

        var result = await _dapper.QueryAsync<NotificationDto>(sql, parameters, ct);
        return result.ToList();
    }
}

public class NotificationByIdQryHandler : IRequestHandler<NotificationByIdQry, NotificationDto?>
{
    private readonly IDapperHelper _dapper;

    public NotificationByIdQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<NotificationDto?> Handle(NotificationByIdQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                   ""IsRead"", ""CreatedAt"", ""ReadAt"", ""Metadata""
            FROM ""Notifications""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<NotificationDto>(sql, new { request.Id }, ct);
        return result;
    }
}

public class NotificationUnreadCountQryHandler : IRequestHandler<NotificationUnreadCountQry, int>
{
    private readonly IDapperHelper _dapper;

    public NotificationUnreadCountQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<int> Handle(NotificationUnreadCountQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT COUNT(*) FROM ""Notifications""
            WHERE ""UserId"" = @UserId AND ""IsRead"" = false AND ""IsDeleted"" = false";

        var result = await _dapper.ExecuteScalarAsync<int>(sql, new { request.UserId }, ct);
        return result;
    }
}

public class NotificationStatsQryHandler : IRequestHandler<NotificationStatsQry, NotificationStatsDto>
{
    private readonly IDapperHelper _dapper;

    public NotificationStatsQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<NotificationStatsDto> Handle(NotificationStatsQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                COUNT(*) as Total,
                SUM(CASE WHEN ""IsRead"" = false THEN 1 ELSE 0 END) as Unread,
                SUM(CASE WHEN ""IsRead"" = true THEN 1 ELSE 0 END) as IsRead,
                SUM(CASE WHEN ""Priority"" = 'urgent' AND ""IsRead"" = false THEN 1 ELSE 0 END) as Urgent
            FROM ""Notifications""
            WHERE ""UserId"" = @UserId AND ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<NotificationStatsDto>(sql, new { request.UserId }, ct);
        return result ?? new NotificationStatsDto();
    }
}