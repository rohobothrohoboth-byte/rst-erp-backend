using MediatR;
using Module.App.Interfaces;
using Module.App.Queries;
using Module.Domain.DTOs;
using Module.Domain.Entities;

namespace Module.App.Commands;

public class AddHierCmd : IRequest<HierListDto> { public AddHierDto AddHierDto { get; set; } = default!; }

public class ModHierCmd : IRequest<HierListDto> { public EditHierDto EditHierDto { get; set; } = default!; }

public class DelHierCmd : IRequest { public Guid Id { get; set; } }

public class AddHierCmdHandler : IRequestHandler<AddHierCmd, HierListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public AddHierCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<HierListDto> Handle(AddHierCmd request, CancellationToken cancellationToken)
    {
        var hier = new Hierarchy
        {
            ParentId = request.AddHierDto.ParentId,
            ChildId = request.AddHierDto.ChildId
        };

        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Hierarchy>().Add(hier);
            await _unitOfWork.Commit();

            var res = new HierListDto();
            var response = await _med.Send(new HierByIdQry { Id = hier.Id }, cancellationToken);
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

public class ModHierCmdHandler : IRequestHandler<ModHierCmd, HierListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public ModHierCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<HierListDto> Handle(ModHierCmd request, CancellationToken cancellationToken)
    {
        var oldHier = await _unitOfWork.Repository<Hierarchy>().GetById(request.EditHierDto.Id);
        if (oldHier == null)
        {
            throw new KeyNotFoundException($"HIERARCHY with Id {request.EditHierDto.Id} NOT FOUND.");
        }

        oldHier.ParentId = request.EditHierDto.ParentId;
        oldHier.ChildId = request.EditHierDto.ChildId;

        await _unitOfWork.Begin();

        try
        {
            var hier = await _unitOfWork.Repository<Hierarchy>().Update(oldHier);
            await _unitOfWork.Commit();

            var res = new HierListDto();
            var response = await _med.Send(new HierByIdQry { Id = hier.Id }, cancellationToken);
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

public class DelHierCmdHandler : IRequestHandler<DelHierCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public DelHierCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(DelHierCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            await _unitOfWork.Repository<Hierarchy>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}