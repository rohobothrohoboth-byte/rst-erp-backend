using MediatR;
using Module.App.Interfaces;
using Module.App.Services;
using Module.Domain.DTOs;
using Module.Domain.Entities;
using Module.Domain.Enums;

namespace Module.App.Queries;

public class AllPeriodQry : IRequest<List<PeriodListDto>> { }

public class PeriodByIdQry : IRequest<PeriodListDto?> { public Guid Id { get; set; } }

public class AllPeriodQryHandler : IRequestHandler<AllPeriodQry, List<PeriodListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILupClient _lupClient;

    public AllPeriodQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient) { _unitOfWork = unitOfWork; _lupClient = lupClient; }

    public async Task<List<PeriodListDto>> Handle(AllPeriodQry request, CancellationToken cancellationToken)
    {
        var dataList = await _unitOfWork.Repository<Period>().GetAll();
        var dataL = new List<PeriodListDto>();

        var quarterL = await _lupClient.QuarterList(cancellationToken);

        foreach (var data in dataList)
        {
            var fYear = await _unitOfWork.Repository<FiscalYear>().GetById(data.FiscalYearId);
            if (fYear == null) continue;
            var quarter = quarterL!.FirstOrDefault(q => q.Id == data.QuarterId);
            var c = new PeriodListDto
            {
                Id = data.Id,
                FiscalYearId = data.FiscalYearId,
                QuarterId = data.QuarterId,
                Name = data.Name,
                FiscYear = fYear.Name,
                Quarter = quarter!.Name,
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
    private readonly ILupClient _lupClient;

    public PeriodByIdQryHandler(IUnitOfWork unitOfWork, ILupClient lupClient) { _unitOfWork = unitOfWork; _lupClient = lupClient; }

    public async Task<PeriodListDto?> Handle(PeriodByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Period>().GetById(request.Id);
        if (data == null) { return null; }

        var fYear = await _unitOfWork.Repository<FiscalYear>().GetById(data.FiscalYearId);
        if (fYear == null) { return null; }
        var quarter = await _lupClient.Quarter(data.QuarterId, cancellationToken);

        var c = new PeriodListDto
        {
            Id = data.Id,
            FiscalYearId = data.FiscalYearId,
            QuarterId = data.QuarterId,
            Name = data.Name,
            FiscYear = fYear.Name,
            Quarter = quarter!.Name,
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
