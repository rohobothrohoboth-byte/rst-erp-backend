using Helpers;
using MediatR;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PerApiAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PerApiListDto> Handle(PerApiAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var res = new PerApiListDto();
            var pMenu = await _med.Send(new PerMenuByKeyQry { Key = request.AddDto.PerMenuKey }, cancellationToken);
            if (pMenu != null)
            {
                var data = new PerApi
                {
                    PerMenuId = pMenu.Id,
                    Key = request.AddDto.Key,
                    Desc = request.AddDto.Desc
                };
                await _unitOfWork.Repository<PerApi>().Add(data);
                await _unitOfWork.Commit();
                var response = await _med.Send(new PerApiByIdQry { Id = data.Id }, cancellationToken);
                if (response == null) { return res; }
                res = response;
            }            

            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class PerApiModCmdHandler : IRequestHandler<PerApiModCmd, PerApiListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PerApiModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PerApiListDto> Handle(PerApiModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PerApi>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"ACCESS PERMISSION with Id {request.ModDto.Id} NOT FOUND."); }
        var pMenu = await _med.Send(new PerMenuByKeyQry { Key = request.ModDto.PerMenuKey }, cancellationToken);
        if (pMenu == null) { throw new DomainException($"MENU PERMISSION with Key {request.ModDto.PerMenuKey} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.PerMenuId = pMenu.Id;
            oldData.Key = request.ModDto.Key;
            oldData.Desc = request.ModDto.Desc;
            var data = await _unitOfWork.Repository<PerApi>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PerApiListDto();
            var response = await _med.Send(new PerApiByIdQry { Id = data.Id }, cancellationToken);
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

public class PerApiDelCmdHandler : IRequestHandler<PerApiDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PerApiDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PerApi>().GetById(request.Id);
            if (data == null) { throw new DomainException($"ACCESS PERMISSION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PerApi>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}