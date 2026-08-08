// Cor.CRM/Queries/PropertyQueries.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Dapper;
using Helpers;
using MediatR;
using System.Text.Json;

namespace Cor.CRM.Queries;

public class PropertyByIdQry : IRequest<PropertyDto?>
{
    public Guid Id { get; set; }
}

public class PropertyAllQry : IRequest<List<PropertyDto>>
{
    public PropertyFilterDto Filter { get; set; } = new();
}

public class PropertyStatsQry : IRequest<PropertyStatsDto> { }

public class PropertyByIdHandler : IRequestHandler<PropertyByIdQry, PropertyDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public PropertyByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<PropertyDto?> Handle(PropertyByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    p.*,
                    c.""Name"" as OwnerName,
                    e.""FirstName"" || ' ' || e.""LastName"" as ListingAgentName,
                    eb.""FirstName"" || ' ' || eb.""LastName"" as BuyingAgentName
                FROM ""Property"" p
                LEFT JOIN ""Customers"" c ON p.""OwnerId"" = c.""Id""
                LEFT JOIN ""LocalEmployees"" e ON p.""ListingAgentId"" = e.""Id""
                LEFT JOIN ""LocalEmployees"" eb ON p.""BuyingAgentId"" = eb.""Id""
                WHERE p.""Id"" = @Id AND p.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var property = await _dapper.QueryFirstOrDefaultAsync<PropertyDto>(sql, parameters, ct);

            if (property != null)
            {
                // Parse features from JSON
                if (!string.IsNullOrEmpty(property.FeaturesJson))
                {
                    try
                    {
                        property.Features = JsonSerializer.Deserialize<List<string>>(property.FeaturesJson);
                    }
                    catch { property.Features = new List<string>(); }
                }

                // Parse images from JSON
                if (!string.IsNullOrEmpty(property.ImagesJson))
                {
                    try
                    {
                        property.Images = JsonSerializer.Deserialize<List<string>>(property.ImagesJson);
                    }
                    catch { property.Images = new List<string>(); }
                }
            }

            return property;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get property by ID: {PropertyId}", request.Id);
            throw;
        }
    }
}

public class PropertyAllHandler : IRequestHandler<PropertyAllQry, List<PropertyDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public PropertyAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<PropertyDto>> Handle(PropertyAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    p.*,
                    c.""Name"" as OwnerName,
                    e.""FirstName"" || ' ' || e.""LastName"" as ListingAgentName,
                    eb.""FirstName"" || ' ' || eb.""LastName"" as BuyingAgentName
                FROM ""Property"" p
                LEFT JOIN ""Customers"" c ON p.""OwnerId"" = c.""Id""
                LEFT JOIN ""LocalEmployees"" e ON p.""ListingAgentId"" = e.""Id""
                LEFT JOIN ""LocalEmployees"" eb ON p.""BuyingAgentId"" = eb.""Id""
                WHERE p.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            // Apply filters
            var filter = request.Filter;
            if (!string.IsNullOrEmpty(filter.Search))
            {
                sql += @" AND (p.""Title"" ILIKE @Search
                         OR p.""Description"" ILIKE @Search
                         OR p.""Address"" ILIKE @Search
                         OR p.""City"" ILIKE @Search)";
                parameters.Add("@Search", $"%{filter.Search}%");
            }

            if (filter.Type.HasValue)
            {
                sql += " AND p.\"Type\" = @Type";
                parameters.Add("@Type", filter.Type.Value);
            }

            if (filter.Status.HasValue)
            {
                sql += " AND p.\"Status\" = @Status";
                parameters.Add("@Status", filter.Status.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                sql += " AND p.\"Price\" >= @MinPrice";
                parameters.Add("@MinPrice", filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                sql += " AND p.\"Price\" <= @MaxPrice";
                parameters.Add("@MaxPrice", filter.MaxPrice.Value);
            }

            if (filter.MinBedrooms.HasValue)
            {
                sql += " AND p.\"Bedrooms\" >= @MinBedrooms";
                parameters.Add("@MinBedrooms", filter.MinBedrooms.Value);
            }

            if (filter.MaxBedrooms.HasValue)
            {
                sql += " AND p.\"Bedrooms\" <= @MaxBedrooms";
                parameters.Add("@MaxBedrooms", filter.MaxBedrooms.Value);
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                sql += " AND p.\"City\" = @City";
                parameters.Add("@City", filter.City);
            }

            if (!string.IsNullOrEmpty(filter.State))
            {
                sql += " AND p.\"State\" = @State";
                parameters.Add("@State", filter.State);
            }

            if (filter.IsFeatured.HasValue)
            {
                sql += " AND p.\"IsFeatured\" = @IsFeatured";
                parameters.Add("@IsFeatured", filter.IsFeatured.Value);
            }

            if (filter.IsPublished.HasValue)
            {
                sql += " AND p.\"IsPublished\" = @IsPublished";
                parameters.Add("@IsPublished", filter.IsPublished.Value);
            }

            if (filter.ListedFrom.HasValue)
            {
                sql += " AND p.\"ListingDate\" >= @ListedFrom";
                parameters.Add("@ListedFrom", filter.ListedFrom.Value);
            }

            if (filter.ListedTo.HasValue)
            {
                sql += " AND p.\"ListingDate\" <= @ListedTo";
                parameters.Add("@ListedTo", filter.ListedTo.Value);
            }

            // Sorting
            var sortBy = filter.SortBy ?? "CreatedAt";
            var sortOrder = filter.SortDescending ? "DESC" : "ASC";
            sql += $" ORDER BY p.\"{sortBy}\" {sortOrder}";

            // Pagination
            var offset = (filter.Page - 1) * filter.PageSize;
            sql += $" OFFSET {offset} LIMIT {filter.PageSize}";

            var properties = await _dapper.QueryAsync<PropertyDto>(sql, parameters, ct);
            var propertyList = properties.ToList();

            // Parse JSON fields
            foreach (var property in propertyList)
            {
                if (!string.IsNullOrEmpty(property.FeaturesJson))
                {
                    try
                    {
                        property.Features = JsonSerializer.Deserialize<List<string>>(property.FeaturesJson);
                    }
                    catch { property.Features = new List<string>(); }
                }

                if (!string.IsNullOrEmpty(property.ImagesJson))
                {
                    try
                    {
                        property.Images = JsonSerializer.Deserialize<List<string>>(property.ImagesJson);
                    }
                    catch { property.Images = new List<string>(); }
                }
            }

            return propertyList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all properties");
            throw;
        }
    }
}

public class PropertyStatsHandler : IRequestHandler<PropertyStatsQry, PropertyStatsDto>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public PropertyStatsHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<PropertyStatsDto> Handle(PropertyStatsQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    COUNT(*) as TotalProperties,
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as Active,
                    COUNT(CASE WHEN ""Status"" = 2 THEN 1 END) as Pending,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Sold,
                    COUNT(CASE WHEN ""Status"" = 4 THEN 1 END) as Rented,
                    COUNT(CASE WHEN ""Status"" = 5 THEN 1 END) as OffMarket,
                    COALESCE(AVG(""Price""), 0) as AveragePrice,
                    COALESCE(SUM(""Price""), 0) as TotalValue,
                    COALESCE(MAX(""Price""), 0) as MaxPrice,
                    COALESCE(MIN(""Price""), 0) as MinPrice
                FROM ""Property""
                WHERE ""IsDeleted"" = false
            ";

            var result = await _dapper.QueryFirstOrDefaultAsync<PropertyStatsDto>(sql, new { }, ct);
            return result ?? new PropertyStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get property stats");
            throw;
        }
    }
}