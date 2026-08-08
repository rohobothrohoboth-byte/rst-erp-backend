using Dapper;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpNameAllQry : IRequest<List<NameList>> { }
public class EmpNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class EmpNameAllHandler : IRequestHandler<EmpNameAllQry, List<NameList>>
{
    private readonly IDapperHelper _dapper;
    public EmpNameAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<NameList>> Handle(EmpNameAllQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id);

        var (sql, parameters) = qb.Build();
        var dataL = new List<NameList>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<EmpJoinRow>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            //var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);
            var ser = NumToWord.GetMonths(data.EmploymentDate, DateTime.UtcNow);

            dataL.Add(new NameList
            {
                Id = data.Id,
                Name = $"{data.FirstName} {data.MiddleName} {data.LastName}"
            });
        }

        return dataL;
    }
}

public class EmpNameByIdHandler : IRequestHandler<EmpNameByIdQry, NameList?>
{
    private readonly IDapperHelper _dapper;
    public EmpNameByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<NameList?> Handle(EmpNameByIdQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.EmploymentType, x => x.WorkArrangement, x => x.JobGradeId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<EmpJoinRow>(sql, parameters, ct);
        if (data == null) return null;

        //var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);
        var ser = NumToWord.GetMonths(data.EmploymentDate, DateTime.UtcNow);
        var c = new NameList
        {
            Id = data.Id,
            Name = $"{data.FirstName} {data.MiddleName} {data.LastName}"
        };
        return c;
    }
}