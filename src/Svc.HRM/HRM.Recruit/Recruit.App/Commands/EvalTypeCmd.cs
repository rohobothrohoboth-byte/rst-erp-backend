using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class EvalTypeAddCmd : IRequest<EvalTypeListDto> { public EvalTypeAddDto AddDto { get; set; } = default!; }
public class EvalTypeModCmd : IRequest<EvalTypeListDto> { public EvalTypeModDto ModDto { get; set; } = default!; }
public class EvalTypeStatCmd : IRequest<EvalTypeListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class EvalTypeDelCmd : IRequest { public Guid Id { get; set; } }



public class EvalTypeAddHandler : IRequestHandler<EvalTypeAddCmd, EvalTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalTypeAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalTypeListDto> Handle(EvalTypeAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new EvaluationType
            {
                Name = request.AddDto.Name,
                MaxScore = request.AddDto.MaxScore,
                IsActive = true
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new EvalTypeListDto();
            var response = await _med.Send(new EvalTypeByIdQry { Id = data.Id }, ct);
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

public class EvalTypeModHandler : IRequestHandler<EvalTypeModCmd, EvalTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalTypeModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalTypeListDto> Handle(EvalTypeModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EvaluationType>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"EVALUATION TYPE with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.MaxScore = request.ModDto.MaxScore;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EvalTypeListDto();
            var response = await _med.Send(new EvalTypeByIdQry { Id = request.ModDto.Id }, ct);
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

public class EvalTypeStatHandler : IRequestHandler<EvalTypeStatCmd, EvalTypeListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EvalTypeStatHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<EvalTypeListDto> Handle(EvalTypeStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<EvaluationType>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, ct);
            if (oldData == null) { throw new DomainException($"EVALUATION TYPE with Id {request.StatDto.Id} NOT FOUND."); }

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new EvalTypeListDto();
            var response = await _med.Send(new EvalTypeByIdQry { Id = request.StatDto.Id }, ct);
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

public class EvalTypeDelHandler : IRequestHandler<EvalTypeDelCmd>
{
    private readonly IUnitOfWork _uow;
    public EvalTypeDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(EvalTypeDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<EvaluationType>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"EVALUATION TYPE with id [{request.Id}] NOT FOUND."); }
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