using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using Module.Domain.Enums;
using MediatR;

namespace Module.App.Queries;
public class AllBranchesQry : IRequest<List<BranchListDto>> { }

public class AllBranchesQryHandler : IRequestHandler<AllBranchesQry, List<BranchListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllBranchesQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<BranchListDto>> Handle(AllBranchesQry request, CancellationToken cancellationToken)
    {
        var bras = await _unitOfWork.Repository<Branch>().GetAll();
        var braL = new List<BranchListDto>();
        foreach (var nBra in bras)
        {
            var comp = await _unitOfWork.Repository<Company>().GetById(nBra.CompId);
            if (comp == null) continue;
            var c = new BranchListDto
            {
                Id = nBra.Id,
                Name = nBra.Name,
                NameAm = nBra.NameAm,
                Code = nBra.Code,
                Location = nBra.Location,
                BranchType = ((BranchType)Enum.Parse(typeof(BranchType), nBra.BranchType)).ToDisplayName(),
                BranchStat = ((BranchStat)Enum.Parse(typeof(BranchStat), nBra.BranchStat)).ToDisplayName(),
                Comp = comp.Name,
                CompAm = comp.NameAm,
                OpenDate = nBra.OpenDate,
                IsDeleted = nBra.IsDeleted,
                DateAdd = nBra.DateAdd,
                DateMod = nBra.DateMod,
                RowVersion = Convert.ToBase64String(nBra.RowVersion)
            };
            braL.Add(c);
        }

        return braL;
    }
}