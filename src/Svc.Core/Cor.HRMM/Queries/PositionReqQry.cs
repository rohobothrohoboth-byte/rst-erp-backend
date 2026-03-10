using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionReqAllQry : IRequest<List<PositionReqListDto>> { public Guid Id { get; set; } }
public class PositionReqByIdQry : IRequest<PositionReqListDto?> { public Guid Id { get; set; } }
public class PosReqByPosIdQry : IRequest<PositionReqListDto?> { public Guid Id { get; set; } }



public class PositionReqAllHandler : IRequestHandler<PositionReqAllQry, List<PositionReqListDto>>
{
    private readonly IDapperHelper _dapper;
    public PositionReqAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PositionReqListDto>> Handle(PositionReqAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PositionReq>(v, x => x.Id, x => x.Gender, x => x.ProfessionType, x => x.SaturdayWorkOption, x => x.SundayWorkOption, x => x.WorkingHours, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<PositionReq>(v)
            .OrderBy<PositionReq>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<PositionReqListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<PositionReqListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new PositionReqListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                ProfessionType = data.ProfessionType,
                Gender = data.Gender,
                SaturdayWorkOption = data.SaturdayWorkOption,
                SundayWorkOption = data.SundayWorkOption,
                GenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.Gender),
                SaturdayWorkOptionStr = MyEnumHelper.FormatEnum<WorkOption>(data.SaturdayWorkOption),
                SundayWorkOptionStr = MyEnumHelper.FormatEnum<WorkOption>(data.SundayWorkOption),
                ProfessionTypeStr = MyEnumHelper.FormatEnum<ProfessionType>(data.ProfessionType),
                WorkingHours = data.WorkingHours,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class PositionReqByIdHandler : IRequestHandler<PositionReqByIdQry, PositionReqListDto?>
{
    private readonly IDapperHelper _dapper;
    public PositionReqByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PositionReqListDto?> Handle(PositionReqByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PositionReq>(v, x => x.Id, x => x.Gender, x => x.ProfessionType, x => x.SaturdayWorkOption, x => x.SundayWorkOption, x => x.WorkingHours, x => x.PositionId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<PositionReq>(v)
            .Where<PositionReq>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PositionReqListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new PositionReqListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            ProfessionType = data.ProfessionType,
            Gender = data.Gender,
            SaturdayWorkOption = data.SaturdayWorkOption,
            SundayWorkOption = data.SundayWorkOption,
            GenderStr = MyEnumHelper.FormatEnum<PositionGender>(data.Gender),
            SaturdayWorkOptionStr = MyEnumHelper.FormatEnum<WorkOption>(data.SaturdayWorkOption),
            SundayWorkOptionStr = MyEnumHelper.FormatEnum<WorkOption>(data.SundayWorkOption),
            ProfessionTypeStr = MyEnumHelper.FormatEnum<ProfessionType>(data.ProfessionType),
            WorkingHours = data.WorkingHours,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}

public class PosReqByPosIdHandler : IRequestHandler<PosReqByPosIdQry, PositionReqListDto?>
{
    private readonly IDapperHelper _dapper;
    public PosReqByPosIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PositionReqListDto?> Handle(PosReqByPosIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PositionReq>(v, x => x.Id, x => x.Gender, x => x.ProfessionType, x => x.SaturdayWorkOption, x => x.SundayWorkOption, x => x.WorkingHours, x => x.PositionId)
            .From<PositionReq>(v)
            .Where<PositionReq>(v, x => x.PositionId == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PositionReqListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new PositionReqListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            Gender = data.Gender,
            ProfessionType = data.ProfessionType,
            SaturdayWorkOption = data.SaturdayWorkOption,
            SundayWorkOption = data.SundayWorkOption,
            WorkingHours = data.WorkingHours
        };
    }
}