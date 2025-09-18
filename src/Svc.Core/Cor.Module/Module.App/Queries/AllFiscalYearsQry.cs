using Module.App.Interfaces;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using MediatR;
using Module.Domain.Enums;

namespace Module.App.Queries;

public class AllFiscalYearsQry : IRequest<List<FiscYearListDto>> { }

public class AllFiscalYearsQryHandler : IRequestHandler<AllFiscalYearsQry, List<FiscYearListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllFiscalYearsQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<FiscYearListDto>> Handle(AllFiscalYearsQry request, CancellationToken cancellationToken)
    {
        var yearFiscs = await _unitOfWork.Repository<FiscalYear>().GetAll();
        var yearFiscL = new List<FiscYearListDto>();
        foreach (var yearFisc in yearFiscs)
        {
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
            yearFiscL.Add(c);
        }

        return yearFiscL;
    }
}
