using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;

namespace Svc.Auth.Commands;

public class PerApiAddCmd : IRequest<PerApiListDto> { public PerApiAddDto AddDto { get; set; } = default!; }
public class PerApiModCmd : IRequest<PerApiListDto> { public PerApiModDto ModDto { get; set; } = default!; }
public class PerApiDelCmd : IRequest { public Guid Id { get; set; } }



public class PerApiAddCmdHandler : IRequestHandler<PerApiAddCmd, PerApiListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PerApiAddCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PerApiListDto> Handle(PerApiAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = new PerApiListDto();
            var pMenu = await _med.Send(new PerMenuByKeyQry { Key = request.AddDto.PerMenuKey }, ct);
            if (pMenu != null)
            {
                var data = new PerApi
                {
                    PerMenuId = pMenu.Id,
                    Key = request.AddDto.Key,
                    Desc = request.AddDto.Desc
                };
                await _uow.Add(data, ct);
                await _uow.Commit(ct);
                var response = await _med.Send(new PerApiByIdQry { Id = data.Id }, ct);
                if (response == null) { return res; }
                res = response;
            }

            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PerApiModCmdHandler(IUnitOfWork _uow, IMediator _med) : IRequestHandler<PerApiModCmd, PerApiListDto>
{
    public async Task<PerApiListDto> Handle(PerApiModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PerApi>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"ACCESS PERMISSION with Id {request.ModDto.Id} NOT FOUND."); }
            var pMenu = await _med.Send(new PerMenuByKeyQry { Key = request.ModDto.PerMenuKey }, ct);
            if (pMenu == null) { throw new DomainException($"MENU PERMISSION with Key {request.ModDto.PerMenuKey} NOT FOUND."); }

            oldData.PerMenuId = pMenu.Id;
            oldData.Key = request.ModDto.Key;
            oldData.Desc = request.ModDto.Desc;
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PerApiListDto();
            var response = await _med.Send(new PerApiByIdQry { Id = request.ModDto.Id }, ct);
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

public class PerApiDelCmdHandler : IRequestHandler<PerApiDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PerApiDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(PerApiDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PerApi>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"ACCESS PERMISSION with id [{request.Id}] NOT FOUND."); }
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