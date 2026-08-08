using Dapper;
using MediatR;
using Svc.Task.Interfaces;
using Svc.Task.Models.Dtos;

namespace Svc.Task.Queries;

// Paginated result class
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult() { }

    public PaginatedResult(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

// Query
public class TaskPaginatedQry : IRequest<PaginatedResult<TaskDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "DueDate";
    public string? SortOrder { get; set; } = "asc";
    public Guid? UserId { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? SearchTerm { get; set; }
}

// Handler
public class TaskPaginatedQryHandler : IRequestHandler<TaskPaginatedQry, PaginatedResult<TaskDto>>
{
    private readonly IDapperHelper _dapper;

    public TaskPaginatedQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<PaginatedResult<TaskDto>> Handle(TaskPaginatedQry request, CancellationToken ct)
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

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            sql += @" AND (""Title"" ILIKE @SearchTerm
                       OR ""Description"" ILIKE @SearchTerm)";
            parameters.Add("@SearchTerm", $"%{request.SearchTerm}%");
        }

        // Get total count
        var countSql = sql.Replace("*", "COUNT(*)");
        var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters, ct);

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortOrder = request.SortOrder?.ToLower() == "desc" ? "DESC" : "ASC";
            sql += $" ORDER BY \"{request.SortBy}\" {sortOrder}";
        }
        else
        {
            sql += " ORDER BY \"DueDate\" ASC";
        }

        // Apply pagination
        var offset = (request.PageNumber - 1) * request.PageSize;
        sql += " LIMIT @PageSize OFFSET @Offset";
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@Offset", offset);

        var items = await _dapper.QueryAsync<TaskDto>(sql, parameters, ct);

        return new PaginatedResult<TaskDto>
        {
            Items = items.ToList(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}