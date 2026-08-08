using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class EvalFlowAddCmd : IRequest<EvalFlowListDto> { public EvalFlowAddDto AddDto { get; set; } = default!; }
public class EvalFlowModCmd : IRequest<EvalFlowListDto> { public EvalFlowModDto ModDto { get; set; } = default!; }
public class EvalFlowStatCmd : IRequest<EvalFlowListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class EvalFlowDelCmd : IRequest { public Guid Id { get; set; } }



public class EvalFlowAddHandler : IRequestHandler<EvalFlowAddCmd, EvalFlowListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalFlowAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalFlowListDto> Handle(EvalFlowAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EvaluationFlow
            {
                Name = request.AddDto.Name,
                IsGlobal = request.AddDto.IsGlobal,
                IsActive = true
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new EvalFlowListDto();
            var response = await _med.Send(new EvalFlowByIdQry { Id = data.Id }, ct);
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

public class EvalFlowModHandler : IRequestHandler<EvalFlowModCmd, EvalFlowListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalFlowModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalFlowListDto> Handle(EvalFlowModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EvaluationFlow>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"EVALUATION FLOW with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.IsGlobal = request.ModDto.IsGlobal;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EvalFlowListDto();
            var response = await _med.Send(new EvalFlowByIdQry { Id = request.ModDto.Id }, ct);
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

public class EvalFlowStatHandler : IRequestHandler<EvalFlowStatCmd, EvalFlowListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalFlowStatHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalFlowListDto> Handle(EvalFlowStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EvaluationFlow>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, ct);
            if (oldData == null) { throw new DomainException($"EVALUATION FLOW with Id {request.StatDto.Id} NOT FOUND."); }

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EvalFlowListDto();
            var response = await _med.Send(new EvalFlowByIdQry { Id = request.StatDto.Id }, ct);
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

public class EvalFlowDelHandler : IRequestHandler<EvalFlowDelCmd>
{
    private readonly IUnitOfWork _uow;
    public EvalFlowDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(EvalFlowDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EvaluationFlow>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"EVALUATION FLOW with id [{request.Id}] NOT FOUND."); }
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