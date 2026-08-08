using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class WoFoPlReviewCmd : IRequest<WorkforcePlanListDto> { public ReviewDto Rvw { get; set; } = default!; }
public class JobReqReviewCmd : IRequest<JobReqListDto> { public ReviewDto Rvw { get; set; } = default!; }
public class JobReqReviewAllCmd : IRequest<List<WfpJobReqListDto>> { public ReviewAllDto Rvw { get; set; } = default!; }



public class WoFoPlReviewHandler : IRequestHandler<WoFoPlReviewCmd, WorkforcePlanListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public WoFoPlReviewHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<WorkforcePlanListDto> Handle(WoFoPlReviewCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var wfp = await _uow.Set<WorkforcePlan>().FirstOrDefaultAsync(x => x.Id == request.Rvw.Id, ct);
            if (wfp == null) { throw new DomainException($"WORKFORCE PLAN with Id {request.Rvw.Id} NOT FOUND."); }

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
            wfp.AppPositions = request.Rvw.AppCount;
            wfp.Status = stat;
            await _uow.Update(wfp);

            var data = new WorkforcePlanReview
            {
                WorkforcePlanId = request.Rvw.Id,
                Comment = request.Rvw.Comment,
                ReqPositions = wfp.TotalPositions,
                AppPositions = request.Rvw.AppCount,
                ReviewById = request.Rvw.ReviewById,
                Status = stat
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = request.Rvw.Id }, ct);
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

public class JobReqReviewHandler : IRequestHandler<JobReqReviewCmd, JobReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobReqReviewHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobReqListDto> Handle(JobReqReviewCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jReq = await _uow.Set<JobRequisition>().FirstOrDefaultAsync(x => x.Id == request.Rvw.Id, ct);
            if (jReq == null) { throw new DomainException($"JOB REQUISITION with Id {request.Rvw.Id} NOT FOUND."); }

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
            await _uow.Update(jReq);

            var data = new JobReqReview
            {
                JobReqId = request.Rvw.Id,
                Comment = request.Rvw.Comment,
                ReqQuantity = jReq.ReqQuantity,
                AppQuantity = request.Rvw.AppCount,
                ReviewById = request.Rvw.ReviewById,
                Status = stat
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JobReqListDto();
            var response = await _med.Send(new JobReqByIdQry { Id = request.Rvw.Id }, ct);
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

public class JobReqReviewAllHandler : IRequestHandler<JobReqReviewAllCmd, List<WfpJobReqListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobReqReviewAllHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<List<WfpJobReqListDto>> Handle(JobReqReviewAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jReqL = _uow.Set<JobRequisition>().Where(r => r.WorkforcePlanId == request.Rvw.Id).ToList();
            if (jReqL.Count <= 0) { throw new DomainException($"JOB REQUISITIONS with Workforce Plan Id {request.Rvw.Id} NOT FOUND."); }

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
                await _uow.Update(jReq);

                var data = new JobReqReview
                {
                    JobReqId = jReq.Id,
                    Comment = request.Rvw.Comment,
                    ReqQuantity = jReq.ReqQuantity,
                    AppQuantity = jReq.ReqQuantity,
                    ReviewById = request.Rvw.ReviewById,
                    Status = stat
                };
                await _uow.Add(data, ct);
            }
            await _uow.Commit(ct);

            var res = new List<WfpJobReqListDto>();
            var response = await _med.Send(new JobReqAllByWfpIdQry { Id = request.Rvw.Id }, ct);
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

