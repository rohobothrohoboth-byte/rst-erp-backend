using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.App.Services;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobPostStartEvalCmd : IRequest<JobPostingListDto> { public Guid Id { get; set; } }



public class JobPostStartEvalHandler : IRequestHandler<JobPostStartEvalCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IJobAppEvalService _jobAppEvalService;

    public JobPostStartEvalHandler(IUnitOfWork uow, IMediator med, IJobAppEvalService jobAppEvalService)
    {
        _uow = uow;
        _med = med;
        _jobAppEvalService = jobAppEvalService;
    }

    public async Task<JobPostingListDto> Handle(JobPostStartEvalCmd request, CancellationToken ct)
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

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = request.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}