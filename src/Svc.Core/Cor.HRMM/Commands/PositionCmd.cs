using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Cor.HRMM.Queries;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.HRMM.Commands;

public class PositionAddCmd : IRequest<PositionListDto> { public PositionAddDto AddDto { get; set; } = default!; }
public class PositionModCmd : IRequest<PositionListDto> { public PositionModDto ModDto { get; set; } = default!; }
public class PositionDelCmd : IRequest { public Guid Id { get; set; } }



public class PositionAddHandler : IRequestHandler<PositionAddCmd, PositionListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PositionAddHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionListDto> Handle(PositionAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = new Position
            {
                Name = request.AddDto.Name,
                NameAm = request.AddDto.NameAm,
                NoOfPosition = request.AddDto.NoOfPosition,
                IsVacant = request.AddDto.IsVacant,
                DepartmentId = request.AddDto.DepartmentId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new PositionListDto();
            var response = await _med.Send(new PositionByIdQry { Id = data.Id }, ct);
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

public class PositionModHandler : IRequestHandler<PositionModCmd, PositionListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public PositionModHandler(IUnitOfWork unitOfWork, IMediator med) { _uow = unitOfWork; _med = med; }

    public async Task<PositionListDto> Handle(PositionModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<Position>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, cancellationToken: ct);
            if (oldData == null) { throw new DomainException($"POSITION with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Name = request.ModDto.Name;
            oldData.NameAm = request.ModDto.NameAm;
            oldData.NoOfPosition = request.ModDto.NoOfPosition;
            oldData.IsVacant = request.ModDto.IsVacant;
            oldData.DepartmentId = request.ModDto.DepartmentId;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PositionListDto();
            var response = await _med.Send(new PositionByIdQry { Id = request.ModDto.Id }, ct);
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

public class PositionDelHandler : IRequestHandler<PositionDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PositionDelHandler(IUnitOfWork unitOfWork) { _uow = unitOfWork; }

    public async Task Handle(PositionDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<Position>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POSITION with id [{request.Id}] NOT FOUND."); }
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