using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;

namespace Module.App.Queries;

public class CompByIdQry : IRequest<CompListDto?>
{
    public Guid Id { get; set; }
}

public class GetCompByIdQryHandler : IRequestHandler<CompByIdQry, CompListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<CompListDto?> Handle(CompByIdQry request, CancellationToken cancellationToken)
    {
        var comp = await _unitOfWork.Repository<Company>().GetById(request.Id);
        if (comp == null)
        {
            return null;
        }

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
        return c;
    }
}
