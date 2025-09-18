using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Commands.Hier;

public class AddHierCmd : IRequest<HierListDto>
{
    public AddHierDto AddHierDto { get; set; } = default!;
}

public class AddHierCmdHandler : IRequestHandler<AddHierCmd, HierListDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddHierCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

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
