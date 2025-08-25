using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Queries;
public class BranchByIdQry : IRequest<BranchListDto?>
{
    public Guid Id { get; set; }
}

public class BranchByIdQryHandler : IRequestHandler<BranchByIdQry, BranchListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public BranchByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<BranchListDto?> Handle(BranchByIdQry request, CancellationToken cancellationToken)
    {
        var nBra = await _unitOfWork.Repository<Branch>().GetById(request.Id);
        if (nBra == null)
        {
            return null;
        }

        var comp = await _unitOfWork.Repository<Company>().GetById(nBra.CompId);
        if (comp == null) return null;
        var c = new BranchListDto
        {
            Id = nBra.Id,
            Name = nBra.Name,
            NameAm = nBra.NameAm,
            Code = nBra.Code,
            Location = nBra.Location,
            BranchType = nBra.BranchType.ToString(),
            BranchStat = nBra.BranchStat.ToString(),
            Comp = nBra.Name,
            CompAm = nBra.Name,
            OpenDate = nBra.OpenDate,
            IsDeleted = nBra.IsDeleted,
            DateAdd = nBra.DateAdd,
            DateMod = nBra.DateMod,
            RowVersion = Convert.ToBase64String(nBra.RowVersion)
        };
        return c;
    }
}