using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeavePolicyConfigAddCmd : IRequest<LeavePolicyConfigListDto> { public LeavePolicyConfigAddDto AddDto { get; set; } = default!; }
public class LeavePolicyConfigModCmd : IRequest<LeavePolicyConfigListDto> { public LeavePolicyConfigModDto ModDto { get; set; } = default!; }
public class LeavePolicyConfigDelCmd : IRequest { public Guid Id { get; set; } }
public class LeavePolicyStatCmd : IRequest { public Guid Id { get; set; } }

public class LeavePolicyConfigAddCmdHandler : IRequestHandler<LeavePolicyConfigAddCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeavePolicyConfigAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeavePolicyConfig
            {
                AnnualEntitlement = request.AddDto.AnnualEntitlement,
                AccrualFrequency = request.AddDto.AccrualFrequency,
                AccrualRate = request.AddDto.AccrualRate,
                MaxDaysPerReq = request.AddDto.MaxDaysPerReq,
                MaxCarryOverDays = request.AddDto.MaxCarryOverDays,
                MinServiceMonths = request.AddDto.MinServiceMonths,
                IsActive = true,
                FiscalYearId = request.AddDto.FiscalYearId,
                LeavePolicyId = request.AddDto.LeavePolicyId
            };
            await _unitOfWork.Repository<LeavePolicyConfig>().Add(data);
            await _unitOfWork.Commit();

            await _med.Send(new LeavePolicyStatCmd { Id = data.Id }, cancellationToken);

            var res = new LeavePolicyConfigListDto();
            var response = await _med.Send(new LeavePolicyConfigByIdQry { Id = data.Id }, cancellationToken);
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

public class LeavePolicyConfigModCmdHandler : IRequestHandler<LeavePolicyConfigModCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeavePolicyConfigModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeavePolicyConfig>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE POLICY CONFIGURATION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.AnnualEntitlement = request.ModDto.AnnualEntitlement;
            oldData.AccrualFrequency = request.ModDto.AccrualFrequency;
            oldData.AccrualRate = request.ModDto.AccrualRate;
            oldData.MaxDaysPerReq = request.ModDto.MaxDaysPerReq;
            oldData.MaxCarryOverDays = request.ModDto.MaxCarryOverDays;
            oldData.MinServiceMonths = request.ModDto.MinServiceMonths;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.FiscalYearId = request.ModDto.FiscalYearId;
            var data = await _unitOfWork.Repository<LeavePolicyConfig>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new LeavePolicyConfigListDto();
            var response = await _med.Send(new LeavePolicyConfigByIdQry { Id = data.Id }, cancellationToken);
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

public class LeavePolicyConfigDelCmdHandler : IRequestHandler<LeavePolicyConfigDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeavePolicyConfigDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeavePolicyConfigDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeavePolicyConfig>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE POLICY CONFIGURATION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeavePolicyConfig>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class LeavePolicyStatHandler : IRequestHandler<LeavePolicyStatCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeavePolicyStatHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeavePolicyStatCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = (await _unitOfWork.Repository<LeavePolicyConfig>().Find(c => c.Id != request.Id)).ToList();
            if (data.Count > 0)
            {
                foreach (var oldData in data)
                {
                    oldData.IsActive = false;
                    var res = await _unitOfWork.Repository<LeavePolicyConfig>().Update(oldData);
                }
                await _unitOfWork.Commit();
            }
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}