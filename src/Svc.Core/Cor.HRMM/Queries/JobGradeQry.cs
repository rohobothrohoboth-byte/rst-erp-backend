using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using MediatR;

namespace Cor.HRMM.Queries;

public class JobGradeAllQry : IRequest<List<JobGradeListDto>> { }
public class JobGradeByIdQry : IRequest<JobGradeListDto?> { public Guid Id { get; set; } }



public class JobGradeAllHandler : IRequestHandler<JobGradeAllQry, List<JobGradeListDto>>
{
    private readonly IDapperHelper _dapper;
    public JobGradeAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<JobGradeListDto>> Handle(JobGradeAllQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<JobGrade>(v, x => x.Id, x => x.Name, x => x.StartSalary, x => x.MaxSalary, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobGrade>(v)
            .OrderBy<JobGrade>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<JobGradeListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<JobGradeListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new JobGradeListDto
            {
                Id = data.Id,
                Name = data.Name,
                StartSalary = data.StartSalary,
                MaxSalary = data.MaxSalary,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class JobGradeByIdHandler : IRequestHandler<JobGradeByIdQry, JobGradeListDto?>
{
    private readonly IDapperHelper _dapper;
    public JobGradeByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<JobGradeListDto?> Handle(JobGradeByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<JobGrade>(v, x => x.Id, x => x.Name, x => x.StartSalary, x => x.MaxSalary, x => x.DateAdd, x => x.DateMod!, x => x.xmin)
            .From<JobGrade>(v)
            .Where<JobGrade>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JobGradeListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new JobGradeListDto
        {
            Id = data.Id,
            Name = data.Name,
            StartSalary = data.StartSalary,
            MaxSalary = data.MaxSalary,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}

