using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllPeriodQry : IRequest<List<PeriodListDto>> { }
public class PeriodByIdQry : IRequest<PeriodListDto?> { public Guid Id { get; set; } }

public class AllPeriodQryHandler : IRequestHandler<AllPeriodQry, List<PeriodListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllPeriodQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork;}

    public async Task<List<PeriodListDto>> Handle(AllPeriodQry request, CancellationToken cancellationToken)
    {
        var dataList = await _unitOfWork.Repository<Period>().GetAll();
        var dataL = new List<PeriodListDto>();

        foreach (var data in dataList)
        {
            var fYear = await _unitOfWork.Repository<FiscalYear>().GetById(data.FiscalYearId);
            if (fYear == null) continue;
            var c = new PeriodListDto
            {
                Id = data.Id,
                FiscalYearId = data.FiscalYearId,
                Quarter = data.Quarter,
                Name = data.Name,
                FiscYear = fYear.Name,
                QuarterStr = ((Quarter)Enum.Parse(typeof(Quarter), data.Quarter)).ToDisplayName(),
                DateStart = data.DateStart,
                DateEnd = data.DateEnd,
                IsActive = data.IsActive,
                IsActiveStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsActive)).ToDisplayName(),
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class PeriodByIdQryHandler : IRequestHandler<PeriodByIdQry, PeriodListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public PeriodByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork;}

    public async Task<PeriodListDto?> Handle(PeriodByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Period>().GetById(request.Id);
        if (data == null) { return null; }

        var fYear = await _unitOfWork.Repository<FiscalYear>().GetById(data.FiscalYearId);
        if (fYear == null) { return null; }

        var c = new PeriodListDto
        {
            Id = data.Id,
            FiscalYearId = data.FiscalYearId,
            Quarter = data.Quarter,
            Name = data.Name,
            FiscYear = fYear.Name,
            QuarterStr = ((Quarter)Enum.Parse(typeof(Quarter), data.Quarter)).ToDisplayName(),
            DateStart = data.DateStart,
            DateEnd = data.DateEnd,
            IsActive = data.IsActive,
            IsActiveStr = ((YesNo)Enum.Parse(typeof(YesNo), data.IsActive)).ToDisplayName(),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}
