using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class EvalTypeAllQry : IRequest<List<EvalTypeListDto>> { }
public class EvalTypeActiveQry : IRequest<List<EvalTypeListDto>> { }
public class EvalTypeByIdQry : IRequest<EvalTypeListDto?> { public Guid Id { get; set; } }
public class EvalFlowAllQry : IRequest<List<EvalFlowListDto>> { }
public class EvalFlowActiveQry : IRequest<List<EvalFlowListDto>> { }
public class EvalFlowByIdQry : IRequest<EvalFlowListDto?> { public Guid Id { get; set; } }
public class EvalStepAllQry : IRequest<List<EvalStepListDto>> { }
public class EvalStepByIdQry : IRequest<EvalStepListDto?> { public Guid Id { get; set; } }
public class EvalStepByFlowIdQry : IRequest<List<EvalStepListDto>> { public Guid Id { get; set; } }
public class JobEvalFlowAllQry : IRequest<List<JobEvalFlowListDto>> { }
public class JobEvalFlowByIdQry : IRequest<JobEvalFlowListDto?> { public Guid Id { get; set; } }



public class EvalTypeAllHandler : IRequestHandler<EvalTypeAllQry, List<EvalTypeListDto>>
{
    private readonly IDapperHelper _dapper;
    public EvalTypeAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EvalTypeListDto>> Handle(EvalTypeAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EvaluationType>(v, x => x.Id, x => x.Name, x => x.MaxScore, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EvaluationType>(v)
            .OrderBy<EvaluationType>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EvalTypeListDto>(ct);

        foreach (var data in list)
        {
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class EvalTypeActiveHandler : IRequestHandler<EvalTypeActiveQry, List<EvalTypeListDto>>
{
    private readonly IDapperHelper _dapper;
    public EvalTypeActiveHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EvalTypeListDto>> Handle(EvalTypeActiveQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EvaluationType>(v, x => x.Id, x => x.Name, x => x.MaxScore, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EvaluationType>(v)
            .Where<EvaluationType>(v, x => x.IsActive == true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EvalTypeListDto>(ct);

        foreach (var data in list)
        {
            data.IsActiveStr = "Active";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class EvalTypeByIdHandler : IRequestHandler<EvalTypeByIdQry, EvalTypeListDto?>
{
    private readonly IDapperHelper _dapper;
    public EvalTypeByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EvalTypeListDto?> Handle(EvalTypeByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EvaluationType>(v, x => x.Id, x => x.Name, x => x.MaxScore, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EvaluationType>(v)
            .Where<EvaluationType>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EvalTypeListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class EvalFlowAllHandler : IRequestHandler<EvalFlowAllQry, List<EvalFlowListDto>>
{
    private readonly IDapperHelper _dapper;
    public EvalFlowAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EvalFlowListDto>> Handle(EvalFlowAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EvaluationFlow>(v, x => x.Id, x => x.Name, x => x.IsGlobal, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EvaluationFlow>(v)
            .OrderBy<EvaluationFlow>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EvalFlowListDto>(ct);

        foreach (var data in list)
        {
            data.IsGlobalStr = BoolToStr.FormatBool(data.IsGlobal);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class EvalFlowActiveHandler : IRequestHandler<EvalFlowActiveQry, List<EvalFlowListDto>>
{
    private readonly IDapperHelper _dapper;
    public EvalFlowActiveHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EvalFlowListDto>> Handle(EvalFlowActiveQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EvaluationFlow>(v, x => x.Id, x => x.Name, x => x.IsGlobal, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EvaluationFlow>(v)
            .Where<EvaluationFlow>(v, x => x.IsActive == true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EvalFlowListDto>(ct);

        foreach (var data in list)
        {
            data.IsGlobalStr = BoolToStr.FormatBool(data.IsGlobal);
            data.IsActiveStr = "Active";
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class EvalFlowByIdHandler : IRequestHandler<EvalFlowByIdQry, EvalFlowListDto?>
{
    private readonly IDapperHelper _dapper;
    public EvalFlowByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EvalFlowListDto?> Handle(EvalFlowByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EvaluationFlow>(v, x => x.Id, x => x.Name, x => x.IsGlobal, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EvaluationFlow>(v)
            .Where<EvaluationFlow>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EvalFlowListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsGlobalStr = BoolToStr.FormatBool(data.IsGlobal);
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class EvalStepAllHandler : IRequestHandler<EvalStepAllQry, List<EvalStepListDto>>
{
    private readonly IDapperHelper _dapper;
    public EvalStepAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EvalStepListDto>> Handle(EvalStepAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string f = "f";
        const string t = "t";
        var qb = new QueryBuilder()
            .Select<EvaluationStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.IsFinal, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EvaluationType, EvalStepListDto>(t, x => x.Name, d => d.EvalType)
            .SelectAs<EvaluationFlow, EvalStepListDto>(f, x => x.Name, d => d.EvaluationFlow)
            .From<EvaluationStep>(v)
            .LeftJoin<EvaluationStep, EvaluationType>(v, t, x => x.EvalTypeId, x => x.Id)
            .LeftJoin<EvaluationStep, EvaluationFlow>(v, f, x => x.EvaluationFlowId, x => x.Id)
            .OrderBy<EvaluationStep>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EvalStepListDto>(ct);

        foreach (var data in list)
        {
            data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class EvalStepByIdHandler : IRequestHandler<EvalStepByIdQry, EvalStepListDto?>
{
    private readonly IDapperHelper _dapper;
    public EvalStepByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EvalStepListDto?> Handle(EvalStepByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string f = "f";
        const string t = "t";
        var qb = new QueryBuilder()
            .Select<EvaluationStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.IsFinal, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EvaluationType, EvalStepListDto>(t, x => x.Name, d => d.EvalType)
            .SelectAs<EvaluationFlow, EvalStepListDto>(f, x => x.Name, d => d.EvaluationFlow)
            .From<EvaluationStep>(v)
            .LeftJoin<EvaluationStep, EvaluationType>(v, t, x => x.EvalTypeId, x => x.Id)
            .LeftJoin<EvaluationStep, EvaluationFlow>(v, f, x => x.EvaluationFlowId, x => x.Id)
            .Where<EvaluationStep>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EvalStepListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class EvalStepByFlowIdHandler : IRequestHandler<EvalStepByFlowIdQry, List<EvalStepListDto>>
{
    private readonly IDapperHelper _dapper;
    public EvalStepByFlowIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EvalStepListDto>> Handle(EvalStepByFlowIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string f = "f";
        const string t = "t";
        var qb = new QueryBuilder()
            .Select<EvaluationStep>(v, x => x.Id, x => x.StepName, x => x.StepOrder, x => x.IsFinal, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EvaluationType, EvalStepListDto>(t, x => x.Name, d => d.EvalType)
            .SelectAs<EvaluationFlow, EvalStepListDto>(f, x => x.Name, d => d.EvaluationFlow)
            .From<EvaluationStep>(v)
            .LeftJoin<EvaluationStep, EvaluationType>(v, t, x => x.EvalTypeId, x => x.Id)
            .LeftJoin<EvaluationStep, EvaluationFlow>(v, f, x => x.EvaluationFlowId, x => x.Id)
            .Where<EvaluationStep>(v, x => x.EvaluationFlowId == request.Id)
            .OrderBy<EvaluationStep>(v, x => x.StepOrder, desc: false);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EvalStepListDto>(ct);

        foreach (var data in list)
        {
            data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobEvalFlowAllHandler : IRequestHandler<JobEvalFlowAllQry, List<JobEvalFlowListDto>>
{
    private readonly IDapperHelper _dapper;
    public JobEvalFlowAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<JobEvalFlowListDto>> Handle(JobEvalFlowAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<JobPostEvalFlow>(v, x => x.Id, x => x.EvaluationFlowId, x => x.JobPostingId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EvaluationFlow, JobEvalFlowListDto>(e, x => x.Name, d => d.FlowName)
            .SelectAs<JobPosting, JobEvalFlowListDto>(p, x => x.PostNumber, d => d.JobPostNum)
            .From<JobPostEvalFlow>(v)
            .LeftJoin<JobPostEvalFlow, EvaluationFlow>(v, e, x => x.EvaluationFlowId, x => x.Id)
            .LeftJoin<JobPostEvalFlow, JobPosting>(v, p, x => x.JobPostingId, x => x.Id)
            .OrderBy<JobPostEvalFlow>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<JobEvalFlowListDto>(ct);

        foreach (var data in list)
        {
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class JobEvalFlowByIdHandler : IRequestHandler<JobEvalFlowByIdQry, JobEvalFlowListDto?>
{
    private readonly IDapperHelper _dapper;
    public JobEvalFlowByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<JobEvalFlowListDto?> Handle(JobEvalFlowByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<JobPostEvalFlow>(v, x => x.Id, x => x.EvaluationFlowId, x => x.JobPostingId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EvaluationFlow, JobEvalFlowListDto>(e, x => x.Name, d => d.FlowName)
            .SelectAs<JobPosting, JobEvalFlowListDto>(p, x => x.PostNumber, d => d.JobPostNum)
            .From<JobPostEvalFlow>(v)
            .LeftJoin<JobPostEvalFlow, EvaluationFlow>(v, e, x => x.EvaluationFlowId, x => x.Id)
            .LeftJoin<JobPostEvalFlow, JobPosting>(v, p, x => x.JobPostingId, x => x.Id)
            .Where<JobPostEvalFlow>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobEvalFlowListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.RowVersion = data.xmin.ToString();
        return data;
    }
}