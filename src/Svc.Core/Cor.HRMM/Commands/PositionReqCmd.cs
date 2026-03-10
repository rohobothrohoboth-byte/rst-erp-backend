using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class PosReqAddCmd : IRequest<PositionReqListDto> { public PositionReqAddDto AddDto { get; set; } = default!; }
public class PosReqModCmd : IRequest<PositionReqListDto> { public PositionReqModDto ModDto { get; set; } = default!; }
public class PosReqDelCmd : IRequest { public Guid Id { get; set; } }



public class PosReqAddHandler : IRequestHandler<PosReqAddCmd, PositionReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosReqAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionReqListDto> Handle(PosReqAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new PositionReq
            {
                Gender = request.AddDto.Gender,
                SaturdayWorkOption = request.AddDto.SaturdayWorkOption,
                SundayWorkOption = request.AddDto.SundayWorkOption,
                WorkingHours = request.AddDto.WorkingHours,
                ProfessionType = request.AddDto.ProfessionType,
                PositionId = request.AddDto.PositionId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new PositionReqListDto();
            var response = await _med.Send(new PositionReqByIdQry { Id = data.Id }, ct);
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

public class PosReqModHandler : IRequestHandler<PosReqModCmd, PositionReqListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PosReqModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionReqListDto> Handle(PosReqModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PositionReq>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"POSITION REQUIREMENT with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Gender = request.ModDto.Gender;
            oldData.SaturdayWorkOption = request.ModDto.SaturdayWorkOption;
            oldData.SundayWorkOption = request.ModDto.SundayWorkOption;
            oldData.WorkingHours = request.ModDto.WorkingHours;
            oldData.ProfessionType = request.ModDto.ProfessionType;
            oldData.PositionId = request.ModDto.PositionId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PositionReqListDto();
            var response = await _med.Send(new PositionReqByIdQry { Id = request.ModDto.Id }, ct);
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

public class PosReqDelHandler : IRequestHandler<PosReqDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PosReqDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(PosReqDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PositionReq>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POSITION REQUIREMENT with id [{request.Id}] NOT FOUND."); }
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