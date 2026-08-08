// Cor.CRM/Queries/ContactQry.cs

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

public class ContactAllQry : IRequest<List<ContactDto>>
{
    public string? CustomerId { get; set; }
    public string? Search { get; set; }
}

public class ContactByIdQry : IRequest<ContactDto?>
{
    public Guid Id { get; set; }
}

// ============================================================
// GET ALL CONTACTS
// ============================================================

public class ContactAllHandler : IRequestHandler<ContactAllQry, List<ContactDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ContactAllHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<List<ContactDto>> Handle(ContactAllQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""FirstName"", c.""LastName"", c.""Email"", c.""Phone"",
                    c.""Mobile"", c.""Title"", c.""Department"", c.""CustomerId"",
                    c.""IsPrimary"", c.""IsDecisionMaker"", c.""Notes"",
                    c.""AcceptsEmail"", c.""AcceptsSMS"", c.""AcceptsCalls"",
                    c.""AcceptsMarketing"", c.""PreferredContactMethod"",
                    c.""LinkedIn"", c.""Twitter"", c.""Facebook"",
                    c.""IsActive"", c.""CreatedAt"", c.""UpdatedAt"",
                    c.""LastContactDate"", c.""ContactCount"",
                    cust.""Name"" as ""CustomerName""
                FROM ""Contacts"" c
                LEFT JOIN ""Customers"" cust ON c.""CustomerId"" = cust.""Id""
                WHERE c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(request.CustomerId) && Guid.TryParse(request.CustomerId, out var customerId))
            {
                sql += " AND c.\"CustomerId\" = @CustomerId";
                parameters.Add("@CustomerId", customerId);
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                sql += @" AND (c.""FirstName"" ILIKE @Search OR c.""LastName"" ILIKE @Search OR c.""Email"" ILIKE @Search)";
                parameters.Add("@Search", $"%{request.Search}%");
            }

            sql += " ORDER BY c.\"IsPrimary\" DESC, c.\"LastName\" ASC";

            var data = await _dapper.QueryAsync<ContactDto>(sql, parameters, ct);

            // Set FullName for each contact
            foreach (var contact in data)
            {
                contact.FullName = $"{contact.FirstName} {contact.LastName}".Trim();
            }

            return data.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all contacts");
            throw;
        }
    }
}

// ============================================================
// GET CONTACT BY ID
// ============================================================

public class ContactByIdHandler : IRequestHandler<ContactByIdQry, ContactDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ILogService _logger;

    public ContactByIdHandler(IDapperHelper dapper, ILogService logger)
    {
        _dapper = dapper;
        _logger = logger;
    }

    public async Task<ContactDto?> Handle(ContactByIdQry request, CancellationToken ct)
    {
        try
        {
            var sql = @"
                SELECT
                    c.""Id"", c.""FirstName"", c.""LastName"", c.""Email"", c.""Phone"",
                    c.""Mobile"", c.""Title"", c.""Department"", c.""CustomerId"",
                    c.""IsPrimary"", c.""IsDecisionMaker"", c.""Notes"",
                    c.""AcceptsEmail"", c.""AcceptsSMS"", c.""AcceptsCalls"",
                    c.""AcceptsMarketing"", c.""PreferredContactMethod"",
                    c.""LinkedIn"", c.""Twitter"", c.""Facebook"",
                    c.""IsActive"", c.""CreatedAt"", c.""UpdatedAt"",
                    c.""LastContactDate"", c.""ContactCount"",
                    cust.""Name"" as ""CustomerName""
                FROM ""Contacts"" c
                LEFT JOIN ""Customers"" cust ON c.""CustomerId"" = cust.""Id""
                WHERE c.""Id"" = @Id AND c.""IsDeleted"" = false
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", request.Id);

            var data = await _dapper.QueryFirstOrDefaultAsync<ContactDto>(sql, parameters, ct);

            if (data != null)
            {
                data.FullName = $"{data.FirstName} {data.LastName}".Trim();
            }

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get contact by ID: {ContactId}", request.Id);
            throw;
        }
    }
}