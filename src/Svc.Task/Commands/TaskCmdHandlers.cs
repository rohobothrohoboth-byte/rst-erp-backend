using Dapper;
using Helpers;
using MediatR;
using Svc.Task.Interfaces;
using Svc.Task.Models.Dtos;
using Svc.Task.Models.Entities;
// Remove Svc.Notification.Services - we want to use Svc.Task.Services.INotificationService
using Svc.Task.Services;  // Keep only this one

namespace Svc.Task.Commands;

public class TaskAddCmdHandler : IRequestHandler<TaskAddCmd, TaskDto>
{
    private readonly IDapperHelper _dapper;
    private readonly INotificationService _notificationService;

    public TaskAddCmdHandler(IDapperHelper dapper, INotificationService notificationService)
    {
        _dapper = dapper;
        _notificationService = notificationService;
    }

    public async System.Threading.Tasks.Task<TaskDto> Handle(TaskAddCmd request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        const string sql = @"
            INSERT INTO ""Tasks"" (
                ""Id"", ""Title"", ""Description"", ""Status"", ""Priority"",
                ""DueDate"", ""CreatedAt"", ""AssignedTo"", ""AssignedBy"",
                ""Category"", ""Module"", ""DateAdd"", ""IsDeleted""
            ) VALUES (
                @Id, @Title, @Description, 'pending', @Priority,
                @DueDate, NOW(), @AssignedTo, @AssignedBy,
                @Category, @Module, NOW(), false
            )
            RETURNING ""Id"", ""Title"", ""Description"", ""Status"", ""Priority"",
                      ""DueDate"", ""CreatedAt"", ""CompletedAt"", ""Category"", ""Module"",
                      ""AssignedTo"", ""AssignedBy""";

        var result = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(sql, new
        {
            Id = id,
            request.AddDto.Title,
            request.AddDto.Description,
            request.AddDto.Priority,
            request.AddDto.DueDate,
            request.AddDto.AssignedTo,
            request.AddDto.AssignedBy,
            request.AddDto.Category,
            request.AddDto.Module
        }, ct);

        // Send notification to assigned employee - Convert Guid to string
        if (result != null)
        {
            await _notificationService.NotifyTaskAssigned(
                request.AddDto.AssignedTo.ToString(),
                result.Title,
                id.ToString()
            );
        }

        return result ?? throw new DomainException("Failed to create task");
    }
}

public class TaskModCmdHandler : IRequestHandler<TaskModCmd, TaskDto>
{
    private readonly IDapperHelper _dapper;
    private readonly INotificationService _notificationService;

    public TaskModCmdHandler(IDapperHelper dapper, INotificationService notificationService)
    {
        _dapper = dapper;
        _notificationService = notificationService;
    }

    public async System.Threading.Tasks.Task<TaskDto> Handle(TaskModCmd request, CancellationToken ct)
    {
        // First get the original task to know who to notify
        var getOriginalSql = "SELECT * FROM \"Tasks\" WHERE \"Id\" = @Id AND \"IsDeleted\" = false";
        var originalTask = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(getOriginalSql, new { request.ModDto.Id }, ct);

        var sql = @"
            UPDATE ""Tasks""
            SET ""DateMod"" = NOW()";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(request.ModDto.Title))
        {
            sql += ", \"Title\" = @Title";
            parameters.Add("@Title", request.ModDto.Title);
        }

        if (request.ModDto.Description != null)
        {
            sql += ", \"Description\" = @Description";
            parameters.Add("@Description", request.ModDto.Description);
        }

        if (!string.IsNullOrEmpty(request.ModDto.Status))
        {
            sql += ", \"Status\" = @Status";
            parameters.Add("@Status", request.ModDto.Status);

            if (request.ModDto.Status == "completed")
            {
                sql += ", \"CompletedAt\" = NOW()";
            }
        }

        if (!string.IsNullOrEmpty(request.ModDto.Priority))
        {
            sql += ", \"Priority\" = @Priority";
            parameters.Add("@Priority", request.ModDto.Priority);
        }

        if (request.ModDto.DueDate.HasValue)
        {
            sql += ", \"DueDate\" = @DueDate";
            parameters.Add("@DueDate", request.ModDto.DueDate.Value);
        }

        if (request.ModDto.Category != null)
        {
            sql += ", \"Category\" = @Category";
            parameters.Add("@Category", request.ModDto.Category);
        }

        if (request.ModDto.Module != null)
        {
            sql += ", \"Module\" = @Module";
            parameters.Add("@Module", request.ModDto.Module);
        }

        if (request.ModDto.AssignedTo.HasValue)
        {
            sql += ", \"AssignedTo\" = @AssignedTo";
            parameters.Add("@AssignedTo", request.ModDto.AssignedTo.Value);
        }

        sql += " WHERE \"Id\" = @Id AND \"IsDeleted\" = false RETURNING *";
        parameters.Add("@Id", request.ModDto.Id);

        var result = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(sql, parameters, ct);

        if (result == null)
            throw new DomainException($"Task with id [{request.ModDto.Id}] not found");

        // Send notification if status changed to completed
        if (originalTask != null && request.ModDto.Status == "completed" && originalTask.Status != "completed")
        {
            string assignedTo;
            if (request.ModDto.AssignedTo.HasValue)
            {
                assignedTo = request.ModDto.AssignedTo.Value.ToString();
            }
            else
            {
                assignedTo = originalTask.AssignedTo.ToString();  // Convert Guid to string
            }

            await _notificationService.NotifyTaskCompleted(
                assignedTo,
                result.Title,
                request.ModDto.Id.ToString()
            );
        }

        // Send notification if reassigned
        if (request.ModDto.AssignedTo.HasValue && originalTask != null)
        {
            string originalAssignedTo = originalTask.AssignedTo.ToString();  // Convert Guid to string
            string newAssignedTo = request.ModDto.AssignedTo.Value.ToString();

            if (originalAssignedTo != newAssignedTo)
            {
                await _notificationService.NotifyTaskAssigned(
                    newAssignedTo,
                    result.Title,
                    request.ModDto.Id.ToString()
                );
            }
        }

        return result;
    }
}

public class TaskUpdateStatusCmdHandler : IRequestHandler<TaskUpdateStatusCmd, Unit>
{
    private readonly IDapperHelper _dapper;
    private readonly INotificationService _notificationService;

    public TaskUpdateStatusCmdHandler(IDapperHelper dapper, INotificationService notificationService)
    {
        _dapper = dapper;
        _notificationService = notificationService;
    }

    public async System.Threading.Tasks.Task<Unit> Handle(TaskUpdateStatusCmd request, CancellationToken ct)
    {
        // Get task before update to know original status
        var getTaskSql = "SELECT * FROM \"Tasks\" WHERE \"Id\" = @Id AND \"IsDeleted\" = false";
        var task = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(getTaskSql, new { request.Id }, ct);

        const string sql = @"
            UPDATE ""Tasks""
            SET ""Status"" = @Status,
                ""CompletedAt"" = CASE WHEN @Status = 'completed' THEN NOW() ELSE NULL END,
                ""DateMod"" = NOW()
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var rowsAffected = await _dapper.ExecuteAsync(sql, new { request.Id, request.Status }, ct);

        if (rowsAffected == 0)
            throw new DomainException($"Task with id [{request.Id}] not found");

        // Send notification based on status change
        if (task != null)
        {
            if (request.Status == "completed" && task.Status != "completed")
            {
                await _notificationService.NotifyTaskCompleted(
                    task.AssignedTo.ToString(),  // Convert Guid to string
                    task.Title,
                    request.Id.ToString()
                );
            }
            else if (request.Status == "pending" && task.Status == "completed")
            {
                await _notificationService.NotifyTaskUpdated(
                    task.AssignedTo.ToString(),  // Convert Guid to string
                    task.Title,
                    request.Id.ToString()
                );
            }
        }

        return Unit.Value;
    }
}

public class TaskDeleteCmdHandler : IRequestHandler<TaskDeleteCmd, Unit>
{
    private readonly IDapperHelper _dapper;
    private readonly INotificationService _notificationService;

    public TaskDeleteCmdHandler(IDapperHelper dapper, INotificationService notificationService)
    {
        _dapper = dapper;
        _notificationService = notificationService;
    }

    public async System.Threading.Tasks.Task<Unit> Handle(TaskDeleteCmd request, CancellationToken ct)
    {
        // Get task before deletion to send notification
        var getTaskSql = "SELECT * FROM \"Tasks\" WHERE \"Id\" = @Id AND \"IsDeleted\" = false";
        var task = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(getTaskSql, new { request.Id }, ct);

        const string sql = @"
            UPDATE ""Tasks""
            SET ""IsDeleted"" = true, ""DateMod"" = NOW()
            WHERE ""Id"" = @Id";

        var rowsAffected = await _dapper.ExecuteAsync(sql, new { request.Id }, ct);

        if (rowsAffected == 0)
            throw new DomainException($"Task with id [{request.Id}] not found");

        // Send notification about deletion
        if (task != null)
        {
            await _notificationService.NotifyTaskUpdated(
                task.AssignedTo.ToString(),  // Convert Guid to string
                task.Title,
                request.Id.ToString()
            );
        }

        return Unit.Value;
    }
}

public class TaskBulkDeleteCmdHandler : IRequestHandler<TaskBulkDeleteCmd, Unit>
{
    private readonly IDapperHelper _dapper;

    public TaskBulkDeleteCmdHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async System.Threading.Tasks.Task<Unit> Handle(TaskBulkDeleteCmd request, CancellationToken ct)
    {
        if (request.Ids == null || request.Ids.Count == 0)
            return Unit.Value;

        const string sql = @"
            UPDATE ""Tasks""
            SET ""IsDeleted"" = true, ""DateMod"" = NOW()
            WHERE ""Id"" = ANY(@Ids)";

        await _dapper.ExecuteAsync(sql, new { Ids = request.Ids.ToArray() }, ct);
        return Unit.Value;
    }
}