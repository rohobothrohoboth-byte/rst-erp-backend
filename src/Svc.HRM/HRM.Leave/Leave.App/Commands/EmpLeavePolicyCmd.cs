using Leave.App.Interfaces;
using Leave.App.Queries;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Helpers;
namespace Leave.App.Commands;

public class EmpLeavePolicyAddCmd : IRequest<EmpLeavePolicyListDto>
{
    public EmpLeavePolicyAddDto AddDto { get; set; } = default!;
}

public class EmpLeavePolicyBulkAddCmd : IRequest<List<EmpLeavePolicyListDto>>
{
    public BatchAssignDto BatchDto { get; set; } = default!;
}

public class EmpLeavePolicyModCmd : IRequest<EmpLeavePolicyListDto>
{
    public EmpLeavePolicyModDto ModDto { get; set; } = default!;
}

public class EmpLeavePolicyDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class EmpLeavePolicyByDeptCmd : IRequest<List<EmpLeavePolicyListDto>>
{
    public DepartmentAssignDto AssignDto { get; set; } = default!;
}

// ==================== ADD HANDLER CLASSES ====================

public class EmpLeavePolicyAddHandler : IRequestHandler<EmpLeavePolicyAddCmd, EmpLeavePolicyListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EmpLeavePolicyAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

 public async Task<EmpLeavePolicyListDto> Handle(EmpLeavePolicyAddCmd request, CancellationToken ct)
    {
        EmpLeavePolicy? existing = null;
        Guid newId = Guid.Empty;

        // Check if employee already has this leave policy
        existing = await _uow.Set<EmpLeavePolicy>()
            .FirstOrDefaultAsync(x => x.EmployeeId == request.AddDto.EmployeeId
                && x.LeaveTypeId == request.AddDto.LeaveTypeId
                && x.IsDeleted == false, ct);

        if (existing != null)
        {
            // Update existing assignment
            existing.AssignedEntitlement = request.AddDto.AssignedEntitlement;
            existing.EffectiveFrom = request.AddDto.EffectiveFrom;
            existing.EffectiveTo = request.AddDto.EffectiveTo;
            existing.AssignmentReason = request.AddDto.AssignmentReason;

            await _uow.Update(existing);
            await _uow.SaveChangesAsync(ct);
            newId = existing.Id;
        }
        else
        {
            // Create new assignment - DO NOT set LeavePolicyId if it's null or empty
            var data = new EmpLeavePolicy
            {
                EmployeeId = request.AddDto.EmployeeId,
                LeaveTypeId = request.AddDto.LeaveTypeId,
                AssignedEntitlement = request.AddDto.AssignedEntitlement,
                EffectiveFrom = request.AddDto.EffectiveFrom,
                EffectiveTo = request.AddDto.EffectiveTo,
                AssignmentReason = request.AddDto.AssignmentReason,
                UsedEntitlement = 0
            };

            // ONLY set LeavePolicyId if it has a valid value (not null and not empty)
            if (request.AddDto.LeavePolicyId.HasValue && request.AddDto.LeavePolicyId.Value != Guid.Empty)
            {
                data.LeavePolicyId = request.AddDto.LeavePolicyId.Value;
            }
            // Otherwise, LeavePolicyId will be null (which is now allowed after your SQL change)

            await _uow.Add(data, ct);
            await _uow.SaveChangesAsync(ct);
            newId = data.Id;
        }

        var result = await _med.Send(new EmpLeavePolicyByIdQry { Id = newId }, ct);
        return result ?? new EmpLeavePolicyListDto();
    }
}

public class EmpLeavePolicyBulkAddHandler : IRequestHandler<EmpLeavePolicyBulkAddCmd, List<EmpLeavePolicyListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EmpLeavePolicyBulkAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<List<EmpLeavePolicyListDto>> Handle(EmpLeavePolicyBulkAddCmd request, CancellationToken ct)
    {
        var results = new List<EmpLeavePolicyListDto>();

        foreach (var employeeId in request.BatchDto.EmployeeIds)
        {
            var addDto = new EmpLeavePolicyAddDto
            {
                EmployeeId = employeeId,
                LeaveTypeId = request.BatchDto.LeaveTypeId,
                LeavePolicyId = request.BatchDto.LeavePolicyId,
                AssignedEntitlement = request.BatchDto.AssignedEntitlement,
                EffectiveFrom = request.BatchDto.EffectiveFrom,
                EffectiveTo = request.BatchDto.EffectiveTo,
                AssignmentReason = request.BatchDto.AssignmentReason
            };

            var command = new EmpLeavePolicyAddCmd { AddDto = addDto };
            var result = await _med.Send(command, ct);
            results.Add(result);
        }

        return results;
    }
}

public class EmpLeavePolicyModHandler : IRequestHandler<EmpLeavePolicyModCmd, EmpLeavePolicyListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public EmpLeavePolicyModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<EmpLeavePolicyListDto> Handle(EmpLeavePolicyModCmd request, CancellationToken ct)
    {
        var existing = await _uow.Set<EmpLeavePolicy>()
            .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && x.IsDeleted == false, ct);

        if (existing == null)
            throw new DomainException($"Leave policy assignment with Id {request.ModDto.Id} not found");

        existing.AssignedEntitlement = request.ModDto.AssignedEntitlement;
        existing.EffectiveTo = request.ModDto.EffectiveTo;
        existing.IsActive = request.ModDto.IsActive;

        await _uow.Update(existing);
        await _uow.SaveChangesAsync(ct);

        var result = await _med.Send(new EmpLeavePolicyByIdQry { Id = existing.Id }, ct);
        return result ?? new EmpLeavePolicyListDto();
    }
}

public class EmpLeavePolicyDelHandler : IRequestHandler<EmpLeavePolicyDelCmd>
{
    private readonly IUnitOfWork _uow;

    public EmpLeavePolicyDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(EmpLeavePolicyDelCmd request, CancellationToken ct)
    {
        var existing = await _uow.Set<EmpLeavePolicy>()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsDeleted == false, ct);

        if (existing == null)
            throw new DomainException($"Leave policy assignment with Id {request.Id} not found");

        existing.IsDeleted = true;
        await _uow.Update(existing);
        await _uow.SaveChangesAsync(ct);
    }
}