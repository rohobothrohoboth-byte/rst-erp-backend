using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeaveBalanceAddCmd : IRequest<LeaveBalanceListDto> { public LeaveBalanceAddDto AddDto { get; set; } = default!; }
public class LeaveBalanceModCmd : IRequest<LeaveBalanceListDto> { public LeaveBalanceModDto ModDto { get; set; } = default!; }
public class LeaveBalanceDelCmd : IRequest { public Guid Id { get; set; } }

public class LeaveBalanceAddCmdHandler : IRequestHandler<LeaveBalanceAddCmd, LeaveBalanceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveBalanceAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveBalanceListDto> Handle(LeaveBalanceAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeaveBalance
            {
                EmployeeId = request.AddDto.EmployeeId,
                FiscalYearId = request.AddDto.FiscalYearId,
                LeavePolicyId = request.AddDto.LeavePolicyId,
                Balance = request.AddDto.Balance,
                Carried = request.AddDto.Carried,
                CarriedExpireDate = request.AddDto.CarriedExpireDate
            };
            await _unitOfWork.Repository<LeaveBalance>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeaveBalanceListDto();
            var response = await _med.Send(new LeaveBalanceByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveBalanceModCmdHandler : IRequestHandler<LeaveBalanceModCmd, LeaveBalanceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveBalanceModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveBalanceListDto> Handle(LeaveBalanceModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveBalance>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE BALANCE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.FiscalYearId = request.ModDto.FiscalYearId;
            oldData.EmployeeId = request.ModDto.EmployeeId;
            oldData.LeavePolicyId = request.ModDto.LeavePolicyId;
            oldData.Balance = request.ModDto.Balance;
            oldData.Carried = request.ModDto.Carried;
            oldData.CarriedExpireDate = request.ModDto.CarriedExpireDate;
            var data = await _unitOfWork.Repository<LeaveBalance>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeaveBalanceListDto();
            var response = await _med.Send(new LeaveBalanceByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveBalanceDelCmdHandler : IRequestHandler<LeaveBalanceDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveBalanceDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeaveBalanceDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeaveBalance>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE BALANCE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeaveBalance>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}