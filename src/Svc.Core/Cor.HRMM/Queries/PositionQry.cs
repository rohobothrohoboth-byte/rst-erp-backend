using Common;
using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionAllQry : IRequest<List<PositionListDto>> { }
public class PositionByIdQry : IRequest<PositionListDto?> { public Guid Id { get; set; } }



public class PositionAllHandler : IRequestHandler<PositionAllQry, List<PositionListDto>>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;

    public PositionAllHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<List<PositionListDto>> Handle(PositionAllQry request, CancellationToken ct)
    {
        var deptTask = await _corMod.GetListDept(ct);
        var deptDict = deptTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        var qb = new QueryBuilder()
            .Select<Position>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.NoOfPosition, x => x.IsVacant, x => x.DepartmentId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<Position>(v)
            .OrderBy<Position>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<PositionListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<PositionListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            deptDict.TryGetValue(data.DepartmentId, out var dept);

            dataL.Add(new PositionListDto
            {
                Id = data.Id,
                DepartmentId = data.DepartmentId,
                IsVacant = data.IsVacant,
                Name = data.Name,
                NameAm = data.NameAm,
                NoOfPosition = data.NoOfPosition,
                IsVacantStr = MyEnumHelper.FormatEnum<YesNo>(data.IsVacant),
                Department = dept?.Name ?? "",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }

        return dataL;
    }
}

public class PositionByIdHandler : IRequestHandler<PositionByIdQry, PositionListDto?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorModClient _corMod;

    public PositionByIdHandler(IDapperHelper dapper, ICorModClient corMod)
    {
        _dapper = dapper;
        _corMod = corMod;
    }

    public async Task<PositionListDto?> Handle(PositionByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<Position>(v, x => x.Id, x => x.Name, x => x.NameAm, x => x.NoOfPosition, x => x.IsVacant, x => x.DepartmentId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<Position>(v)
            .Where<Position>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PositionListDto>(sql, parameters, ct);
        if (data == null) return null;

        var dept = await _corMod.GetDept(data.DepartmentId.ToString(), ct);

        return new PositionListDto
        {
            Id = data.Id,
            DepartmentId = data.DepartmentId,
            IsVacant = data.IsVacant,
            Name = data.Name,
            NameAm = data.NameAm,
            NoOfPosition = data.NoOfPosition,
            IsVacantStr = MyEnumHelper.FormatEnum<YesNo>(data.IsVacant),
            Department = dept.Res?.Name ?? "",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}