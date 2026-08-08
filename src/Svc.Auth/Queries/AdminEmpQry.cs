using Dapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Auth.Queries;

public class GetAdminEmployeesQry : IRequest<List<AdminEmpListDto>> { }



public class GetAdminEmployeesHandler : IRequestHandler<GetAdminEmployeesQry, List<AdminEmpListDto>>
{
    private readonly IConfiguration _configuration;

    public GetAdminEmployeesHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<AdminEmpListDto>> Handle(GetAdminEmployeesQry request, CancellationToken ct)
    {
        var connectionString = _configuration.GetConnectionString("authMgrCon");
        using var connection = new NpgsqlConnection(connectionString);

        const string sql = @"
            SELECT
                e.""Id""::text as Id,
                e.""Code"",
                e.""FirstName"",
                e.""MiddleName"",
                e.""LastName"",
                e.""FirstNameAm"",
                e.""MiddleNameAm"",
                e.""LastNameAm"",
                e.""Gender"",
                e.""EmpState"",
                e.""EmploymentType"",
                e.""EmploymentNature"",
                e.""WorkArrangement"",
                e.""Email"",
                e.""Phone"",
                e.""PositionId"",
                e.""DepartmentId"",
                e.""JobGradeId"",
                e.""AppUserId"",
                p.""Name"" as PositionName,
                d.""Name"" as DepartmentName,
                b.""Name"" as BranchName,
                j.""Name"" as JobGradeName,
                CASE WHEN u.""Id"" IS NOT NULL THEN true ELSE false END as HasAccount,
                CASE WHEN u.""Id"" IS NOT NULL AND u.""IsActive"" = true THEN true ELSE false END as IsAccountActive
            FROM ""Employees"" e
            LEFT JOIN ""Positions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""Branches"" b ON d.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""JobGrades"" j ON e.""JobGradeId"" = j.""Id"" AND j.""IsDeleted"" = false
            LEFT JOIN ""AppUser"" u ON e.""Id"" = u.""EmployeeId"" AND u.""IsDeleted"" = false
            WHERE
            ORDER BY e.""FirstName"", e.""LastName""";

        var employees = await connection.QueryAsync<AdminEmpListDto>(sql);
        return employees.AsList();
    }
}