using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class LeaveAppStepAddCmd : IRequest<LeaveAppStepListDto> { public LeaveAppStepAddDto AddDto { get; set; } = default!; }
public class LeaveAppStepModCmd : IRequest<LeaveAppStepListDto> { public LeaveAppStepModDto ModDto { get; set; } = default!; }
public class LeaveAppStepDelCmd : IRequest { public Guid Id { get; set; } }



public class LeaveAppStepAddCmdHandler : IRequestHandler<LeaveAppStepAddCmd, LeaveAppStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppStepAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<LeaveAppStepListDto> Handle(LeaveAppStepAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var chain = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(c => c.LeavePolicyId == request.AddDto.LeavePolicyId && c.IsActive == true, ct);
            if (chain == null) { throw new DomainException($"Leave Approval Step NOT CREATED, NO ACTIVE Approval chain was found."); }

            var data = new LeaveAppStep
            {
                StepName = request.AddDto.StepName,
                StepOrder = request.AddDto.StepOrder,
                Role = request.AddDto.Role,
                EmployeeId = request.AddDto.EmployeeId,
                IsFinal = request.AddDto.IsFinal,
                LeaveAppChainId = chain.Id
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new LeaveAppStepListDto();
            var response = await _med.Send(new LeaveAppStepByIdQry { Id = data.Id }, ct);
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

public class LeaveAppStepModCmdHandler : IRequestHandler<LeaveAppStepModCmd, LeaveAppStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppStepModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<LeaveAppStepListDto> Handle(LeaveAppStepModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeaveAppStep>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"LEAVE APPROVAL STEP with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.StepName = request.ModDto.StepName;
            oldData.StepOrder = request.ModDto.StepOrder;
            oldData.Role = request.ModDto.Role;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            oldData.IsFinal = request.ModDto.IsFinal;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeaveAppStepListDto();
            var response = await _med.Send(new LeaveAppStepByIdQry { Id = request.ModDto.Id }, ct);
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

public class LeaveAppStepDelCmdHandler : IRequestHandler<LeaveAppStepDelCmd>
{
    private readonly IUnitOfWork _uow;
    public LeaveAppStepDelCmdHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(LeaveAppStepDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<LeaveAppStep>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"LEAVE APPROVAL STEP with id [{request.Id}] NOT FOUND."); }
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
