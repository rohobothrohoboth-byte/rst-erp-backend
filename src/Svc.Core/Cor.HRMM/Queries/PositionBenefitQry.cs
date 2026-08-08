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
        // ✅ Use explicit SQL with quoted column names
        var sql = @"
            SELECT
                v.""Id"",
                v.""BenefitSettingId"",
                v.""PositionId"",
                v.""DateAdd"",
                v.""DateMod"",
                v.""xmin"",
                v.""IsDeleted"",
                bs.""BenefitValue"",
                bs.""Per""
            FROM ""PositionBenefit"" v
            INNER JOIN ""BenefitSetting"" bs ON v.""BenefitSettingId"" = bs.""Id""
            WHERE v.""PositionId"" = @Id
            ORDER BY v.""DateAdd"" DESC
        ";

        var parameters = new { Id = request.Id };

        var data = await _dapper.QueryAsync<PosBenefitListDto>(sql, parameters, ct);

        // Map the results
        return data.Select(item => new PosBenefitListDto
        {
            Id = item.Id,
            BenefitSettingId = item.BenefitSettingId,
            PositionId = item.PositionId,
            Benefit = $"{item.BenefitValue:#,##0.##}",
            PerStr = MyEnumHelper.FormatEnum<EmpType>(item.Per),
            IsDeleted = item.IsDeleted,
            DateAdd = item.DateAdd,
            DateMod = item.DateMod,
            RowVersion = item.xmin.ToString()
        }).ToList();
    }
}

public class PosBenefitByIdHandler : IRequestHandler<PosBenefitByIdQry, PosBenefitListDto?>
{
    private readonly IDapperHelper _dapper;
    public PosBenefitByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PosBenefitListDto?> Handle(PosBenefitByIdQry request, CancellationToken ct)
    {
        // ✅ Use explicit SQL with quoted column names
        var sql = @"
            SELECT
                v.""Id"",
                v.""BenefitSettingId"",
                v.""PositionId"",
                v.""DateAdd"",
                v.""DateMod"",
                v.""xmin"",
                v.""IsDeleted"",
                bs.""BenefitValue"",
                bs.""Per""
            FROM ""PositionBenefit"" v
            INNER JOIN ""BenefitSetting"" bs ON v.""BenefitSettingId"" = bs.""Id""
            WHERE v.""Id"" = @Id
            LIMIT 1
        ";

        var parameters = new { Id = request.Id };

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