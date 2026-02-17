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

public class JobPostingCloseHandler : IRequestHandler<JobPostingCloseCmd, JobPostingListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobPostingCloseHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobPostingListDto> Handle(JobPostingCloseCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<JobPosting>().GetById(request.Id);
        if (oldData == null) { throw new DomainException($"JOB POSTING with Id {request.Id} NOT FOUND."); }

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