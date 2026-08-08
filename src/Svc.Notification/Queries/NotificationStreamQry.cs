using Dapper;
using Helpers;
using MediatR;
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Models.Entities;

namespace Svc.Notification.Queries;

public class NotificationStreamQry : IRequest<List<NotificationDto>>
{
    public Guid UserId { get; set; }
    public DateTime Since { get; set; }
}

public class NotificationStreamQryHandler : IRequestHandler<NotificationStreamQry, List<NotificationDto>>
{
    private readonly IDapperHelper _dapper;

    public NotificationStreamQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<NotificationDto>> Handle(NotificationStreamQry request, CancellationToken ct)
    {
        var sql = @"
            SELECT
                ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                ""IsRead"", ""CreatedAt"", ""ReadAt"", ""Metadata""
            FROM ""Notifications""
            WHERE ""UserId"" = @UserId
                AND ""CreatedAt"" > @Since
                AND ""IsDeleted"" = false
            ORDER BY ""CreatedAt"" DESC
            LIMIT 50";

        var notifications = await _dapper.QueryAsync<NotificationDto>(sql, new
        {
            UserId = request.UserId,
            Since = request.Since
        }, ct);

        return notifications.ToList();
    }
}