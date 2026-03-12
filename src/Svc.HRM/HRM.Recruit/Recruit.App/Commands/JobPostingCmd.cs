using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobPostingAddCmd : IRequest<JobPostingListDto> { public JobPostingAddDto AddDto { get; set; } = default!; }
public class JobPostingAddAllCmd : IRequest<JobPostingListDto> { public JobPostingAddDto AddDto { get; set; } = default!; }
public class JobPostingModCmd : IRequest<JobPostingListDto> { public JobPostingModDto ModDto { get; set; } = default!; }
public class JobPostPublishCmd : IRequest<JobPostingListDto> { public PostPublish Rvw { get; set; } = default!; }
public class JobPostPublishAllCmd : IRequest<List<JobPostingListDto>> { public PostPublish Rvw { get; set; } = default!; }
public class JobPostingCloseCmd : IRequest<JobPostingListDto> { public Guid Id { get; set; } }
public class JobPostingDelCmd : IRequest { public Guid Id { get; set; } }



public class JobPostingAddHandler : IRequestHandler<JobPostingAddCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new JobPosting
            {
                Status = BoolToStr.EnumToString(PostingStatus.Pending),
                PostType = request.AddDto.PostType,
                PublishedDate = DateTime.UtcNow,
                DeadlineDate = request.AddDto.DeadlineDate,
                JobReqId = request.AddDto.Id,
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = data.Id }, ct);
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

public class JobPostingAddAllHandler : IRequestHandler<JobPostingAddAllCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingAddAllHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingAddAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Approved);
            var jReqL = _uow.Set<JobRequisition>().Where(r => r.WorkforcePlanId == request.AddDto.Id && r.Status == stat).ToList();
            var lId = new Guid();
            if (jReqL.Count > 0)
            {
                var statP = BoolToStr.EnumToString(PostingStatus.Pending);
                foreach (var jReq in jReqL)
                {
                    var data = new JobPosting
                    {
                        Status = statP,
                        PostType = request.AddDto.PostType,
                        PublishedDate = DateTime.UtcNow,
                        DeadlineDate = request.AddDto.DeadlineDate,
                        JobReqId = jReq.Id,
                    };
                    await _uow.Add(data, ct);
                }
            }
            await _uow.Commit(ct);

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = lId }, ct);
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

public class JobPostingModHandler : IRequestHandler<JobPostingModCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"JOB POSTING with Id {request.ModDto.Id} NOT FOUND."); }

            if (request.ModDto.Status == BoolToStr.EnumToString(PostStatus.Pending))
            {
                oldData.Status = BoolToStr.EnumToString(PostingStatus.Pending);
            }
            if (request.ModDto.Status == BoolToStr.EnumToString(PostStatus.OnHold))
            {
                oldData.Status = BoolToStr.EnumToString(PostingStatus.OnHold);
            }
            if (request.ModDto.Status == BoolToStr.EnumToString(PostStatus.Cancelled))
            {
                oldData.Status = BoolToStr.EnumToString(PostingStatus.Cancelled);
            }

            oldData.PostType = request.ModDto.PostType;
            oldData.DeadlineDate = request.ModDto.DeadlineDate;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = request.ModDto.Id }, ct);
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

public class JobPostPublishHandler : IRequestHandler<JobPostPublishCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostPublishHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostPublishCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jPost = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == request.Rvw.Id, ct);
            if (jPost == null) { throw new DomainException($"JOB POSTING with Id {request.Rvw.Id} NOT FOUND."); }
            var pStat = BoolToStr.EnumToString(PostingStatus.Pending);
            if (jPost.Status != pStat) { throw new DomainException($"Only PENDING Job Posting can be published!"); }

            jPost.Status = BoolToStr.EnumToString(PostingStatus.Published);
            jPost.PublishedDate = DateTime.UtcNow;
            await _uow.Update(jPost);

            var data = new JobPostReview
            {
                JobPostingId = request.Rvw.Id,
                Comment = request.Rvw.Comment,
                ReviewById = request.Rvw.ReviewById,
                Status = BoolToStr.EnumToString(PostingStatus.Published)
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = request.Rvw.Id }, ct);
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

public class JobPostPublishAllHandler : IRequestHandler<JobPostPublishAllCmd, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostPublishAllHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    private async Task UpdatePosting(Guid id, PostPublish request, CancellationToken ct)
    {
        var dataL = new List<JobPosting>();
        var pStat = BoolToStr.EnumToString(PostingStatus.Pending);
        var dbData = _uow.Set<JobPosting>().Where(p => p.JobReqId == id && p.Status == pStat).ToList();
        if (dbData.Count > 0)
        {
            var stat = BoolToStr.EnumToString(PostingStatus.Published);
            foreach (var data in dbData)
            {
                data.Status = stat;
                data.PublishedDate = DateTime.UtcNow;
                await _uow.Update(data);

                var dataR = new JobPostReview
                {
                    Comment = request.Comment,
                    ReviewById = request.ReviewById,
                    JobPostingId = data.Id,
                    Status = stat
                };
                await _uow.Add(dataR, ct);
            }
        }
    }

    public async Task<List<JobPostingListDto>> Handle(JobPostPublishAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Approved);
            var jReqL = _uow.Set<JobRequisition>().Where(r => r.WorkforcePlanId == request.Rvw.Id && r.Status == stat).ToList();
            if (jReqL.Count <= 0) { throw new DomainException($"JOB POST for Workforce Plan with Id {request.Rvw.Id} NOT FOUND."); }

            foreach (var jReq in jReqL)
            {
                await UpdatePosting(jReq.Id, request.Rvw, ct);
            }

            await _uow.Commit(ct);
            var res = new List<JobPostingListDto>();
            var response = await _med.Send(new JobPostingByWfpIdQry { Id = request.Rvw.Id }, ct);
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

public class JobPostingCloseHandler : IRequestHandler<JobPostingCloseCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingCloseHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingCloseCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (oldData == null) { throw new DomainException($"JOB POSTING with Id {request.Id} NOT FOUND."); }
            var pStat1 = BoolToStr.EnumToString(PostingStatus.Published);
            var pStat2 = BoolToStr.EnumToString(PostingStatus.OnHold);
            if (oldData.Status != pStat1 || oldData.Status != pStat2) { throw new DomainException($"Only PUBLISHED or ON_HOLD Job Postings can be closed!"); }

            oldData.Status = BoolToStr.EnumToString(PostingStatus.Closed);
            oldData.ClosedDate = DateTime.UtcNow;
            await _uow.Update(oldData);
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

public class JobPostingDelHandler : IRequestHandler<JobPostingDelCmd>
{
    private readonly IUnitOfWork _uow;
    public JobPostingDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(JobPostingDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobPosting>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"JOB POSTING with id [{request.Id}] NOT FOUND."); }
            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}