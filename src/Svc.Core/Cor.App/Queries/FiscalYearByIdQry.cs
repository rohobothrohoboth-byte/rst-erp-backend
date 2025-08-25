using Cor.App.Interfaces;
using Cor.Domain.DTOs;
using Cor.Domain.Entities;
using MediatR;

namespace Cor.App.Queries;
public class FiscalYearByIdQry : IRequest<FiscYearListDto?>
{
    public Guid Id { get; set; }
}

public class FiscalYearByIdQryHandler : IRequestHandler<FiscalYearByIdQry, FiscYearListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public FiscalYearByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<FiscYearListDto?> Handle(FiscalYearByIdQry request, CancellationToken cancellationToken)
    {
        var yearFisc = await _unitOfWork.Repository<FiscalYear>().GetById(request.Id);
        if (yearFisc == null)
        {
            return null;
        }

        var c = new FiscYearListDto
        {
            Id = yearFisc.Id,
            Name = yearFisc.Name,
            DateStart = yearFisc.DateStart,
            DateEnd = yearFisc.DateEnd,
            IsActive = yearFisc.IsActive.ToString(),
            IsDeleted = yearFisc.IsDeleted,
            DateAdd = yearFisc.DateAdd,
            DateMod = yearFisc.DateMod,
            RowVersion = Convert.ToBase64String(yearFisc.RowVersion)
        };
        return c;
    }
}
