using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using MediatR;

namespace Cor.Module.Queries;

public class AllHolidayQry : IRequest<List<HolidayListDto>> { }
public class HolidayByIdQry : IRequest<HolidayListDto?> { public Guid Id { get; set; } }
public class GetHolidaysInRange : IRequest<List<HolidaySerListDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool PublicOnly { get; set; } = true;
}
public class HolidaysByFiscalYear : IRequest<List<HolidaySerListDto>> { public Guid Id { get; set; } }
public class IsHoliday : IRequest<bool> { public DateTime Date { get; set; } public bool PublicOnly { get; set; } = true; }
public class GetHolidayByDate : IRequest<HolidaySerListDto?> { public DateTime Date { get; set; } }

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

public class GetHolidaysInRangeHandler : IRequestHandler<GetHolidaysInRange, List<HolidaySerListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetHolidaysInRangeHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<HolidaySerListDto>> Handle(GetHolidaysInRange request, CancellationToken cancellationToken)
    {
        var hDayL = new List<HolidaySerListDto>();
        var data = (await _unitOfWork.Repository<Holiday>().Find(h => h.Date >= request.StartDate && h.Date <= request.EndDate)).ToList();
        if (request.PublicOnly)
        {
            data = [.. data.Where(h => h.IsPublic)];
        }
        if (data.Count <= 0) { return hDayL; }

        foreach (var hd in data)
        {
            var c = new HolidaySerListDto
            {
                Id = hd.Id,
                Name = hd.Name,
                Date = hd.Date,
                IsPublic = hd.IsPublic,
                FiscalYearId = hd.FiscalYearId,
                DateAdd = hd.DateAdd,
                DateMod = hd.DateMod
            };
            hDayL.Add(c);
        }

        return [.. hDayL.OrderBy(h => h.Date)];
    }
}

public class HolidaysByFiscalYearHandler : IRequestHandler<HolidaysByFiscalYear, List<HolidaySerListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public HolidaysByFiscalYearHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<HolidaySerListDto>> Handle(HolidaysByFiscalYear request, CancellationToken cancellationToken)
    {
        var hDayL = new List<HolidaySerListDto>();
        var data = (await _unitOfWork.Repository<Holiday>().Find(h => h.FiscalYearId >= request.Id)).ToList();
        if (data.Count <= 0) { return hDayL; }

        foreach (var hd in data)
        {
            var c = new HolidaySerListDto
            {
                Id = hd.Id,
                Name = hd.Name,
                Date = hd.Date,
                IsPublic = hd.IsPublic,
                FiscalYearId = hd.FiscalYearId,
                DateAdd = hd.DateAdd,
                DateMod = hd.DateMod
            };
            hDayL.Add(c);
        }

        return [.. hDayL.OrderBy(h => h.Date)];
    }
}

public class IsHolidayHandler : IRequestHandler<IsHoliday, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public IsHolidayHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<bool> Handle(IsHoliday request, CancellationToken cancellationToken)
    {
        var data = (await _unitOfWork.Repository<Holiday>().Find(h => h.Date >= request.Date)).ToList();
        if (request.PublicOnly) { data = [.. data.Where(h => h.IsPublic)]; }
        if (data.Count <= 0) { return false; }

        return data.Count != 0;
    }
}

public class GetHolidayByDateHandler : IRequestHandler<GetHolidayByDate, HolidaySerListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetHolidayByDateHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<HolidaySerListDto?> Handle(GetHolidayByDate request, CancellationToken cancellationToken)
    {
        var hd = await _unitOfWork.Repository<Holiday>().GetFoD(h => h.Date == request.Date);
        if (hd == null) { return null; }

        var c = new HolidaySerListDto
        {
            Id = hd.Id,
            Name = hd.Name,
            Date = hd.Date,
            IsPublic = hd.IsPublic,
            FiscalYearId = hd.FiscalYearId,
            DateAdd = hd.DateAdd,
            DateMod = hd.DateMod
        };

        return c;
    }
}