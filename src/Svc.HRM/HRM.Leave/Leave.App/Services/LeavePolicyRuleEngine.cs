using Helpers;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave.App.Services;

public static class LeavePolicyRuleEngine
{
    public static Task<List<ResolvePolicy>> Resolve(EmpPolicyCtx emp, List<PolicyCondCtx> conditions, Func<Guid, List<LeavePolicyConfig>> getConfigs)
    {
        var resolvedPolicies = new List<ResolvePolicy>();

        // Null checks
        if (emp == null)
        {
            Console.WriteLine("Emp is null in Resolve");
            return Task.FromResult(resolvedPolicies);
        }

        if (conditions == null || conditions.Count == 0)
        {
            Console.WriteLine("Conditions is null or empty in Resolve");
            return Task.FromResult(resolvedPolicies);
        }

        try
        {
            var grouped = conditions.GroupBy(c => c.PolAssignRuleId);
            var priorityOrder = new List<string> { "High", "Medium", "Low" };
            var sortedGroups = grouped.OrderBy(g => priorityOrder.IndexOf(g.First().Priority)).ToList();
            int serviceMonths = (int)emp.SerYear;

            foreach (var group in grouped)
            {
                bool allMatch = true;
                foreach (var cond in group)
                {
                    try
                    {
                        if (!EvaluateCondition(emp, cond))
                        {
                            allMatch = false;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error evaluating condition for rule {group.Key}: {ex.Message}");
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

                // FIXED: Get LeaveTypeId from the policy using the policyId
                // You need to get the LeaveTypeId from the LeavePolicy table
                // Since best doesn't have LeavePolicy navigation property loaded,
                // we need to query it or get it from the group

                // For now, use a temporary solution - you'll need to pass LeaveTypeId
                // Let's get it from the first condition or pass it differently
                var leaveTypeId = group.First().LeaveTypeId; // You need to add this to PolicyCondCtx

                // If LeaveTypeId is still empty, log a warning and skip
                if (leaveTypeId == Guid.Empty)
                {
                    Console.WriteLine($"Warning: LeaveTypeId is empty for policy {policyId}. Skipping assignment.");
                    continue;
                }

                resolvedPolicies.Add(new ResolvePolicy
                {
                    LeavePolicyId = policyId,
                    LeaveTypeId = leaveTypeId,
                    AssignedEntitlement = best.AnnualEntitlement,
                    EffectiveFrom = effectiveFrom == DateTime.MinValue ? DateTime.UtcNow : effectiveFrom
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Resolve method: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        return Task.FromResult(resolvedPolicies);
    }

    // Rest of the methods remain the same...
    private static bool EvaluateCondition(EmpPolicyCtx emp, PolicyCondCtx cond)
    {
        try
        {
            if (emp == null || cond == null)
            {
                Console.WriteLine("Emp or Cond is null in EvaluateCondition");
                return false;
            }

            if (string.IsNullOrEmpty(cond.Operator) || string.IsNullOrEmpty(cond.Field))
            {
                Console.WriteLine($"Operator or Field is null/empty - Operator: '{cond.Operator}', Field: '{cond.Field}'");
                return false;
            }

            if (!Enum.TryParse<ConditionOperator>(cond.Operator, true, out var op))
            {
                Console.WriteLine($"Failed to parse Operator: '{cond.Operator}'");
                return false;
            }

            if (!Enum.TryParse<ConditionField>(cond.Field, true, out var field))
            {
                Console.WriteLine($"Failed to parse Field: '{cond.Field}'");
                return false;
            }

            return field switch
            {
                ConditionField.EmpNat => CompareString(emp.EmpType ?? "", cond.Value ?? "", op),
                ConditionField.Gender => CompareGender(emp.Gender ?? "", cond.Value ?? "", op),
                ConditionField.SerYear => CompareNumber(emp.SerYear, cond.Value ?? "0", op),
                ConditionField.WorkAr => CompareString(emp.WorkAr ?? "", cond.Value ?? "", op),
                ConditionField.Jg => CompareString(emp.Jg ?? "", cond.Value ?? "", op),
                _ => false
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in EvaluateCondition: {ex.Message}");
            return false;
        }
    }

    private static bool CompareGender(string empVal, string condVal, ConditionOperator op)
    {
        try
        {
            if (!Enum.TryParse<PolicyGender>(empVal, true, out var empGender))
            {
                return CompareString(empVal, condVal, op);
            }

            if (!Enum.TryParse<PolicyGender>(condVal, true, out var condGender))
                return false;

            if (condGender == PolicyGender.Both)
                return true;

            return CompareString(empGender.ToString(), condGender.ToString(), op);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CompareGender: {ex.Message}");
            return false;
        }
    }

    private static bool CompareNumber(double empVal, string condVal, ConditionOperator op)
    {
        try
        {
            if (!double.TryParse(condVal, out var condNum))
                return false;

            return op switch
            {
                ConditionOperator.Equal => Math.Abs(empVal - condNum) < 0.001,
                ConditionOperator.NotEquals => Math.Abs(empVal - condNum) >= 0.001,
                ConditionOperator.GreaterThan => empVal > condNum,
                ConditionOperator.LessThan => empVal < condNum,
                ConditionOperator.GreaterOrEqual => empVal >= condNum,
                ConditionOperator.LessOrEqual => empVal <= condNum,
                _ => false
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CompareNumber: {ex.Message}");
            return false;
        }
    }

    private static bool CompareString(string empVal, string condVal, ConditionOperator op)
    {
        try
        {
            empVal = empVal ?? "";
            condVal = condVal ?? "";

            return op switch
            {
                ConditionOperator.Equal => string.Equals(empVal, condVal, StringComparison.OrdinalIgnoreCase),
                ConditionOperator.NotEquals => !string.Equals(empVal, condVal, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CompareString: {ex.Message}");
            return false;
        }
    }
}