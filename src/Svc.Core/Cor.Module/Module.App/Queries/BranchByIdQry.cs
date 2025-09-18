using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;
using Module.Domain.Enums;

namespace Module.App.Queries;
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
            BranchType = ((BranchType)Enum.Parse(typeof(BranchType), nBra.BranchType)).ToDisplayName(),
            BranchStat = ((BranchStat)Enum.Parse(typeof(BranchStat), nBra.BranchStat)).ToDisplayName(),
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