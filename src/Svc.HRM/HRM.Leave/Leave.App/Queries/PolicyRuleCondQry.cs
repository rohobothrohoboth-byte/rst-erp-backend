using Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class PolicyRuleCondByRuleIdQry : IRequest<List<PolicyRuleCondListDto>> { public Guid Id { get; set; } }
public class PolicyRuleCondByIdQry : IRequest<PolicyRuleCondListDto?> { public Guid Id { get; set; } }

public class PolicyRuleCondByPolicyIdHandler : IRequestHandler<PolicyRuleCondByRuleIdQry, List<PolicyRuleCondListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public PolicyRuleCondByPolicyIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PolicyRuleCondListDto>> Handle(PolicyRuleCondByRuleIdQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<PolicyRuleCondition>().Find(c => c.PolicyAssignmentRuleId == request.Id)).ToList();
        var dataL = new List<PolicyRuleCondListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var rule = await _unitOfWork.Repository<PolicyAssignmentRule>().GetById(request.Id);

        foreach (var data in dbData)
        {
            var c = new PolicyRuleCondListDto
            {
                Id = data.Id,
                Field = data.Field,
                Operator = data.Operator,
                Value = data.Value,
                FieldStr = ((ConditionField)Enum.Parse(typeof(ConditionField), data.Field)).ToDisplayName(),
                OperatorStr = ((ConditionOperator)Enum.Parse(typeof(ConditionOperator), data.Operator)).ToDisplayName(),
                RuleName = rule != null ? rule.Name : "NOT AVAILABLE",
                IsDeleted = data.IsDeleted,
                DateAdd = data.DateAdd,
                DateMod = data.DateMod,
                RowVersion = Convert.ToBase64String(data.RowVersion)
            };
            dataL.Add(c);
        }

        return dataL;
    }
}

public class PolicyRuleCondByIdHandler : IRequestHandler<PolicyRuleCondByIdQry, PolicyRuleCondListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PolicyRuleCondByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PolicyRuleCondListDto?> Handle(PolicyRuleCondByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PolicyRuleCondition>().GetById(request.Id);
        if (data == null) { return null; }
        var rule = await _unitOfWork.Repository<PolicyAssignmentRule>().GetById(data.PolicyAssignmentRuleId);

        var c = new PolicyRuleCondListDto
        {
            Id = data.Id,
            Field = data.Field,
            Operator = data.Operator,
            Value = data.Value,
            FieldStr = ((ConditionField)Enum.Parse(typeof(ConditionField), data.Field)).ToDisplayName(),
            OperatorStr = ((ConditionOperator)Enum.Parse(typeof(ConditionOperator), data.Operator)).ToDisplayName(),
            RuleName = rule != null ? rule.Name : "NOT AVAILABLE",
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}