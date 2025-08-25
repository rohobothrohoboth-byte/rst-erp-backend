using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Queries;
public class HierByIdQry : IRequest<HierListDto?>
{
    public Guid Id { get; set; }
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