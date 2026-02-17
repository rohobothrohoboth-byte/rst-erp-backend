using Helpers;
using MediatR;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobRequisitionAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobReqListDto> Handle(JobRequisitionAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
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
            await _unitOfWork.Repository<JobDec>().Add(jd);

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
            await _unitOfWork.Repository<JobRequisition>().Add(data);
            await _unitOfWork.Commit();

            var res = new JobReqListDto();
            var response = await _med.Send(new JobReqByIdQry { Id = data.Id }, cancellationToken);
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

public class JobRequisitionModCmdHandler : IRequestHandler<JobRequisitionModCmd, JobReqListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public JobRequisitionModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<JobReqListDto> Handle(JobRequisitionModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<JobRequisition>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"JOB REQUISITION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            var oldJd = await _unitOfWork.Repository<JobDec>().GetById(oldData.JobDecId);
            oldJd!.Title = request.ModDto.Title;
            oldJd.Desc = request.ModDto.Desc;
            oldJd.Qualification = request.ModDto.Qualification;
            oldJd.KeySkills = request.ModDto.KeySkills;
            oldJd.WorkLocation = request.ModDto.WorkLocation;
            oldJd.PreGender = request.ModDto.PreGender;
            oldJd.ContractType = request.ModDto.ContractType;
            var jd = await _unitOfWork.Repository<JobDec>().Update(oldJd);

            oldData.ReqReason = request.ModDto.ReqReason;
            oldData.BudgetCode = request.ModDto.BudgetCode;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.JgStepId = request.ModDto.JgStepId;
            oldData.Status = BoolToStr.EnumToString(ReqStatus.Pending);
            oldData.JobDecId = jd.Id;
            var data = await _unitOfWork.Repository<JobRequisition>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new JobReqListDto();
            var response = await _med.Send(new JobReqByIdQry { Id = data.Id }, cancellationToken);
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

public class JobRequisitionDelCmdHandler : IRequestHandler<JobRequisitionDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public JobRequisitionDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(JobRequisitionDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<JobRequisition>().GetById(request.Id);
            if (data == null) { throw new DomainException($"JOB REQUISITION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<JobRequisition>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}