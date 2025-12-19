using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class PerApiAllQry : IRequest<List<MenuPerApiListDto>> { }
public class PerApiByIdQry : IRequest<PerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByMenuIdQry : IRequest<MenuPerApiListDto?> { public Guid Id { get; set; } }

public class PerApiAllQryHandler : IRequestHandler<PerApiAllQry, List<MenuPerApiListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<MenuPerApiListDto>> Handle(PerApiAllQry request, CancellationToken cancellationToken)
    {
        var allMenu = (await _unitOfWork.Repository<PerModule>().GetAll()).ToList();
        var dataL = new List<MenuPerApiListDto>();
        foreach (var mod in allMenu)
        {
            var modId = mod.Id;
            var m = new MenuPerApiListDto
            {
                PerMenuId = modId,
                PerMenu = mod.Desc
            };
            var dbData = (await _unitOfWork.Repository<PerApi>().Find(p => p.PerMenuId == modId)).ToList();
            if (dbData.Count > 0)
            {
                var perL = dbData.Select(data => new NameList { Id = data.Id, Name = data.Desc, }).ToList();
                m.PerApiList = perL;
            }
            dataL.Add(m);
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
        var menu = await _unitOfWork.Repository<PerModule>().GetById(data.PerMenuId);
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
        var mod = await _unitOfWork.Repository<PerModule>().GetById(request.Id);
        if (mod == null) { return null; }
        var allMod = (await _unitOfWork.Repository<PerApi>().Find(p => p.PerMenuId == request.Id)).ToList();
        if (allMod.Count <= 0) { return null; }
        var perL = allMod.Select(data => new NameList { Id = data.Id, Name = data.Desc, }).ToList();

        var dataL = new MenuPerApiListDto
        {
            PerMenuId = mod.Id,
            PerMenu = mod.Desc,
            PerApiList = perL
        };
        return dataL;
    }
}