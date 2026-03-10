using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class PolicyAssignCmd : IRequest<string> { }

public class PolicyAssignHandler : IRequestHandler<PolicyAssignCmd, string>
{
    private readonly IDapperHelper _dapper;
    private readonly IUnitOfWork _uow;
    private readonly IHrmProfileClient _hrmPro;
    public PolicyAssignHandler(IDapperHelper dapper, IUnitOfWork uow, IHrmProfileClient hrmPro)
    {
        _dapper = dapper;
        _uow = uow;
        _hrmPro = hrmPro;
    }

    private async Task<List<PolicyCondCtx>> GetCondList(CancellationToken ct)
    {
        const string p = "p";
        const string r = "r";
        const string c = "c";
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        var qb = new QueryBuilder()
            .SelectAs<LeavePolicy, PolicyCondCtx>(p, x => x.Id, d => d.PolicyId)
            .SelectAs<PolicyAssignmentRule, PolicyCondCtx>(r, x => x.Id, d => d.PolAssignRuleId)
            .SelectAs<PolicyAssignmentRule, PolicyCondCtx>(r, x => x.EffectiveFrom, d => d.EffectiveFrom)
            .SelectAs<PolicyAssignmentRule, PolicyCondCtx>(r, x => x.Priority, d => d.Priority)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Id, d => d.PolRuleCondId)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Field, d => d.Field)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Operator, d => d.Operator)
            .SelectAs<PolicyRuleCondition, PolicyCondCtx>(c, x => x.Value, d => d.Value)
            .From<LeavePolicy>(p)
            .Join<LeavePolicy, PolicyAssignmentRule>(p, r, x => x.Id, x => x.LeavePolicyId)
            .Join<PolicyAssignmentRule, PolicyRuleCondition>(r, c, x => x.Id, x => x.PolicyAssignmentRuleId)
            .Where<LeavePolicy>(p, x => x.Status == stat)
            .Where<PolicyAssignmentRule>(r, x => x.IsActive)
            .OrderBy<PolicyAssignmentRule>(r, x => x.EffectiveFrom);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyCondCtx>(ct);
        return list;
    }

    private async Task<List<EmpPolicyCtx>> GetEmpList(CancellationToken ct)
    {
        var empList = new List<EmpPolicyCtx>();
        var empL = await _hrmPro.GetListEmpPolicy(ct);
        if (empL.Res.Count > 0)
        {
            empList.AddRange(from emp in empL.Res
                             let vm = new EmpPolicyCtx
                             {
                                 EmployeeId = Guid.Parse(emp.Id),
                                 Name = emp.Name,
                                 SerYear = double.Parse(emp.SerYear),
                                 EmpType = emp.EmpType,
                                 WorkAr = emp.WorkAr,
                                 Gender = emp.Gender,
                                 Jg = emp.Jg,
                             }
                             select vm);
        }

        return empList;
    }

    private bool IsSamePolicy(EmpLeavePolicy active, ResolvePolicy resolved)
    {
        return active.LeavePolicyId == resolved.LeavePolicyId && Math.Abs(active.AssignedEntitlement - resolved.AssignedEntitlement) < 0.0001 && active.EffectiveFrom == resolved.EffectiveFrom;
    }

    public async Task<string> Handle(PolicyAssignCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var empList = await GetEmpList(ct);
            if (!empList.Any()) { throw new DomainException("NO EMPLOYEES FOUND for leave policy assignment."); }

            var allCondList = await GetCondList(ct);
            if (!allCondList.Any()) { throw new DomainException("NO ACTIVE LEAVE POLICIES OR CONDITIONS."); }

            const string pc = "pc";
            const string lp = "lp";
            var qbPolicy = new QueryBuilder()
                .SelectAs<LeavePolicyConfig, ResolvePolicy>(pc, x => x.LeavePolicyId, d => d.LeavePolicyId)
                .SelectAs<LeavePolicyConfig, ResolvePolicy>(pc, x => x.AnnualEntitlement, d => d.AssignedEntitlement)
                .SelectAs<LeavePolicy, ResolvePolicy>(lp, x => x.LeaveTypeId, d => d.LeaveTypeId)
                .From<LeavePolicyConfig>(pc)
                .Join<LeavePolicyConfig, LeavePolicy>(pc, lp, x => x.LeavePolicyId, x => x.Id)
                .Where<LeavePolicyConfig>(pc, x => x.IsActive);
            var (sqlPol, paramPol) = qbPolicy.Build();
            await using var readerPol = await _dapper.ExecuteReaderAsync(sqlPol, paramPol, ct);
            var policyConfigs = await readerPol.ToListAsync<LeavePolicyConfig>(ct);
            var configLookup = policyConfigs.GroupBy(x => x.LeavePolicyId).ToDictionary(g => g.Key, g => g.ToList());

            const string ep = "ep";
            var qbActive = new QueryBuilder()
                .Select<EmpLeavePolicy>(ep, x => x.Id, x => x.EmployeeId, x => x.LeaveTypeId, x => x.LeavePolicyId, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.AssignedEntitlement)
                .From<EmpLeavePolicy>(ep)
                .WhereRaw<EmpLeavePolicy>(ep, x => x.EffectiveTo, "IS NULL");
            var (sqlAct, paramAct) = qbActive.Build();
            await using var readerAct = await _dapper.ExecuteReaderAsync(sqlAct, paramAct, ct);
            var activePolicies = await readerAct.ToListAsync<EmpLeavePolicy>(ct);

            var resolvedPolicies = new List<ResolvePolicy>();
            foreach (var emp in empList)
            {
                var res = await LeavePolicyRuleEngine.Resolve(emp, allCondList, policyId => configLookup.TryGetValue(policyId, out var list) ? list : []);
                foreach (var r in res)
                {
                    r.EmployeeId = emp.EmployeeId;
                    resolvedPolicies.Add(r);
                }
            }

            if (!resolvedPolicies.Any()) { throw new DomainException("NO RESOLVED LEAVE POLICIES FOR EMPLOYEES."); }

            var activeDict = activePolicies.GroupBy(x => (x.EmployeeId, x.LeaveTypeId)).ToDictionary(g => g.Key, g => g.First());
            var updates = new List<EmpLeavePolicy>();
            var inserts = new List<EmpLeavePolicy>();
            var reason = BoolToStr.EnumToString(EmpLeavePolReason.PolChange);
            var today = DateTime.UtcNow;
            var assignedCount = 0;

            foreach (var res in resolvedPolicies)
            {
                var key = (res.EmployeeId, res.LeaveTypeId);
                if (activeDict.TryGetValue(key, out var activePolicy))
                {
                    if (IsSamePolicy(activePolicy, res)) continue;

                    activePolicy.EffectiveTo = today;
                    updates.Add(activePolicy);
                }

                inserts.Add(new EmpLeavePolicy
                {
                    EmployeeId = res.EmployeeId,
                    LeaveTypeId = res.LeaveTypeId,
                    LeavePolicyId = res.LeavePolicyId,
                    EffectiveFrom = res.EffectiveFrom,
                    AssignedEntitlement = res.AssignedEntitlement,
                    Reason = reason
                });
            }

            foreach (var upd in updates) { await _uow.Update(upd); }
            foreach (var ins in inserts)
            {
                await _uow.Add(ins, ct);
                assignedCount++;
            }

            await _uow.Commit(ct);
            return $"{assignedCount} EMPLOYEES LEAVE POLICY ASSIGNMENTS processed successfully.";
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}