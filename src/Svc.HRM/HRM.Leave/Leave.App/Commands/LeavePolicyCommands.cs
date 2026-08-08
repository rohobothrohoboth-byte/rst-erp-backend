using Common;
using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.App.Services;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

// ==================== LEAVE POLICY COMMANDS ====================

public class LeavePolicyAddCmd : IRequest<LeavePolicyListDto>
{
    public LeavePolicyAddDto AddDto { get; set; } = default!;
}

public class LeavePolicyModCmd : IRequest<LeavePolicyListDto>
{
    public LeavePolicyModDto ModDto { get; set; } = default!;
}

public class LeavePolicyDelCmd : IRequest
{
    public Guid Id { get; set; }
}



public class LeavePolicyModHandler : IRequestHandler<LeavePolicyModCmd, LeavePolicyListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeavePolicyListDto> Handle(LeavePolicyModCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<LeavePolicy>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"LEAVE POLICY with Id {request.ModDto.Id} NOT FOUND.");

        var existing = await _uow.Set<LeavePolicy>()
            .FirstOrDefaultAsync(x => x.Code == request.ModDto.Code && x.Id != request.ModDto.Id, ct);

        if (existing != null)
            throw new DomainException($"Leave policy with code '{request.ModDto.Code}' already exists.");

        oldData.Name = request.ModDto.Name;
        oldData.Code = request.ModDto.Code;
        oldData.LeaveTypeId = request.ModDto.LeaveTypeId;
        oldData.AllowEncashment = request.ModDto.AllowEncashment;
        oldData.RequiresAttachment = request.ModDto.RequiresAttachment;
        oldData.Status = request.ModDto.Status;
        oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeavePolicyByIdQry { Id = request.ModDto.Id }, ct) ?? new LeavePolicyListDto();
    }
}

public class LeavePolicyDelHandler : IRequestHandler<LeavePolicyDelCmd>
{
    private readonly IUnitOfWork _uow;

    public LeavePolicyDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(LeavePolicyDelCmd request, CancellationToken ct)
    {
        var data = await _uow.Set<LeavePolicy>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (data == null)
            throw new DomainException($"LEAVE POLICY with id [{request.Id}] NOT FOUND.");

        await _uow.Delete(data);
        await _uow.SaveChangesAsync(ct);
    }
}

// ==================== LEAVE POLICY CONFIG COMMANDS ====================

public class LeavePolicyConfigAddCmd : IRequest<LeavePolicyConfigListDto>
{
    public LeavePolicyConfigAddDto AddDto { get; set; } = default!;
}

public class LeavePolicyConfigModCmd : IRequest<LeavePolicyConfigListDto>
{
    public LeavePolicyConfigModDto ModDto { get; set; } = default!;
}

public class LeavePolicyConfigDelCmd : IRequest
{
    public Guid Id { get; set; }
}



public class LeavePolicyConfigModHandler : IRequestHandler<LeavePolicyConfigModCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyConfigModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigModCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<LeavePolicyConfig>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"LEAVE POLICY CONFIG with Id {request.ModDto.Id} NOT FOUND.");

        oldData.AnnualEntitlement = request.ModDto.AnnualEntitlement;
        oldData.AccrualFrequency = request.ModDto.AccrualFrequency;
        oldData.AccrualRate = request.ModDto.AccrualRate;
        oldData.MaxDaysPerReq = request.ModDto.MaxDaysPerReq;
        oldData.MaxCarryOverDays = request.ModDto.MaxCarryOverDays;
        oldData.MinServiceMonths = request.ModDto.MinServiceMonths;
        oldData.IsActive = request.ModDto.IsActive;
        oldData.FiscalYearId = request.ModDto.FiscalYearId;
        oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeavePolicyConfigByIdQry { Id = request.ModDto.Id }, ct) ?? new LeavePolicyConfigListDto();
    }
}

public class LeavePolicyConfigDelHandler : IRequestHandler<LeavePolicyConfigDelCmd>
{
    private readonly IUnitOfWork _uow;

