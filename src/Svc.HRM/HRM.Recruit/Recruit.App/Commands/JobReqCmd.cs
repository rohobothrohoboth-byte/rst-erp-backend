using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class JobRequisitionAddCmd : IRequest<JobReqListDto> { public JobReqAddDto AddDto { get; set; } = default!; }
public class JobRequisitionModCmd : IRequest<JobReqListDto> { public JobReqModDto ModDto { get; set; } = default!; }
public class JobRequisitionDelCmd : IRequest { public Guid Id { get; set; } }



public class JobRequisitionAddCmdHandler : IRequestHandler<JobRequisitionAddCmd, JobReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobRequisitionAddCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobReqListDto> Handle(JobRequisitionAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var jd = new JobDec
            {
                Title = request.AddDto.Title,
                Desc = request.AddDto.Desc,
                Qualification = request.AddDto.Qualification,
                KeySkills = request.AddDto.KeySkills,
                WorkLocation = request.AddDto.WorkLocation,
                PreGender = request.AddDto.PreGender,
                ContractType = request.AddDto.ContractType
            };
            await _uow.Add(jd, ct);

            var data = new JobRequisition
            {
                ReqNumber = request.AddDto.ReqNumber,
                ReqReason = request.AddDto.ReqReason,
                ReqQuantity = request.AddDto.ReqPositions,
                BudgetCode = request.AddDto.BudgetCode,
                Status = BoolToStr.EnumToString(ReqStatus.Pending),
                StartDate = request.AddDto.StartDate,
                PositionId = request.AddDto.PositionId,
                JgStepId = request.AddDto.JgStepId,
                WorkforcePlanId = request.AddDto.WorkforcePlanId,
                JobDecId = jd.Id
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new JobReqListDto();
            var response = await _med.Send(new JobReqByIdQry { Id = data.Id }, ct);
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

public class JobRequisitionModCmdHandler : IRequestHandler<JobRequisitionModCmd, JobReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobRequisitionModCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<JobReqListDto> Handle(JobRequisitionModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<JobRequisition>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"JOB REQUISITION with Id {request.ModDto.Id} NOT FOUND."); }

            var oldJd = await _uow.Set<JobDec>().FirstOrDefaultAsync(x => x.Id == oldData.JobDecId, ct);
            oldJd!.Title = request.ModDto.Title;
            oldJd.Desc = request.ModDto.Desc;
            oldJd.Qualification = request.ModDto.Qualification;
            oldJd.KeySkills = request.ModDto.KeySkills;
            oldJd.WorkLocation = request.ModDto.WorkLocation;
            oldJd.PreGender = request.ModDto.PreGender;
            oldJd.ContractType = request.ModDto.ContractType;
            await _uow.Update(oldJd);

            oldData.ReqReason = request.ModDto.ReqReason;
            oldData.BudgetCode = request.ModDto.BudgetCode;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.JgStepId = request.ModDto.JgStepId;
            oldData.Status = BoolToStr.EnumToString(ReqStatus.Pending);
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new JobReqListDto();
            var response = await _med.Send(new JobReqByIdQry { Id = request.ModDto.Id }, ct);
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

public class JobRequisitionDelCmdHandler : IRequestHandler<JobRequisitionDelCmd>
{
    private readonly IUnitOfWork _uow;
    public JobRequisitionDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(JobRequisitionDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<JobRequisition>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"JOB REQUISITION with id [{request.Id}] NOT FOUND."); }
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