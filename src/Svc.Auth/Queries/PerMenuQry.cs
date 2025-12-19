using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class PerMenuAllQry : IRequest<List<ModPerMenuListDto>> { }
public class PerMenuByIdQry : IRequest<PerMenuListDto?> { public Guid Id { get; set; } }
public class PerMenuByModIdQry : IRequest<ModPerMenuListDto?> { public Guid Id { get; set; } }

public class PerMenuAllQryHandler : IRequestHandler<PerMenuAllQry, List<ModPerMenuListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<ModPerMenuListDto>> Handle(PerMenuAllQry request, CancellationToken cancellationToken)
    {
        var allMod = (await _unitOfWork.Repository<PerModule>().GetAll()).ToList();
        var dataL = new List<ModPerMenuListDto>();
        foreach (var mod in allMod)
        {
            var modId = mod.Id;
            var m = new ModPerMenuListDto
            {
                PerModuleId = modId,
                PerModule = mod.Desc
            };
            var dbData = (await _unitOfWork.Repository<PerMenu>().Find(p => p.PerModuleId == modId)).ToList();
            if (dbData.Count > 0)
            {
                var perL = dbData.Select(data => new NameList { Id = data.Id, Name = data.Desc, }).ToList();
                m.PerMenuList = perL;
            }
            dataL.Add(m);
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
            PerModuleId = data.PerModuleId,
            Key = data.Key,
            Name = data.Desc,
            Module = mod.Desc
        };
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
        var perL = allMod.Select(data => new NameList { Id = data.Id, Name = data.Desc, }).ToList();

        var dataL = new ModPerMenuListDto
        {
            PerModuleId = mod.Id,
            PerModule = mod.Desc,
            PerMenuList = perL
        };
        return dataL;
    }
}