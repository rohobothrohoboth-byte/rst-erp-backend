using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class ModuleNameAllQry : IRequest<List<NameList>> { }
public class ModuleNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class PerMenuNameAllQry : IRequest<List<NameList>> { }
public class PerMenuNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }
public class PerApiNameAllQry : IRequest<List<NameList>> { }
public class PerApiNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class ModuleNameAllQryHandler : IRequestHandler<ModuleNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public ModuleNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<NameList>> Handle(ModuleNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PerModule>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Desc }).ToList();
    }
}

public class ModuleNameByIdQryHandler : IRequestHandler<ModuleNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public ModuleNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<NameList?> Handle(ModuleNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<PerModule>().GetById(request.Id);
        if (nData == null) { return null; }
        var c = new NameList { Id = nData.Id, Name = nData.Desc };
        return c;
    }
}

public class PerMenuNameAllQryHandler : IRequestHandler<PerMenuNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<NameList>> Handle(PerMenuNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PerMenu>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Desc }).ToList();
    }
}

public class PerMenuNameByIdQryHandler : IRequestHandler<PerMenuNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerMenuNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<NameList?> Handle(PerMenuNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<PerMenu>().GetById(request.Id);
        if (nData == null) { return null; }
        var c = new NameList { Id = nData.Id, Name = nData.Desc };
        return c;
    }
}

public class PerApiNameAllQryHandler : IRequestHandler<PerApiNameAllQry, List<NameList>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiNameAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<List<NameList>> Handle(PerApiNameAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PerApi>().GetAll();
        return dbData.Select(data => new NameList { Id = data.Id, Name = data.Desc }).ToList();
    }
}

public class PerApiNameByIdQryHandler : IRequestHandler<PerApiNameByIdQry, NameList?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PerApiNameByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task<NameList?> Handle(PerApiNameByIdQry request, CancellationToken cancellationToken)
    {
        var nData = await _unitOfWork.Repository<PerApi>().GetById(request.Id);
        if (nData == null) { return null; }
        var c = new NameList { Id = nData.Id, Name = nData.Desc };
        return c;
    }
}
