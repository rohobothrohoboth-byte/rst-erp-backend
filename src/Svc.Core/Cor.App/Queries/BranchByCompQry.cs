using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using Cor.Domain.Enums;
using MediatR;

namespace Cor.App.Queries;

public class BranchByCompQry : IRequest<List<BranchListDto>>
{
    public Guid Id { get; set; }
}

public class BranchByCompQryHandler : IRequestHandler<BranchByCompQry, List<BranchListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BranchByCompQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<BranchListDto>> Handle(BranchByCompQry request, CancellationToken cancellationToken)
    {
        var bras = await _unitOfWork.Repository<Branch>().Find(c => c.CompId == request.Id);
        var braL = new List<BranchListDto>();
        var nBras = bras.ToList();
        if (nBras.Count <= 0) return braL;
        foreach (var nBra in nBras)
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