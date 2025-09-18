using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;
using Module.Domain.Enums;

namespace Module.App.Queries;
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
            IsActive = ((YesNo)Enum.Parse(typeof(YesNo), yearFisc.IsActive)).ToDisplayName(),
            IsDeleted = yearFisc.IsDeleted,
            DateAdd = yearFisc.DateAdd,
            DateMod = yearFisc.DateMod,
            RowVersion = Convert.ToBase64String(yearFisc.RowVersion)
        };
        return c;
    }
}
