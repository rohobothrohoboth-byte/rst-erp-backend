// Profile.App/Queries/GetRecentActivitiesQry.cs

using Dapper;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;

namespace Profile.App.Queries;

public class GetRecentActivitiesQry : IRequest<List<ActivityDto>>
{
    public int Limit { get; set; } = 10;
}

public class GetRecentActivitiesHandler : IRequestHandler<GetRecentActivitiesQry, List<ActivityDto>>
{
    private readonly IDapperHelper _dapper;

    public GetRecentActivitiesHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<ActivityDto>> Handle(GetRecentActivitiesQry request, CancellationToken ct)
    {
        // ✅ FIXED: Join with Person table to get names
        const string sql = @"
            SELECT
                e.""Id"" as EmployeeId,
                p.""FirstName"" || ' ' || p.""MiddleName"" || ' ' || p.""LastName"" as EmployeeName,
                p.""FirstNameAm"" || ' ' || p.""MiddleNameAm"" || ' ' || p.""LastNameAm"" as EmployeeNameAm,
                e.""Code"" as EmployeeCode,
                d.""Name"" as DepartmentName,
                pos.""Name"" as PositionName,
                e.""EmpState"" as Status,
                e.""DateAdd"" as ActivityDate,
                'status_change' as ActivityType
            FROM ""Employee"" e
            INNER JOIN ""Person"" p ON e.""PersonId"" = p.""Id""
            LEFT JOIN ""Departments"" d ON e.""DepartmentId"" = d.""Id""
            LEFT JOIN ""Positions"" pos ON e.""PositionId"" = pos.""Id""
            WHERE e.""IsDeleted"" = false
            ORDER BY e.""DateAdd"" DESC
            LIMIT @Limit";

        var result = await _dapper.QueryAsync<ActivityDto>(sql, new { request.Limit }, ct);
        return result.ToList();
    }
}