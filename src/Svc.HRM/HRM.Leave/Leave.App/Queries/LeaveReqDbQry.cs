using Common;
using Helpers;
using Leave.Domain.DTOs;
using Leave.App.Interfaces;
using Leave.Domain.Entities;

namespace Leave.App.Queries;

public class LvReqPendDbQry : IQuery<List<LeaveReqDbList>> { }



public class LvReqPendDb(IDapperHelper _dapper, IHrmProfileClient _hrmProfile) : IQueryHandler<LvReqPendDbQry, List<LeaveReqDbList>>
{
    public async Task<List<LeaveReqDbList>> Handle(LvReqPendDbQry request, CancellationToken ct)
    {
        var empTask = await _hrmProfile.GetEmpNameList(ct);
        var empDict = empTask.Res.ToDictionary(d => Guid.Parse(d.Id));

        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<LeaveRequest>(v, x => x.DaysRequested, x => x.EmployeeId)
            .SelectAs<LeaveType, LeaveReqDbList>(c, x => x.Name, d => d.LeaveType)
            .From<LeaveRequest>(v)
            .Join<LeaveRequest, LeaveType>(v, c, x => x.LeaveTypeId, x => x.Id)
            .Limit(5)
            .OrderBy<LeaveRequest>(v, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReader(sql, parameters, ct);
        var list = await reader.ToListAsync<LeaveReqDbList>(ct);

        foreach (var data in list)
        {
            empDict.TryGetValue(data.EmployeeId, out var emp);
            data.Name = emp?.Name ?? "";
            data.NameAm = emp?.NameAm ?? "";
        }

        return list;
    }
}