using MediatR;
using Svc.Auth.Helpers;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PerMenuAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PerMenuListDto> Handle(PerMenuAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            Guid? parentId = null;
            if (request.AddDto.IsChild)
            {
                var pa = await _med.Send(new PerMenuByKeyQry { Key = request.AddDto.ParentKey }, cancellationToken);
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
            await _unitOfWork.Repository<PerMenu>().Add(data);
            await _unitOfWork.Commit();

            var res = new PerMenuListDto();
            var response = await _med.Send(new PerMenuByIdQry { Id = data.Id }, cancellationToken);
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

public class PerMenuModCmdHandler : IRequestHandler<PerMenuModCmd, PerMenuListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PerMenuModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PerMenuListDto> Handle(PerMenuModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PerMenu>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"MENU PERMISSION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            Guid? parentId = null;
            if (request.ModDto.IsChild)
            {
                var pa = await _med.Send(new PerMenuByKeyQry { Key = request.ModDto.ParentKey }, cancellationToken);
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
            var data = await _unitOfWork.Repository<PerMenu>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PerMenuListDto();
            var response = await _med.Send(new PerMenuByIdQry { Id = data.Id }, cancellationToken);
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

public class PerMenuDelCmdHandler : IRequestHandler<PerMenuDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PerMenuDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PerMenu>().GetById(request.Id);
            if (data == null) { throw new DomainException($"MENU PERMISSION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PerMenu>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}