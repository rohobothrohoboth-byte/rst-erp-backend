using MediatR;
using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;

namespace Module.App.Queries;

public class AllHiersQry : IRequest<List<HierListDto>> { }

public class HierByIdQry : IRequest<HierListDto?> { public Guid Id { get; set; } }

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

public class HierByIdQryHandler : IRequestHandler<HierByIdQry, HierListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public HierByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<HierListDto?> Handle(HierByIdQry request, CancellationToken cancellationToken)
    {
        var hier = await _unitOfWork.Repository<Hierarchy>().GetById(request.Id);
        if (hier == null)
        {
            return null;
        }

        var par = await _unitOfWork.Repository<Company>().GetById(hier.ParentId);
        var chi = await _unitOfWork.Repository<Company>().GetById(hier.ChildId);
        if (par == null || chi == null) return null;
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
        return c;
    }
}