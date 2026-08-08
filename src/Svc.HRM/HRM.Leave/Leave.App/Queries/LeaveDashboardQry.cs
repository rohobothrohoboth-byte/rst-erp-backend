using Common;
using Dapper;
using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using MediatR;

namespace Leave.App.Queries;

// ============================================================
// LEAVE DASHBOARD QUERIES
// ============================================================

public class GetPendingLeaveRequestsQry : IRequest<List<LeaveRequestPendingDto>>
{
    public int Limit { get; set; } = 10;
}

public class GetEmployeesOnLeaveQry : IRequest<List<EmployeeOnLeaveDto>> { }

public class GetLeaveBalanceSummaryQry : IRequest<LeaveBalanceSummaryDto> { }

public class GetLeaveStatisticsQry : IRequest<LeaveStatisticsDto> { }

// ============================================================
// DTOs
// ============================================================

public class LeaveRequestPendingDto
{
    public Guid RequestId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNameAm { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public string DaysRequestedStr { get; set; } = string.Empty;
    public bool IsHalfDay { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public string DepartmentAm { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string BranchAm { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string RowVersion { get; set; } = string.Empty;
}

public class EmployeeOnLeaveDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeNameAm { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DaysRequested { get; set; }
    public string DaysRequestedStr { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string DepartmentAm { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class LeaveBalanceByTypeDto
{
    public string LeaveType { get; set; } = string.Empty;
    public double Assigned { get; set; }
    public double Used { get; set; }
    public double Remaining { get; set; }
    public double PercentageUsed { get; set; }
    public string AssignedStr { get; set; } = string.Empty;
    public string UsedStr { get; set; } = string.Empty;
    public string RemainingStr { get; set; } = string.Empty;
}

public class LeaveBalanceSummaryDto
{
    public List<LeaveBalanceByTypeDto> LeaveBalances { get; set; } = new();
    public double TotalAssigned { get; set; }
    public double TotalUsed { get; set; }
    public double TotalRemaining { get; set; }
}

public class LeaveStatisticsDto
{
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Cancelled { get; set; }
    public int Total { get; set; }
    public int NewThisMonth { get; set; }
    public int ApprovedThisMonth { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class GetPendingLeaveRequestsHandler : IRequestHandler<GetPendingLeaveRequestsQry, List<LeaveRequestPendingDto>>
{
    private readonly IDapperHelper _dapper;

    public GetPendingLeaveRequestsHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<LeaveRequestPendingDto>> Handle(GetPendingLeaveRequestsQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                lr.""Id"" AS RequestId,
                lr.""EmployeeId"",
                lr.""StartDate"",
                lr.""EndDate"",
                lr.""DaysRequested"",
                lr.""IsHalfDay"",
                lr.""Status"",
                lr.""DateAdd"" AS RequestedDate,
                lt.""Name"" AS LeaveType,
                e.""FirstName"" || ' ' || e.""LastName"" AS EmployeeName,
                e.""FirstNameAm"" || ' ' || e.""LastNameAm"" AS EmployeeNameAm,
                e.""Code"" AS EmployeeCode,
                e.""Gender"",
                d.""Name"" AS Department,
                d.""NameAm"" AS DepartmentAm,
                b.""Name"" AS Branch,
                b.""NameAm"" AS BranchAm,
                p.""Name"" AS Position
            FROM ""LeaveRequest"" lr
            INNER JOIN ""LeaveType"" lt ON lr.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            INNER JOIN ""LocalEmployees"" e ON lr.""EmployeeId"" = e.""Id"" AND e.""IsDeleted"" = false
            LEFT JOIN ""LocalDepartments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""LocalBranches"" b ON e.""BranchId"" = b.""Id"" AND b.""IsDeleted"" = false
            LEFT JOIN ""LocalPositions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            WHERE lr.""Status"" = '0'
              AND lr.""IsDeleted"" = false
            ORDER BY lr.""DateAdd"" DESC
            LIMIT @Limit";

        var results = await _dapper.QueryAsync<LeaveRequestPendingDto>(sql, new { request.Limit }, ct);
        var list = results.ToList();

        foreach (var item in list)
        {
            item.DaysRequestedStr = BoolToStr.ToLvReqDay(item.DaysRequested, item.IsHalfDay);
            item.Status = "Pending";
        }

        return list;
    }
}

public class GetEmployeesOnLeaveHandler : IRequestHandler<GetEmployeesOnLeaveQry, List<EmployeeOnLeaveDto>>
{
    private readonly IDapperHelper _dapper;

    public GetEmployeesOnLeaveHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<EmployeeOnLeaveDto>> Handle(GetEmployeesOnLeaveQry request, CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;

        const string sql = @"
            SELECT
                lr.""EmployeeId"",
                lr.""StartDate"",
                lr.""EndDate"",
                lr.""DaysRequested"",
                lr.""IsHalfDay"",
                lr.""Status"",
                lt.""Name"" AS LeaveType,
                e.""FirstName"" || ' ' || e.""LastName"" AS EmployeeName,
                e.""FirstNameAm"" || ' ' || e.""LastNameAm"" AS EmployeeNameAm,
                e.""Code"" AS EmployeeCode,
                e.""Gender"",
                d.""Name"" AS Department,
                d.""NameAm"" AS DepartmentAm,
                p.""Name"" AS Position
            FROM ""LeaveRequest"" lr
            INNER JOIN ""LeaveType"" lt ON lr.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            INNER JOIN ""LocalEmployees"" e ON lr.""EmployeeId"" = e.""Id"" AND e.""IsDeleted"" = false
            LEFT JOIN ""LocalDepartments"" d ON e.""DepartmentId"" = d.""Id"" AND d.""IsDeleted"" = false
            LEFT JOIN ""LocalPositions"" p ON e.""PositionId"" = p.""Id"" AND p.""IsDeleted"" = false
            WHERE lr.""Status"" = '1'
              AND lr.""IsDeleted"" = false
              AND lr.""StartDate"" <= @Today
              AND lr.""EndDate"" >= @Today
            ORDER BY lr.""EndDate"" ASC";

        var results = await _dapper.QueryAsync<EmployeeOnLeaveDto>(sql, new { Today = today }, ct);
        var list = results.ToList();

        foreach (var item in list)
        {
            item.DaysRequestedStr = BoolToStr.ToLvReqDay(item.DaysRequested, false);
            item.Status = "On Leave";
        }

        return list;
    }
}

public class GetLeaveBalanceSummaryHandler : IRequestHandler<GetLeaveBalanceSummaryQry, LeaveBalanceSummaryDto>
{
    private readonly IDapperHelper _dapper;

    public GetLeaveBalanceSummaryHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<LeaveBalanceSummaryDto> Handle(GetLeaveBalanceSummaryQry request, CancellationToken ct)
    {
        const string sql = @"
            SELECT
                lt.""Name"" AS LeaveType,
                COALESCE(SUM(ep.""AssignedEntitlement""), 0) AS Assigned,
                COALESCE(SUM(ep.""UsedEntitlement""), 0) AS Used,
                COALESCE(SUM(ep.""AssignedEntitlement"" - ep.""UsedEntitlement""), 0) AS Remaining
            FROM ""EmpLeavePolicy"" ep
            INNER JOIN ""LeaveType"" lt ON ep.""LeaveTypeId"" = lt.""Id"" AND lt.""IsDeleted"" = false
            WHERE ep.""IsDeleted"" = false
              AND (ep.""EffectiveTo"" IS NULL OR ep.""EffectiveTo"" >= CURRENT_DATE)
            GROUP BY lt.""Id"", lt.""Name""
            ORDER BY lt.""Name""";

        var results = await _dapper.QueryAsync<LeaveBalanceByTypeDto>(sql, null, ct);
        var list = results.ToList();

        foreach (var item in list)
        {
            item.PercentageUsed = item.Assigned > 0 ? (item.Used / item.Assigned) * 100 : 0;
            item.AssignedStr = $"{item.Assigned:F2} days";
            item.UsedStr = $"{item.Used:F2} days";
            item.RemainingStr = $"{item.Remaining:F2} days";
        }

        return new LeaveBalanceSummaryDto
        {
            LeaveBalances = list,
            TotalAssigned = list.Sum(x => x.Assigned),
            TotalUsed = list.Sum(x => x.Used),
            TotalRemaining = list.Sum(x => x.Remaining)
        };
    }
}

public class GetLeaveStatisticsHandler : IRequestHandler<GetLeaveStatisticsQry, LeaveStatisticsDto>
{
    private readonly IDapperHelper _dapper;

    public GetLeaveStatisticsHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<LeaveStatisticsDto> Handle(GetLeaveStatisticsQry request, CancellationToken ct)
    {
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        // ✅ FIXED: Use proper column names and ensure correct casting
        const string sql = @"
            SELECT
                COALESCE(COUNT(*) FILTER (WHERE ""Status"" = '0'), 0) AS Pending,
                COALESCE(COUNT(*) FILTER (WHERE ""Status"" = '1'), 0) AS Approved,
                COALESCE(COUNT(*) FILTER (WHERE ""Status"" = '2'), 0) AS Rejected,
                COALESCE(COUNT(*) FILTER (WHERE ""Status"" = '3'), 0) AS Cancelled,
                COALESCE(COUNT(*), 0) AS Total,
                COALESCE(COUNT(*) FILTER (WHERE ""Status"" = '0' AND ""DateAdd"" >= @FirstDay), 0) AS NewThisMonth,
                COALESCE(COUNT(*) FILTER (WHERE ""Status"" = '1' AND ""DateMod"" >= @FirstDay), 0) AS ApprovedThisMonth
            FROM ""LeaveRequest""
            WHERE ""IsDeleted"" = false";

        var result = await _dapper.QueryFirstOrDefaultAsync<LeaveStatisticsDto>(sql, new { FirstDay = firstDayOfMonth }, ct);

        // ✅ If result is null, return default
        if (result == null)
        {
            return new LeaveStatisticsDto
            {
                Pending = 0,
                Approved = 0,
                Rejected = 0,
                Cancelled = 0,
                Total = 0,
                NewThisMonth = 0,
                ApprovedThisMonth = 0
            };
        }

        return result;
    }
}