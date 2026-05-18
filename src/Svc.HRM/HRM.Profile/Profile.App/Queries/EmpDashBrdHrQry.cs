using Common;
using Helpers;
using MediatR;
using Profile.App.Interfaces;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Queries;

public class EmpDbRepQry : IRequest<EmpDbReport> { }
public class EmpDbPendQry : IRequest<List<EmpDbPendList>> { }



public class EmpDbRepHandler(IDapperHelper _dapper) : IRequestHandler<EmpDbRepQry, EmpDbReport>
{
    public async Task<EmpDbReport> Handle(EmpDbRepQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<Employee>(v, x => x.EmpState)
            .From<Employee>(v)
            .OrderBy<Employee>(v, x => x.DateAdd, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpStateList>(ct);
        if (list.Count <= 0) { return new EmpDbReport(); }

        return new EmpDbReport
        {
            EmpTot = list.Count,
            EmpAct = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Active)).ToList().Count,
            EmpPen = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Pen)).ToList().Count,
            EmpSus = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Sus)).ToList().Count,
            EmpRet = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Retire)).ToList().Count,
            EmpStd = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.StandBy)).ToList().Count,
            EmpTer = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Term)).ToList().Count,
            EmpLeave = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Leave)).ToList().Count,
            EmpRej = list.Where(e => e.EmpState == BoolToStr.EnumToString(EmpState.Rej)).ToList().Count,
        };
    }
}

public class EmpDbPendHandler(IDapperHelper _dapper, ICorHrmmClient _corHRMM, ICorModClient _corMod) : IRequestHandler<EmpDbPendQry, List<EmpDbPendList>>
{
    public async Task<List<EmpDbPendList>> Handle(EmpDbPendQry request, CancellationToken ct)
    {
        var deptTask = _corMod.GetListDept(ct);
        var posTask = _corHRMM.GetListPosition(ct);
        var jgTask = _corHRMM.GetListJobGrade(ct);
        await Task.WhenAll(deptTask, posTask,jgTask);

        var deptDict = deptTask.Result.Res.ToDictionary(d => Guid.Parse(d.Id));
        var posDict = posTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        var jgDict = jgTask.Result.Res.ToDictionary(p => Guid.Parse(p.Id));
        var penEmp = BoolToStr.EnumToString(EmpState.Pen);
        const string e = "e";
        const string p = "p";
        var qb = new QueryBuilder()
            .Select<Employee>(e, x => x.Id, x => x.Code, x => x.EmpState,x => x.JobGradeId, x => x.DepartmentId, x => x.PositionId)
            .Select<Person>(p, x => x.FirstName, x => x.MiddleName, x => x.LastName, x => x.FirstNameAm, x => x.MiddleNameAm, x => x.LastNameAm, x => x.Gender)
            .From<Employee>(e)
            .Join<Employee, Person>(e, p, x => x.PersonId, x => x.Id)
            .Where<Employee>(e, x => x.EmpState == penEmp)
            .OrderBy<Employee>(e, x => x.DateAdd, desc: false);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpDbPendJoin>(ct);
        if (list.Count <= 0) { return []; }

        var result = new List<EmpDbPendList>();
        foreach (var data in list)
        {
            deptDict.TryGetValue(data.DepartmentId, out var dept);
            posDict.TryGetValue(data.PositionId, out var pos);
            jgDict.TryGetValue(data.JobGradeId, out var jg);

            result.Add(new EmpDbPendList
            {
                Code = data.Code,
                EmpFullName = $"{data.FirstName} {data.MiddleName} {data.LastName}",
                EmpFullNameAm = $"{data.FirstNameAm} {data.MiddleNameAm} {data.LastNameAm}",                
                Gender = MyEnumHelper.FormatEnum<Gender>(data.Gender),
                Branch = dept?.NameAm ?? "",
                Department = dept?.Name ?? "",
                Position = pos?.Name ?? "",
                JobGrade = jg?.Name ?? "",
            });
        }

        return result;
    }
}