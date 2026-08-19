using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.App.Services;
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
    private readonly IFinanceBudgetClient _budget;

    public WoFoPlReviewHandler(IUnitOfWork uow, IMediator med, IFinanceBudgetClient budget)
    {
        _uow = uow;
        _med = med;
        _budget = budget;
    }

    public async Task<WorkforcePlanListDto> Handle(WoFoPlReviewCmd request, CancellationToken ct)
    {
        var reserved = false;
        var reservedRefId = Guid.Empty;
        await _uow.Begin(ct);
        try
        {
            var wfp = await _uow.Set<WorkforcePlan>().FirstOrDefaultAsync(x => x.Id == request.Rvw.Id, ct);
            if (wfp == null) { throw new DomainException($"WORKFORCE PLAN with Id {request.Rvw.Id} NOT FOUND."); }

            var approving = request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.App);

            // Budget gate: when the plan is tied to a Finance budget, encumber (reserve) the
            // planned amount on approval and hard-block if there isn't enough available.
            if (approving && wfp.BudgetId.HasValue && (wfp.Budget ?? 0m) > 0m)
            {
                var avail = await _budget.ReserveAsync(
                    wfp.BudgetId.Value, wfp.Id, "WorkforcePlan", wfp.Budget!.Value,
                    $"Workforce plan {wfp.PlanCode}", ct);
                if (!avail.Ok) { throw new DomainException(avail.Message); }
                reserved = true;
                reservedRefId = wfp.Id;
            }

            var stat = BoolToStr.EnumToString(ReqStatus.Rejected);
            if (approving)
            {
                stat = BoolToStr.EnumToString(ReqStatus.Approved);
            }
            if (request.Rvw.Status == BoolToStr.EnumToString(ReviewStat.ReWork))
            {
                stat = BoolToStr.EnumToString(ReqStatus.Pending);
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

            // On reject, release any previously reserved budget for this plan.
            if (!approving && wfp.BudgetId.HasValue)
            {
                try { await _budget.ReleaseAsync(wfp.Id, "WorkforcePlan", ct); } catch { /* best-effort */ }
            }

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = request.Rvw.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            // Compensate: release the reservation we created if the local commit failed.
            if (reserved && reservedRefId != Guid.Empty)
            {
                try { await _budget.ReleaseAsync(reservedRefId, "WorkforcePlan", ct); } catch { /* best-effort */ }
            }
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

