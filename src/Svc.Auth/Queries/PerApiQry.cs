using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class PerApiAllQry : IRequest<List<PerApiListDto>> { }
public class PerApiByIdQry : IRequest<PerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByMenuIdQry : IRequest<MenuPerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByUserIdQry : IRequest<List<MenuPerApiListDto>> { public string Id { get; set; } = default!; }


public class PerApiAllQryHandler : IRequestHandler<PerApiAllQry, List<PerApiListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PerApiListDto>> Handle(PerApiAllQry request, CancellationToken cancellationToken)
    {
        var allApiPer = (await _unitOfWork.Repository<PerApi>().GetAll()).ToList();
        var allMenuPer =(await _unitOfWork.Repository<PerMenu>().GetAll()).ToList();
        var dataL = new List<PerApiListDto>();
        foreach (var per in allApiPer)
        {
            var mPer = allMenuPer.Find(p => p.Id == per.PerMenuId);
            if (mPer != null)
            {
                var m = new PerApiListDto
                {
                    Id = per.Id,
                    Key = per.Key,
                    Name = per.Desc,
                    PerMenuKey = mPer.Key,
                    PerMenuId = per.PerMenuId,
                    PerMenu = mPer.Desc
                };
                dataL.Add(m);
            }
        }
        return dataL;
    }
}

public class PerApiByIdQryHandler : IRequestHandler<PerApiByIdQry, PerApiListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<PerApiListDto?> Handle(PerApiByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PerApi>().GetById(request.Id);
        if (data == null) { return null; }
        var menu = await _unitOfWork.Repository<PerMenu>().GetById(data.PerMenuId);
        if (menu == null) { return null; }
        var c = new PerApiListDto
        {
            Id = data.Id,
            PerMenuId = data.PerMenuId,
            Key = data.Key,
            Name = data.Desc,
            PerMenu = menu.Desc
        };
        return c;
    }
}

public class PerApiByMenuIdQryHandler : IRequestHandler<PerApiByMenuIdQry, MenuPerApiListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiByMenuIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<MenuPerApiListDto?> Handle(PerApiByMenuIdQry request, CancellationToken cancellationToken)
    {
        var mod = await _unitOfWork.Repository<PerMenu>().GetById(request.Id);
        if (mod == null) { return null; }
        var allMenu = (await _unitOfWork.Repository<PerApi>().Find(p => p.PerMenuId == request.Id)).ToList();
        if (allMenu.Count <= 0) { return null; }
        var perL = allMenu.Select(data => new NameList { Id = data.Id, Name = data.Desc, }).ToList();

        var dataL = new MenuPerApiListDto
        {
            PerMenuId = mod.Id,
            PerMenu = mod.Desc,
            PerApiList = perL
        };
        return dataL;
    }
}

public class PerApiByUserIdQryHandler : IRequestHandler<PerApiByUserIdQry, List<MenuPerApiListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;
    public PerApiByUserIdQryHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<List<MenuPerApiListDto>> Handle(PerApiByUserIdQry request, CancellationToken cancellationToken)
    {
        var aPerApi = new List<MenuPerApiListDto>();
        var uPerMenu = (await _unitOfWork.Repository<UserPerMenu>().Find(p => p.UserId == request.Id)).ToList();
        if (uPerMenu.Count <= 0) { return aPerApi; }

        foreach (var per in uPerMenu)
        {
            var menu = await _med.Send(new PerApiByMenuIdQry { Id = per.PerMenuId }, cancellationToken);
            if (menu != null)
            {
                aPerApi.Add(menu);
            }
        }

        return aPerApi;
    }
}