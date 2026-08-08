// Leave.App/Queries/LvReqOnLeaveDbQry.cs

using Dapper;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using MediatR;

namespace Leave.App.Queries;

public class LvReqOnLeaveDbQry : IRequest<List<LeaveReqOnLeaveDto>> { }

public class LvReqOnLeaveDbHandler : IRequestHandler<LvReqOnLeaveDbQry, List<LeaveReqOnLeaveDto>>
{
    private readonly IDapperHelper _dapper;

    public LvReqOnLeaveDbHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<LeaveReqOnLeaveDto>> Handle(LvReqOnLeaveDbQry request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;

        const string sql = @"
            SELECT
                lr.""Id"" as RequestId,
                lr.""EmployeeId"",
                lr.""LeaveTypeId"",
                lr.""StartDate"",
                lr.""EndDate"",
                lr.""DaysRequested"",
                lr.""IsHalfDay"",
                lr.""Status"",
                lr.""Comments"",
                lr.""DateAdd"" as RequestedDate,
                lt.""Name"" as LeaveTypeName,
                e.""FirstName"" || ' ' || e.""LastName"" as EmployeeName,
                e.""FirstNameAm"" || ' ' || e.""LastNameAm"" as EmployeeNameAm,
                e.""Code"" as EmployeeCode,
                e.""Gender"" as Gender,
                d.""Name"" as DepartmentName,
                d.""NameAm"" as DepartmentNameAm,
                p.""Name"" as PositionName,
                p.""NameAm"" as PositionNameAm,
                b.""Name"" as BranchName
            FROM ""LeaveRequest"" lr
            INNER JOIN ""LeaveType"" lt ON lr.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            INNER JOIN ""LocalEmployees"" e ON lr.""EmployeeId"" = e.""Id"" AND e.""IsDeleted"" = false
            LEFT JOIN ""LocalDepartments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""LocalPositions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            LEFT JOIN ""LocalBranches"" b ON e.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            WHERE lr.""Status"" = 'Approved'
                AND lr.""IsDeleted"" = false
                AND lr.""StartDate"" <= @Today
                AND lr.""EndDate"" >= @Today
            ORDER BY lr.""EndDate"" ASC";

        var result = await _dapper.QueryAsync<LeaveReqOnLeaveDto>(sql, new { Today = today }, ct);
        return result.ToList();
    }
}