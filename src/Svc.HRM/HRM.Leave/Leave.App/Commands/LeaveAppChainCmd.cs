using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class LeaveAppChainAddCmd : IRequest<LeaveAppChainListDto> { public LeaveAppChainAddDto AddDto { get; set; } = default!; }
public class LeaveAppChainModCmd : IRequest<LeaveAppChainListDto> { public LeaveAppChainModDto ModDto { get; set; } = default!; }
public class LeaveAppChainStatCmd : IRequest<LeaveAppChainListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class LeaveAppChainDelCmd : IRequest { public Guid Id { get; set; } }
public class AppChainStatCmd : IRequest { public Guid Id { get; set; } public Guid PolicyId { get; set; } }



public class LeaveAppChainAddHandler : IRequestHandler<LeaveAppChainAddCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppChainAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new LeaveAppChain
            {
                LeavePolicyId = request.AddDto.LeavePolicyId,
                EffectiveFrom = request.AddDto.EffectiveFrom,
                EffectiveTo = request.AddDto.EffectiveTo,
                IsActive = true
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            await _med.Send(new AppChainStatCmd { Id = data.Id, PolicyId = request.AddDto.LeavePolicyId }, ct);
            var res = new LeaveAppChainListDto();
            var response = await _med.Send(new LeaveAppChainByIdQry { Id = data.Id }, ct);
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

public class LeaveAppChainModHandler : IRequestHandler<LeaveAppChainModCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppChainModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"LEAVE APPROVAL CHAIN with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.LeavePolicyId = request.ModDto.LeavePolicyId;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.EffectiveTo = request.ModDto.EffectiveTo;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            await _med.Send(new AppChainStatCmd { Id = oldData.Id, PolicyId = request.ModDto.LeavePolicyId }, ct);
            var res = new LeaveAppChainListDto();
            var response = await _med.Send(new LeaveAppChainByIdQry { Id = request.ModDto.Id }, ct);
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

public class LeaveAppChainStatHandler : IRequestHandler<LeaveAppChainStatCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppChainStatHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, ct);
            if (oldData == null) { throw new DomainException($"LEAVE APPROVAL CHAIN with Id {request.StatDto.Id} NOT FOUND."); }

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            await _med.Send(new AppChainStatCmd { Id = request.StatDto.Id, PolicyId = oldData.LeavePolicyId }, ct);
            var res = new LeaveAppChainListDto();
            var response = await _med.Send(new LeaveAppChainByIdQry { Id = request.StatDto.Id }, ct);
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

public class LeaveAppChainDelHandler : IRequestHandler<LeaveAppChainDelCmd>
{
    private readonly IUnitOfWork _uow;
    public LeaveAppChainDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(LeaveAppChainDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"LEAVE APPROVAL CHAIN with id [{request.Id}] NOT FOUND."); }
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

public class AppChainStatHandler : IRequestHandler<AppChainStatCmd>
{
    private readonly IUnitOfWork _uow;
    public AppChainStatHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(AppChainStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = _uow.Set<LeaveAppChain>().Where(c => c.Id != request.Id && c.LeavePolicyId == request.PolicyId).ToList();
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