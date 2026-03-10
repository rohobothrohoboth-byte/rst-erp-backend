using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class PolicyRuleCondByRuleIdQry : IRequest<List<PolicyRuleCondListDto>> { public Guid Id { get; set; } }
public class PolicyRuleCondByIdQry : IRequest<PolicyRuleCondListDto?> { public Guid Id { get; set; } }



public class PolicyRuleCondByPolicyIdHandler : IRequestHandler<PolicyRuleCondByRuleIdQry, List<PolicyRuleCondListDto>>
{
    private readonly IDapperHelper _dapper;
    public PolicyRuleCondByPolicyIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PolicyRuleCondListDto>> Handle(PolicyRuleCondByRuleIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PolicyRuleCondition>(v, x => x.Id, x => x.Field, x => x.Operator, x => x.Value, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<PolicyAssignmentRule, PolicyRuleCondListDto>(c, x => x.Name, d => d.RuleName)
            .From<PolicyRuleCondition>(v)
            .Join<PolicyRuleCondition, PolicyAssignmentRule>(v, c, x => x.PolicyAssignmentRuleId, x => x.Id)
            .Where<PolicyRuleCondition>(v, x => x.PolicyAssignmentRuleId == request.Id)
            .OrderBy<PolicyRuleCondition>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyRuleCondListDto>(ct);

        foreach (var data in list)
        {
            data.FieldStr = MyEnumHelper.FormatEnum<ConditionField>(data.Field);
            data.OperatorStr = MyEnumHelper.FormatEnum<ConditionOperator>(data.Operator);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class PolicyRuleCondByIdHandler : IRequestHandler<PolicyRuleCondByIdQry, PolicyRuleCondListDto?>
{
    private readonly IDapperHelper _dapper;
    public PolicyRuleCondByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PolicyRuleCondListDto?> Handle(PolicyRuleCondByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        const string c = "c";
        var qb = new QueryBuilder()
            .Select<PolicyRuleCondition>(v, x => x.Id, x => x.Field, x => x.Operator, x => x.Value, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .SelectAs<PolicyAssignmentRule, PolicyRuleCondListDto>(c, x => x.Name, d => d.RuleName)
            .From<PolicyRuleCondition>(v)
            .Join<PolicyRuleCondition, PolicyAssignmentRule>(v, c, x => x.PolicyAssignmentRuleId, x => x.Id)
            .Where<PolicyRuleCondition>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PolicyRuleCondListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.FieldStr = MyEnumHelper.FormatEnum<ConditionField>(data.Field);
        data.OperatorStr = MyEnumHelper.FormatEnum<ConditionOperator>(data.Operator);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}