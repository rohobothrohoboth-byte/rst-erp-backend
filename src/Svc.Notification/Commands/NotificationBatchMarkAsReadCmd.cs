// Svc.Notification/Commands/NotificationBatchMarkAsReadCmd.cs
using Helpers;
using MediatR;
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Models.Entities;

namespace Svc.Notification.Commands;

public class NotificationBatchMarkAsReadCmd : IRequest<int>
{
    public List<Guid> NotificationIds { get; set; } = new();
}

public class NotificationBatchMarkAsReadHandler : IRequestHandler<NotificationBatchMarkAsReadCmd, int>
{
    private readonly IDapperHelper _dapper;

    public async Task<int> Handle(NotificationBatchMarkAsReadCmd request, CancellationToken ct)
    {
        var sql = @"
            UPDATE ""Notifications""
            SET ""IsRead"" = true, ""ReadAt"" = NOW()
            WHERE ""Id"" = ANY(@Ids) AND ""IsRead"" = false";

        return await _dapper.ExecuteAsync(sql, new { Ids = request.NotificationIds.ToArray() }, ct);
    }
}

// Svc.Notification/Commands/NotificationCleanupCmd.cs
public class NotificationCleanupCmd : IRequest<int>
{
    public int DaysOld { get; set; } = 90;
}

public class NotificationCleanupHandler : IRequestHandler<NotificationCleanupCmd, int>
{
    private readonly IDapperHelper _dapper;

    public async Task<int> Handle(NotificationCleanupCmd request, CancellationToken ct)
    {
        var sql = @"
            UPDATE ""Notifications""
            SET ""IsDeleted"" = true
            WHERE ""CreatedAt"" < NOW() - INTERVAL '@DaysOld days'
            AND ""IsDeleted"" = false";

        return await _dapper.ExecuteAsync(sql, new { DaysOld = request.DaysOld }, ct);
    }
}