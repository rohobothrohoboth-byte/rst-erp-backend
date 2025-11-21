using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class EmpLeavePolicyAddCmd : IRequest<EmpLeavePolicyListDto> { public EmpLeavePolicyAddDto AddDto { get; set; } = default!; }
public class EmpLeavePolicyModCmd : IRequest<EmpLeavePolicyListDto> { public EmpLeavePolicyModDto ModDto { get; set; } = default!; }
public class EmpLeavePolicyDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpLeavePolicyAddCmdHandler : IRequestHandler<EmpLeavePolicyAddCmd, EmpLeavePolicyListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpLeavePolicyAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpLeavePolicyListDto> Handle(EmpLeavePolicyAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new EmpLeavePolicy
            {
                LeavePolicyId = request.AddDto.LeavePolicyId,
                EmployeeId = request.AddDto.EmployeeId
            };
            await _unitOfWork.Repository<EmpLeavePolicy>().Add(data);
            await _unitOfWork.Commit();

            var res = new EmpLeavePolicyListDto();
            var response = await _med.Send(new EmpLeavePolicyByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpLeavePolicyModCmdHandler : IRequestHandler<EmpLeavePolicyModCmd, EmpLeavePolicyListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public EmpLeavePolicyModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<EmpLeavePolicyListDto> Handle(EmpLeavePolicyModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<EmpLeavePolicy>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"EMPLOYEE LEAVE POLICY with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.LeavePolicyId = request.ModDto.LeavePolicyId;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            var data = await _unitOfWork.Repository<EmpLeavePolicy>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new EmpLeavePolicyListDto();
            var response = await _med.Send(new EmpLeavePolicyByIdQry { Id = data.Id }, cancellationToken);
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

public class EmpLeavePolicyDelCmdHandler : IRequestHandler<EmpLeavePolicyDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public EmpLeavePolicyDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(EmpLeavePolicyDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<EmpLeavePolicy>().GetById(request.Id);
            if (data == null) { throw new DomainException($"EMPLOYEE LEAVE POLICY with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<EmpLeavePolicy>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}