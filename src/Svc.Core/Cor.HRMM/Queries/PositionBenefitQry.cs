using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class PosBenefitAllQry : IRequest<List<PosBenefitListDto>> { public Guid Id { get; set; } }
public class PosBenefitByIdQry : IRequest<PosBenefitListDto?> { public Guid Id { get; set; } }


public class PosBenefitAllHandler : IRequestHandler<PosBenefitAllQry, List<PosBenefitListDto>>
{
    private readonly IDapperHelper _dapper;
    public PosBenefitAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PosBenefitListDto>> Handle(PosBenefitAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string bs = "bs";
        var qb = new QueryBuilder()
            .Select<PositionBenefit>(v, x => x.Id, x => x.BenefitSettingId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<BenefitSetting>(bs, x => x.BenefitValue, x => x.Per)
            .From<BenefitSetting>(v)
            .Join<PositionBenefit, BenefitSetting>(v, bs, x => x.BenefitSettingId, x => x.Id)
            .OrderBy<BenefitSetting>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<PosBenefitListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<PosBenefitListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new PosBenefitListDto
            {
                Id = data.Id,
                BenefitSettingId = data.BenefitSettingId,
                PositionId = data.PositionId,
                Benefit = $"{data.BenefitValue:#,##0.##}",
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

public class PosBenefitByIdHandler : IRequestHandler<PosBenefitByIdQry, PosBenefitListDto?>
{
    private readonly IDapperHelper _dapper;
    public PosBenefitByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PosBenefitListDto?> Handle(PosBenefitByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string bs = "bs";
        var qb = new QueryBuilder()
            .Select<PositionBenefit>(v, x => x.Id, x => x.BenefitSettingId, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .Select<BenefitSetting>(bs, x => x.BenefitValue, x => x.Per)
            .From<BenefitSetting>(v)
            .Join<PositionBenefit, BenefitSetting>(v, bs, x => x.BenefitSettingId, x => x.Id)
            .Where<BenefitSetting>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PosBenefitListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new PosBenefitListDto
        {
            Id = data.Id,
            BenefitSettingId = data.BenefitSettingId,
            PositionId = data.PositionId,
            Benefit = $"{data.BenefitValue:#,##0.##}",
            PerStr = MyEnumHelper.FormatEnum<EmpType>(data.Per),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}