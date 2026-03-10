using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Leave.App.Commands;

public class PolicyRuleCondAddCmd : IRequest<PolicyRuleCondListDto> { public PolicyRuleCondAddDto AddDto { get; set; } = default!; }
public class PolicyRuleCondModCmd : IRequest<PolicyRuleCondListDto> { public PolicyRuleCondModDto ModDto { get; set; } = default!; }
public class PolicyRuleCondDelCmd : IRequest { public Guid Id { get; set; } }



public class PolicyRuleCondAddCmdHandler : IRequestHandler<PolicyRuleCondAddCmd, PolicyRuleCondListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyRuleCondAddCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PolicyRuleCondListDto> Handle(PolicyRuleCondAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var res = new PolicyRuleCondListDto();
            var added = await _uow.Set<PolicyRuleCondition>().FirstOrDefaultAsync(c => c.Field == request.AddDto.Field && c.PolicyAssignmentRuleId == request.AddDto.PolicyAssRuleId, ct);
            if (added != null)
            {
                added.Operator = request.AddDto.Operator;
                added.Value = request.AddDto.Value;
                added.SetRowVersion(added.xmin);
                await _uow.Update(added);
                res = await _med.Send(new PolicyRuleCondByIdQry { Id = added.Id }, ct);
            }

            var data = new PolicyRuleCondition
            {
                Field = request.AddDto.Field,
                Operator = request.AddDto.Operator,
                Value = request.AddDto.Value,
                PolicyAssignmentRuleId = request.AddDto.PolicyAssRuleId
            };
            await _uow.Add(data, ct);
            await _uow.Commit(ct);
            res = await _med.Send(new PolicyRuleCondByIdQry { Id = data.Id }, ct);
            return res!;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class PolicyRuleCondModCmdHandler : IRequestHandler<PolicyRuleCondModCmd, PolicyRuleCondListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public PolicyRuleCondModCmdHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<PolicyRuleCondListDto> Handle(PolicyRuleCondModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var oldData = await _uow.Set<PolicyRuleCondition>().FirstOrDefaultAsync(x => x.Id == request.ModDto.Id);
            if (oldData == null) { throw new DomainException($"POLICY ASSIGNMNET CONDITION with Id {request.ModDto.Id} NOT FOUND."); }

            oldData.Field = request.ModDto.Field;
            oldData.Operator = request.ModDto.Operator;
            oldData.Value = request.ModDto.Value;
            oldData.SetRowVersion(uint.Parse(request.ModDto.RowVersion));
            await _uow.Update(oldData);
            await _uow.Commit(ct);

            var res = new PolicyRuleCondListDto();
            var response = await _med.Send(new PolicyRuleCondByIdQry { Id = request.ModDto.Id }, ct);
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

public class PolicyRuleCondDelCmdHandler : IRequestHandler<PolicyRuleCondDelCmd>
{
    private readonly IUnitOfWork _uow;
    public PolicyRuleCondDelCmdHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(PolicyRuleCondDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var data = await _uow.Set<PolicyRuleCondition>().FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (data == null) { throw new DomainException($"POLICY ASSIGNMNET CONDITION with id [{request.Id}] NOT FOUND."); }
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