    public LeavePolicyConfigDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(LeavePolicyConfigDelCmd request, CancellationToken ct)
    {
        var data = await _uow.Set<LeavePolicyConfig>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (data == null)
            throw new DomainException($"LEAVE POLICY CONFIG with id [{request.Id}] NOT FOUND.");

        await _uow.Delete(data);
        await _uow.SaveChangesAsync(ct);
    }
}

// ==================== APPROVAL CHAIN COMMANDS ====================

public class LeaveAppChainAddCmd : IRequest<LeaveAppChainListDto>
{
    public LeaveAppChainAddDto AddDto { get; set; } = default!;
}

public class LeaveAppChainModCmd : IRequest<LeaveAppChainListDto>
{
    public LeaveAppChainModDto ModDto { get; set; } = default!;
}

public class LeaveAppChainDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class LeaveAppChainStatCmd : IRequest<LeaveAppChainListDto>
{
    public StatChangeDto StatDto { get; set; } = default!;
}

// Leave.App/Commands/LeavePolicyCommands.cs

public class LeaveAppChainAddHandler : IRequestHandler<LeaveAppChainAddCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppChainAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainAddCmd request, CancellationToken ct)
    {
        // Check if we have either LeavePolicyId or LeaveTypeId
        if (request.AddDto.LeavePolicyId == null && request.AddDto.LeaveTypeId == null)
            throw new DomainException("Either LeavePolicyId or LeaveTypeId is required");

        var data = new LeaveAppChain
        {
            Id = Guid.NewGuid(),
            LeavePolicyId = request.AddDto.LeavePolicyId,
            LeaveTypeId = request.AddDto.LeaveTypeId,  // Add this property
            EffectiveFrom = request.AddDto.EffectiveFrom,
            EffectiveTo = request.AddDto.EffectiveTo,
            IsActive = request.AddDto.IsActive,
            DateAdd = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Add(data, ct);
        await _uow.SaveChangesAsync(ct);

        // Create steps if provided
        if (request.AddDto.Steps != null && request.AddDto.Steps.Any())
        {
            foreach (var step in request.AddDto.Steps)
            {
                var approvalStep = new LeaveAppStep
                {
                    Id = Guid.NewGuid(),
                    LeaveAppChainId = data.Id,
                    StepOrder = step.StepOrder,
                    StepName = step.StepName,
                    Role = step.Role,
                    IsFinal = step.IsFinal,
                    EmployeeId = step.EmployeeId,
                    TimeoutHours = step.TimeoutHours,
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _uow.Add(approvalStep, ct);
            }
            await _uow.SaveChangesAsync(ct);
        }

        return await _med.Send(new LeaveAppChainByIdQry { Id = data.Id }, ct) ?? new LeaveAppChainListDto();
    }
}

public class LeaveAppChainModHandler : IRequestHandler<LeaveAppChainModCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppChainModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainModCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"LEAVE APP CHAIN with Id {request.ModDto.Id} NOT FOUND.");

        oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
        oldData.EffectiveTo = request.ModDto.EffectiveTo;
        oldData.IsActive = request.ModDto.IsActive;
        oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeaveAppChainByIdQry { Id = request.ModDto.Id }, ct) ?? new LeaveAppChainListDto();
    }
}

public class LeaveAppChainDelHandler : IRequestHandler<LeaveAppChainDelCmd>
{
    private readonly IUnitOfWork _uow;

    public LeaveAppChainDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(LeaveAppChainDelCmd request, CancellationToken ct)
    {
        var data = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (data == null)
            throw new DomainException($"LEAVE APP CHAIN with id [{request.Id}] NOT FOUND.");

        await _uow.Delete(data);
        await _uow.SaveChangesAsync(ct);
    }
}

public class LeaveAppChainStatHandler : IRequestHandler<LeaveAppChainStatCmd, LeaveAppChainListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppChainStatHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveAppChainListDto> Handle(LeaveAppChainStatCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<LeaveAppChain>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"LEAVE APP CHAIN with Id {request.StatDto.Id} NOT FOUND.");

        oldData.IsActive = request.StatDto.Stat;
        oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeaveAppChainByIdQry { Id = request.StatDto.Id }, ct) ?? new LeaveAppChainListDto();
    }
}

