using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class PolicyAssignmentRuleAddCmd : IRequest<PolicyAssignmentRuleListDto> { public PolicyAssignmentRuleAddDto AddDto { get; set; } = default!; }
public class PolicyAssignmentRuleModCmd : IRequest<PolicyAssignmentRuleListDto> { public PolicyAssignmentRuleModDto ModDto { get; set; } = default!; }
public class PolicyAssignmentRuleStatCmd : IRequest<PolicyAssignmentRuleListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class PolicyAssignmentRuleDelCmd : IRequest { public Guid Id { get; set; } }



public class PolicyAssignmentRuleAddHandler : IRequestHandler<PolicyAssignmentRuleAddCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyAssignmentRuleAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = new PolicyAssignmentRuleListDto();
            var lvPo = await _uow.Set<LeavePolicy>().FirstOrDefaultAsync(x => x.Id == request.AddDto.LeavePolicyId);
            if (lvPo == null) { return res; }

            var data = new PolicyAssignmentRule
            {
                Code = request.AddDto.Code,
                Name = request.AddDto.Name,
                Priority = request.AddDto.Priority,
                IsActive = true,
                EffectiveFrom = request.AddDto.EffectiveFrom,
                EffectiveTo = request.AddDto.EffectiveTo,
                LeavePolicyId = request.AddDto.LeavePolicyId,
                LeaveTypeId = lvPo.LeaveTypeId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);

            var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = data.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PolicyAssignmentRuleModHandler : IRequestHandler<PolicyAssignmentRuleModCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyAssignmentRuleModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PolicyAssignmentRule>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id, ct);
            if (oldData == null) { throw new DomainException($"POLICY ASSIGNMENT RULE with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Code = request.ModDto.Code;
            oldData.Name = request.ModDto.Name;
            oldData.Priority = request.ModDto.Priority;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.EffectiveTo = request.ModDto.EffectiveTo;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.EffectiveTo = request.ModDto.EffectiveTo;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PolicyAssignmentRuleListDto();
            var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PolicyAssignmentRuleStatHandler : IRequestHandler<PolicyAssignmentRuleStatCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyAssignmentRuleStatHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleStatCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PolicyAssignmentRule>().FirstOrDefaultAsync(x => x.Id == request.StatDto.Id, ct);
            if (oldData == null) { throw new DomainException($"POLICY ASSIGNMENT RULE with Id {request.StatDto.Id} NOT FOUND."); }

            oldData.IsActive = request.StatDto.Stat;
            oldData.SetRowVersion(uint.Parse(request.StatDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PolicyAssignmentRuleListDto();
            var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = request.StatDto.Id }, ct);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PolicyAssignmentRuleDelHandler : IRequestHandler<PolicyAssignmentRuleDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PolicyAssignmentRuleDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(PolicyAssignmentRuleDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PolicyAssignmentRule>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POLICY ASSIGNMENT RULE with id [{request.Id}] NOT FOUND."); }
            await _uow.Delete(data);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}