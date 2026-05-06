using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class EmpLeaveBalQry : IRequest<List<EmpLeaveBal>> { public Guid Id { get; set; } }



public class EmpLeaveBalHandler : IRequestHandler<EmpLeaveBalQry, List<EmpLeaveBal>>
{
    private readonly IDapperHelper _dapper;
    public EmpLeaveBalHandler(IDapperHelper dapper) { _dapper = dapper; }

    private async Task<double?> GetRemain(Guid empId, Guid ltId, Guid lpId, CancellationToken ct)
    {
        const string e = "e";
        var qb = new QueryBuilder()
            .SelectAs<LeaveBalance, DoubleDto>(e, x => x.Balance, x => x.Value)
            .From<LeaveBalance>(e)
            .Where<LeaveBalance>(e, x => x.EmployeeId == empId && x.LeaveTypeId == ltId && x.LeavePolicyId == lpId)
            .Limit(1);
        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<DoubleDto>(sql, parameters, ct);
        return data?.Value;
    }

    public async Task<List<EmpLeaveBal>> Handle(EmpLeaveBalQry request, CancellationToken ct)
    {
        const string e = "e";
        const string lt = "lt";
        var qb = new QueryBuilder()
            .Select<EmpLeavePolicy>(e, x => x.Id, x => x.AssignedEntitlement, x => x.EmployeeId, x => x.LeaveTypeId, x => x.LeavePolicyId)
            .SelectAs<LeaveType, EmpLeaveBal>(lt, x => x.Name, x => x.LeaveType)
            .From<EmpLeavePolicy>(e)
            .Join<EmpLeavePolicy, LeaveType>(e, lt, x => x.LeaveTypeId, x => x.Id)
            .Where<EmpLeavePolicy>(e, x => x.EmployeeId == request.Id && x.EffectiveTo == null)
            .OrderBy<EmpLeavePolicy>(e, x => x.DateAdd, desc: true);
        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<EmpLeaveBal>(ct);

        foreach (var data in list)
        {
            var gRmn = await GetRemain(data.EmployeeId, data.LeaveTypeId, data.LeavePolicyId, ct);
            var rmn = data.AssignedEntitlement;
            if (gRmn != null) { rmn = (double)gRmn; }

            var usd = data.AssignedEntitlement - rmn;
            data.Percent = (usd * 100) / data.AssignedEntitlement;
            data.TotalDays = $"{data.AssignedEntitlement} days";
            data.RemainDays = $"{rmn} days";
            data.UsedDays = $"{usd} days";
        }

        return list;
    }
}