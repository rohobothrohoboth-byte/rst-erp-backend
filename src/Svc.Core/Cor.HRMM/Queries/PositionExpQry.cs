using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionExpAllQry : IRequest<List<PositionExpListDto>> { public Guid Id { get; set; } }
public class PositionExpByIdQry : IRequest<PositionExpListDto?> { public Guid Id { get; set; } }



public class PositionExpAllHandler : IRequestHandler<PositionExpAllQry, List<PositionExpListDto>>
{
    private readonly IDapperHelper _dapper;
    public PositionExpAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PositionExpListDto>> Handle(PositionExpAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PositionExp>(v, x => x.Id, x => x.SamePosExp, x => x.OtherPosExp, x => x.MinAge, x => x.MaxAge, x => x.PositionId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<PositionExp>(v)
            .Where<PositionExp>(v, x => x.PositionId == request.Id)
            .OrderBy<PositionExp>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<PositionExpListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<PositionExpListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new PositionExpListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                SamePosExp = data.SamePosExp,
                OtherPosExp = data.OtherPosExp,
                MinAge = data.MinAge,
                MaxAge = data.MaxAge,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class PositionExpByIdHandler : IRequestHandler<PositionExpByIdQry, PositionExpListDto?>
{
    private readonly IDapperHelper _dapper;
    public PositionExpByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PositionExpListDto?> Handle(PositionExpByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PositionExp>(v, x => x.Id, x => x.SamePosExp, x => x.OtherPosExp, x => x.MinAge, x => x.MaxAge, x => x.PositionId, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<PositionExp>(v)
            .OrderBy<PositionExp>(v, x => x.DateAdd, desc: true)
            .Where<PositionExp>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PositionExpListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new PositionExpListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            SamePosExp = data.SamePosExp,
            OtherPosExp = data.OtherPosExp,
            MinAge = data.MinAge,
            MaxAge = data.MaxAge,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}

