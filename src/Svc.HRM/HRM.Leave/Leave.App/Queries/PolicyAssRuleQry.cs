using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Leave.Domain.Enums;
using MediatR;

namespace Leave.App.Queries;

public class AssignmentRuleByPolicyIdQry : IRequest<List<PolicyAssignmentRuleListDto>> { public Guid Id { get; set; } }
public class PolicyAssignmentRuleByIdQry : IRequest<PolicyAssignmentRuleListDto?> { public Guid Id { get; set; } }
public class ActiveAssignmentRulesQry : IRequest<List<PolicyAssignmentRuleListDto>> { public Guid Id { get; set; } }

public class AssignmentRuleByPolicyIdHandler : IRequestHandler<AssignmentRuleByPolicyIdQry, List<PolicyAssignmentRuleListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public AssignmentRuleByPolicyIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PolicyAssignmentRuleListDto>> Handle(AssignmentRuleByPolicyIdQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<PolicyAssignmentRule>().Find(c => c.LeavePolicyId == request.Id)).ToList();
        var dataL = new List<PolicyAssignmentRuleListDto>();
        if (dbData.Count <= 0) { return dataL; }

        foreach (var data in dbData)
        {
            var c = new PolicyAssignmentRuleListDto
            {
                Id = data.Id,
                Code = data.Code,
                Name = data.Name,
                Priority = data.Priority,
                IsActive = data.IsActive,
                EffectiveFrom = data.EffectiveFrom,
                EffectiveTo = data.EffectiveTo,
                PriorityStr = ((Priority)Enum.Parse(typeof(Priority), data.Priority)).ToDisplayName(),
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
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

public class PolicyAssignmentRuleByIdHandler : IRequestHandler<PolicyAssignmentRuleByIdQry, PolicyAssignmentRuleListDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    public PolicyAssignmentRuleByIdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PolicyAssignmentRuleListDto?> Handle(PolicyAssignmentRuleByIdQry request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<PolicyAssignmentRule>().GetById(request.Id);
        if (data == null) { return null; }

        var c = new PolicyAssignmentRuleListDto
        {
            Id = data.Id,
            Code = data.Code,
            Name = data.Name,
            Priority = data.Priority,
            IsActive = data.IsActive,
            EffectiveFrom = data.EffectiveFrom,
            EffectiveTo = data.EffectiveTo,
            PriorityStr = ((Priority)Enum.Parse(typeof(Priority), data.Priority)).ToDisplayName(),
            IsActiveStr = BoolToStr.FormatStat(data.IsActive),
            IsDeleted = data.IsDeleted,
            DateAdd = data.DateAdd,
            DateMod = data.DateMod,
            RowVersion = Convert.ToBase64String(data.RowVersion)
        };
        return c;
    }
}

public class ActiveAssignmentRulesHandler : IRequestHandler<ActiveAssignmentRulesQry, List<PolicyAssignmentRuleListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public ActiveAssignmentRulesHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<List<PolicyAssignmentRuleListDto>> Handle(ActiveAssignmentRulesQry request, CancellationToken cancellationToken)
    {
        var dbData = (await _unitOfWork.Repository<PolicyAssignmentRule>().Find(c => c.IsActive == true && c.LeavePolicyId == request.Id)).ToList();
        var dataL = new List<PolicyAssignmentRuleListDto>();
        if (dbData.Count <= 0) { return dataL; }
        var lvPoL = await _unitOfWork.Repository<LeavePolicy>().GetAll();
        var lvTyL = await _unitOfWork.Repository<LeaveType>().GetAll();

        foreach (var data in dbData)
        {
            var c = new PolicyAssignmentRuleListDto
            {
                Id = data.Id,
                Code = data.Code,
                Name = data.Name,
                Priority = data.Priority,
                IsActive = data.IsActive,
                EffectiveFrom = data.EffectiveFrom,
                EffectiveTo = data.EffectiveTo,
                PriorityStr = ((Priority)Enum.Parse(typeof(Priority), data.Priority)).ToDisplayName(),
                IsActiveStr = BoolToStr.FormatStat(data.IsActive),
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