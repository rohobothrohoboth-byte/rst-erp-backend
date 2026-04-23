using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobPostStartEvalCmd : IRequest<string> { public Guid Id { get; set; } }
public class JobAppEvaluateCmd : IRequest<string> { public JpAppEvalDto EvalDto { get; set; } = default!; }



public class JobPostStartEvalHandler : IRequestHandler<JobPostStartEvalCmd, string>
{
    private readonly IUnitOfWork _uow;
    private readonly IJobAppEvalService _jobAppEvalService;

    public JobPostStartEvalHandler(IUnitOfWork uow, IJobAppEvalService jobAppEvalService)
    {
        _uow = uow;
        _jobAppEvalService = jobAppEvalService;
    }

    public async Task<string> Handle(JobPostStartEvalCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jobPosting = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (jobPosting == null) { throw new DomainException($"JOB POSTING with Id {request.Id} NOT FOUND."); }

            var jpStat = BoolToStr.EnumToString(PostingStatus.Closed);
            if (jobPosting.Status != jpStat) { throw new DomainException("Evaluation starts only after job posting is closed."); }

            var jobPostFlow = await _uow.Set<JobPostEvalFlow>().FirstOrDefaultAsync(x => x.JobPostingId == request.Id, ct);
            if (jobPostFlow == null) { throw new DomainException("No evaluation flow assigned."); }

            var steps = (_uow.Set<EvaluationStep>().Where(x => x.EvaluationFlowId == jobPostFlow.EvaluationFlowId)).ToList();
            if (!steps.Any()) { throw new DomainException("Evaluation flow has no steps."); }

            await _jobAppEvalService.StartEvaluation(request.Id, ct);
            await _uow.Commit(ct);

            return "";
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobAppEvaluateHandler : IRequestHandler<JobAppEvaluateCmd, string>
{
    private readonly IUnitOfWork _uow;
    private readonly IJobAppEvalService _jobAppEvalService;

    public JobAppEvaluateHandler(IUnitOfWork uow, IJobAppEvalService jobAppEvalService)
    {
        _uow = uow;
        _jobAppEvalService = jobAppEvalService;
    }

    public async Task<string> Handle(JobAppEvaluateCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var progress = await _uow.Set<JobAppEvalProgress>().FirstOrDefaultAsync(x => x.JobAppId == request.EvalDto.Id, ct);
            if (progress == null || progress.IsCompleted) { throw new DomainException("Evaluation not started or already completed."); }

            var currentStep = await _uow.Set<EvaluationStep>().FirstOrDefaultAsync(x => x.Id == progress.CurrentStepId, ct);
            if (currentStep == null) { throw new DomainException("Invalid evaluation step."); }

            var score = request.EvalDto.Score;
            if (score < currentStep.MinScore || score > currentStep.MaxScore) { throw new DomainException("Score out of allowed range."); }

            await _jobAppEvalService.EvalScore(request.EvalDto, ct);
            await _uow.Commit(ct);
            return "";
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}