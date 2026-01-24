using Leave.App.Helpers;
using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;

namespace Leave.App.Commands;

public class PolicyAssignmentRuleAddCmd : IRequest<PolicyAssignmentRuleListDto> { public PolicyAssignmentRuleAddDto AddDto { get; set; } = default!; }
public class PolicyAssignmentRuleModCmd : IRequest<PolicyAssignmentRuleListDto> { public PolicyAssignmentRuleModDto ModDto { get; set; } = default!; }
public class PolicyAssignmentRuleStatCmd : IRequest<PolicyAssignmentRuleListDto> { public StatChangeDto StatDto { get; set; } = default!; }
public class PolicyAssignmentRuleDelCmd : IRequest { public Guid Id { get; set; } }

public class PolicyAssignmentRuleAddHandler : IRequestHandler<PolicyAssignmentRuleAddCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PolicyAssignmentRuleAddHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleAddCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var res = new PolicyAssignmentRuleListDto();
            var lvPo = await _unitOfWork.Repository<LeavePolicy>().GetById(request.AddDto.LeavePolicyId);
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
            await _unitOfWork.Repository<PolicyAssignmentRule>().Add(data);
            await _unitOfWork.Commit();

            var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = data.Id }, cancellationToken);
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

public class PolicyAssignmentRuleModHandler : IRequestHandler<PolicyAssignmentRuleModCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PolicyAssignmentRuleModHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleModCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PolicyAssignmentRule>().GetById(request.ModDto.Id);
        if (oldData == null) { throw new DomainException($"POLICY ASSIGNMENT RULE with Id {request.ModDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.Code = request.ModDto.Code;
            oldData.Name = request.ModDto.Name;
            oldData.Priority = request.ModDto.Priority;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.EffectiveTo = request.ModDto.EffectiveTo;
            oldData.IsActive = request.ModDto.IsActive;
            oldData.EffectiveFrom = request.ModDto.EffectiveFrom;
            oldData.EffectiveTo = request.ModDto.EffectiveTo;
            var data = await _unitOfWork.Repository<PolicyAssignmentRule>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PolicyAssignmentRuleListDto();
            var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = data.Id }, cancellationToken);
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

public class PolicyAssignmentRuleStatHandler : IRequestHandler<PolicyAssignmentRuleStatCmd, PolicyAssignmentRuleListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _med;

    public PolicyAssignmentRuleStatHandler(IUnitOfWork unitOfWork, IMediator med) { _unitOfWork = unitOfWork; _med = med; }

    public async Task<PolicyAssignmentRuleListDto> Handle(PolicyAssignmentRuleStatCmd request, CancellationToken cancellationToken)
    {
        var oldData = await _unitOfWork.Repository<PolicyAssignmentRule>().GetById(request.StatDto.Id);
        if (oldData == null) { throw new DomainException($"POLICY ASSIGNMENT RULE with Id {request.StatDto.Id} NOT FOUND."); }

        await _unitOfWork.Begin();
        try
        {
            oldData.IsActive = request.StatDto.Stat;
            var data = await _unitOfWork.Repository<PolicyAssignmentRule>().Update(oldData);
            await _unitOfWork.Commit();

            var res = new PolicyAssignmentRuleListDto();
            var response = await _med.Send(new PolicyAssignmentRuleByIdQry { Id = data.Id }, cancellationToken);
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

public class PolicyAssignmentRuleDelHandler : IRequestHandler<PolicyAssignmentRuleDelCmd>
{
    private readonly IUnitOfWork _unitOfWork;
    public PolicyAssignmentRuleDelHandler(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task Handle(PolicyAssignmentRuleDelCmd request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Begin();
        try
        {
            var data = await _unitOfWork.Repository<PolicyAssignmentRule>().GetById(request.Id);
            if (data == null) { throw new DomainException($"POLICY ASSIGNMENT RULE with id [{request.Id}] NOT FOUND."); }
            await _unitOfWork.Repository<PolicyAssignmentRule>().Delete(request.Id);
            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
            throw;
        }
    }
}