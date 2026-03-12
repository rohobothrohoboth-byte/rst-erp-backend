using Common;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class WorkforcePlanAllQry : IRequest<List<WorkforcePlanListDto>> { public Guid Id { get; set; } }
public class WorkforcePlanByIdQry : IRequest<WorkforcePlanListDto?> { public Guid Id { get; set; } }



public class WorkforcePlanAllHandler : IRequestHandler<WorkforcePlanAllQry, List<WorkforcePlanListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    private readonly IHrmProfileClient _hrmProfile;

    public WorkforcePlanAllHandler(IDapperHelper dapper, ICorModClient corMod, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _corMod = corMod;
        _hrmProfile = hrmProfile;
    }

    public async Task<List<WorkforcePlanListDto>> Handle(WorkforcePlanAllQry request, CancellationToken ct)
    {
        var deptTask = _corMod.GetListDept(ct);
        var perTask = _corMod.GetListPeriod(ct);
        var empTask = _hrmProfile.GetListEmp(ct);
        await Task.WhenAll(empTask, deptTask, perTask);
        var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var empDict = empTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var perDict = perTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

        const string v = "v";
        var qb = new QueryBuilder()
            .Select<WorkforcePlan>(v, x => x.Id, x => x.PlanCode, x => x.Title, x => x.Desc, x => x.StartDate, x => x.EndDate, x => x.TotalPositions, x => x.AppPositions, x => x.Status, x => x.DepartmentId, x => x.PeriodId, x => x.RequistionById, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<WorkforcePlan>(v)
            .OrderBy<WorkforcePlan>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<WorkforcePlanListDto>(ct);

        foreach (var data in list)
        {
            deptDict.TryGetValue(data.DepartmentId, out var dept);
            empDict.TryGetValue(data.RequistionById, out var emp);
            var per = "";
            if (data.PeriodId.HasValue)
            {
                perDict.TryGetValue((Guid)data.PeriodId, out var per2);
                per = per2?.Name ?? "";
            }

            data.Department = dept?.Name ?? "";
            data.RequistionBy = emp?.Name ?? "";
            data.Period = per;
            data.StatusStr = MyEnumHelper.FormatEnum<ReqStatus>(data.Status);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class WorkforcePlanByIdHandler : IRequestHandler<WorkforcePlanByIdQry, WorkforcePlanListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;
    private readonly IHrmProfileClient _hrmProfile;

    public WorkforcePlanByIdHandler(IDapperHelper dapper, ICorModClient corMod, IHrmProfileClient hrmProfile)
    {
        _dapper = dapper;
        _corMod = corMod;
        _hrmProfile = hrmProfile;
    }

    public async Task<WorkforcePlanListDto?> Handle(WorkforcePlanByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<WorkforcePlan>(v, x => x.Id, x => x.PlanCode, x => x.Title, x => x.Desc, x => x.StartDate, x => x.EndDate, x => x.TotalPositions, x => x.AppPositions, x => x.Status, x => x.DepartmentId, x => x.PeriodId, x => x.RequistionById, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<WorkforcePlan>(v)
            .OrderBy<WorkforcePlan>(v, x => x.DateAdd, desc: true)
            .Where<WorkforcePlan>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<WorkforcePlanListDto>(sql, parameters, ct);
        if (data == null) return null;

        var deptTask = _corMod.GetDept(data.DepartmentId.ToString(), ct);
        var empTask = _hrmProfile.GetEmp(data.RequistionById.ToString(), ct);
        await Task.WhenAll(empTask, deptTask);
        var dept = deptTask.Result.Res;
        var emp = empTask.Result.Res;
        var per = "";
        if (data.PeriodId.HasValue)
        {
            var perTask = await _corMod.GetPeriod(data.PeriodId!.ToString(), ct);
            per = perTask?.Name ?? "";
        }

        data.Department = dept?.Name ?? "";
        data.RequistionBy = emp?.Name ?? "";
        data.Period = per;
        data.StatusStr = MyEnumHelper.FormatEnum<ReqStatus>(data.Status);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}