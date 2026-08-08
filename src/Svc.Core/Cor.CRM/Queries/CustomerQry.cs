// Cor.CRM/Queries/CustomerQry.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;
namespace Cor.CRM.Queries;

// ============================================================
// QUERIES
// ============================================================

public class CustomerAllQry : IRequest<List<CustomerDto>>
{
    public CustomerFilterDto? Filter { get; set; }
}

public class CustomerByIdQry : IRequest<CustomerDto?>
{
    public Guid Id { get; set; }
}

// ============================================================
// GET ALL CUSTOMERS
// ============================================================

public class CustomerAllHandler : IRequestHandler<CustomerAllQry, List<CustomerDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CustomerAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<CustomerDto>> Handle(CustomerAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""Name"", c.""CompanyName"", c.""Email"", c.""Phone"",
                    c.""Mobile"", c.""Address"", c.""City"", c.""State"", c.""PostalCode"",
                    c.""Country"", c.""Status"", c.""Type"", c.""Industry"", c.""Description"",
                    c.""AnnualRevenue"", c.""EmployeeCount"", c.""Website"", c.""Tags"",
                    c.""LifetimeValue"", c.""TotalOrders"", c.""IsActive"",
                    c.""CreatedAt"", c.""UpdatedAt"",
                    (SELECT COUNT(*) FROM ""Contacts"" WHERE ""CustomerId"" = c.""Id"" AND ""IsDeleted"" = false) as ""ContactCount"",
                    (SELECT COUNT(*) FROM ""Opportunities"" WHERE ""CustomerId"" = c.""Id"" AND ""IsDeleted"" = false) as ""OpportunityCount""
                FROM ""Customers"" c
                WHERE c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (request.Filter != null)
            {
                if (!string.IsNullOrEmpty(request.Filter.Search))
                {
                    sql += @" AND (c.""Name"" ILIKE @Search OR c.""Email"" ILIKE @Search OR c.""CompanyName"" ILIKE @Search)";
                    parameters.Add("@Search", $"%{request.Filter.Search}%");
                }

                if (!string.IsNullOrEmpty(request.Filter.Status))
                {
                    var status = Enum.Parse<CustomerStatus>(request.Filter.Status);
                    sql += @" AND c.""Status"" = @Status";
                    parameters.Add("@Status", status);
                }

                if (!string.IsNullOrEmpty(request.Filter.Type))
                {
                    var type = Enum.Parse<CustomerType>(request.Filter.Type);
                    sql += @" AND c.""Type"" = @Type";
                    parameters.Add("@Type", type);
                }

                if (!string.IsNullOrEmpty(request.Filter.Industry))
                {
                    var industry = Enum.Parse<Industry>(request.Filter.Industry);
                    sql += @" AND c.""Industry"" = @Industry";
                    parameters.Add("@Industry", industry);
                }

                if (request.Filter.Page.HasValue && request.Filter.PageSize.HasValue)
                {
                    var offset = (request.Filter.Page.Value - 1) * request.Filter.PageSize.Value;
                    sql += $" ORDER BY c.\"CreatedAt\" DESC LIMIT {request.Filter.PageSize.Value} OFFSET {offset}";
                }
            }

            var data = await _dapper.QueryAsync<CustomerDto>(sql, parameters, ct);
            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all customers");
            throw;
        }
    }
}

// ============================================================
// GET CUSTOMER BY ID
// ============================================================

public class CustomerByIdHandler : IRequestHandler<CustomerByIdQry, CustomerDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public CustomerByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<CustomerDto?> Handle(CustomerByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""Name"", c.""CompanyName"", c.""Email"", c.""Phone"",
                    c.""Mobile"", c.""Address"", c.""City"", c.""State"", c.""PostalCode"",
                    c.""Country"", c.""Status"", c.""Type"", c.""Industry"", c.""Description"",
                    c.""AnnualRevenue"", c.""EmployeeCount"", c.""Website"", c.""Tags"",
                    c.""LifetimeValue"", c.""TotalOrders"", c.""IsActive"",
                    c.""CreatedAt"", c.""UpdatedAt"",
                    (SELECT COUNT(*) FROM ""Contacts"" WHERE ""CustomerId"" = c.""Id"" AND ""IsDeleted"" = false) as ""ContactCount"",
                    (SELECT COUNT(*) FROM ""Opportunities"" WHERE ""CustomerId"" = c.""Id"" AND ""IsDeleted"" = false) as ""OpportunityCount""
                FROM ""Customers"" c
                WHERE c.""Id"" = @Id AND c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<CustomerDto>(sql, parameters, ct);
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer by ID: {CustomerId}", request.Id);
            throw;
        }
    }
}