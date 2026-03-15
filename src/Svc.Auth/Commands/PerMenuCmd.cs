using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Queries;

namespace Svc.Auth.Commands;

public class PerMenuAddCmd : IRequest<PerMenuListDto> { public PerMenuAddDto AddDto { get; set; } = default!; }
public class PerMenuModCmd : IRequest<PerMenuListDto> { public PerMenuModDto ModDto { get; set; } = default!; }
public class PerMenuDelCmd : IRequest { public Guid Id { get; set; } }



public class PerMenuAddCmdHandler : IRequestHandler<PerMenuAddCmd, PerMenuListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PerMenuAddCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PerMenuListDto> Handle(PerMenuAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            Guid? parentId = null;
            if (request.AddDto.IsChild)
            {
                var pa = await _med.Send(new PerMenuByKeyQry { Key = request.AddDto.ParentKey }, ct);
                if (pa != null) { parentId = pa.Id; }
                else { throw new DomainException($"PARENT MENU with Parent key {request.AddDto.ParentKey} NOT FOUND."); }
            }

            var data = new PerMenu
            {
                PerModuleId = request.AddDto.PerModuleId,
                Key = request.AddDto.Key,
                Label = request.AddDto.Label,
                Path = request.AddDto.Path,
                Icon = request.AddDto.Icon,
                IsChild = request.AddDto.IsChild,
                ParentId = parentId,
                Order = request.AddDto.Order
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var res = new PerMenuListDto();
            var response = await _med.Send(new PerMenuByIdQry { Id = data.Id }, ct);
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

public class PerMenuModCmdHandler : IRequestHandler<PerMenuModCmd, PerMenuListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PerMenuModCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PerMenuListDto> Handle(PerMenuModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PerMenu>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"MENU PERMISSION with Id {request.ModDto.Id} NOT FOUND."); }

            Guid? parentId = null;
            if (request.ModDto.IsChild)
            {
                var pa = await _med.Send(new PerMenuByKeyQry { Key = request.ModDto.ParentKey }, ct);
                if (pa != null) { parentId = pa.Id; }
            }
            oldData.PerModuleId = request.ModDto.PerModuleId;
            oldData.Key = request.ModDto.Key;
            oldData.Label = request.ModDto.Label;
            oldData.Path = request.ModDto.Path;
            oldData.Icon = request.ModDto.Icon;
            oldData.IsChild = request.ModDto.IsChild;
            oldData.ParentId = parentId;
            oldData.Order = request.ModDto.Order;
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PerMenuListDto();
            var response = await _med.Send(new PerMenuByIdQry { Id = request.ModDto.Id }, ct);
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

public class PerMenuDelCmdHandler : IRequestHandler<PerMenuDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PerMenuDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(PerMenuDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PerMenu>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"MENU PERMISSION with id [{request.Id}] NOT FOUND."); }
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