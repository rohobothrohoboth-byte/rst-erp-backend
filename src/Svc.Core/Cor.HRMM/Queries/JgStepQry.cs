using Cor.HRMM.Interfaces;
using Cor.HRMM.Models.DTOs;
using Cor.HRMM.Models.Entities;
using Dapper;
using MediatR;

namespace Cor.HRMM.Queries;

public class JgStepAllQry : IRequest<List<JgStepListDto>> { public Guid Id { get; set; } }
public class JgStepByIdQry : IRequest<JgStepListDto?> { public Guid Id { get; set; } }


public class JgStepAllHandler : IRequestHandler<JgStepAllQry, List<JgStepListDto>>
{
    private readonly IDapperHelper _dapper;
    public JgStepAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<JgStepListDto>> Handle(JgStepAllQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(v, x => x.Id, x => x.Name, x => x.Salary, x => x.JobGradeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<JobGrade, JgStepListDto>(jg, x => x.Name, d => d.JobGrade)
            .From<JgStep>(v)
            .Join<JgStep, JobGrade>(v, jg, x => x.JobGradeId, x => x.Id)
            .Where<JgStep>(v, x => x.Id == request.Id)
            .OrderBy<JgStep>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        var dataL = new List<JgStepListDto>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<JgStepListDto>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            dataL.Add(new JgStepListDto
            {
                Id = data.Id,
                Name = data.Name,
                Salary = data.Salary,
                JobGradeId = data.JobGradeId,
                JobGrade = data.JobGrade,
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = data.xmin.ToString()
            });
        }
        return dataL;
    }
}

public class JgStepByIdQryHandler : IRequestHandler<JgStepByIdQry, JgStepListDto?>
{
    private readonly IDapperHelper _dapper;
    public JgStepByIdQryHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<JgStepListDto?> Handle(JgStepByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string jg = "jg";
        var qb = new QueryBuilder()
            .Select<JgStep>(v, x => x.Id, x => x.Name, x => x.Salary, x => x.JobGradeId, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<JobGrade, JgStepListDto>(jg, x => x.Name, d => d.JobGrade)
            .From<JgStep>(v)
            .Join<JgStep, JobGrade>(v, jg, x => x.JobGradeId, x => x.Id)
            .Where<JgStep>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<JgStepListDto>(sql, parameters, ct);
        if (data == null) return null;

        return new JgStepListDto
        {
            Id = data.Id,
            Name = data.Name,
            Salary = data.Salary,
            JobGradeId = data.JobGradeId,
            JobGrade = data.JobGrade,
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = data.xmin.ToString()
        };
    }
}