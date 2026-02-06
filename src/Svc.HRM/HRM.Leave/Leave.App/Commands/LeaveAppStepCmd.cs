using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeaveAppStepAddCmd : IRequest<LeaveAppStepListDto> { public LeaveAppStepAddDto AddDto { get; set; } = default!; }
public class LeaveAppStepModCmd : IRequest<LeaveAppStepListDto> { public LeaveAppStepModDto ModDto { get; set; } = default!; }
public class LeaveAppStepDelCmd : IRequest { public Guid Id { get; set; } }

public class LeaveAppStepAddCmdHandler : IRequestHandler<LeaveAppStepAddCmd, LeaveAppStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveAppStepAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveAppStepListDto> Handle(LeaveAppStepAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var chain = await _unitOfWork.Repository<LeaveAppChain>().GetFoD(c => c.LeavePolicyId == request.AddDto.LeavePolicyId && c.IsActive == true);
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
            await _unitOfWork.Repository<LeaveAppStep>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeaveAppStepListDto();
            var response = await _med.Send(new LeaveAppStepByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveAppStepModCmdHandler : IRequestHandler<LeaveAppStepModCmd, LeaveAppStepListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveAppStepModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveAppStepListDto> Handle(LeaveAppStepModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveAppStep>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE APPROVAL STEP with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.StepName = request.ModDto.StepName;
            oldData.StepOrder = request.ModDto.StepOrder;
            oldData.Role = request.ModDto.Role;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            oldData.IsFinal = request.ModDto.IsFinal;
            var data = await _unitOfWork.Repository<LeaveAppStep>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeaveAppStepListDto();
            var response = await _med.Send(new LeaveAppStepByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveAppStepDelCmdHandler : IRequestHandler<LeaveAppStepDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveAppStepDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeaveAppStepDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeaveAppStep>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE APPROVAL STEP with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeaveAppStep>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}