// ==================== APPROVAL STEP COMMANDS ====================

 // ==================== FIXED APPROVAL STEP HANDLER ====================



 // ==================== FIXED RULE CONDITION HANDLER ====================


public class LeaveAppStepModCmd : IRequest<LeaveAppStepListDto>
{
    public LeaveAppStepModDto ModDto { get; set; } = default!;
}

public class LeaveAppStepDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class LeaveAppStepModHandler : IRequestHandler<LeaveAppStepModCmd, LeaveAppStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppStepModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveAppStepListDto> Handle(LeaveAppStepModCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<LeaveAppStep>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"LEAVE APP STEP with Id {request.ModDto.Id} NOT FOUND.");

        oldData.StepName = request.ModDto.StepName;
        oldData.StepOrder = request.ModDto.StepOrder;
        oldData.Role = request.ModDto.Role;
        oldData.IsFinal = request.ModDto.IsFinal;
        oldData.EmployeeId = request.ModDto.EmployeeId;
        oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeaveAppStepByIdQry { Id = request.ModDto.Id }, ct) ?? new LeaveAppStepListDto();
    }
}

public class LeaveAppStepDelHandler : IRequestHandler<LeaveAppStepDelCmd>
{
    private readonly IUnitOfWork _uow;

    public LeaveAppStepDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(LeaveAppStepDelCmd request, CancellationToken ct)
    {
        var data = await _uow.Set<LeaveAppStep>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (data == null)
            throw new DomainException($"LEAVE APP STEP with id [{request.Id}] NOT FOUND.");

        await _uow.Delete(data);
        await _uow.SaveChangesAsync(ct);
    }
}

// ==================== ASSIGNMENT RULE COMMANDS ====================

public class PolicyAssignmentRuleAddCmd : IRequest<PolicyAssignmentRuleListDto>
{
    public PolicyAssignmentRuleAddDto AddDto { get; set; } = default!;
}

public class PolicyAssignmentRuleModCmd : IRequest<PolicyAssignmentRuleListDto>
{
    public PolicyAssignmentRuleModDto ModDto { get; set; } = default!;
}

public class PolicyAssignmentRuleDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class PolicyAssignmentRuleModHandler : IRequestHandler<PolicyAssignmentRuleModCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyAssignmentRuleModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleModCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<PolicyAssignmentRule>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"POLICY ASSIGNMENT RULE with Id {request.ModDto.Id} NOT FOUND.");

        oldData.Name = request.ModDto.Name;
        oldData.Code = request.ModDto.Code;
        oldData.Priority = request.ModDto.Priority;
        oldData.IsActive = request.ModDto.IsActive;
        oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
        oldData.EffectiveTo = request.ModDto.EffectiveTo;
        oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new PolicyAssignmentRuleByIdQry { Id = request.ModDto.Id }, ct) ?? new PolicyAssignmentRuleListDto();
    }
}

public class PolicyAssignmentRuleDelHandler : IRequestHandler<PolicyAssignmentRuleDelCmd>
{
    private readonly IUnitOfWork _uow;

    public PolicyAssignmentRuleDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(PolicyAssignmentRuleDelCmd request, CancellationToken ct)
    {
        var data = await _uow.Set<PolicyAssignmentRule>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (data == null)
            throw new DomainException($"POLICY ASSIGNMENT RULE with id [{request.Id}] NOT FOUND.");

        await _uow.Delete(data);
        await _uow.SaveChangesAsync(ct);
    }
}

// ==================== RULE CONDITION COMMANDS ====================

public class PolicyRuleCondAddCmd : IRequest<PolicyRuleCondListDto>
{
    public PolicyRuleCondAddDto AddDto { get; set; } = default!;
}

public class PolicyRuleCondModCmd : IRequest<PolicyRuleCondListDto>
{
    public PolicyRuleCondModDto ModDto { get; set; } = default!;
}

