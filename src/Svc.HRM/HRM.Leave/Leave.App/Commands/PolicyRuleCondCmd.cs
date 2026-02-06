using Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class PolicyRuleCondAddCmd : IRequest<PolicyRuleCondListDto> { public PolicyRuleCondAddDto AddDto { get; set; } = default!; }
public class PolicyRuleCondModCmd : IRequest<PolicyRuleCondListDto> { public PolicyRuleCondModDto ModDto { get; set; } = default!; }
public class PolicyRuleCondDelCmd : IRequest { public Guid Id { get; set; } }

public class PolicyRuleCondAddCmdHandler : IRequestHandler<PolicyRuleCondAddCmd, PolicyRuleCondListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PolicyRuleCondAddCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PolicyRuleCondListDto> Handle(PolicyRuleCondAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var res = new PolicyRuleCondListDto();
            var added = await _unitOfWork.Repository<PolicyRuleCondition>().GetFoD(c => c.Field == request.AddDto.Field && c.PolicyAssignmentRuleId == request.AddDto.PolicyAssRuleId);
            if (added != null)
            {
                added.Operator = request.AddDto.Operator;
                added.Value = request.AddDto.Value;
                var aData = await _unitOfWork.Repository<PolicyRuleCondition>().Update(added);
                await _unitOfWork.Commit();
                res = await _med.Send(new PolicyRuleCondByIdQry { Id = added.Id }, cancellationToken);
            }

            var data = new PolicyRuleCondition
            {
                Field = request.AddDto.Field,
                Operator = request.AddDto.Operator,
                Value = request.AddDto.Value,
                PolicyAssignmentRuleId = request.AddDto.PolicyAssRuleId
            };
            await _unitOfWork.Repository<PolicyRuleCondition>().Add(data);
            await _unitOfWork.Commit();
            res = await _med.Send(new PolicyRuleCondByIdQry { Id = data.Id }, cancellationToken);
            return res!;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class PolicyRuleCondModCmdHandler : IRequestHandler<PolicyRuleCondModCmd, PolicyRuleCondListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PolicyRuleCondModCmdHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PolicyRuleCondListDto> Handle(PolicyRuleCondModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PolicyRuleCondition>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"POLICY ASSIGNMNET CONDITION with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Field = request.ModDto.Field;
            oldData.Operator = request.ModDto.Operator;
            oldData.Value = request.ModDto.Value;
            var data = await _unitOfWork.Repository<PolicyRuleCondition>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PolicyRuleCondListDto();
            var response = await _med.Send(new PolicyRuleCondByIdQry { Id = data.Id }, cancellationToken);
            if (response == null) { return res; }
            res = response;
            return res;
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}

public class PolicyRuleCondDelCmdHandler : IRequestHandler<PolicyRuleCondDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PolicyRuleCondDelCmdHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PolicyRuleCondDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PolicyRuleCondition>().GetById(request.Id);
            if (data == null) { throw new DomainException($"POLICY ASSIGNMNET CONDITION with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PolicyRuleCondition>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}