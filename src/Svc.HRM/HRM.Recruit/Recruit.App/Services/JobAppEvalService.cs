using Helpers;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Services;

public interface IJobAppEvalService
{
    Task StartEvaluation(Guid jobPostingId, CancellationToken ct);
    Task EvalScore(JpAppEvalDto evalDto, CancellationToken ct);
}

public class JobAppEvalService : IJobAppEvalService
{
    private readonly IUnitOfWork _uow;
    public JobAppEvalService(IUnitOfWork uow) { _uow = uow; }

    public async Task StartEvaluation(Guid jobPostingId, CancellationToken ct)
    {
        var jobPostFlow = await _uow.Set<JobPostEvalFlow>().FirstOrDefaultAsync(x => x.JobPostingId == jobPostingId, ct);
        var steps = (_uow.Set<EvaluationStep>().Where(x => x.EvaluationFlowId == jobPostFlow!.EvaluationFlowId)).ToList();
        var sOrdered = steps.OrderBy(s => s.StepOrder).ToList();
        var firstStep = sOrdered.First();
        var applications = await _uow.Set<JobApplication>().Where(x => x.JobPostingId == jobPostingId).ToListAsync(ct);

        foreach (var app in applications)
        {
            var progress = new JobAppEvalProgress
            {
                JobAppId = app.Id,
                CurrentStepId = firstStep.Id,
                IsCompleted = false
            };

            await _uow.Add(progress, ct);

            app.Status = BoolToStr.EnumToString(ApplicationStatus.UnderReview);
            await _uow.Update(app);
        }
    }

    public async Task EvalScore(JpAppEvalDto evalDto, CancellationToken ct)
    {
        var progress = await _uow.Set<JobAppEvalProgress>().FirstOrDefaultAsync(x => x.JobAppId == evalDto.Id, ct);
        var currentStep = await _uow.Set<EvaluationStep>().FirstOrDefaultAsync(x => x.Id == progress!.CurrentStepId, ct);

        var evalScore = new EvaluationScore
        {
            JobAppId = evalDto.Id,
            EvaluationStepId = currentStep!.Id,
            Score = evalDto.Score,
            Feedback = evalDto.Feedback,
            EvaluatorId = evalDto.EvaluatorId,
            IsCurrent = true
        };
        await _uow.Add(evalScore, ct);

        if (evalDto.Score < currentStep.MinScore)
        {
            await RejectApp(evalDto.Id, progress!, ct);
            return;
        }

        await MoveToNextStep(evalDto.Id, progress!, currentStep, ct);
    }

    private async Task MoveToNextStep(Guid jobAppId, JobAppEvalProgress progress, EvaluationStep currentStep, CancellationToken ct)
    {
        var steps = await _uow.Set<EvaluationStep>().Where(s => s.EvaluationFlowId == currentStep.EvaluationFlowId).ToListAsync(cancellationToken: ct);
        var nextStep = steps.OrderBy(s => s.StepOrder).FirstOrDefault(s => s.StepOrder > currentStep.StepOrder);
        var app = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == jobAppId, cancellationToken: ct);

        if (currentStep.IsFinal || nextStep == null)
        {
             // Passed the final evaluation step -> the applicant has passed evaluation.
             if (app != null)
                    {
                       app.Status = BoolToStr.EnumToString(ApplicationStatus.PassEval);
                       await _uow.Update(app);
                    }


            progress.IsCompleted = true;
            await _uow.Update(progress);
            return;
        }

        progress.CurrentStepId = nextStep.Id;
        await _uow.Update(progress);
    }

    private async Task RejectApp(Guid jobAppId, JobAppEvalProgress progress, CancellationToken ct)
    {
        var app = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == jobAppId, cancellationToken: ct);

        if (app != null)
        {
            app.Status = BoolToStr.EnumToString(ApplicationStatus.Rejected);
            await _uow.Update(app);
        }

        progress.IsCompleted = true;
        await _uow.Update(progress);
    }
}