using Helpers;
using MediatR;
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;
using Svc.Notification.Models.Entities;

namespace Svc.Notification.Commands;

public class NotificationAddCmdHandler : IRequestHandler<NotificationAddCmd, NotificationDto>
{
    private readonly IDapperHelper _dapper;

    public NotificationAddCmdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<NotificationDto> Handle(NotificationAddCmd request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        const string sql = @"
            INSERT INTO ""Notifications"" (
                ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                ""IsRead"", ""CreatedAt"", ""Metadata"", ""DateAdd"", ""IsDeleted""
            ) VALUES (
                @Id, @UserId, @Title, @Message, @Type, @Priority,
                false, NOW(), @Metadata, NOW(), false
            )
            RETURNING ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                      ""IsRead"", ""CreatedAt"", ""ReadAt"", ""Metadata""";

        var result = await _dapper.QueryFirstOrDefaultAsync<NotificationDto>(sql, new
        {
            Id = id,
            request.AddDto.UserId,
            request.AddDto.Title,
            request.AddDto.Message,
            request.AddDto.Type,
            request.AddDto.Priority,
            request.AddDto.Metadata
        }, ct);

        return result ?? throw new DomainException("Failed to create notification");
    }
}

public class NotificationMarkAsReadCmdHandler : IRequestHandler<NotificationMarkAsReadCmd, Unit>
{
    private readonly IDapperHelper _dapper;

    public NotificationMarkAsReadCmdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<Unit> Handle(NotificationMarkAsReadCmd request, CancellationToken ct)
    {
        const string sql = @"
            UPDATE ""Notifications""
            SET ""IsRead"" = true, ""ReadAt"" = NOW(), ""DateMod"" = NOW()
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var rowsAffected = await _dapper.ExecuteAsync(sql, new { request.Id }, ct);

        if (rowsAffected == 0)
            throw new DomainException($"Notification with id [{request.Id}] not found");

        return Unit.Value;
    }
}

public class NotificationMarkAllAsReadCmdHandler : IRequestHandler<NotificationMarkAllAsReadCmd, Unit>
{
    private readonly IDapperHelper _dapper;

    public NotificationMarkAllAsReadCmdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<Unit> Handle(NotificationMarkAllAsReadCmd request, CancellationToken ct)
    {
        const string sql = @"
            UPDATE ""Notifications""
            SET ""IsRead"" = true, ""ReadAt"" = NOW(), ""DateMod"" = NOW()
            WHERE ""UserId"" = @UserId AND ""IsRead"" = false AND ""IsDeleted"" = false";

        await _dapper.ExecuteAsync(sql, new { request.UserId }, ct);
        return Unit.Value;
    }
}

public class NotificationDeleteCmdHandler : IRequestHandler<NotificationDeleteCmd, Unit>
{
    private readonly IDapperHelper _dapper;

    public NotificationDeleteCmdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<Unit> Handle(NotificationDeleteCmd request, CancellationToken ct)
    {
        const string sql = @"
            UPDATE ""Notifications""
            SET ""IsDeleted"" = true, ""DateMod"" = NOW()
            WHERE ""Id"" = @Id";

        var rowsAffected = await _dapper.ExecuteAsync(sql, new { request.Id }, ct);

        if (rowsAffected == 0)
            throw new DomainException($"Notification with id [{request.Id}] not found");

        return Unit.Value;
    }
}

public class NotificationClearAllCmdHandler : IRequestHandler<NotificationClearAllCmd, Unit>
{
    private readonly IDapperHelper _dapper;

    public NotificationClearAllCmdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<Unit> Handle(NotificationClearAllCmd request, CancellationToken ct)
    {
        const string sql = @"
            UPDATE ""Notifications""
            SET ""IsDeleted"" = true, ""DateMod"" = NOW()
            WHERE ""UserId"" = @UserId";

        await _dapper.ExecuteAsync(sql, new { request.UserId }, ct);
        return Unit.Value;
    }
}