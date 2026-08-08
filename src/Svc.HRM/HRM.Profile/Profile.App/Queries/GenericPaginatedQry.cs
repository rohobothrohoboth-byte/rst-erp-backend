// Profile.App/Queries/GenericPaginatedQry.cs
using MediatR;
using Profile.Domain.DTOs;
using Dapper;
using Profile.App.Interfaces;

namespace Profile.App.Queries;

public class GenericPaginatedQry<T> : IRequest<PaginatedResult<T>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "DateAdd";
    public string? SortOrder { get; set; } = "desc";
    public string? SearchTerm { get; set; }
    public Dictionary<string, object>? Filters { get; set; }
    public string? SearchColumn { get; set; }
}

public class GenericPaginatedQryHandler<T> : IRequestHandler<GenericPaginatedQry<T>, PaginatedResult<T>>
{
    private readonly IDapperHelper _dapper;
    private readonly string _tableName;
    private readonly Dictionary<string, string> _columnMappings;

    public GenericPaginatedQryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
        _tableName = typeof(T).Name;
        _columnMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", "Id" },
            { "DateAdd", "DateAdd" },
            { "DateMod", "DateMod" },
            { "Code", "Code" },
            { "Name", "Name" },
            { "EmpState", "EmpState" }
        };
    }

    public async Task<PaginatedResult<T>> Handle(GenericPaginatedQry<T> request, CancellationToken ct)
    {
        var parameters = new DynamicParameters();
        var whereConditions = new List<string>();

        // Add IsDeleted filter if the property exists
        var isDeletedProp = typeof(T).GetProperty("IsDeleted");
        if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
        {
            whereConditions.Add("\"IsDeleted\" = false");
        }

        // Add search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchColumn = request.SearchColumn ?? GetDefaultSearchColumn();
            whereConditions.Add($"\"{searchColumn}\" ILIKE @SearchTerm");
            parameters.Add("@SearchTerm", $"%{request.SearchTerm}%");
        }

        // Add custom filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                if (filter.Value != null && !string.IsNullOrEmpty(filter.Value.ToString()))
                {
                    var columnName = GetColumnName(filter.Key);
                    whereConditions.Add($"\"{columnName}\" = @{filter.Key}");
                    parameters.Add($"@{filter.Key}", filter.Value);
                }
            }
        }

        var whereClause = whereConditions.Any()
            ? "WHERE " + string.Join(" AND ", whereConditions)
            : "";

        // Get total count
        var countSql = $"SELECT COUNT(*) FROM \"{_tableName}\" {whereClause}";
        var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters, ct);

        // Apply sorting
        var sortColumn = GetColumnName(request.SortBy ?? "DateAdd");
        var sortDirection = request.SortOrder?.ToLower() == "desc" ? "DESC" : "ASC";

        // Get paginated data
        var offset = (request.PageNumber - 1) * request.PageSize;
        var dataSql = $@"
SELECT * FROM ""{_tableName}""
{whereClause}
ORDER BY ""{sortColumn}"" {sortDirection}
LIMIT @PageSize OFFSET @Offset";

        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@Offset", offset);

        var items = await _dapper.QueryAsync<T>(dataSql, parameters, ct);

     // In GenericPaginatedQryHandler.cs, replace the return with:
     var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

     return new PaginatedResult<T>
     {
         Items = items.ToList(),
         PageNumber = request.PageNumber,
         PageSize = request.PageSize,
         TotalCount = totalCount,
         TotalPages = totalPages,
         HasPreviousPage = request.PageNumber > 1,
         HasNextPage = request.PageNumber < totalPages
     };
    }

    private string GetDefaultSearchColumn()
    {
        // Try to find a common search column
        var props = typeof(T).GetProperties();
        if (props.Any(p => p.Name == "Code")) return "Code";
        if (props.Any(p => p.Name == "Name")) return "Name";
        if (props.Any(p => p.Name == "FirstName")) return "FirstName";
        return props.First().Name;
    }

    private string GetColumnName(string propertyName)
    {
        return _columnMappings.GetValueOrDefault(propertyName, propertyName);
    }
}