public class PolicyRuleCondDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class PolicyRuleCondModHandler : IRequestHandler<PolicyRuleCondModCmd, PolicyRuleCondListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyRuleCondModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<PolicyRuleCondListDto> Handle(PolicyRuleCondModCmd request, CancellationToken ct)
    {
        var oldData = await _uow.Set<PolicyRuleCondition>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
        if (oldData == null)
            throw new DomainException($"POLICY RULE CONDITION with Id {request.ModDto.Id} NOT FOUND.");

        oldData.Field = request.ModDto.Field;
        oldData.Operator = request.ModDto.Operator;
        oldData.Value = request.ModDto.Value;
        oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));

        await _uow.Update(oldData);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new PolicyRuleCondByIdQry { Id = request.ModDto.Id }, ct) ?? new PolicyRuleCondListDto();
    }
}

public class PolicyRuleCondDelHandler : IRequestHandler<PolicyRuleCondDelCmd>
{
    private readonly IUnitOfWork _uow;

    public PolicyRuleCondDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(PolicyRuleCondDelCmd request, CancellationToken ct)
    {
        var data = await _uow.Set<PolicyRuleCondition>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (data == null)
            throw new DomainException($"POLICY RULE CONDITION with id [{request.Id}] NOT FOUND.");

        await _uow.Delete(data);
        await _uow.SaveChangesAsync(ct);
    }
}




// ==================== FIXED LEAVE POLICY COMMANDS ====================

public class LeavePolicyAddHandler : IRequestHandler<LeavePolicyAddCmd, LeavePolicyListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeavePolicyListDto> Handle(LeavePolicyAddCmd request, CancellationToken ct)
    {
        var existing = await _uow.Set<LeavePolicy>()
            .FirstOrDefaultAsync(x => x.Code == request.AddDto.Code, ct);

        if (existing != null)
            throw new DomainException($"Leave policy with code '{request.AddDto.Code}' already exists.");

        var data = new LeavePolicy
        {
            Name = request.AddDto.Name,
            Code = request.AddDto.Code,
            LeaveTypeId = request.AddDto.LeaveTypeId,
            AllowEncashment = request.AddDto.AllowEncashment,
            RequiresAttachment = request.AddDto.RequiresAttachment,
            Status = "Active" // Default status if not in DTO
        };

        await _uow.Add(data, ct);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeavePolicyByIdQry { Id = data.Id }, ct) ?? new LeavePolicyListDto();
    }
}

// ==================== FIXED LEAVE POLICY CONFIG HANDLER ====================

public class LeavePolicyConfigAddHandler : IRequestHandler<LeavePolicyConfigAddCmd, LeavePolicyConfigListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeavePolicyConfigAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeavePolicyConfigListDto> Handle(LeavePolicyConfigAddCmd request, CancellationToken ct)
    {
        var data = new LeavePolicyConfig
        {
            LeavePolicyId = request.AddDto.LeavePolicyId,
            AnnualEntitlement = request.AddDto.AnnualEntitlement,
            AccrualFrequency = request.AddDto.AccrualFrequency,
            AccrualRate = request.AddDto.AccrualRate,
            MaxDaysPerReq = request.AddDto.MaxDaysPerReq,
            MaxCarryOverDays = request.AddDto.MaxCarryOverDays,
            MinServiceMonths = request.AddDto.MinServiceMonths,
            IsActive = true, // Default to active if not in DTO
            FiscalYearId = request.AddDto.FiscalYearId
        };

        await _uow.Add(data, ct);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeavePolicyConfigByIdQry { Id = data.Id }, ct) ?? new LeavePolicyConfigListDto();
    }
}

// ==================== FIXED APPROVAL STEP HANDLER ====================


// ==================== FIXED ASSIGNMENT RULE HANDLER ====================

