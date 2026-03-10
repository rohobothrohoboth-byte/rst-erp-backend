using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class BenefitSetAllQry : IRequest<List<BenefitSetListDto>> { }
public class BenefitSetByIdQry : IRequest<BenefitSetListDto?> { public Guid Id { get; set; } }


public class BenefitSetAllHandler : IRequestHandler<BenefitSetAllQry, List<BenefitSetListDto>>
{
    private readonly IDapperHelper _dapper;
    public BenefitSetAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<BenefitSetListDto>> Handle(BenefitSetAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<BenefitSetting>(v, x => x.Id, x => x.Name, x => x.BenefitValue, x => x.Per, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<BenefitSetting>(v)
            .OrderBy<BenefitSetting>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<BenefitSetListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<BenefitSetting>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new BenefitSetListDto
            {
                Id = data.Id,
                Name = data.Name,
                Benefit = data.BenefitValue,
                Per = data.Per,
                PerStr = MyEnumHelper.FormatEnum<EmpType>(data.Per),
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class BenefitSetByIdHandler : IRequestHandler<BenefitSetByIdQry, BenefitSetListDto?>
{
    private readonly IDapperHelper _dapper;
    public BenefitSetByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<BenefitSetListDto?> Handle(BenefitSetByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<BenefitSetting>(v, x => x.Id, x => x.Name, x => x.BenefitValue, x => x.Per, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<BenefitSetting>(v)
            .Where<BenefitSetting>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<BenefitSetting>(sql, parameters, ct);
        if (data == null) return null;

        return new BenefitSetListDto
        {
            Id = data.Id,
            Name = data.Name,
            Benefit = data.BenefitValue,
            Per = data.Per,
            PerStr = MyEnumHelper.FormatEnum<EmpType>(data.Per),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}