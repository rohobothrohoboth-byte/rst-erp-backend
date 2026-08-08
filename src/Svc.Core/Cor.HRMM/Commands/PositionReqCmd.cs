using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cor.HRMM.Commands;

public class PosReqAddCmd : IRequest<PositionReqListDto> { public PositionReqAddDto AddDto { get; set; } = default!; }
public class PosReqModCmd : IRequest<PositionReqListDto> { public PositionReqModDto ModDto { get; set; } = default!; }
public class PosReqDelCmd : IRequest { public Guid Id { get; set; } }

public class PosReqAddHandler : IRequestHandler<PosReqAddCmd, PositionReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosReqAddHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PositionReqListDto> Handle(PosReqAddCmd request, CancellationToken ct)
    {
        PositionReq data = null!;

        await _uow.ExecuteAsync(async token =>
        {
            data = new PositionReq
            {
                Gender = request.AddDto.Gender,
                SaturdayWorkOption = request.AddDto.SaturdayWorkOption,
                SundayWorkOption = request.AddDto.SundayWorkOption,
                WorkingHours = request.AddDto.WorkingHours,
                ProfessionType = request.AddDto.ProfessionType,
                PositionId = request.AddDto.PositionId
            };
            await _uow.AddAsync(data, token);
        }, ct: ct);

        var response = await _med.Send(new PositionReqByIdQry { Id = data.Id }, ct);
        return response ?? new PositionReqListDto();
    }
}

public class PosReqModHandler : IRequestHandler<PosReqModCmd, PositionReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PosReqModHandler(IUnitOfWork unitOfWork, IMediator med)
    {
        _uow = unitOfWork;
        _med = med;
    }

    public async Task<PositionReqListDto> Handle(PosReqModCmd request, CancellationToken ct)
    {
        PositionReq? oldData = null!;

        await _uow.ExecuteAsync(async token =>
        {
            oldData = await _uow.Set<PositionReq>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, token);

            if (oldData == null)
            {
                throw new DomainException($"POSITION REQUIREMENT with Id {request.ModDto.Id} NOT FOUND.");
            }

            oldData.Gender = request.ModDto.Gender;
            oldData.SaturdayWorkOption = request.ModDto.SaturdayWorkOption;
            oldData.SundayWorkOption = request.ModDto.SundayWorkOption;
            oldData.WorkingHours = request.ModDto.WorkingHours;
            oldData.ProfessionType = request.ModDto.ProfessionType;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            _uow.Update(oldData);
        }, ct: ct);

        var response = await _med.Send(new PositionReqByIdQry { Id = request.ModDto.Id }, ct);
        return response ?? new PositionReqListDto();
    }
}

public class PosReqDelHandler : IRequestHandler<PosReqDelCmd>
{
    private readonly IUnitOfWork _uow;

    public PosReqDelHandler(IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
    }

    public async Task Handle(PosReqDelCmd request, CancellationToken ct)
    {
        await _uow.ExecuteAsync(async token =>
        {
            var data = await _uow.Set<PositionReq>()
                .FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (data == null)
            {
                throw new DomainException($"POSITION REQUIREMENT with id [{request.Id}] NOT FOUND.");
            }

            _uow.Delete(data);
        }, ct: ct);
    }
}