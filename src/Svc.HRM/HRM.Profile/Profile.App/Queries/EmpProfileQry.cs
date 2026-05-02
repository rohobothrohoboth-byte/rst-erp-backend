using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class ProfileInfoQry : IRequest<ProfileInfo?> { public Guid Id { get; set; } }
public class ProfileCardQry : IRequest<ProfileCard?> { public Guid Id { get; set; } }



public class ProfileInfoHandler : IRequestHandler<ProfileInfoQry, ProfileInfo?>
{
    private readonly IDapperHelper _dapper;
    private readonly ICorHrmmClient _corHRMM;
    private readonly ICorModClient _corMod;

    public ProfileInfoHandler(IDapperHelper dapper, ICorHrmmClient corHRMM, ICorModClient corMod)
    {
        _dapper = dapper;
        _corHRMM = corHRMM;
        _corMod = corMod;
    }

    public async Task<ProfileInfo?> Handle(ProfileInfoQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.DepartmentId, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<ProfileInfoJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var deptTask = _corMod.GetDept(row.DepartmentId.ToString(), ct);
        var positionTask = _corHRMM.GetPosition(row.PositionId.ToString(), ct);
        await Task.WhenAll(deptTask, positionTask);
        var dept = deptTask.Result?.Res;
        var position = positionTask.Result?.Res;

        return new ProfileInfo
        {
            Code = row.Code,
            FullName = $"{row.FirstName} {row.MiddleName} {row.LastName}".Trim(),
            FullNameAm = $"{row.FirstNameAm} {row.MiddleNameAm} {row.LastNameAm}".Trim(),
            Department = dept?.Name ?? "",
            Position = position?.Name ?? "",
        };
    }
}

public class ProfileCardHandler : IRequestHandler<ProfileCardQry, ProfileCard?>
{
    private readonly IDapperHelper _dapper;
    public ProfileCardHandler(IDapperHelper dapper)
    {
        _dapper = dapper;
    }

    public async Task<ProfileCard?> Handle(ProfileCardQry request, CancellationToken ct)
    {
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.EmploymentDate)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.Id == request.Id)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var row = await _dapper.QueryFirstOrDefaultAsync<ProfileCardJoin>(sql, parameters, ct);
        if (row is null) { return null; }

        var serStr = NumToWord.FullServDur(row.EmploymentDate);
        return new ProfileCard
        {
            Tenure = serStr,
            Performance = $"{4.5} / {5}".Trim(),
            Training = "2",
            Attendance = "95%",
            RepToName = "Sarah Johnson",
            RepToPos = "Team Lead"
        };
    }
}