using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllHolidayQry : IRequest<List<HolidayListDto>> { }
public class HolidayByIdQry : IRequest<HolidayListDto?> { public Guid Id { get; set; } }



public class AllHolidayHandler : IRequestHandler<AllHolidayQry, List<HolidayListDto>>
{
    private readonly IDapperHelper _dapper;
    public AllHolidayHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<HolidayListDto>> Handle(AllHolidayQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Holiday>(v, x => x.Id, x => x.Name, x => x.Date, x => x.IsPublic, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<FiscalYear, HolidayListDto>(c, x => x.Name, d => d.FiscYear)
            .From<Holiday>(v)
            .Join<Holiday, FiscalYear>(v, c, x => x.FiscalYearId, x => x.Id)
            .OrderBy<Holiday>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<HolidayListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<HolidayListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new HolidayListDto
            {
                Id = data.Id,
                Name = data.Name,
                Date = data.Date,
                IsPublic = data.IsPublic,
                FiscalYearId = data.FiscalYearId,
                IsPublicStr = BoolToStr.FormatBool(data.IsPublic),
                FiscYear = data.FiscYear,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class HolidayByIdHandler : IRequestHandler<HolidayByIdQry, HolidayListDto?>
{
    private readonly IDapperHelper _dapper;
    public HolidayByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<HolidayListDto?> Handle(HolidayByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<Holiday>(v, x => x.Id, x => x.Name, x => x.Date, x => x.IsPublic, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<FiscalYear, HolidayListDto>(c, x => x.Name, d => d.FiscYear)
            .From<Holiday>(v)
            .Join<Holiday, FiscalYear>(v, c, x => x.FiscalYearId, x => x.Id)
            .Where<Holiday>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<HolidayListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new HolidayListDto
        {
            Id = data.Id,
            Name = data.Name,
            Date = data.Date,
            IsPublic = data.IsPublic,
            FiscalYearId = data.FiscalYearId,
            IsPublicStr = BoolToStr.FormatBool(data.IsPublic),
            FiscYear = data.FiscYear,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}