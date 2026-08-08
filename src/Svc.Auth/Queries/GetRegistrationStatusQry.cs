using MediatR;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Svc.Auth.Queries;

public class GetRegistrationStatusQry : IRequest<RegistrationStatusDto>
{
    public string EmployeeId { get; set; } = default!;
}

public class RegistrationStatusDto
{
    public bool EmployeeExists { get; set; }
    public bool HasAccount { get; set; }
    public string? UserId { get; set; }
    public string? EmployeeName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class GetRegistrationStatusHandler : IRequestHandler<GetRegistrationStatusQry, RegistrationStatusDto>
{
    private readonly IConfiguration _configuration;

    public GetRegistrationStatusHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<RegistrationStatusDto> Handle(GetRegistrationStatusQry request, CancellationToken ct)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeGuid))
        {
            return new RegistrationStatusDto
            {
                EmployeeExists = false,
                HasAccount = false
            };
        }

        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        // Check employee from LOCAL COPY
        const string empSql = @"
            SELECT
                e.""Id"",
                e.""FirstName"",
                e.""LastName"",
                e.""Email""
            FROM ""Employees"" e
            WHERE e.""Id"" = @EmployeeId
            AND e.""IsDeleted"" = false";

        var employee = await connection.QueryFirstOrDefaultAsync<dynamic>(
            empSql,
            new { EmployeeId = employeeGuid });

        if (employee == null)
        {
            return new RegistrationStatusDto
            {
                EmployeeExists = false,
                HasAccount = false
            };
        }

        // Check if user exists
        const string userSql = @"
            SELECT ""Id"", ""IsActive""
            FROM ""AppUser""
            WHERE ""EmployeeId"" = @EmployeeId";

        var appUser = await connection.QueryFirstOrDefaultAsync<dynamic>(
            userSql,
            new { EmployeeId = employeeGuid });

        return new RegistrationStatusDto
        {
            EmployeeExists = true,
            HasAccount = appUser != null,
            UserId = appUser?.Id?.ToString(),
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            Email = employee.Email,
            IsActive = appUser?.IsActive == true
        };
    }
}