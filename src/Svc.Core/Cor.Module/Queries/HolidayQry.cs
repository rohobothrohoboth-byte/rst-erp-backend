using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using MediatR;

namespace Cor.Module.Queries;

public class AllHolidayQry : IRequest<List<HolidayListDto>> { }
public class HolidayByIdQry : IRequest<HolidayListDto?> { public Guid Id { get; set; } }

public class AllHolidayQryHandler : IRequestHandler<AllHolidayQry, List<HolidayListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AllHolidayQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<HolidayListDto>> Handle(AllHolidayQry request, CancellationToken cancellationToken)
    {
        var dataList = await _unitOfWork.Repository<Holiday>().GetAll();
        var dataL = new List<HolidayListDto>();

        foreach (var data in dataList)
        {
            var fYear = await _unitOfWork.Repository<FiscalYear>().GetById(data.FiscalYearId);
            if (fYear == null) continue;
            var c = new HolidayListDto
            {
                Id = data.Id,
                Name = data.Name,
                Date = data.Date,
                IsPublic = data.IsPublic,
                FiscalYearId = data.FiscalYearId,
                IsPublicStr = data.IsPublic.ToString(),
                FiscYear = fYear.Name,
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

public class HolidayByIdQryHandler : IRequestHandler<HolidayByIdQry, HolidayListDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public HolidayByIdQryHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<HolidayListDto?> Handle(HolidayByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Holiday>().GetById(request.Id);
        if (data == null) { return null; }

        var fYear = await _unitOfWork.Repository<FiscalYear>().GetById(data.FiscalYearId);
        if (fYear == null) { return null; }

        var c = new HolidayListDto
        {
            Id = data.Id,
            Name = data.Name,
            Date = data.Date,
            IsPublic = data.IsPublic,
            FiscalYearId = data.FiscalYearId,
            IsPublicStr = data.IsPublic.ToString(),
            FiscYear = fYear.Name,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}