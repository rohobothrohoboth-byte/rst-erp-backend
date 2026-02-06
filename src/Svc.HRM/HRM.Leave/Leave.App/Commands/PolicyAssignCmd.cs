using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Commands;

public class PolicyAssignCmd : IRequest<string> { }

public class PolicyAssignHandler : IRequestHandler<PolicyAssignCmd, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHrmProfileClient _hrmPro;
    public PolicyAssignHandler(IUnitOfWork unitOfWork, IHrmProfileClient hrmPro)
    {
        _unitOfWork = unitOfWork;
        _hrmPro = hrmPro;
    }

    private async Task<List<PolicyCondCtx>> GetCondList()
    {
        var condList = new List<PolicyCondCtx>();
        var stat = BoolToStr.EnumToString(PolicyStatus.Active);
        var poL = (await _unitOfWork.Repository<LeavePolicy>().Find(p => p.Status == stat)).Select(p => p.Id).ToList();

        if (poL.Count > 0)
        {
            var pAss = new List<PolicyAssignmentRule>();
            foreach (var policyId in poL)
            {
                var items = (await _unitOfWork.Repository<PolicyAssignmentRule>().Find(p => p.LeavePolicyId == policyId && p.IsActive)).ToList();
                pAss.AddRange(items);
            }

            if (pAss.Count > 0)
            {
                foreach (var po in pAss)
                {
                    var condL = (await _unitOfWork.Repository<PolicyRuleCondition>().Find(c => c.PolicyAssignmentRuleId == po.Id)).ToList();
                    if (condL.Count > 0)
                    {
                        foreach (var cond in condL)
                        {
                            var co = new PolicyCondCtx
                            {
                                PolicyId = po.LeavePolicyId,
                                PolAssignRuleId = po.Id,
                                PolRuleCondId = cond.Id,
                                EffectiveFrom = po.EffectiveFrom,
                                Priority = po.Priority,
                                Field = cond.Field,
                                Operator = cond.Operator,
                                Value = cond.Value
                            };
                            condList.Add(co);
                        }
                    }
                }
            }
        }

        return condList;
    }

    private async Task<List<EmpPolicyCtx>> GetEmpList(CancellationToken cancellationToken)
    {
        var empList = new List<EmpPolicyCtx>();
        var empL = await _hrmPro.GetListEmpPolicy(cancellationToken);
        if (empL.Res.Count > 0)
        {
            foreach (var emp in empL.Res)
            {
                var vm = new EmpPolicyCtx
                {
                    EmployeeId = Guid.Parse(emp.Id),
                    Name = emp.Name,
                    SerYear = double.Parse(emp.SerYear),
                    EmpType = emp.EmpType,
                    WorkAr = emp.WorkAr,
                    Gender = emp.Gender,
                    Jg = emp.Jg,
                };
                empList.Add(vm);
            }
        }

        return empList;
    }

    private bool IsSamePolicy(EmpLeavePolicy active, ResolvePolicy resolved)
    {
        return active.LeavePolicyId == resolved.LeavePolicyId && Math.Abs(active.AssignedEntitlement - resolved.AssignedEntitlement) < 0.0001 && active.EffectiveFrom == resolved.EffectiveFrom;
    }

    public async Task<string> Handle(PolicyAssignCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var today = DateTime.UtcNow;
            var assignedCount = 0;
            var empList = await GetEmpList(cancellationToken);
            if (!empList.Any()) { throw new DomainException("NO EMPLOYEES FOUND for leave policy assignment."); }

            var allCondList = await GetCondList();
            if (!allCondList.Any()) { throw new DomainException("NO ACTIVE LEAVE POLICIES or CONDITIONS FOUND."); }

            var resolvedPolicies = new List<ResolvePolicy>();
            foreach (var emp in empList)
            {
                var res = await LeavePolicyRuleEngine.Resolve(emp, allCondList, async policyId => (await _unitOfWork.Repository<LeavePolicyConfig>().Find(c => c.LeavePolicyId == policyId)).ToList());

                if (res.Count > 0)
                {
                    foreach (var r in res)
                    {
                        var lp = await _unitOfWork.Repository<LeavePolicy>().GetById(r.LeavePolicyId);
                        if (lp != null)
                        {
                            r.EmployeeId = emp.EmployeeId;
                            r.LeaveTypeId = lp.LeaveTypeId;
                            resolvedPolicies.Add(r);
                        }
                    }
                }
            }

            if (!resolvedPolicies.Any()) { throw new DomainException("NO RESOLVED LEAVE POLICIES for employees."); }

            var empIds = resolvedPolicies.Select(x => x.EmployeeId).Distinct().ToList();
            var leaveTypeIds = resolvedPolicies.Select(x => x.LeaveTypeId).Distinct().ToList();

            var allPolicies = await _unitOfWork.Repository<EmpLeavePolicy>().GetAll();
            var activePolicies = new List<EmpLeavePolicy>();
            foreach (var empId in empIds)
            {
                foreach (var leaveTypeId in leaveTypeIds)
                {
                    var policy = allPolicies.FirstOrDefault(p => p.EmployeeId == empId && p.LeaveTypeId == leaveTypeId && p.EffectiveTo == null);
                    if (policy != null) { activePolicies.Add(policy); }
                }
            }

            var activeDict = activePolicies.GroupBy(x => (x.EmployeeId, x.LeaveTypeId)).ToDictionary(g => g.Key, g => g.First());

            var updates = new List<EmpLeavePolicy>();
            var inserts = new List<EmpLeavePolicy>();
            var reason = BoolToStr.EnumToString(EmpLeavePolReason.PolChange);

            foreach (var res in resolvedPolicies)
            {
                var key = (res.EmployeeId, res.LeaveTypeId);

                if (activeDict.TryGetValue(key, out var activePolicy))
                {
                    if (IsSamePolicy(activePolicy, res)) { continue; }

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

            foreach (var upd in updates)
            {
                await _unitOfWork.Repository<EmpLeavePolicy>().Update(upd);
                //assignedCount++;
            }

            foreach (var ins in inserts)
            {
                await _unitOfWork.Repository<EmpLeavePolicy>().Add(ins);
                assignedCount++;
            }

            await _unitOfWork.Commit();

            return $"{assignedCount} EMPLOYEES LEAVE POLICY ASSIGNMENTS processed successfully.";
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}