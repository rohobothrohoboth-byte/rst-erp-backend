using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllPeriodQry : IRequest<List<PeriodListDto>> { }
public class PeriodByIdQry : IRequest<PeriodListDto?> { public Guid Id { get; set; } }



public class AllPeriodHandler : IRequestHandler<AllPeriodQry, List<PeriodListDto>>
{
    private readonly IDapperHelper _dapper;
    public AllPeriodHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PeriodListDto>> Handle(AllPeriodQry request, CancellationToken ct)
    {
        const string v = "v";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<Period>(v, x => x.Id, x => x.Name, x => x.DateStart, x => x.DateEnd, x => x.IsActive, x => x.Quarter, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<FiscalYear, PeriodListDto>(b, x => x.Name, d => d.FiscYear)
            .Join<Period, FiscalYear>(v, b, x => x.FiscalYearId, x => x.Id)
            .From<Period>(v)
            .OrderBy<Period>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<PeriodListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<PeriodListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new PeriodListDto
            {
                Id = data.Id,
                FiscalYearId = data.FiscalYearId,
                Quarter = data.Quarter,
                Name = data.Name,
                FiscYear = data.FiscYear,
                QuarterStr = MyEnumHelper.FormatEnum<Quarter>(data.Quarter),
                DateStart = data.DateStart,
                DateEnd = data.DateEnd,
                IsActive = data.IsActive,
                IsActiveStr = MyEnumHelper.FormatEnum<YesNo>(data.IsActive),
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class PeriodByIdHandler : IRequestHandler<PeriodByIdQry, PeriodListDto?>
{
    private readonly IDapperHelper _dapper;
    public PeriodByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PeriodListDto?> Handle(PeriodByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string b = "b";
        var qb = new QueryBuilder()
            .Select<Period>(v, x => x.Id, x => x.Name, x => x.DateStart, x => x.DateEnd, x => x.IsActive, x => x.Quarter, x => x.FiscalYearId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<FiscalYear, PeriodListDto>(b, x => x.Name, d => d.FiscYear)
            .From<Period>(v)
            .Where<Period>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PeriodListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new PeriodListDto
        {
            Id = data.Id,
            FiscalYearId = data.FiscalYearId,
            Quarter = data.Quarter,
            Name = data.Name,
            FiscYear = data.FiscYear,
            QuarterStr = MyEnumHelper.FormatEnum<Quarter>(data.Quarter),
            DateStart = data.DateStart,
            DateEnd = data.DateEnd,
            IsActive = data.IsActive,
            IsActiveStr = MyEnumHelper.FormatEnum<YesNo>(data.IsActive),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}