public class PolicyAssignmentRuleAddHandler : IRequestHandler<PolicyAssignmentRuleAddCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyAssignmentRuleAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleAddCmd request, CancellationToken ct)
    {
        var data = new PolicyAssignmentRule
        {
            LeavePolicyId = request.AddDto.LeavePolicyId,
            Name = request.AddDto.Name,
            Code = request.AddDto.Code,
            Priority = request.AddDto.Priority,
            IsActive = true, // Default to active if not in DTO
            EffectiveFrom = request.AddDto.EffectiveFrom,
            EffectiveTo = request.AddDto.EffectiveTo
        };

        await _uow.Add(data, ct);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new PolicyAssignmentRuleByIdQry { Id = data.Id }, ct) ?? new PolicyAssignmentRuleListDto();
    }
}

// ==================== FIXED RULE CONDITION HANDLER ====================

public class LeaveAppStepAddCmd : IRequest<LeaveAppStepListDto>
{
    public LeaveAppStepAddDto AddDto { get; set; } = default!;
}
// ==================== FIXED APPROVAL STEP HANDLER ====================

// ==================== FIXED RULE CONDITION HANDLER ====================



// ==================== FIXED APPROVAL STEP HANDLER (Flexible version) ====================
// Leave.App/Commands/LeavePolicyCommands.cs
public class LeaveAppStepAddHandler : IRequestHandler<LeaveAppStepAddCmd, LeaveAppStepListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public LeaveAppStepAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<LeaveAppStepListDto> Handle(LeaveAppStepAddCmd request, CancellationToken ct)
    {
        // Validate Chain ID
        if (request.AddDto.LeaveAppChainId == Guid.Empty)
        {
            throw new DomainException("LeaveAppChainId is required.");
        }

        // Verify the chain exists
        var chain = await _uow.Set<LeaveAppChain>()
            .FirstOrDefaultAsync(x => x.Id == request.AddDto.LeaveAppChainId && !x.IsDeleted, ct);

        if (chain == null)
        {
            throw new DomainException($"Approval chain with ID {request.AddDto.LeaveAppChainId} not found.");
        }

        var data = new LeaveAppStep
        {
            Id = Guid.NewGuid(),
            LeaveAppChainId = request.AddDto.LeaveAppChainId,
            StepName = request.AddDto.StepName,
            StepOrder = request.AddDto.StepOrder,
            Role = request.AddDto.Role,
            IsFinal = request.AddDto.IsFinal,
            EmployeeId = request.AddDto.EmployeeId,
            TimeoutHours = request.AddDto.TimeoutHours,
            DateAdd = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Add(data, ct);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new LeaveAppStepByIdQry { Id = data.Id }, ct) ?? new LeaveAppStepListDto();
    }
}

// ==================== FIXED RULE CONDITION HANDLER (Flexible version) ====================

public class PolicyRuleCondAddHandler : IRequestHandler<PolicyRuleCondAddCmd, PolicyRuleCondListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyRuleCondAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<PolicyRuleCondListDto> Handle(PolicyRuleCondAddCmd request, CancellationToken ct)
    {
        // Try to find the Rule ID property dynamically
        var dtoType = request.AddDto.GetType();
        var ruleIdProperty = dtoType.GetProperty("PolicyAssignmentRuleId") ??
                             dtoType.GetProperty("RuleId") ??
                             dtoType.GetProperty("AssignmentRuleId");

        if (ruleIdProperty == null)
            throw new DomainException("Unable to find Rule ID property in PolicyRuleCondAddDto");

    var ruleIdValue = ruleIdProperty.GetValue(request.AddDto);
    if (ruleIdValue == null)
        throw new DomainException("Rule ID cannot be null");
    var ruleId = (Guid)ruleIdValue;

        var data = new PolicyRuleCondition
        {
            PolicyAssignmentRuleId = ruleId,
            Field = request.AddDto.Field,
            Operator = request.AddDto.Operator,
            Value = request.AddDto.Value
        };

        await _uow.Add(data, ct);
        await _uow.SaveChangesAsync(ct);

        return await _med.Send(new PolicyRuleCondByIdQry { Id = data.Id }, ct) ?? new PolicyRuleCondListDto();
    }
}