using Common;
using Helpers;
using Leave.Domain.DTOs;
using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Queries;

public class MyPendLeaveQry : IQuery<MyPendLvList?> { public Guid Id { get; set; } }
public class DeptPendLeaveQry : IQuery<List<PendLvReqList>> { public Guid Id { get; set; } }
public class BraPendLeaveQry : IQuery<List<PendLvReqList>> { public Guid Id { get; set; } }
public class AllPendLeaveQry : IQuery<List<PendLvReqList>> { }



public class MyPendLeave(IDapperHelper _dapper, ILvReqAppService _lvReqApp) : IQueryHandler<MyPendLeaveQry, MyPendLvList?>
{
    public async Task<MyPendLvList?> Handle(MyPendLeaveQry request, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(Status.Pending);
        const string v = "v";
        const string c = "c";
      var qb = new QueryBuilder()
          .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.Comments, x => x.EmployeeId, x => x.LeaveTypeId, x => x.CurrentAppStep, x => x.xmin)
          .SelectAs<LeaveRequest, MyPendLvList>(v, x => x.DateAdd, d => d.DateRequested)
          .SelectAs<LeaveType, MyPendLvList>(c, x => x.Name, d => d.LeaveType)
          .From<LeaveRequest>(v)
          .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
          .Where<LeaveRequest>(v, x => x.EmployeeId == request.Id && x.Status == stat)
          .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QryFoD<MyPendLvList>(sql, parameters, ct);
        if (data == null) { return null; }

        var dto = new AppStepQryDto
        {
            Id = data.Id,
            EmpId = data.EmployeeId,
            LeaveTypeId = data.LeaveTypeId,
            Cur = data.CurrentAppStep
        };
        var steps = await _lvReqApp.AppSteps(dto, ct);
        data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
        data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
        data.AppStep = [.. steps.OrderBy(x => x.Step)];
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class DeptPendLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro) : IQueryHandler<DeptPendLeaveQry, List<PendLvReqList>>
{
    public async Task<List<PendLvReqList>> Handle(DeptPendLeaveQry request, CancellationToken ct)
    {

        var eCodeTask = _hrmPro.GetEmpCodeList(ct);
        var eIdsTask = _hrmPro.GetEmpId(request.Id.ToString(), ct);
        await Task.WhenAll(eCodeTask, eIdsTask);
        var eCodeDict = eCodeTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var eIds = eIdsTask.Result;
        if (string.IsNullOrWhiteSpace(eIds.DeptId)) { return []; }

        var stat = BoolToStr.EnumToString(Status.Pending);
        var deptId = Guid.Parse(eIds.DeptId);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.EmployeeId, x => x.xmin)
            .SelectAs<LeaveRequest, PendLvReqList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, PendLvReqList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.DeptId == deptId && x.Status == stat)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<PendLvReqList>(ct);

        foreach (var data in list)
        {
            eCodeDict.TryGetValue(data.EmployeeId, out var emp);
            data.EmpName = emp?.Name ?? "";
            data.Code = emp?.Code ?? "";
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class BraPendLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro) : IQueryHandler<BraPendLeaveQry, List<PendLvReqList>>
{
    public async Task<List<PendLvReqList>> Handle(BraPendLeaveQry request, CancellationToken ct)
    {
        var eCodeTask = _hrmPro.GetEmpCodeList(ct);
        var eIdsTask = _hrmPro.GetEmpId(request.Id.ToString(), ct);
        await Task.WhenAll(eCodeTask, eIdsTask);
        var eCodeDict = eCodeTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var eIds = eIdsTask.Result;
        if (string.IsNullOrWhiteSpace(eIds.BranchId)) { return []; }

        var stat = BoolToStr.EnumToString(Status.Pending);
        var braId = Guid.Parse(eIds.BranchId);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.EmployeeId, x => x.xmin)
            .SelectAs<LeaveRequest, PendLvReqList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, PendLvReqList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.BranchId == braId && x.Status == stat)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<PendLvReqList>(ct);

        foreach (var data in list)
        {
            eCodeDict.TryGetValue(data.EmployeeId, out var emp);
            data.EmpName = emp?.Name ?? "";
            data.Code = emp?.Code ?? "";
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class AllPendLeave(IDapperHelper _dapper, IHrmProfileClient _hrmPro) : IQueryHandler<AllPendLeaveQry, List<PendLvReqList>>
{
    public async Task<List<PendLvReqList>> Handle(AllPendLeaveQry request, CancellationToken ct)
    {
        var eCodeTask = await _hrmPro.GetEmpCodeList(ct);
        var eCodeDict = eCodeTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        var stat = BoolToStr.EnumToString(Status.Pending);
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.Id, x => x.StartDate, x => x.EndDate, x => x.DaysRequested, x => x.IsHalfDay, x => x.PerApp, x => x.Status, x => x.EmployeeId, x => x.xmin)
            .SelectAs<LeaveRequest, PendLvReqList>(v, x => x.DateAdd, d => d.DateRequested)
            .SelectAs<LeaveType, PendLvReqList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Where<LeaveRequest>(v, x => x.Status == stat)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<PendLvReqList>(ct);

        foreach (var data in list)
        {
            eCodeDict.TryGetValue(data.EmployeeId, out var emp);
            data.EmpName = emp?.Name ?? "";
            data.Code = emp?.Code ?? "";
            data.Status = MyEnumHelper.FormatEnum<Status>(data.Status);
            data.DaysRequestedStr = BoolToStr.ToLvReqDay(data.DaysRequested, data.IsHalfDay);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}