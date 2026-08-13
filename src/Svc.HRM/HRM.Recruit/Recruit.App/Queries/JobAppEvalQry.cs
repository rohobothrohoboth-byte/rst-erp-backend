using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Queries;

// Read the evaluation progress for a single job application (current step + score history).
public class JobAppEvalProgressQry : IRequest<JobAppEvalProgressDto?>
{
    public Guid JobAppId { get; set; }
}

public class JobAppEvalProgressHandler : IRequestHandler<JobAppEvalProgressQry, JobAppEvalProgressDto?>
{
    private readonly IUnitOfWork _uow;
    public JobAppEvalProgressHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task<JobAppEvalProgressDto?> Handle(JobAppEvalProgressQry request, CancellationToken ct)
    {
        var app = await _uow.Set<JobApplication>()
            .FirstOrDefaultAsync(x => x.Id == request.JobAppId, ct);
        if (app == null) { return null; }

        var dto = new JobAppEvalProgressDto
        {
            JobAppId = request.JobAppId,
            AppStatus = app.Status,
            IsStarted = false,
        };

        var progress = await _uow.Set<JobAppEvalProgress>()
            .FirstOrDefaultAsync(x => x.JobAppId == request.JobAppId, ct);
        if (progress == null) { return dto; }

        dto.IsStarted = true;
        dto.IsCompleted = progress.IsCompleted;
        dto.CurrentStepId = progress.CurrentStepId;

        var currentStep = await _uow.Set<EvaluationStep>()
            .FirstOrDefaultAsync(x => x.Id == progress.CurrentStepId, ct);

        if (currentStep != null)
        {
            dto.CurrentStepName = currentStep.StepName;
            dto.CurrentStepOrder = currentStep.StepOrder;
            dto.MinScore = currentStep.MinScore;
            dto.MaxScore = currentStep.MaxScore;
            dto.IsFinal = currentStep.IsFinal;

            dto.TotalSteps = await _uow.Set<EvaluationStep>()
                .CountAsync(x => x.EvaluationFlowId == currentStep.EvaluationFlowId, ct);
        }

        var scores = await _uow.Set<EvaluationScore>()
            .Where(x => x.JobAppId == request.JobAppId)
            .Join(_uow.Set<EvaluationStep>(),
                sc => sc.EvaluationStepId,
                st => st.Id,
                (sc, st) => new EvalScoreHistDto
                {
                    StepId = st.Id,
                    StepName = st.StepName,
                    StepOrder = st.StepOrder,
                    Score = sc.Score,
                    Feedback = sc.Feedback,
                })
            .OrderBy(x => x.StepOrder)
            .ToListAsync(ct);

        dto.Scores = scores;
        dto.CompletedSteps = scores.Count;
        return dto;
    }
}
