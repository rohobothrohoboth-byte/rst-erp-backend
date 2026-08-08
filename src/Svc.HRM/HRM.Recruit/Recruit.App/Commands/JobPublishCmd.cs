using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobPostPublishCmd : IRequest<JobPostingListDto>
{
    public PostPublish Rvw { get; set; } = default!;
}

public class JobPostPublishAllCmd : IRequest<List<JobPostingListDto>>
{
    public PostPublish Rvw { get; set; } = default!;
}

public class JobPostingCloseCmd : IRequest<JobPostingListDto>
{
    public Guid Id { get; set; }
}

public class JobPostPublishHandler : IRequestHandler<JobPostPublishCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostPublishHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobPostingListDto> Handle(JobPostPublishCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jPost = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.Id == request.Rvw.Id && !x.IsDeleted, ct);

            if (jPost == null)
                throw new DomainException($"JOB POSTING with Id {request.Rvw.Id} NOT FOUND.");

            // ? Validate if can be published using Helpers service
            JobPostingStatusService.ValidatePublish(jPost.Status);

            // ? Parse current status
            var currentStatus = JobPostingStatusService.ParseStatus(jPost.Status);

            // ? If already published, return success (idempotent)
            if (currentStatus == PostingStatus.Published)
            {
                var existing = await _med.Send(new JobPostingViewQry { Id = request.Rvw.Id }, ct);
                return existing != null ? JobPostingMapping.MapViewToListDto(existing) : new JobPostingListDto();
            }

            // ? Update to Published
            jPost.Status = JobPostingStatusService.Published;
            jPost.PublishedDate = DateTime.UtcNow;
            jPost.DateMod = DateTime.UtcNow;
            await _uow.Update(jPost);

            // ? Create review record
            var data = new JobPostReview
            {
                Id = Guid.CreateVersion7(),
                JobPostingId = request.Rvw.Id,
                Comment = request.Rvw.Comment ?? "",
                ReviewById = request.Rvw.ReviewById,
                Status = JobPostingStatusService.Published,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            // ? Use JobPostingViewQry instead of JobPostingByIdQry
            var response = await _med.Send(new JobPostingViewQry { Id = request.Rvw.Id }, ct);
            if (response == null) { return new JobPostingListDto(); }

            // ? Map ViewDto to ListDto
            return JobPostingMapping.MapViewToListDto(response);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobPostPublishAllHandler : IRequestHandler<JobPostPublishAllCmd, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostPublishAllHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    private async Task UpdatePosting(Guid jobReqId, PostPublish request, CancellationToken ct)
    {
        var dbData = await _uow.Set<JobPosting>()
            .Where(p => p.JobReqId == jobReqId && !p.IsDeleted)
            .ToListAsync(ct);

        foreach (var data in dbData)
        {
            var currentStatus = JobPostingStatusService.ParseStatus(data.Status);

            // ? Skip if already published
            if (currentStatus == PostingStatus.Published)
                continue;

            // ? Validate if can be published using Helpers service
            JobPostingStatusService.ValidatePublish(data.Status);

            // ? Update to Published
            data.Status = JobPostingStatusService.Published;
            data.PublishedDate = DateTime.UtcNow;
            data.DateMod = DateTime.UtcNow;
            await _uow.Update(data);

            var dataR = new JobPostReview
            {
                Id = Guid.CreateVersion7(),
                Comment = request.Comment ?? "",
                ReviewById = request.ReviewById,
                JobPostingId = data.Id,
                Status = JobPostingStatusService.Published,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(dataR, ct);
        }
    }

    public async Task<List<JobPostingListDto>> Handle(JobPostPublishAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Approved);
            var jReqL = await _uow.Set<JobRequisition>()
                .Where(r => r.WorkforcePlanId == request.Rvw.Id && r.Status == stat && !r.IsDeleted)
                .ToListAsync(ct);

            if (jReqL.Count <= 0)
                throw new DomainException($"JOB POST for Workforce Plan with Id {request.Rvw.Id} NOT FOUND.");

            foreach (var jReq in jReqL)
            {
                await UpdatePosting(jReq.Id, request.Rvw, ct);
            }

            await _uow.Commit(ct);

            // ? Use JobPostingByWfpIdQry (returns List<JobPostingListDto>)
            var response = await _med.Send(new JobPostingByWfpIdQry { Id = request.Rvw.Id }, ct);
            return response ?? new List<JobPostingListDto>();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class JobPostingCloseHandler : IRequestHandler<JobPostingCloseCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingCloseHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobPostingListDto> Handle(JobPostingCloseCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (oldData == null)
                throw new DomainException($"JOB POSTING with Id {request.Id} NOT FOUND.");

            // ? Validate if can be closed using Helpers service
            JobPostingStatusService.ValidateClose(oldData.Status);

            // ? Parse current status
            var currentStatus = JobPostingStatusService.ParseStatus(oldData.Status);

            // ? If already closed, return success (idempotent)
            if (currentStatus == PostingStatus.Closed)
            {
                var existing = await _med.Send(new JobPostingViewQry { Id = request.Id }, ct);
                return existing != null ? JobPostingMapping.MapViewToListDto(existing) : new JobPostingListDto();
            }

            // ? Update to Closed
            oldData.Status = JobPostingStatusService.Closed;
            oldData.ClosedDate = DateTime.UtcNow;
            oldData.DateMod = DateTime.UtcNow;
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            // ? Use JobPostingViewQry instead of JobPostingByIdQry
            var response = await _med.Send(new JobPostingViewQry { Id = request.Id }, ct);
            if (response == null) { return new JobPostingListDto(); }

            // ? Map ViewDto to ListDto
            return JobPostingMapping.MapViewToListDto(response);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}