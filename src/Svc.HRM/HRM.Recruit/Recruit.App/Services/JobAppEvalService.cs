using Helpers;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.Domain.Entities;

namespace Recruit.App.Services;

public interface IJobAppEvalService
{
    Task StartEvaluationAsync(Guid jobPostingId, CancellationToken ct);
    Task EvaluateAsync(Guid jobAppId, double score, string feedback, Guid evaluatorId, CancellationToken ct);
}

public class JobAppEvalService : IJobAppEvalService
{
    private readonly IUnitOfWork _uow;
    public JobAppEvalService(IUnitOfWork uow) { _uow = uow; }

    public async Task StartEvaluationAsync(Guid jobPostingId, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jobPosting = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == jobPostingId, ct);
            if (jobPosting == null) { throw new DomainException("Job posting not found."); }

            var jpStat = BoolToStr.EnumToString(PostingStatus.Closed);
            if (jobPosting.Status != jpStat) { throw new DomainException("Evaluation starts only after job posting is closed."); }

            var jobPostFlow = await _uow.Set<JobPostEvalFlow>().FirstOrDefaultAsync(x => x.JobPostingId == jobPostingId, ct);
            if (jobPostFlow == null) { throw new DomainException("No evaluation flow assigned."); }

            var steps = jobPostFlow.EvaluationFlow.Steps.OrderBy(s => s.StepOrder).ToList();
            if (!steps.Any()) { throw new DomainException("Evaluation flow has no steps."); }

            var firstStep = steps.First();
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
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    public async Task EvaluateAsync(Guid jobAppId, double score, string feedback, Guid evaluatorId, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var progress = await _uow.Set<JobAppEvalProgress>().FirstOrDefaultAsync(x => x.Id == jobAppId, ct);
            if (progress == null || progress.IsCompleted) { throw new DomainException("Evaluation not started or already completed."); }

            var currentStep = await _uow.Set<EvaluationStep>().FirstOrDefaultAsync(x => x.Id == progress.CurrentStepId, ct);
            if (currentStep == null) { throw new DomainException("Invalid evaluation step."); }

            if (score < currentStep.MinScore || score > currentStep.MaxScore) { throw new DomainException("Score out of allowed range."); }

            var evalScore = new EvaluationScore
            {
                JobAppId = jobAppId,
                EvaluationStepId = currentStep.Id,
                Score = score,
                Feedback = feedback,
                EvaluatorId = evaluatorId,
                IsCurrent = true
            };

            await _uow.Add(evalScore, ct);
            await _uow.Commit(ct);

            if (score < currentStep.MinScore)
            {
                await RejectApp(jobAppId, progress, ct);
                return;
            }

            await MoveToNextStep(jobAppId, progress, currentStep, ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }


    }

    private async Task MoveToNextStep(Guid jobAppId, JobAppEvalProgress progress, EvaluationStep currentStep, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var steps = await _uow.Set<EvaluationStep>().Where(s => s.EvaluationFlowId == currentStep.EvaluationFlowId).ToListAsync(cancellationToken: ct);
            var nextStep = steps.OrderBy(s => s.StepOrder).FirstOrDefault(s => s.StepOrder > currentStep.StepOrder);
            var app = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == jobAppId, cancellationToken: ct);

            if (currentStep.IsFinal || nextStep == null)
            {
                app.Status = BoolToStr.EnumToString(ApplicationStatus.PassEval);
                await _uow.Update(app);
                progress.IsCompleted = true;
                await _uow.Update(progress);
                return;
            }

            progress.CurrentStepId = nextStep.Id;
            await _uow.Update(progress);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private async Task RejectApp(Guid jobAppId, JobAppEvalProgress progress, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var app = await _uow.Set<JobApplication>().FirstOrDefaultAsync(x => x.Id == jobAppId, cancellationToken: ct);
            app.Status = BoolToStr.EnumToString(ApplicationStatus.Rejected);
            await _uow.Update(app);
            progress.IsCompleted = true;
            await _uow.Update(progress);

            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}