using MediatR;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;

namespace Svc.Auth.Queries;

public class ModuleAllQry : IRequest<List<ModuleListDto>> { }
public class ModuleByIdQry : IRequest<ModuleListDto?> { public Guid Id { get; set; }}

public class ModuleAllQryHandler : IRequestHandler<ModuleAllQry, List<ModuleListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public ModuleAllQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<ModuleListDto>> Handle(ModuleAllQry request, CancellationToken cancellationToken)
    {
        var dbData = await _unitOfWork.Repository<PerModule>().GetAll();
        var dataL = dbData.Select(data => new ModuleListDto { Id = data.Id, Key = data.Key, Name = data.Desc }).ToList();
        return dataL;
    }
}

public class ModuleByIdQryHandler : IRequestHandler<ModuleByIdQry, ModuleListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public ModuleByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<ModuleListDto?> Handle(ModuleByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PerModule>().GetById(request.Id);
        if (data == null) { return null; }
        var c = new ModuleListDto
        {
            Id = data.Id,
            Key = data.Key,
            Name = data.Desc
        };
        return c;
    }
}