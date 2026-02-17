using Common;
using Helpers;
using MediatR;
using Recruit.App.Helpers;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class WorkforcePlanAddCmd : IRequest<WorkforcePlanListDto> { public WorkforcePlanAddDto AddDto { get; set; } = default!; }
public class WorkforcePlanModCmd : IRequest<WorkforcePlanListDto> { public WorkforcePlanModDto ModDto { get; set; } = default!; }
public class WorkforcePlanDelCmd : IRequest { public Guid Id { get; set; } }


public class WorkforcePlanAddHandler : IRequestHandler<WorkforcePlanAddCmd, WorkforcePlanListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;
    private readonly IHrmProfileClient _hrmProfileClient;

    public WorkforcePlanAddHandler(IUnitOfWork unitOfWork, IMediator med, IHrmProfileClient hrmProfileClient)
    {
        _unitOfWork = unitOfWork;
        _med = med;
        _hrmProfileClient = hrmProfileClient;
    }

    public async Task<WorkforcePlanListDto> Handle(WorkforcePlanAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var code = await new CodeGen(_unitOfWork).GetPlanCode();
            var stat = BoolToStr.EnumToString(ReqStatus.Pending);
            var dept = await _hrmProfileClient.GetEmpId(request.AddDto.RequistionById.ToString());
            var data = new WorkforcePlan
            {
                PlanCode = code,
                Title = request.AddDto.Title,
                Desc = request.AddDto.Desc,
                StartDate = request.AddDto.StartDate,
                EndDate = request.AddDto.EndDate,
                TotalPositions = request.AddDto.TotalPositions,
                AppPositions = 0,
                Status = stat,
                DepartmentId = dept != null ? Guid.Parse(dept!.DeptId) : Guid.Empty,
                PeriodId = request.AddDto.PeriodId,
                RequistionById = request.AddDto.RequistionById
            };
            await _unitOfWork.Repository<WorkforcePlan>().Add(data);
            await _unitOfWork.Commit();

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = data.Id }, cancellationToken);
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

public class WorkforcePlanModHandler : IRequestHandler<WorkforcePlanModCmd, WorkforcePlanListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public WorkforcePlanModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<WorkforcePlanListDto> Handle(WorkforcePlanModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<WorkforcePlan>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"WORKFORCE PLAN with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Title = request.ModDto.Title;
            oldData.Desc = request.ModDto.Desc;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.EndDate = request.ModDto.EndDate;
            oldData.TotalPositions = request.ModDto.TotalPositions;
            var data = await _unitOfWork.Repository<WorkforcePlan>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = data.Id }, cancellationToken);
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

public class WorkforcePlanDelHandler : IRequestHandler<WorkforcePlanDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public WorkforcePlanDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(WorkforcePlanDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<WorkforcePlan>().GetById(request.Id);
            if (data == null) { throw new DomainException($"WORKFORCE PLAN with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<WorkforcePlan>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}