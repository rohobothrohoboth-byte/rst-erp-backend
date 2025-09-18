using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Queries;

public class AllCompsQry : IRequest<List<CompListDto>> { }

public class GetCompsQryHandler : IRequestHandler<AllCompsQry, List<CompListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompsQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<CompListDto>> Handle(AllCompsQry request, CancellationToken cancellationToken)
    {
        var comps = await _unitOfWork.Repository<Company>().GetAll();
        var compL = new List<CompListDto>();
        foreach (var comp in comps)
        {
            var bra = await _unitOfWork.Repository<Branch>().Find(b => b.CompId == comp.Id);
            var c = new CompListDto
            {
                Id = comp.Id,
                Name = comp.Name,
                NameAm = comp.NameAm,
                IsDeleted = comp.IsDeleted,
                BranchCount = $"{bra.Count()}",
                DateAdd = comp.DateAdd,
                DateMod = comp.DateMod,
                RowVersion = Convert.ToBase64String(comp.RowVersion),
            };
            compL.Add(c);
        }

        return compL;
    }
}
