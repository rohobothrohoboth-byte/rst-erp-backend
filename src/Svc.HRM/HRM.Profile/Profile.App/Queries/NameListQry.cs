using Dapper;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpPolicyAllQry : IRequest<List<EmpPolicyCtx>> { }
public class EmpPolicyByIdQry : IRequest<EmpPolicyCtx?> { public Guid Id { get; set; } }
public class EmpNameAllQry : IRequest<List<NameList>> { }
public class EmpNameByIdQry : IRequest<NameList?> { public Guid Id { get; set; } }



public class EmpPolicyAllHandler : IRequestHandler<EmpPolicyAllQry, List<EmpPolicyCtx>>
{
    private readonly IDapperHelper _dapper;
    public EmpPolicyAllHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<EmpPolicyCtx>> Handle(EmpPolicyAllQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.EmploymentType, x => x.WorkArrangement, x => x.JobGradeId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id);

        var (sql, parameters) = qb.Build();
        var dataL = new List<EmpPolicyCtx>();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var parser = reader.GetRowParser<EmpJoinRow>();

        while (await reader.ReadAsync(ct))
        {
            var data = parser(reader);
            var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);

            dataL.Add(new EmpPolicyCtx
            {
                EmployeeId = data.Id,
                Name = $"{data.FirstName} {data.MiddleName} {data.LastName}",
                Gender = data.Gender,
                EmpType = data.EmploymentType,
                Jg = data.JobGradeId.ToString(),
                WorkAr = data.WorkArrangement,
                SerYear = ser
            });
        }

        return dataL;
    }
}

public class EmpPolicyByIdHandler : IRequestHandler<EmpPolicyByIdQry, EmpPolicyCtx?>
{
    private readonly IDapperHelper _dapper;
    public EmpPolicyByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<EmpPolicyCtx?> Handle(EmpPolicyByIdQry request, CancellationToken ct)
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

        var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);
        var c = new EmpPolicyCtx
        {
            EmployeeId = data.Id,
            Name = $"{data.FirstName} {data.MiddleName} {data.LastName}",
            Gender = data.Gender,
            EmpType = data.EmploymentType,
            Jg = data.JobGradeId.ToString(),
            WorkAr = data.WorkArrangement,
            SerYear = ser
        };
        return c;
    }
}

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
            var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);

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

        var ser = new NumToWord().GetMonths(data.EmploymentDate, DateTime.UtcNow);
        var c = new NameList
        {
            Id = data.Id,
            Name = $"{data.FirstName} {data.MiddleName} {data.LastName}"
        };
        return c;
    }
}