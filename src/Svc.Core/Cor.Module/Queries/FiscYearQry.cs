using Cor.Module.Interfaces;
using Cor.Module.Models.DTOs;
using Cor.Module.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.Module.Queries;

public class AllFiscalYearsQry : IRequest<List<FiscYearListDto>> { }
public class FiscalYearByIdQry : IRequest<FiscYearListDto?> { public Guid Id { get; set; } }
public class ActiveFiscalYearQry : IRequest<FiscYearListDto> { }



public class AllFiscalYearsHandler : IRequestHandler<AllFiscalYearsQry, List<FiscYearListDto>>
{
    private readonly IDapperHelper _dapper;
    public AllFiscalYearsHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<FiscYearListDto>> Handle(AllFiscalYearsQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<FiscalYear>(v, x => x.Id, x => x.Name, x => x.DateStart, x => x.DateEnd, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<FiscalYear>(v)
            .OrderBy<FiscalYear>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<FiscYearListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<FiscYearListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new FiscYearListDto
            {
                Id = data.Id,
                Name = data.Name,
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

public class FiscalYearByIdHandler : IRequestHandler<FiscalYearByIdQry, FiscYearListDto?>
{
    private readonly IDapperHelper _dapper;
    public FiscalYearByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<FiscYearListDto?> Handle(FiscalYearByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<FiscalYear>(v, x => x.Id, x => x.Name, x => x.DateStart, x => x.DateEnd, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<FiscalYear>(v)
            .Where<FiscalYear>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<FiscYearListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new FiscYearListDto
        {
            Id = data.Id,
            Name = data.Name,
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

public class ActiveFiscalYearHandler : IRequestHandler<ActiveFiscalYearQry, FiscYearListDto?>
{
    private readonly IDapperHelper _dapper;
    public ActiveFiscalYearHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<FiscYearListDto?> Handle(ActiveFiscalYearQry request, CancellationToken ct)
    {
        var stat = BoolToStr.EnumToString(YesNo.Yes);
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<FiscalYear>(v, x => x.Id, x => x.Name, x => x.DateStart, x => x.DateEnd, x => x.IsActive, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<FiscalYear>(v)
            .Where<FiscalYear>(v, x => x.IsActive == stat)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<FiscYearListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new FiscYearListDto
        {
            Id = data.Id,
            Name = data.Name,
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