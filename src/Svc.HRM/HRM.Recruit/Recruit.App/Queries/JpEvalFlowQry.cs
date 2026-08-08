// Recruit.App/Queries/JpEvalFlowQry.cs

using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

public class JpEvalFlowByJpIdQry : IRequest<List<JpEvalFlowListDto>>
{
    public Guid Id { get; set; }
}

public class JpEvalFlowByIdQry : IRequest<JpEvalFlowListDto?>
{
    public Guid Id { get; set; }
}

public class JpEvalFlowByJpIdHandler : IRequestHandler<JpEvalFlowByJpIdQry, List<JpEvalFlowListDto>>
{
    private readonly IDapperHelper _dapper;
    public JpEvalFlowByJpIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    private async Task<List<EvalSteps>> GetSteps(Guid id, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<EvaluationStep>(v, x => x.StepName, x => x.MaxScore, x => x.MinScore, x => x.IsFinal, x => x.StepOrder)
            .SelectAs<EvaluationType, EvalSteps>(c, x => x.Name, d => d.EvalType)
            .From<EvaluationStep>(v)
            .Join<EvaluationStep, EvaluationType>(v, c, x => x.EvalTypeId, x => x.Id)
            .Where<EvaluationStep>(v, x => x.EvaluationFlowId == id && x.IsDeleted == false)
            .OrderBy<EvaluationStep>(v, x => x.StepOrder);
        var (sql, parameters) = qb.Build();
        var list = (await _dapper.QueryAsync<EvalSteps>(sql, parameters, ct)).ToList();
        if (list.Count == 0) { return []; }

        foreach (var data in list)
        {
            data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
        }
        return list;
    }

    public async Task<List<JpEvalFlowListDto>> Handle(JpEvalFlowByJpIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<JobPostEvalFlow>(v,
                x => x.Id,
                x => x.EvaluationFlowId,
                x => x.JobPostingId,
                x => x.EffectiveFrom,
                x => x.EffectiveTo!,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobPosting>(p,
                x => x.PostNumber,
                x => x.PostType,
                x => x.PublishedDate)
            .SelectAs<EvaluationFlow, JpEvalFlowListDto>(c,
                x => x.Name,
                d => d.EvalFlowName)
            .From<JobPostEvalFlow>(v)
            .Join<JobPostEvalFlow, JobPosting>(v, p, x => x.JobPostingId, x => x.Id)
            .LeftJoin<JobPostEvalFlow, EvaluationFlow>(v, c, x => x.EvaluationFlowId, x => x.Id)
            .Where<JobPostEvalFlow>(v, x => x.JobPostingId == request.Id && x.IsDeleted == false);

        var (sql, parameters) = qb.Build();
        var list = (await _dapper.QueryAsync<JpEvalFlowListDto>(sql, parameters, ct)).ToList();

        // ? Return empty list if no data found
        if (list.Count == 0)
        {
            return new List<JpEvalFlowListDto>();
        }

        // Process each item
        foreach (var data in list)
        {
            // ? Get steps for each evaluation flow
            var steps = await GetSteps(data.EvaluationFlowId, ct);

            // ? Format the post type
            data.PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType);

            // ? Set the row version
            data.RowVersion = data.xmin.ToString();

            // ? Set the steps (sorted by StepOrder)
            data.Steps = [.. steps.OrderBy(x => x.StepOrder)];

            // ? Note: EffeDateFrom and EffeDateTo are computed properties
            // They will automatically format EffectiveFrom and EffectiveTo
        }

        return list;
    }
}

public class JpEvalFlowByIdHandler : IRequestHandler<JpEvalFlowByIdQry, JpEvalFlowListDto?>
{
    private readonly IDapperHelper _dapper;
    public JpEvalFlowByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    private async Task<List<EvalSteps>> GetSteps(Guid id, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<EvaluationStep>(v, x => x.StepName, x => x.MaxScore, x => x.MinScore, x => x.IsFinal, x => x.StepOrder)
            .SelectAs<EvaluationType, EvalSteps>(c, x => x.Name, d => d.EvalType)
            .From<EvaluationStep>(v)
            .Join<EvaluationStep, EvaluationType>(v, c, x => x.EvalTypeId, x => x.Id)
            .Where<EvaluationStep>(v, x => x.EvaluationFlowId == id && x.IsDeleted == false)
            .OrderBy<EvaluationStep>(v, x => x.StepOrder);
        var (sql, parameters) = qb.Build();
        var list = (await _dapper.QueryAsync<EvalSteps>(sql, parameters, ct)).ToList();
        if (list.Count == 0) { return []; }

        foreach (var data in list)
        {
            data.IsFinalStr = BoolToStr.FormatBool(data.IsFinal);
        }
        return list;
    }

    public async Task<JpEvalFlowListDto?> Handle(JpEvalFlowByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string p = "p";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<JobPostEvalFlow>(v,
                x => x.Id,
                x => x.EvaluationFlowId,
                x => x.JobPostingId,
                x => x.EffectiveFrom,
                x => x.EffectiveTo!,
                x => x.DateAdd,
                x => x.DateMod!,
                x => x.xmin)
            .Select<JobPosting>(p,
                x => x.PostNumber,
                x => x.PostType,
                x => x.PublishedDate)
            .SelectAs<EvaluationFlow, JpEvalFlowListDto>(c,
                x => x.Name,
                d => d.EvalFlowName)
            .From<JobPostEvalFlow>(v)
            .Join<JobPostEvalFlow, JobPosting>(v, p, x => x.JobPostingId, x => x.Id)
            .LeftJoin<JobPostEvalFlow, EvaluationFlow>(v, c, x => x.EvaluationFlowId, x => x.Id)
            .Where<JobPostEvalFlow>(v, x => x.Id == request.Id && x.IsDeleted == false)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JpEvalFlowListDto>(sql, parameters, ct);

        if (data == null) return null;

        // ? Get steps for the evaluation flow
        var steps = await GetSteps(data.EvaluationFlowId, ct);

        // ? Format the post type
        data.PostTypeStr = MyEnumHelper.FormatEnum<JobPostingType>(data.PostType);

        // ? Set the row version
        data.RowVersion = data.xmin.ToString();

        // ? Set the steps (sorted by StepOrder)
        data.Steps = [.. steps.OrderBy(x => x.StepOrder)];

        // ? Note: EffeDateFrom and EffeDateTo are computed properties
        // They will automatically format EffectiveFrom and EffectiveTo

        return data;
    }
}