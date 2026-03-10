using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Queries;

public class AssignmentRuleByPolicyIdQry : IRequest<List<PolicyAssignmentRuleListDto>> { public Guid Id { get; set; } }
public class PolicyAssignmentRuleByIdQry : IRequest<PolicyAssignmentRuleListDto?> { public Guid Id { get; set; } }
public class ActiveAssignmentRulesQry : IRequest<List<PolicyAssignmentRuleListDto>> { public Guid Id { get; set; } }



public class AssignmentRuleByPolicyIdHandler : IRequestHandler<AssignmentRuleByPolicyIdQry, List<PolicyAssignmentRuleListDto>>
{
    private readonly IDapperHelper _dapper;
    public AssignmentRuleByPolicyIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PolicyAssignmentRuleListDto>> Handle(AssignmentRuleByPolicyIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PolicyAssignmentRule>(v, x => x.Id, x => x.Name, x => x.Code, x => x.Code, x => x.Priority, x => x.IsActive, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<PolicyAssignmentRule>(v)
            .OrderBy<PolicyAssignmentRule>(v, x => x.DateAdd, desc: true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyAssignmentRuleListDto>(ct);

        foreach (var data in list)
        {
            data.PriorityStr = MyEnumHelper.FormatEnum<Priority>(data.Priority);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}

public class PolicyAssignmentRuleByIdHandler : IRequestHandler<PolicyAssignmentRuleByIdQry, PolicyAssignmentRuleListDto?>
{
    private readonly IDapperHelper _dapper;
    public PolicyAssignmentRuleByIdHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<PolicyAssignmentRuleListDto?> Handle(PolicyAssignmentRuleByIdQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PolicyAssignmentRule>(v, x => x.Id, x => x.Name, x => x.Code, x => x.Code, x => x.Priority, x => x.IsActive, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<PolicyAssignmentRule>(v)
            .Where<PolicyAssignmentRule>(v, x => x.Id == request.Id)
            .Limit(1);

        var (sql, parameters) = qb.Build();
        var data = await _dapper.QueryFirstOrDefaultAsync<PolicyAssignmentRuleListDto>(sql, parameters, ct);
        if (data == null) return null;

        data.PriorityStr = MyEnumHelper.FormatEnum<Priority>(data.Priority);
        data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
        data.RowVersion = data.xmin.ToString();
        return data;
    }
}

public class ActiveAssignmentRulesHandler : IRequestHandler<ActiveAssignmentRulesQry, List<PolicyAssignmentRuleListDto>>
{
    private readonly IDapperHelper _dapper;
    public ActiveAssignmentRulesHandler(IDapperHelper dapper) { _dapper = dapper; }

    public async Task<List<PolicyAssignmentRuleListDto>> Handle(ActiveAssignmentRulesQry request, CancellationToken ct)
    {
        const string v = "v";
        var qb = new QueryBuilder()
            .Select<PolicyAssignmentRule>(v, x => x.Id, x => x.Name, x => x.Code, x => x.Code, x => x.Priority, x => x.IsActive, x => x.EffectiveFrom, x => x.EffectiveTo, x => x.DateAdd, x => x.DateMod, x => x.xmin)
            .From<PolicyAssignmentRule>(v)
            .Where<PolicyAssignmentRule>(v, x => x.LeavePolicyId == request.Id && x.IsActive == true);

        var (sql, parameters) = qb.Build();
        await using var reader = await _dapper.ExecuteReaderAsync(sql, parameters, ct);
        var list = await reader.ToListAsync<PolicyAssignmentRuleListDto>(ct);

        foreach (var data in list)
        {
            data.PriorityStr = MyEnumHelper.FormatEnum<Priority>(data.Priority);
            data.IsActiveStr = BoolToStr.FormatStat(data.IsActive);
            data.RowVersion = data.xmin.ToString();
        }

        return list;
    }
}