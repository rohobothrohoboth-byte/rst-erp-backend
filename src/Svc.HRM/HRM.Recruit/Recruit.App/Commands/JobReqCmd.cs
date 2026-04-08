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
            var wfpId = request.AddDto.WorkforcePlanId;
            var appPos = await _uow.Set<WorkforcePlan>().Where(x => x.Id == wfpId).Select(x => x.AppPositions).FirstOrDefaultAsync(ct);
            var addJr = _uow.Set<JobRequisition>().Where(x => x.WorkforcePlanId == wfpId).Sum(x => x.ReqQuantity);

            var rmn = appPos - addJr;
            if (request.AddDto.ReqPositions > rmn)
            {
                throw new DomainException($"MAXIMUM ALLOWED Requested Position for selected Work force plan is {rmn}.");
            }

            var jd = new JobDec
            {
                KeyRespo = request.AddDto.KeyRespo,
                Desc = request.AddDto.Desc,
                ReqQual = request.AddDto.ReqQual,
                KeySkills = request.AddDto.KeySkills,
                WorkLocation = request.AddDto.WorkLocation,
                PreGender = request.AddDto.PreGender,
                EmpNature = request.AddDto.EmpNature,
                WorkArr = request.AddDto.WorkArr
            };
            await _uow.Add(jd, ct);

            var data = new JobRequisition
            {
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

            var wfpId = oldData.WorkforcePlanId;
            var appPos = await _uow.Set<WorkforcePlan>().Where(x => x.Id == wfpId).Select(x => x.AppPositions).FirstOrDefaultAsync(ct);
            var addJr = _uow.Set<JobRequisition>().Where(x => x.WorkforcePlanId == wfpId && x.Id != request.ModDto.Id).Sum(x => x.ReqQuantity);

            var rmn = appPos - addJr;
            if (request.ModDto.ReqPositions > rmn)
            {
                throw new DomainException($"MAXIMUM ALLOWED Requested Position for selected Work force plan is {rmn}.");
            }

            var oldJd = await _uow.Set<JobDec>().FirstOrDefaultAsync(x => x.Id == oldData.JobDecId, ct);
            oldJd!.KeyRespo = request.ModDto.KeyRespo;
            oldJd.Desc = request.ModDto.Desc;
            oldJd.ReqQual = request.ModDto.ReqQual;
            oldJd.KeySkills = request.ModDto.KeySkills;
            oldJd.WorkLocation = request.ModDto.WorkLocation;
            oldJd.PreGender = request.ModDto.PreGender;
            oldJd.EmpNature = request.ModDto.EmpNature;
            oldJd.WorkArr = request.ModDto.WorkArr;
            await _uow.Update(oldJd);

            oldData.ReqReason = request.ModDto.ReqReason;
            oldData.ReqQuantity = request.ModDto.ReqPositions;
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