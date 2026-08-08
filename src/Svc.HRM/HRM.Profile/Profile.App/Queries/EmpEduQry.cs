using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpEduAllQry : IRequest<List<EmpEduListDto>> { public Guid Id { get; set; } }
public class EmpEduByIdQry : IRequest<EmpEduListDto?> { public Guid Id { get; set; } }

public class EmpEduAllHandler(IDapperHelper _dapper) : IRequestHandler<EmpEduAllQry, List<EmpEduListDto>>
{
    public async Task<List<EmpEduListDto>> Handle(EmpEduAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            // ✅ ADD EmployeeId to the select
            .Select<EmpEducation>(v,
                x => x.Id,
                x => x.EmployeeId,
                x => x.EduLevel,
                x => x.Institution,
                x => x.FieldOfStudy,
                x => x.StartDate,
                x => x.EndDate!,
                x => x.GPA!,
                x => x.Status,
                x => x.DateAdd!,
                x => x.DateMod!
            )
            .From<EmpEducation>(v)
            .Where<EmpEducation>(v, x => x.EmployeeId == request.Id)
            .OrderBy<EmpEducation>(v, x => x.DateAdd, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpEduListDto>(ct);

        foreach (var data in list)
        {
            data.EduLevel = MyEnumHelper.FormatEnum<EducationLevel>(data.EduLevel);
            data.Status = MyEnumHelper.FormatEnum<ApprovalStatus>(data.Status);
        }

        return list;
    }
}

public class EmpEduByIdHandler(IDapperHelper _dapper) : IRequestHandler<EmpEduByIdQry, EmpEduListDto?>
{
    public async Task<EmpEduListDto?> Handle(EmpEduByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            // ✅ ADD EmployeeId to the select
            .Select<EmpEducation>(v,
                x => x.Id,
                x => x.EmployeeId,        // ✅ ADD THIS
                x => x.EduLevel,
                x => x.Institution,
                x => x.FieldOfStudy,
                x => x.StartDate,
                x => x.EndDate,
                x => x.GPA!,
                x => x.Status,
                x => x.DateAdd,
                x => x.DateMod!
            )
            .From<EmpEducation>(v)
            .Where<EmpEducation>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpEduListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.EduLevel = MyEnumHelper.FormatEnum<EducationLevel>(data.EduLevel);
        data.Status = MyEnumHelper.FormatEnum<ApprovalStatus>(data.Status);
        return data;
    }
}