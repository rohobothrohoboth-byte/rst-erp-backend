using Helpers;
using MediatR;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class WoFoPlReviewCmd : IRequest<WorkforcePlanListDto> { public ReviewDto Rvw { get; set; } = default!; }
public class JobReqReviewCmd : IRequest<JobReqListDto> { public ReviewDto Rvw { get; set; } = default!; }
public class JobReqReviewAllCmd : IRequest<List<JobReqListDto>> { public ReviewAllDto Rvw { get; set; } = default!; }
public class JobPostPublishCmd : IRequest<JobPostingListDto> { public PostPublish Rvw { get; set; } = default!; }
public class JobPostPublishAllCmd : IRequest<List<JobPostingListDto>> { public PostPublish Rvw { get; set; } = default!; }



public class WoFoPlReviewHandler : IRequestHandler<WoFoPlReviewCmd, WorkforcePlanListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public WoFoPlReviewHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<WorkforcePlanListDto> Handle(WoFoPlReviewCmd request, CancellationToken cancellationToken)
    {
        var wfp = await _unitOfWork.Repository<WorkforcePlan>().GetById(request.Rvw.Id);
        if (wfp == null) { throw new DomainException($"WORKFORCE PLAN with Id {request.Rvw.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Rejected);
            if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.App))
            {
                stat = BoolToStr.EnumToString(ReqStatus.Approved);
                wfp.Status = stat;
            }
            if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.ReWork))
            {
                stat = BoolToStr.EnumToString(ReqStatus.Pending);
                wfp.Status = stat;
            }
            wfp.Status = stat;
            await _unitOfWork.Repository<WorkforcePlan>().Update(wfp);

            var data = new WorkforcePlanReview
            {
                WorkforcePlanId = request.Rvw.Id,
                Comment = request.Rvw.Comment,
                ReqPositions = wfp.TotalPositions,
                AppPositions = request.Rvw.AppCount,
                ReviewById = request.Rvw.ReviewById,
                Status = stat
            };
            await _unitOfWork.Repository<WorkforcePlanReview>().Add(data);
            await _unitOfWork.Commit();

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = request.Rvw.Id }, cancellationToken);
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

public class JobReqReviewHandler : IRequestHandler<JobReqReviewCmd, JobReqListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobReqReviewHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobReqListDto> Handle(JobReqReviewCmd request, CancellationToken cancellationToken)
    {
        var jReq = await _unitOfWork.Repository<JobRequisition>().GetById(request.Rvw.Id);
        if (jReq == null) { throw new DomainException($"JOB REQUISITION with Id {request.Rvw.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Rejected);
            if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.App))
            {
                stat = BoolToStr.EnumToString(ReqStatus.Approved);
                jReq.Status = stat;
            }
            if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.ReWork))
            {
                stat = BoolToStr.EnumToString(ReqStatus.Pending);
                jReq.Status = stat;
            }
            jReq.Status = stat;
            await _unitOfWork.Repository<JobRequisition>().Update(jReq);

            var data = new JobReqReview
            {
                JobReqId = request.Rvw.Id,
                Comment = request.Rvw.Comment,
                ReqQuantity = jReq.ReqQuantity,
                AppQuantity = request.Rvw.AppCount,
                ReviewById = request.Rvw.ReviewById,
                Status = stat
            };
            await _unitOfWork.Repository<JobReqReview>().Add(data);
            await _unitOfWork.Commit();

            var res = new JobReqListDto();
            var response = await _med.Send(new JobReqByIdQry { Id = request.Rvw.Id }, cancellationToken);
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

public class JobReqReviewAllHandler : IRequestHandler<JobReqReviewAllCmd, List<JobReqListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobReqReviewAllHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<List<JobReqListDto>> Handle(JobReqReviewAllCmd request, CancellationToken cancellationToken)
    {
        var jReqL = (await _unitOfWork.Repository<JobRequisition>().Find(r => r.WorkforcePlanId == request.Rvw.Id)).ToList();
        if (jReqL.Count <= 0) { throw new DomainException($"JOB REQUISITIONS with Workforce Plan Id {request.Rvw.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Rejected);
            foreach (var jReq in jReqL)
            {
                if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.App))
                {
                    stat = BoolToStr.EnumToString(ReqStatus.Approved);
                    jReq.Status = stat;
                }
                if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.ReWork))
                {
                    stat = BoolToStr.EnumToString(ReqStatus.Pending);
                    jReq.Status = stat;
                }
                jReq.Status = stat;
                await _unitOfWork.Repository<JobRequisition>().Update(jReq);

                var data = new JobReqReview
                {
                    JobReqId = request.Rvw.Id,
                    Comment = request.Rvw.Comment,
                    ReqQuantity = jReq.ReqQuantity,
                    AppQuantity = jReq.ReqQuantity,
                    ReviewById = request.Rvw.ReviewById,
                    Status = stat
                };
                await _unitOfWork.Repository<JobReqReview>().Add(data);
            }
            await _unitOfWork.Commit();

            var res = new List<JobReqListDto>();
            var response = await _med.Send(new JobReqAllQry { Id = request.Rvw.Id }, cancellationToken);
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
        var dbData = (await _unitOfWork.Repository<JobPosting>().Find(p => p.JobReqId == id)).ToList();
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

