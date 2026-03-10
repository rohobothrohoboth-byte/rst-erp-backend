using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using MediatR;

namespace Cor.HRMM.Queries;

public class EducationQualAllQry : IRequest<List<EducationQualListDto>> { }
public class EducationQualByIdQry : IRequest<EducationQualListDto?> { public Guid Id { get; set; } }



public class EducationQualAllHandler : IRequestHandler<EducationQualAllQry, List<EducationQualListDto>>
{
    private readonly IDapperHelper _dapper;
    public EducationQualAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EducationQualListDto>> Handle(EducationQualAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EducationQual>(v, x => x.Id, x => x.Name, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EducationQual>(v)
            .OrderBy<EducationQual>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<EducationQualListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<EducationQual>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new EducationQualListDto
            {
                Id = data.Id,
                Name = data.Name,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class EducationQualByIdHandler : IRequestHandler<EducationQualByIdQry, EducationQualListDto?>
{
    private readonly IDapperHelper _dapper;
    public EducationQualByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EducationQualListDto?> Handle(EducationQualByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<EducationQual>(v, x => x.Id, x => x.Name, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<EducationQual>(v)
            .Where<EducationQual>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EducationQual>(sql, parameters, ct);
        if (data == null) return null;

        var c = new EducationQualListDto
        {
            Id = data.Id,
            Name = data.Name,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
        return c;
    }
}