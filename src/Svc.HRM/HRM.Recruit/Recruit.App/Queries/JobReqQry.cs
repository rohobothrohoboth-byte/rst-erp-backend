using Common;
using Dapper;
using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JobReqAllQry : IRequest<List<JobReqListDto>> { }
public class JobReqAllByWfpIdQry : IRequest<List<WfpJobReqListDto>> { public Guid Id { get; set; } }
public class JobReqByIdQry : IRequest<JobReqListDto?> { public Guid Id { get; set; } }
public class JobReqDetailQry : IRequest<JobReqDetailDto?> { public Guid Id { get; set; } }



public class JobReqAllQryHandler : IRequestHandler<JobReqAllQry, List<JobReqListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    public JobReqAllQryHandler(IDapperHelper dapper, ICorHrmmClient corHrmm)
    {
        _dapper = dapper;
        _corHrmm = corHrmm;
    }
    public async Task<List<JobReqListDto>> Handle(JobReqAllQry request, CancellationToken ct)
    {
        var jgsTask = _corHrmm.GetListJgStep(ct);
        var posTask = _corHrmm.GetListPosition(ct);
        await Task.WhenAll(jgsTask, posTask);
        var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

        const string e = "e";
        const string w = "w";
        var qb = new QueryBuilder()
            .Select<JobRequisition>(e, x => x.Id, x => x.WorkforcePlanId, x => x.JobDecId, x => x.ReqNumber, x => x.ReqReason, x => x.ReqQuantity, x => x.BudgetCode, x => x.Status, x => x.StartDate, x => x.PositionId, x => x.JgStepId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .SelectAs<WorkforcePlan, JobReqListDto>(w, x => x.PlanCode, x => x.WfpCode)
            .From<JobRequisition>(e)
            .Join<JobRequisition, WorkforcePlan>(e, w, x => x.WorkforcePlanId, x => x.Id)
            .OrderBy<JobRequisition>(e, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JobReqListDto>(ct);

        foreach (var data in list)
        {
            jgsDict.TryGetValue(data.JgStepId, out var jgs);
            posDict.TryGetValue(data.PositionId, out var pos);

            data.StatusStr = MyEnumHelper.FormatEnum<ReqStatus>(data.Status);
            data.Position = pos?.Name ?? "";
            data.JgStep = jgs?.Name ?? "";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobReqAllByWfpIdHandler : IRequestHandler<JobReqAllByWfpIdQry, List<WfpJobReqListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    public JobReqAllByWfpIdHandler(IDapperHelper dapper, ICorHrmmClient corHrmm)
    {
        _dapper = dapper;
        _corHrmm = corHrmm;
    }
    public async Task<List<WfpJobReqListDto>> Handle(JobReqAllByWfpIdQry request, CancellationToken ct)
    {
        var jgsTask = _corHrmm.GetListJgStep(ct);
        var posTask = _corHrmm.GetListPosition(ct);
        await Task.WhenAll(jgsTask, posTask);
        var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));

        const string e = "e";
        var qb = new QueryBuilder()
            .Select<JobRequisition>(e, x => x.Id, x => x.JobDecId, x => x.ReqNumber, x => x.ReqReason, x => x.ReqQuantity, x => x.BudgetCode, x => x.Status, x => x.StartDate, x => x.PositionId, x => x.JgStepId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobRequisition>(e)
            .Where<JobRequisition>(e, x => x.WorkforcePlanId == request.Id)
            .OrderBy<JobRequisition>(e, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<WfpJobReqListDto>(ct);

        foreach (var data in list)
        {
            jgsDict.TryGetValue(data.JgStepId, out var jgs);
            posDict.TryGetValue(data.PositionId, out var pos);

            data.StatusStr = MyEnumHelper.FormatEnum<ReqStatus>(data.Status);
            data.Position = pos?.Name ?? "";
            data.JgStep = jgs?.Name ?? "";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobReqDetailHandler : IRequestHandler<JobReqDetailQry, JobReqDetailDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    public JobReqDetailHandler(IDapperHelper dapper, ICorHrmmClient corHrmm)
    {
        _dapper = dapper;
        _corHrmm = corHrmm;
    }
    public async Task<JobReqDetailDto?> Handle(JobReqDetailQry request, CancellationToken ct)
    {
        const string e = "e";
        const string jd = "jd";
        var qb = new QueryBuilder()
            .Select<JobRequisition>(e, x => x.Id, x => x.JobDecId, x => x.ReqNumber, x => x.ReqReason, x => x.ReqQuantity, x => x.BudgetCode, x => x.Status, x => x.StartDate, x => x.PositionId, x => x.JgStepId, x => x.xmin)
            .Select<JobDec>(jd, x => x.KeyRespo, x => x.Desc, x => x.ReqQual, x => x.KeySkills, x => x.WorkLocation, x => x.PreGender, x => x.EmpNature, x => x.WorkArr)
            .From<JobRequisition>(e)
            .Join<JobRequisition, JobDec>(e, jd, x => x.JobDecId, x => x.Id)
            .Where<JobRequisition>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobReqDetailDto>(sql, parameters, ct);
        if (data == null) return null;

        var jgsTask = _corHrmm.GetListJgStep(ct);
        var posTask = _corHrmm.GetListPosition(ct);
        await Task.WhenAll(jgsTask, posTask);
        var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        jgsDict.TryGetValue(data.JgStepId, out var jgs);
        posDict.TryGetValue(data.PositionId, out var pos);

        data.StatusStr = MyEnumHelper.FormatEnum<ReqStatus>(data.Status);
        data.Position = pos?.Name ?? "";
        data.JgStep = jgs?.Name ?? "";
        data.RowVersion = data.xmin.ToString();

        return data;
    }
}

public class JobReqByIdHandler : IRequestHandler<JobReqByIdQry, JobReqListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHrmm;
    public JobReqByIdHandler(IDapperHelper dapper, ICorHrmmClient corHrmm)
    {
        _dapper = dapper;
        _corHrmm = corHrmm;
    }
    public async Task<JobReqListDto?> Handle(JobReqByIdQry request, CancellationToken ct)
    {
        const string e = "e";
        var qb = new QueryBuilder()
            .Select<JobRequisition>(e, x => x.Id, x => x.JobDecId, x => x.ReqNumber, x => x.ReqReason, x => x.ReqQuantity, x => x.BudgetCode, x => x.Status, x => x.StartDate, x => x.PositionId, x => x.JgStepId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobRequisition>(e)
            .Where<JobRequisition>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobReqListDto>(sql, parameters, ct);
        if (data == null) return null;

        var jgsTask = _corHrmm.GetListJgStep(ct);
        var posTask = _corHrmm.GetListPosition(ct);
        await Task.WhenAll(jgsTask, posTask);
        var jgsDict = jgsTask.Result.Res.ToDictionary(j => Guid.Parse(j.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        jgsDict.TryGetValue(data.JgStepId, out var jgs);
        posDict.TryGetValue(data.PositionId, out var pos);

        data.StatusStr = MyEnumHelper.FormatEnum<ReqStatus>(data.Status);
        data.Position = pos?.Name ?? "";
        data.JgStep = jgs?.Name ?? "";
        data.RowVersion = data.xmin.ToString();

        return data;
    }
}