using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeavePolicyAccrualAddCmd : IRequest<LeavePolicyAccrualListDto> { public LeavePolicyAccrualAddDto AddDto { get; set; } = default!; }
public class LeavePolicyAccrualModCmd : IRequest<LeavePolicyAccrualListDto> { public LeavePolicyAccrualModDto ModDto { get; set; } = default!; }
public class LeavePolicyAccrualDelCmd : IRequest { public Guid Id { get; set; } }

public class LeavePolicyAccrualAddCmdHandler : IRequestHandler<LeavePolicyAccrualAddCmd, LeavePolicyAccrualListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeavePolicyAccrualAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeavePolicyAccrualListDto> Handle(LeavePolicyAccrualAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeavePolicyAccrual
            {
                LeavePolicyId = request.AddDto.LeavePolicyId,
                Entitlement = request.AddDto.Entitlement,
                Frequency = request.AddDto.Frequency,
                AccrualRate = request.AddDto.AccrualRate,
                MinServiceMonths = request.AddDto.MinServiceMonths,
                MaxCarryoverDays = request.AddDto.MaxCarryoverDays,
                CarryoverExpiryDays = request.AddDto.CarryoverExpiryDays
            };
            await _unitOfWork.Repository<LeavePolicyAccrual>().Add(data);
            await _unitOfWork.Commit();

            var res = new LeavePolicyAccrualListDto();
            var response = await _med.Send(new LeavePolicyAccrualByIdQry { Id = data.Id }, cancellationToken);
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

public class LeavePolicyAccrualModCmdHandler : IRequestHandler<LeavePolicyAccrualModCmd, LeavePolicyAccrualListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeavePolicyAccrualModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeavePolicyAccrualListDto> Handle(LeavePolicyAccrualModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeavePolicyAccrual>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE POLICY with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.LeavePolicyId = request.ModDto.LeavePolicyId;
            oldData.Entitlement = request.ModDto.Entitlement;
            oldData.Frequency = request.ModDto.Frequency;
            oldData.AccrualRate = request.ModDto.AccrualRate;
            oldData.MinServiceMonths = request.ModDto.MinServiceMonths;
            oldData.MaxCarryoverDays = request.ModDto.MaxCarryoverDays;
            oldData.CarryoverExpiryDays = request.ModDto.CarryoverExpiryDays;
            var data = await _unitOfWork.Repository<LeavePolicyAccrual>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeavePolicyAccrualListDto();
            var response = await _med.Send(new LeavePolicyAccrualByIdQry { Id = data.Id }, cancellationToken);
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

public class LeavePolicyAccrualDelCmdHandler : IRequestHandler<LeavePolicyAccrualDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeavePolicyAccrualDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeavePolicyAccrualDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeavePolicyAccrual>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE POLICY with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeavePolicyAccrual>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}