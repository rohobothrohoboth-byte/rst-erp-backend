using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class LeaveAppChainAddCmd : IRequest<LeaveAppChainListDto> { public LeaveAppChainAddDto AddDto { get; set; } = default!; }
public class LeaveAppChainModCmd : IRequest<LeaveAppChainListDto> { public LeaveAppChainModDto ModDto { get; set; } = default!; }
public class LeaveAppChainStatCmd : IRequest<LeaveAppChainListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class LeaveAppChainDelCmd : IRequest { public Guid Id { get; set; } }
public class AppChainStatCmd : IRequest { public Guid Id { get; set; } public Guid PolicyId { get; set; } }

public class LeaveAppChainAddHandler : IRequestHandler<LeaveAppChainAddCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveAppChainAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = new LeaveAppChain
            {
                LeavePolicyId = request.AddDto.LeavePolicyId,
                EffectiveFrom = request.AddDto.EffectiveFrom,
                EffectiveTo = request.AddDto.EffectiveTo,
                IsActive = true
            };
            await _unitOfWork.Repository<LeaveAppChain>().Add(data);
            await _unitOfWork.Commit();

            await _med.Send(new AppChainStatCmd { Id = data.Id, PolicyId = request.AddDto.LeavePolicyId }, cancellationToken);
            var res = new LeaveAppChainListDto();
            var response = await _med.Send(new LeaveAppChainByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveAppChainModHandler : IRequestHandler<LeaveAppChainModCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveAppChainModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveAppChain>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE APPROVAL CHAIN with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.LeavePolicyId = request.ModDto.LeavePolicyId;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.EffectiveTo = request.ModDto.EffectiveTo;
            oldData.IsActive = request.ModDto.IsActive;
            var data = await _unitOfWork.Repository<LeaveAppChain>().Update(oldData);
            await _unitOfWork.Commit();

            await _med.Send(new AppChainStatCmd { Id = oldData.Id, PolicyId = request.ModDto.LeavePolicyId }, cancellationToken);
            var res = new LeaveAppChainListDto();
            var response = await _med.Send(new LeaveAppChainByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveAppChainStatHandler : IRequestHandler<LeaveAppChainStatCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public LeaveAppChainStatHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainStatCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<LeaveAppChain>().GetById(request.StatDto.Id);
        if (oldData == null) { throw new DomainException($"LEAVE APPROVAL CHAIN with Id {request.StatDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.IsActive = request.StatDto.Stat;
            var data = await _unitOfWork.Repository<LeaveAppChain>().Update(oldData);
            await _unitOfWork.Commit();

            await _med.Send(new AppChainStatCmd { Id = oldData.Id, PolicyId = oldData.LeavePolicyId }, cancellationToken);
            var res = new LeaveAppChainListDto();
            var response = await _med.Send(new LeaveAppChainByIdQry { Id = data.Id }, cancellationToken);
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

public class LeaveAppChainDelHandler : IRequestHandler<LeaveAppChainDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public LeaveAppChainDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(LeaveAppChainDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<LeaveAppChain>().GetById(request.Id);
            if (data == null) { throw new DomainException($"LEAVE APPROVAL CHAIN with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<LeaveAppChain>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class AppChainStatHandler : IRequestHandler<AppChainStatCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public AppChainStatHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(AppChainStatCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = (await _unitOfWork.Repository<LeaveAppChain>().Find(c => c.Id != request.Id && c.LeavePolicyId == request.PolicyId)).ToList();
            if (data.Count > 0)
            {
                foreach (var oldData in data)
                {
                    oldData.IsActive = false;
                    var res = await _unitOfWork.Repository<LeaveAppChain>().Update(oldData);
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