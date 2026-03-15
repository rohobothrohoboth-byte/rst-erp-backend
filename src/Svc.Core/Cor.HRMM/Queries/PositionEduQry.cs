using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using Helpers;
using MediatR;

namespace Cor.HRMM.Queries;

public class PositionEduAllQry : IRequest<List<PositionEduListDto>> { public Guid Id { get; set; } }
public class PositionEduByIdQry : IRequest<PositionEduListDto?> { public Guid Id { get; set; } }



public class PositionEduAllHandler : IRequestHandler<PositionEduAllQry, List<PositionEduListDto>>
{
    private readonly IDapperHelper _dapper;
    public PositionEduAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PositionEduListDto>> Handle(PositionEduAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string q = "q";
        var qb = new QueryBuilder()
            .Select<PositionEducation>(v, x => x.Id, x => x.EducationLevel, x => x.PositionId, x => x.EducationQualId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EducationQual, PositionEduListDto>(q, x => x.Name, x => x.EducationQual)
            .From<PositionEducation>(v)
            .Join<PositionEducation, EducationQual>(v, q, x => x.EducationQualId, x => x.Id)
            .Where<PositionEducation>(v, x => x.PositionId == request.Id)
            .OrderBy<PositionEducation>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<PositionEduListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<PositionEduListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new PositionEduListDto
            {
                Id = data.Id,
                PositionId = data.PositionId,
                EducationQualId = data.EducationQualId,
                EducationLevel = data.EducationLevel,
                EducationQual = data.EducationQual,
                EducationLevelStr = MyEnumHelper.FormatEnum<EducationLevel>(data.EducationLevel),
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class PositionEduByIdHandler : IRequestHandler<PositionEduByIdQry, PositionEduListDto?>
{
    private readonly IDapperHelper _dapper;
    public PositionEduByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PositionEduListDto?> Handle(PositionEduByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string q = "q";
        var qb = new QueryBuilder()
            .Select<PositionEducation>(v, x => x.Id, x => x.EducationLevel, x => x.PositionId, x => x.EducationQualId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<EducationQual, PositionEduListDto>(q, x => x.Name, x => x.EducationQual)
            .From<PositionEducation>(v)
            .Join<PositionEducation, EducationQual>(v, q, x => x.EducationQualId, x => x.Id)
            .Where<PositionEducation>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PositionEduListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new PositionEduListDto
        {
            Id = data.Id,
            PositionId = data.PositionId,
            EducationQualId = data.EducationQualId,
            EducationLevel = data.EducationLevel,
            EducationQual = data.EducationQual,
            EducationLevelStr = MyEnumHelper.FormatEnum<EducationLevel>(data.EducationLevel),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}