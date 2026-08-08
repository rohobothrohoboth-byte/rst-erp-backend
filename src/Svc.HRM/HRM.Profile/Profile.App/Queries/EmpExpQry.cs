using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpExpAllQry : IRequest<List<EmpExpListDto>> { public Guid Id { get; set; } }
public class EmpExpByIdQry : IRequest<EmpExpListDto?> { public Guid Id { get; set; } }

public class EmpExpAll(IDapperHelper _dapper) : IRequestHandler<EmpExpAllQry, List<EmpExpListDto>>
{
    public async Task<List<EmpExpListDto>> Handle(EmpExpAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            // ✅ ADD EmployeeId to the select
            .Select<EmpExperience>(v,
                x => x.Id,
                x => x.EmployeeId,        // ✅ ADD THIS
                x => x.Company,
                x => x.PosTitle,
                x => x.Location,
                x => x.StartDate,
                x => x.EndDate,
                x => x.Respo,
                x => x.Status,
                x => x.DateAdd,
                x => x.DateMod!
            )
            .From<EmpExperience>(v)
            .Where<EmpExperience>(v, x => x.EmployeeId == request.Id)
            .OrderBy<EmpExperience>(v, x => x.DateAdd, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpExpListDto>(ct);

        foreach (var data in list)
        {
            data.Status = MyEnumHelper.FormatEnum<ApprovalStatus>(data.Status);
        }

        return list;
    }
}

public class EmpExpById(IDapperHelper _dapper) : IRequestHandler<EmpExpByIdQry, EmpExpListDto?>
{
    public async Task<EmpExpListDto?> Handle(EmpExpByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            // ✅ ADD EmployeeId to the select
            .Select<EmpExperience>(v,
                x => x.Id,
                x => x.EmployeeId,        // ✅ ADD THIS
                x => x.Company,
                x => x.PosTitle,
                x => x.Location,
                x => x.StartDate,
                x => x.EndDate,
                x => x.Respo,
                x => x.Status,
                x => x.DateAdd,
                x => x.DateMod!
            )
            .From<EmpExperience>(v)
            .Where<EmpExperience>(v, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpExpListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.Status = MyEnumHelper.FormatEnum<ApprovalStatus>(data.Status);
        return data;
    }
}