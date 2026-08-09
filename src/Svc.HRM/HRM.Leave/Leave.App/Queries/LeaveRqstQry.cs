using Common;
using Helpers;
using Leave.Domain.DTOs;
using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.Entities;

namespace Leave.App.Queries;

public class MyHistLeaveQry : IQuery<List<MyHistLvList>> { public Guid Id { get; set; } }
public class DeptHistLeaveQry : IQuery<List<HistLvReqList>> { public Guid Id { get; set; } }
public class BraHistLeaveQry : IQuery<List<HistLvReqList>> { public Guid Id { get; set; } }
public class AllHistLeaveQry : IQuery<List<HistLvReqList>> { }
public class ViewLeaveQry : IQuery<ViewLvReqDto?> { public Guid Id { get; set; } }



public class MyHistLeave(IDapperHelper _dapper) : IQueryHandler<MyHistLeaveQry, List<MyHistLvList>>
{
    public async Task<List<MyHistLvList>> Handle(MyHistLeaveQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status)
            .SelectAs<LeaveRequest, MyHistLvList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, MyHistLvList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<MyHistLvList>(ct);

        foreach (var data in list)
        {
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
        }

        return list;
    }
}

public class DeptHistLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro) : IQueryHandler<DeptHistLeaveQry, List<HistLvReqList>>
{
    public async Task<List<HistLvReqList>> Handle(DeptHistLeaveQry request, CancellationToken ct)
    {
        var eCodeTask = _hrmPro.GetEmpCodeList(ct);
        var eIdsTask = _hrmPro.GetEmpId(request.Id.ToString(), ct);
        await Task.WhenAll(eCodeTask, eIdsTask);
        var eCodeDict = eCodeTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var eIds = eIdsTask.Result;
        if (string.IsNullOrWhiteSpace(eIds.DeptId)) { return []; }

        var deptId = Guid.Parse(eIds.DeptId);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.EmployeeId)
            .SelectAs<LeaveRequest, HistLvReqList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, HistLvReqList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.DeptId == deptId)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<HistLvReqList>(ct);

        foreach (var data in list)
        {
            eCodeDict.TryGetValue(data.EmployeeId, out var emp);
            data.EmpName = emp?.Name ?? "";
            data.Code = emp?.Code ?? "";
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
        }

        return list;
    }
}

public class BraHistLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro) : IQueryHandler<BraHistLeaveQry, List<HistLvReqList>>
{
    public async Task<List<HistLvReqList>> Handle(BraHistLeaveQry request, CancellationToken ct)
    {
        var eCodeTask = _hrmPro.GetEmpCodeList(ct);
        var eIdsTask = _hrmPro.GetEmpId(request.Id.ToString(), ct);
        await Task.WhenAll(eCodeTask, eIdsTask);
        var eCodeDict = eCodeTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var eIds = eIdsTask.Result;
        if (string.IsNullOrWhiteSpace(eIds.BranchId)) { return []; }

        var braId = Guid.Parse(eIds.BranchId);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.EmployeeId)
            .SelectAs<LeaveRequest, HistLvReqList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, HistLvReqList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.BranchId == braId)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<HistLvReqList>(ct);

        foreach (var data in list)
        {
            eCodeDict.TryGetValue(data.EmployeeId, out var emp);
            data.EmpName = emp?.Name ?? "";
            data.Code = emp?.Code ?? "";
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
        }

        return list;
    }
}

public class AllHistLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro) : IQueryHandler<AllHistLeaveQry, List<HistLvReqList>>
{
    public async Task<List<HistLvReqList>> Handle(AllHistLeaveQry request, CancellationToken ct)
    {
        var eCodeTask = await _hrmPro.GetEmpCodeList(ct);
        var eCodeDict = eCodeTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.EmployeeId)
            .SelectAs<LeaveRequest, HistLvReqList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, HistLvReqList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<HistLvReqList>(ct);

        foreach (var data in list)
        {
            eCodeDict.TryGetValue(data.EmployeeId, out var emp);
            data.EmpName = emp?.Name ?? "";
            data.Code = emp?.Code ?? "";
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
        }

        return list;
    }
}

public class ViewLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro, ILvReqAppService _lvReqApp) : IQueryHandler<ViewLeaveQry, ViewLvReqDto?>
{
    public async Task<ViewLvReqDto?> Handle(ViewLeaveQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.Status, x => x.Comments, x => x.CurrentAppStep, x => x.PerApp, x => x.DateApp!, x => x.EmployeeId, x => x.BranchId, x => x.DeptId, x => x.DateAdd, x => x.LeaveTypeId)
            .SelectAs<LeaveType, ViewLvReqJoin>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QryFoD<ViewLvReqJoin>(sql, parameters, ct);
        if (data is null) { return null; }

        var dto = new AppStepQryDto
        {
            Id = data.Id,
            EmpId = data.EmployeeId,
            LeaveTypeId = data.LeaveTypeId,
            Cur = data.CurrentAppStep
        };

        var eInfoTask = _hrmPro.GetEmpBasicInfo(data.EmployeeId.ToString(), ct);
        var stepsTask = _lvReqApp.AppSteps(dto, ct);
        await Task.WhenAll(eInfoTask, stepsTask);

        var emp = eInfoTask.Result;
        var steps = stepsTask.Result;
        var res = new ViewLvReqDto
        {
            EmployeeId = data.EmployeeId,
            TotalDaysReq = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay),
            Status = MyEnumHelper.FormatEnum<Status>(data.Status),
            StartDate = data.StartDate.ToString("MMMM dd, yyyy"),
            EndDate = data.EndDate.ToString("MMMM dd, yyyy"),
            DateRequested = data.DateAdd.ToString("MMMM dd, yyyy"),
            DateApp = data.DateApp?.ToString("MMMM dd, yyyy") ?? string.Empty,
            EmpName = emp?.EmpFullName ?? string.Empty,
            EmpNameAm = emp?.EmpFullNameAm ?? string.Empty,
            Code = emp?.Code ?? string.Empty,
            Gender = emp?.Gender ?? string.Empty,
            Branch = emp?.Branch ?? string.Empty,
            Dept = emp?.Department ?? string.Empty,
            Position = emp?.Position ?? string.Empty,
            LeaveType = data.LeaveType,
            PerApp = data.PerApp,
            Comments = data.Comments,
            AppStep = steps
        };

        return res;
    }
}