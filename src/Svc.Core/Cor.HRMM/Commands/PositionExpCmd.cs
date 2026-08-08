using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.HRMM.Commands;

public class PositionExpAddCmd : IRequest<PositionExpListDto> { public PositionExpAddDto AddDto { get; set; } = default!; }
public class PositionExpModCmd : IRequest<PositionExpListDto> { public PositionExpModDto ModDto { get; set; } = default!; }
public class PositionExpDelCmd : IRequest { public Guid Id { get; set; } }

public class PosExpAddHandler : IRequestHandler<PositionExpAddCmd, PositionExpListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosExpAddHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PositionExpListDto> Handle(PositionExpAddCmd request, CancellationToken ct)
    {
        PositionExp data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new PositionExp
            {
                SamePosExp = request.AddDto.SamePosExp,
                OtherPosExp = request.AddDto.OtherPosExp,
                MinAge = request.AddDto.MinAge,
                MaxAge = request.AddDto.MaxAge,
                PositionId = request.AddDto.PositionId
            };
            await _uow.AddAsync(data, token);
        }, ct: ct);

        var response = await _med.Send(new PositionExpByIdQry { Id = data.Id }, ct);
        return response ?? new PositionExpListDto();
    }
}

public class PosExpModHandler : IRequestHandler<PositionExpModCmd, PositionExpListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosExpModHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PositionExpListDto> Handle(PositionExpModCmd request, CancellationToken ct)
    {
        PositionExp? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<PositionExp>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"POSITION EXPERIENCE with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.SamePosExp = request.ModDto.SamePosExp;
            oldData.OtherPosExp = request.ModDto.OtherPosExp;
            oldData.MinAge = request.ModDto.MinAge;
            oldData.MaxAge = request.ModDto.MaxAge;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            _uow.Update(oldData);
        }, ct: ct);

        var response = await _med.Send(new PositionExpByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new PositionExpListDto();
    }
}

public class PosExpDelHandler : IRequestHandler<PositionExpDelCmd>
{
    private readonly IUnitOfWork _uow;

    public PosExpDelHandler(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task Handle(PositionExpDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<PositionExp>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"POSITION EXPERIENCE with id [{request.Id}] NOT FOUND.");
            }

            _uow.Delete(data);
        }, ct: ct);
    }
}