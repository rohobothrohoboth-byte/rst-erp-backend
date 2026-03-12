using Common;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    private readonly IHrmProfileClient _hrmProfile;

    public WorkforcePlanAddHandler(IUnitOfWork uow, IMediator med, IHrmProfileClient hrmProfile)
    {
        _uow = uow;
        _med = med;
        _hrmProfile = hrmProfile;
    }

    public async Task<WorkforcePlanListDto> Handle(WorkforcePlanAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var stat = BoolToStr.EnumToString(ReqStatus.Pending);
            var dept = await _hrmProfile.GetEmpId(request.AddDto.RequistionById.ToString(), ct);
            var data = new WorkforcePlan
            {
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
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = data.Id }, ct);
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

public class WorkforcePlanModHandler : IRequestHandler<WorkforcePlanModCmd, WorkforcePlanListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public WorkforcePlanModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<WorkforcePlanListDto> Handle(WorkforcePlanModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<WorkforcePlan>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"WORKFORCE PLAN with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Title = request.ModDto.Title;
            oldData.Desc = request.ModDto.Desc;
            oldData.StartDate = request.ModDto.StartDate;
            oldData.EndDate = request.ModDto.EndDate;
            oldData.TotalPositions = request.ModDto.TotalPositions;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new WorkforcePlanListDto();
            var response = await _med.Send(new WorkforcePlanByIdQry { Id = request.ModDto.Id }, ct);
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

public class WorkforcePlanDelHandler : IRequestHandler<WorkforcePlanDelCmd>
{
    private readonly IUnitOfWork _uow;
    public WorkforcePlanDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(WorkforcePlanDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<WorkforcePlan>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"WORKFORCE PLAN with id [{request.Id}] NOT FOUND."); }
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