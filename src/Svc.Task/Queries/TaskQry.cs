using Dapper;
using Helpers;
using MediatR;
using Svc.Task.Interfaces;
using Svc.Task.Models.Dtos;

namespace Svc.Task.Queries;

// ==================== QUERY DEFINITIONS ====================

public class TaskAllQry : IRequest<List<TaskDto>>
{
    public Guid? UserId { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
}

public class TaskByIdQry : IRequest<TaskDto?>
{
    public Guid Id { get; set; }
}

public class TaskStatsQry : IRequest<TaskStatsDto>
{
    public Guid UserId { get; set; }
}

// ==================== HANDLERS ====================

public class TaskAllQryHandler : IRequestHandler<TaskAllQry, List<TaskDto>>
{
    private readonly IDapperHelper _dapper;

    public TaskAllQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<TaskDto>> Handle(TaskAllQry request, CancellationToken ct)
    {
        var sql = @"
            SELECT ""Id"", ""Title"", ""Description"", ""Status"", ""Priority"",
                   ""DueDate"", ""CreatedAt"", ""CompletedAt"", ""Category"", ""Module"",
                   ""AssignedTo"", ""AssignedBy""
            FROM ""Tasks""
            WHERE ""IsDeleted"" = false";

        var parameters = new DynamicParameters();

        if (request.UserId.HasValue)
        {
            sql += " AND \"AssignedTo\" = @UserId";
            parameters.Add("@UserId", request.UserId.Value);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            sql += " AND \"Status\" = @Status";
            parameters.Add("@Status", request.Status);
        }

        if (!string.IsNullOrEmpty(request.Priority))
        {
            sql += " AND \"Priority\" = @Priority";
            parameters.Add("@Priority", request.Priority);
        }

        if (request.DueDateFrom.HasValue)
        {
            sql += " AND \"DueDate\" >= @DueDateFrom";
            parameters.Add("@DueDateFrom", request.DueDateFrom.Value);
        }

        if (request.DueDateTo.HasValue)
        {
            sql += " AND \"DueDate\" <= @DueDateTo";
            parameters.Add("@DueDateTo", request.DueDateTo.Value);
        }

        sql += " ORDER BY \"DueDate\" ASC";

        var result = await _dapper.QueryAsync<TaskDto>(sql, parameters, ct);
        return result.ToList();
    }
}

public class TaskByIdQryHandler : IRequestHandler<TaskByIdQry, TaskDto?>
{
    private readonly IDapperHelper _dapper;

    public TaskByIdQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<TaskDto?> Handle(TaskByIdQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT ""Id"", ""Title"", ""Description"", ""Status"", ""Priority"",
                   ""DueDate"", ""CreatedAt"", ""CompletedAt"", ""Category"", ""Module"",
                   ""AssignedTo"", ""AssignedBy""
            FROM ""Tasks""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<TaskDto>(sql, new { request.Id }, ct);
        return result;
    }
}

public class TaskStatsQryHandler : IRequestHandler<TaskStatsQry, TaskStatsDto>
{
    private readonly IDapperHelper _dapper;

    public TaskStatsQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<TaskStatsDto> Handle(TaskStatsQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                COUNT(*) as Total,
                SUM(CASE WHEN ""Status"" = 'completed' THEN 1 ELSE 0 END) as Completed,
                SUM(CASE WHEN ""Status"" = 'in_progress' THEN 1 ELSE 0 END) as InProgress,
                SUM(CASE WHEN ""Status"" = 'pending' THEN 1 ELSE 0 END) as Pending,
                SUM(CASE WHEN ""Status"" != 'completed' AND ""DueDate"" < NOW() THEN 1 ELSE 0 END) as Overdue
            FROM ""Tasks""
            WHERE ""AssignedTo"" = @UserId AND ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<TaskStatsDto>(sql, new { request.UserId }, ct);

        if (result != null && result.Total > 0)
        {
            result.CompletionRate = (result.Completed * 100) / result.Total;
        }

        return result ?? new TaskStatsDto();
    }
}