using MediatR;
using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using Module.Domain.Enums;

namespace Module.App.Queries;

public class AllBranchesQry : IRequest<List<BranchListDto>> { }

public class BranchByIdQry : IRequest<BranchListDto?> { public Guid Id { get; set; } }

public class BranchByCompQry : IRequest<List<BranchListDto>> { public Guid Id { get; set; } }

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