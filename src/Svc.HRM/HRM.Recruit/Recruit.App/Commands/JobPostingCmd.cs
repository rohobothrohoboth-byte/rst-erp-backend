using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobPostingAddCmd : IRequest<JobPostingListDto> { public JobPostingAddDto AddDto { get; set; } = default!; }
public class JobPostingAddAllCmd : IRequest<List<JobPostingListDto>> { public JobPostingAddDto AddDto { get; set; } = default!; }
public class JobPostingModCmd : IRequest<JobPostingListDto> { public JobPostingModDto ModDto { get; set; } = default!; }
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

public class JobPostingAddAllHandler : IRequestHandler<JobPostingAddAllCmd, List<JobPostingListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobPostingAddAllHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<List<JobPostingListDto>> Handle(JobPostingAddAllCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Approved);
            var jReqL = _uow.Set<JobRequisition>().Where(r => r.WorkforcePlanId == request.AddDto.Id && r.Status == stat).ToList();
            if (jReqL.Count <= 0) { throw new DomainException($"APPROVED JOB REQUISITIONS NOT FOUND for selected work force plan."); }
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
            await _uow.Commit(ct);

            var res = new List<JobPostingListDto>();
            var response = await _med.Send(new JobPostingByWfpIdQry { Id = request.AddDto.Id }, ct);
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