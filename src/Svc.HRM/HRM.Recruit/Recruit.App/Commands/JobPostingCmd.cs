using Helpers;
using MediatR;
using Recruit.App.Helpers;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostingAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var code = await new CodeGen(_unitOfWork).GetPostNumber();
            var data = new JobPosting
            {
                PostNumber = code,
                Status = BoolToStr.EnumToString(PostingStatus.Pending),
                PostType = request.AddDto.PostType,
                PublishedDate = DateTime.UtcNow,
                DeadlineDate = request.AddDto.DeadlineDate,
                JobReqId = request.AddDto.Id,
            };
            await _unitOfWork.Repository<JobPosting>().Add(data);
            await _unitOfWork.Commit();

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobPostingAddAllHandler : IRequestHandler<JobPostingAddAllCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostingAddAllHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingAddAllCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Approved);
            var jReqL = (await _unitOfWork.Repository<JobRequisition>().Find(r => r.WorkforcePlanId == request.AddDto.Id && r.Status == stat)).ToList();
            var lId = new Guid();
            if (jReqL.Count > 0)
            {
                var statP = BoolToStr.EnumToString(PostingStatus.Pending);
                foreach (var jReq in jReqL)
                {
                    var code = await new CodeGen(_unitOfWork).GetPostNumber();
                    var data = new JobPosting
                    {
                        PostNumber = code,
                        Status = statP,
                        PostType = request.AddDto.PostType,
                        PublishedDate = DateTime.UtcNow,
                        DeadlineDate = request.AddDto.DeadlineDate,
                        JobReqId = jReq.Id,
                    };
                    await _unitOfWork.Repository<JobPosting>().Add(data);
                }
            }
            await _unitOfWork.Commit();

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = lId }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobPostingModHandler : IRequestHandler<JobPostingModCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostingModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<JobPosting>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"JOB POSTING with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
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
            var data = await _unitOfWork.Repository<JobPosting>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobPostPublishHandler : IRequestHandler<JobPostPublishCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostPublishHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostPublishCmd request, CancellationToken cancellationToken)
    {
        var jPost = await _unitOfWork.Repository<JobPosting>().GetById(request.Rvw.Id);
        if (jPost == null) { throw new DomainException($"JOB POSTING with Id {request.Rvw.Id} NOT FOUND."); }
        var pStat = BoolToStr.EnumToString(PostingStatus.Pending);
        if (jPost.Status != pStat) { throw new DomainException($"Only PENDING Job Posting can be published!"); }

        await _unitOfWork.Begin();
        try
        {
            jPost.Status = BoolToStr.EnumToString(PostingStatus.Published);
            jPost.PublishedDate = DateTime.UtcNow;
            await _unitOfWork.Repository<JobPosting>().Update(jPost);

            var data = new JobPostReview
            {
                JobPostingId = request.Rvw.Id,
                Comment = request.Rvw.Comment,
                ReviewById = request.Rvw.ReviewById,
                Status = BoolToStr.EnumToString(PostingStatus.Published)
            };
            await _unitOfWork.Repository<JobPostReview>().Add(data);
            await _unitOfWork.Commit();

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = request.Rvw.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobPostPublishAllHandler : IRequestHandler<JobPostPublishAllCmd, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostPublishAllHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    private async Task UpdatePosting(Guid id, PostPublish request)
    {
        var dataL = new List<JobPosting>();
        var pStat = BoolToStr.EnumToString(PostingStatus.Pending);
        var dbData = (await _unitOfWork.Repository<JobPosting>().Find(p => p.JobReqId == id && p.Status == pStat)).ToList();
        if (dbData.Count > 0)
        {
            var stat = BoolToStr.EnumToString(PostingStatus.Published);
            foreach (var data in dbData)
            {
                data.Status = stat;
                data.PublishedDate = DateTime.UtcNow;
                await _unitOfWork.Repository<JobPosting>().Update(data);

                var dataR = new JobPostReview
                {
                    Comment = request.Comment,
                    ReviewById = request.ReviewById,
                    JobPostingId = data.Id,
                    Status = stat
                };
                await _unitOfWork.Repository<JobPostReview>().Add(dataR);
            }
        }
    }

    public async Task<List<JobPostingListDto>> Handle(JobPostPublishAllCmd request, CancellationToken cancellationToken)
    {
        var stat = BoolToStr.EnumToString(ReqStatus.Approved);
        var jReqL = (await _unitOfWork.Repository<JobRequisition>().Find(r => r.WorkforcePlanId == request.Rvw.Id && r.Status == stat)).ToList();
        if (jReqL.Count <= 0) { throw new DomainException($"JOB POST for Workforce Plan with Id {request.Rvw.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            foreach (var jReq in jReqL)
            {
                await UpdatePosting(jReq.Id, request.Rvw);
            }

            await _unitOfWork.Commit();
            var res = new List<JobPostingListDto>();
            var response = await _med.Send(new JobPostingByWfpIdQry { Id = request.Rvw.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobPostingCloseHandler : IRequestHandler<JobPostingCloseCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostingCloseHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingCloseCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<JobPosting>().GetById(request.Id);
        if (oldData == null) { throw new DomainException($"JOB POSTING with Id {request.Id} NOT FOUND."); }
        var pStat1 = BoolToStr.EnumToString(PostingStatus.Published);
        var pStat2 = BoolToStr.EnumToString(PostingStatus.OnHold);
        if (oldData.Status != pStat1 || oldData.Status != pStat2) { throw new DomainException($"Only PUBLISHED or ON_HOLD Job Postings can be closed!"); }

        await _unitOfWork.Begin();
        try
        {            
            oldData.Status = BoolToStr.EnumToString(PostingStatus.Closed);
            oldData.ClosedDate = DateTime.UtcNow;
            var data = await _unitOfWork.Repository<JobPosting>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new JobPostingListDto();
            var response = await _med.Send(new JobPostingByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class JobPostingDelHandler : IRequestHandler<JobPostingDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobPostingDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(JobPostingDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<JobPosting>().GetById(request.Id);
            if (data == null) { throw new DomainException($"JOB POSTING with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<JobPosting>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}