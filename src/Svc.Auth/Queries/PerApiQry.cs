using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

//public class PerApiAllQry : IRequest<List<MenuPerApiListDto>> { }
public class PerApiAllQry : IRequest<List<PerApiListDto>> { }
public class PerApiByIdQry : IRequest<PerApiListDto?> { public Guid Id { get; set; } }
public class PerApiByMenuIdQry : IRequest<MenuPerApiListDto?> { public Guid Id { get; set; } }

//public class PerApiAllQryHandler : IRequestHandler<PerApiAllQry, List<MenuPerApiListDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    public PerApiAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

//    public async Task<List<MenuPerApiListDto>> Handle(PerApiAllQry request, CancellationToken cancellationToken)
//    {
//        var allMenu = (await _unitOfWork.Repository<PerMenu>().GetAll()).ToList();
//        var dataL = new List<MenuPerApiListDto>();
//        foreach (var mod in allMenu)
//        {
//            var menuId = mod.Id;
//            var m = new MenuPerApiListDto
//            {
//                PerMenuId = menuId,
//                PerMenu = mod.Desc
//            };
//            var dbData = (await _unitOfWork.Repository<PerApi>().Find(p => p.PerMenuId == menuId)).ToList();
//            if (dbData.Count > 0)
//            {
//                var perL = dbData.Select(data => new NameList { Id = data.Id, Name = data.Desc, }).ToList();
//                m.PerApiList = perL;
//            }
//            dataL.Add(m);
//        }
//        return dataL;
//    }
//}


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