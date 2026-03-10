using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class PositionExpAddCmd : IRequest<PositionExpListDto> { public PositionExpAddDto AddDto { get; set; } = default!; }
public class PositionExpModCmd : IRequest<PositionExpListDto> { public PositionExpModDto ModDto { get; set; } = default!; }
public class PositionExpDelCmd : IRequest { public Guid Id { get; set; } }



public class PosExpAddHandler : IRequestHandler<PositionExpAddCmd, PositionExpListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosExpAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionExpListDto> Handle(PositionExpAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new PositionExp
            {
                SamePosExp = request.AddDto.SamePosExp,
                OtherPosExp = request.AddDto.OtherPosExp,
                MinAge = request.AddDto.MinAge,
                MaxAge = request.AddDto.MaxAge,
                PositionId = request.AddDto.PositionId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new PositionExpListDto();
            var response = await _med.Send(new PositionExpByIdQry { Id = data.Id }, ct);
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

public class PosExpModHandler : IRequestHandler<PositionExpModCmd, PositionExpListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosExpModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionExpListDto> Handle(PositionExpModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PositionExp>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"POSITION EXPERIENCE with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.SamePosExp = request.ModDto.SamePosExp;
            oldData.OtherPosExp = request.ModDto.OtherPosExp;
            oldData.MinAge = request.ModDto.MinAge;
            oldData.MaxAge = request.ModDto.MaxAge;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PositionExpListDto();
            var response = await _med.Send(new PositionExpByIdQry { Id = request.ModDto.Id }, ct);
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

public class PosExpDelHandler : IRequestHandler<PositionExpDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PosExpDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(PositionExpDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PositionExp>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POSITION EXPERIENCE with id [{request.Id}] NOT FOUND."); }
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