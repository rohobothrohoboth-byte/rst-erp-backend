using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Queries;

public class AllHiersQry : IRequest<List<HierListDto>> { }

public class AllHiersQryHandler : IRequestHandler<AllHiersQry, List<HierListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllHiersQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<HierListDto>> Handle(AllHiersQry request, CancellationToken cancellationToken)
    {
        var hiers = await _unitOfWork.Repository<Hierarchy>().GetAll();
        var hierL = new List<HierListDto>();
        foreach (var hier in hiers)
        {
            var par = await _unitOfWork.Repository<Company>().GetById(hier.ParentId);
            var chi = await _unitOfWork.Repository<Company>().GetById(hier.ChildId);
            if (par == null || chi == null) continue;
            var c = new HierListDto
            {
                Id = hier.Id,
                Parent = par.Name,
                ParentAm = par.NameAm,
                Child = chi.Name,
                ChildAm = chi.NameAm,
                IsDeleted = hier.IsDeleted,
                DateAdd = chi.DateAdd,
                DateMod = chi.DateMod,
                RowVersion = Convert.ToBase64String(hier.RowVersion)
            };
            hierL.Add(c);
        }

        return hierL;
    }
}
