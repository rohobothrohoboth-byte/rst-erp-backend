using MediatR;
using Dapper;
using Microsoft.Extensions.Logging;
using Svc.Notification.Interfaces;
using Svc.Notification.Models.Dtos;
using System.Text;

namespace Svc.Notification.Queries;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public int UnreadCount { get; set; }
}

public class NotificationPaginatedQry : IRequest<PaginatedResult<NotificationDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
    public Guid? UserId { get; set; }
    public bool? IsRead { get; set; }
    public string? Type { get; set; }
    public string? Priority { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class NotificationPaginatedQryHandler : IRequestHandler<NotificationPaginatedQry, PaginatedResult<NotificationDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogger<NotificationPaginatedQryHandler> _logger;

    public NotificationPaginatedQryHandler(IDapperHelper dapper, ILogger<NotificationPaginatedQryHandler> logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

public async Task<PaginatedResult<NotificationDto>> Handle(NotificationPaginatedQry request, CancellationToken ct)
{
    try
    {
        // ========== TOTAL COUNT (all notifications, not deleted) ==========
        var totalCountSql = @"
            SELECT COUNT(*)
            FROM ""Notifications""
            WHERE ""IsDeleted"" = false AND ""UserId"" = @UserId";

        var totalCount = await _dapper.ExecuteScalarAsync<int>(totalCountSql, new { UserId = request.UserId }, ct);

        // ========== UNREAD COUNT (only unread, not deleted) ==========
        var unreadCountSql = @"
            SELECT COUNT(*)
            FROM ""Notifications""
            WHERE ""IsDeleted"" = false AND ""UserId"" = @UserId AND ""IsRead"" = false";

        var unreadCount = await _dapper.ExecuteScalarAsync<int>(unreadCountSql, new { UserId = request.UserId }, ct);

        if (totalCount == 0)
        {
            return new PaginatedResult<NotificationDto>
            {
                Items = new List<NotificationDto>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0,
                UnreadCount = 0
            };
        }

        // ========== BUILD WHERE CONDITIONS FOR DATA QUERY ==========
        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();

        // Always exclude soft-deleted notifications
        whereConditions.Add("\"IsDeleted\" = false");

        // Add UserId filter
        if (request.UserId.HasValue)
        {
            whereConditions.Add("\"UserId\" = @UserId");
            parameters.Add("@UserId", request.UserId.Value);
        }

        // Add IsRead filter only if explicitly provided
        if (request.IsRead.HasValue)
        {
            whereConditions.Add("\"IsRead\" = @IsRead");
            parameters.Add("@IsRead", request.IsRead.Value);
        }

        // Add type filter
        if (!string.IsNullOrEmpty(request.Type))
        {
            whereConditions.Add("\"Type\" = @Type");
            parameters.Add("@Type", request.Type);
        }

        // Add priority filter
        if (!string.IsNullOrEmpty(request.Priority))
        {
            whereConditions.Add("\"Priority\" = @Priority");
            parameters.Add("@Priority", request.Priority);
        }

        // Add date filters
        if (request.FromDate.HasValue)
        {
            whereConditions.Add("\"CreatedAt\" >= @FromDate");
            parameters.Add("@FromDate", request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            whereConditions.Add("\"CreatedAt\" <= @ToDate");
            parameters.Add("@ToDate", request.ToDate.Value);
        }

        var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

        // ========== GET PAGINATED DATA ==========
        var sortBy = string.IsNullOrEmpty(request.SortBy) ? "CreatedAt" : request.SortBy;
        var sortOrder = request.SortOrder?.ToLower() == "desc" ? "DESC" : "ASC";
        var offset = (request.PageNumber - 1) * request.PageSize;

        var dataSql = $@"
            SELECT
                ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Priority"",
                ""IsRead"", ""CreatedAt"", ""ReadAt"", ""Metadata""
            FROM ""Notifications""
            {whereClause}
            ORDER BY ""{sortBy}"" {sortOrder}
            LIMIT @PageSize OFFSET @Offset";

        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@Offset", offset);

        var items = await _dapper.QueryAsync<NotificationDto>(dataSql, parameters, ct);

        var result = new PaginatedResult<NotificationDto>
        {
            Items = items.ToList(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            UnreadCount = unreadCount
        };

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting paginated notifications for user {UserId}", request.UserId);
        return new PaginatedResult<NotificationDto>
        {
            Items = new List<NotificationDto>(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = 0,
            UnreadCount = 0
        };
    }
}
}