using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Commands.Hier;

public class ModHierCmd : IRequest<HierListDto>
{
    public EditHierDto EditHierDto { get; set; } = default!;
}

public class ModHierCmdHandler : IRequestHandler<ModHierCmd, HierListDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ModHierCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

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
            var nHier = await _unitOfWork.Repository<Hierarchy>().GetById(hier.Id);
            if (nHier == null) { return res; }

            var par = await _unitOfWork.Repository<Company>().GetById(nHier.ParentId);
            var chi = await _unitOfWork.Repository<Company>().GetById(nHier.ChildId);
            if (par == null || chi == null) return res;
            res.Id = nHier.Id;
            res.Parent = par.Name;
            res.ParentAm = par.NameAm;
            res.Child = chi.Name;
            res.ChildAm = chi.NameAm;
            res.IsDeleted = nHier.IsDeleted;
            res.DateAdd = nHier.DateAdd;
            res.DateMod = nHier.DateMod;
            res.RowVersion = Convert.ToBase64String(nHier.RowVersion);
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}