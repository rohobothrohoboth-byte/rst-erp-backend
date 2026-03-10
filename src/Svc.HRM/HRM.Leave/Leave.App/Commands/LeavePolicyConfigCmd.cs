using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class LeavePolicyConfigAddCmd : IRequest<LeavePolicyConfigListDto> { public LeavePolicyConfigAddDto AddDto { get; set; } = default!; }
public class LeavePolicyConfigModCmd : IRequest<LeavePolicyConfigListDto> { public LeavePolicyConfigModDto ModDto { get; set; } = default!; }
public class LeavePolicyConfigStatCmd : IRequest<LeavePolicyConfigListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class LeavePolicyConfigDelCmd : IRequest { public Guid Id { get; set; } }
public class LeavePolicyStatCmd : IRequest { public Guid Id { get; set; } public Guid PolicyId { get; set; } }



public class LeavePolicyConfigAddHandler : IRequestHandler<LeavePolicyConfigAddCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyConfigAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
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
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            await _med.Send(new LeavePolicyStatCmd { Id = data.Id, PolicyId = request.AddDto.LeavePolicyId }, ct);

            var res = new LeavePolicyConfigListDto();
            var response = await _med.Send(new LeavePolicyConfigByIdQry { Id = data.Id }, ct);
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

public class LeavePolicyConfigModHandler : IRequestHandler<LeavePolicyConfigModCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyConfigModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeavePolicyConfig>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id);
            if (oldData == null) { throw new DomainException($"LEAVE POLICY CONFIGURATION with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.AnnualEntitlement = request.ModDto.AnnualEntitlement;
            oldData.AccrualFrequency = request.ModDto.AccrualFrequency;
            oldData.AccrualRate = request.ModDto.AccrualRate;
            oldData.MaxDaysPerReq = request.ModDto.MaxDaysPerReq;
            oldData.MaxCarryOverDays = request.ModDto.MaxCarryOverDays;
            oldData.MinServiceMonths = request.ModDto.MinServiceMonths;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.FiscalYearId = request.ModDto.FiscalYearId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeavePolicyConfigListDto();
            var response = await _med.Send(new LeavePolicyConfigByIdQry { Id = request.ModDto.Id }, ct);
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

public class LeavePolicyConfigStatHandler : IRequestHandler<LeavePolicyConfigStatCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyConfigStatHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeavePolicyConfig>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id);
            if (oldData == null) { throw new DomainException($"LEAVE POLICY CONFIGURATION with Id {request.StatDto.Id} NOT FOUND."); }

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new LeavePolicyConfigListDto();
            var response = await _med.Send(new LeavePolicyConfigByIdQry { Id = request.StatDto.Id }, ct);
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

public class LeavePolicyConfigDelHandler : IRequestHandler<LeavePolicyConfigDelCmd>
{
    private readonly IUnitOfWork _uow;
    public LeavePolicyConfigDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(LeavePolicyConfigDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<LeavePolicyConfig>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"LEAVE POLICY CONFIGURATION with id [{request.Id}] NOT FOUND."); }
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

public class LeavePolicyStatHandler : IRequestHandler<LeavePolicyStatCmd>
{
    private readonly IUnitOfWork _uow;
    public LeavePolicyStatHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(LeavePolicyStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = _uow.Set<LeavePolicyConfig>().Where(c => c.Id != request.Id && c.LeavePolicyId == request.PolicyId).ToList();
            if (data.Count > 0)
            {
                foreach (var oldData in data)
                {
                    oldData.IsActive = false;
                    await _uow.Update(oldData);
                }
                await _uow.Commit(ct);
            }
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}