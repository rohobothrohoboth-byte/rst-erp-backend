using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class PerMenuAllQry : IRequest<List<PerMenuListDto>> { }
public class PerMenuByIdQry : IRequest<PerMenuListDto?> { public Guid Id { get; set; } }
public class PerMenuByKeyQry : IRequest<NameList?> { public string Key { get; set; } = default!; }
public class PerMenuByModIdQry : IRequest<ModPerMenuListDto?> { public Guid Id { get; set; } }
public class PerMenuByUserIdQry : IRequest<List<ModPerMenuListDto>> { public string Id { get; set; } = default!; }

public class PerMenuAllQryHandler : IRequestHandler<PerMenuAllQry, List<PerMenuListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PerMenuListDto>> Handle(PerMenuAllQry request, CancellationToken cancellationToken)
    {
        var dataL = new List<PerMenuListDto>();
        var allMenuPer = (await _unitOfWork.Repository<PerMenu>().GetAll()).ToList();
        if (allMenuPer.Count <= 0) { return dataL; }
        var allMod = (await _unitOfWork.Repository<PerModule>().GetAll()).ToList();
        foreach (var per in allMenuPer)
        {
            var mod = allMod.Find(m => m.Id == per.PerModuleId);
            if (mod != null)
            {
                var m = new PerMenuListDto
                {
                    Id = per.Id,
                    Key = per.Key,
                    Name = per.Label,
                    Module = mod.Desc
                };
                dataL.Add(m);
            }
        }
        return dataL;
    }
}

public class PerMenuByIdQryHandler : IRequestHandler<PerMenuByIdQry, PerMenuListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PerMenuListDto?> Handle(PerMenuByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PerMenu>().GetById(request.Id);
        if (data == null) { return null; }
        var mod = await _unitOfWork.Repository<PerModule>().GetById(data.PerModuleId);
        if (mod == null) { return null; }
        var c = new PerMenuListDto
        {
            Id = data.Id,
            Key = data.Key,
            Name = data.Label,
            Module = mod.Desc
        };
        return c;
    }
}

public class PerMenuByKeyQryHandler : IRequestHandler<PerMenuByKeyQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuByKeyQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<NameList?> Handle(PerMenuByKeyQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PerMenu>().GetFoD(p => p.Key == request.Key);
        if (data == null) { return null; }
        var c = new NameList { Id = data.Id, Name = data.Label };
        return c;
    }
}

public class PerMenuByModIdQryHandler : IRequestHandler<PerMenuByModIdQry, ModPerMenuListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuByModIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<ModPerMenuListDto?> Handle(PerMenuByModIdQry request, CancellationToken cancellationToken)
    {
        var mod = await _unitOfWork.Repository<PerModule>().GetById(request.Id);
        if (mod == null) { return null; }
        var allMod = (await _unitOfWork.Repository<PerMenu>().Find(p => p.PerModuleId == request.Id)).ToList();
        if (allMod.Count <= 0) { return null; }
        var perL = allMod.Select(data => new NameList { Id = data.Id, Name = data.Label, }).ToList();

        var dataL = new ModPerMenuListDto { PerModuleId = mod.Id, PerModule = mod.Desc, PerMenuList = perL };
        return dataL;
    }
}

public class PerMenuByUserIdQryHandler : IRequestHandler<PerMenuByUserIdQry, List<ModPerMenuListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;
    public PerMenuByUserIdQryHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<List<ModPerMenuListDto>> Handle(PerMenuByUserIdQry request, CancellationToken cancellationToken)
    {
        var aPerMenu = new List<ModPerMenuListDto>();
        var uPerModule = (await _unitOfWork.Repository<UserPerModule>().Find(p => p.UserId == request.Id)).ToList();
        if (uPerModule.Count <= 0) { return aPerMenu; }

        foreach (var per in uPerModule)
        {
            var menu = await _med.Send(new PerMenuByModIdQry { Id = per.PerModuleId }, cancellationToken);
            if (menu != null)
            {
                aPerMenu.Add(menu);
            }
        }

        return aPerMenu;
    }
}