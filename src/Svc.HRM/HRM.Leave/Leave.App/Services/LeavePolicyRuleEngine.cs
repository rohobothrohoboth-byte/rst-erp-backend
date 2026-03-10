using Helpers;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

namespace Leave.App.Services;

public static class LeavePolicyRuleEngine
{
    public static Task<List<ResolvePolicy>> Resolve(EmpPolicyCtx emp, List<PolicyCondCtx> conditions, Func<Guid, List<LeavePolicyConfig>> getConfigs)
    {
        var resolvedPolicies = new List<ResolvePolicy>();
        if (conditions == null || conditions.Count == 0) { return Task.FromResult(resolvedPolicies); }

        var grouped = conditions.GroupBy(c => c.PolAssignRuleId);
        var priorityOrder = new List<string> { "High", "Medium", "Low" };
        var sortedGroups = grouped.OrderBy(g => priorityOrder.IndexOf(g.First().Priority)).ToList();
        int serviceMonths = (int)emp.SerYear;

        foreach (var group in grouped)
        {
            bool allMatch = true;
            foreach (var cond in group)
            {
                if (!EvaluateCondition(emp, cond))
                {
                    allMatch = false;
                    break;
                }
            }

            if (!allMatch) { continue; }

            var policyId = group.First().PolicyId;
            var effectiveFrom = group.First().EffectiveFrom;
            var configs = getConfigs(policyId);
            if (configs == null || configs.Count == 0) { continue; }

            LeavePolicyConfig? best = null;
            foreach (var c in configs)
            {
                if (!c.IsActive) continue;
                if (c.MinServiceMonths > serviceMonths) continue;
                if (best == null || c.MinServiceMonths > best.MinServiceMonths) { best = c; }
            }

            if (best == null) { continue; }
            resolvedPolicies.Add(new ResolvePolicy
            {
                LeavePolicyId = policyId,
                LeaveTypeId = best.LeavePolicy.LeaveTypeId,
                AssignedEntitlement = best.AnnualEntitlement,
                EffectiveFrom = effectiveFrom
            });
        }

        return Task.FromResult(resolvedPolicies);
    }

    private static bool EvaluateCondition(EmpPolicyCtx emp, PolicyCondCtx cond)
    {
        var op = ((ConditionOperator)Enum.Parse(typeof(ConditionOperator), cond.Operator));
        var field = ((ConditionField)Enum.Parse(typeof(ConditionField), cond.Field));

        return field switch
        {
            ConditionField.EmpNat => CompareString(emp.EmpType, cond.Value, op),
            ConditionField.Gender => CompareGender(emp.Gender, cond.Value, op),
            ConditionField.SerYear => CompareNumber(emp.SerYear, cond.Value, op),
            ConditionField.WorkAr => CompareString(emp.WorkAr, cond.Value, op),
            ConditionField.Jg => CompareString(emp.Jg, cond.Value, op),
            _ => false
        };
    }

    private static bool CompareGender(string empVal, string condVal, ConditionOperator op)
    {
        if (!Enum.TryParse<PolicyGender>(empVal, out var empGender))
            return false;
        if (!Enum.TryParse<PolicyGender>(condVal, out var condGender))
            return false;

        if (condGender == PolicyGender.Both)
            return true;

        return CompareString(empGender.ToString(), condGender.ToString(), op);
    }

    private static bool CompareNumber(double empVal, string condVal, ConditionOperator op)
    {
        if (!double.TryParse(condVal, out var condNum))
            return false;

        return op switch
        {
            ConditionOperator.Equal => empVal == condNum,
            ConditionOperator.NotEquals => empVal != condNum,
            ConditionOperator.GreaterThan => empVal > condNum,
            ConditionOperator.LessThan => empVal < condNum,
            ConditionOperator.GreaterOrEqual => empVal >= condNum,
            ConditionOperator.LessOrEqual => empVal <= condNum,
            _ => false
        };
    }

    private static bool CompareString(string empVal, string condVal, ConditionOperator op)
    {
        return op switch
        {
            ConditionOperator.Equal => empVal == condVal,
            ConditionOperator.NotEquals => empVal != condVal,
            _ => false
        };
    }
}