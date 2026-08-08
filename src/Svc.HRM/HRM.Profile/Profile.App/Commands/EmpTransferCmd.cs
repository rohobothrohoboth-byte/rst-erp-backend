using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpTransferAddCmd : IRequest<EmpTransferListDto> { public EmpTransferAddDto Dto { get; set; } = default!; }
public class EmpTransferApproveCmd : IRequest<EmpTransferListDto> { public EmpTransferDecisionDto Dto { get; set; } = default!; }
public class EmpTransferRejectCmd : IRequest<EmpTransferListDto> { public EmpTransferDecisionDto Dto { get; set; } = default!; }
public class EmpTransferApplyCmd : IRequest<EmpTransferListDto> { public EmpTransferDecisionDto Dto { get; set; } = default!; }
public class EmpTransferDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpTransferAddHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTransferAddCmd, EmpTransferListDto>
{
    public async Task<EmpTransferListDto> Handle(EmpTransferAddCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Dto.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            var entity = new EmpTransfer
            {
                Id = Guid.CreateVersion7(),
                Status = BoolToStr.EnumToString(HrChangeStatus.Pending),
                EffectiveDate = DateTime.SpecifyKind(request.Dto.EffectiveDate, DateTimeKind.Utc),
                Reason = request.Dto.Reason?.Trim(),
                EmployeeId = emp.Id,
                FromDepartmentId = emp.DepartmentId,
                FromPositionId = emp.PositionId,
                FromJobGradeId = emp.JobGradeId,
                ToDepartmentId = request.Dto.ToDepartmentId,
                ToPositionId = request.Dto.ToPositionId,
                ToJobGradeId = request.Dto.ToJobGradeId ?? emp.JobGradeId,
                DateAdd = DateTime.UtcNow
            };

            await uow.Add(entity, ct);
            await uow.Commit(ct);
            return await med.Send(new EmpTransferByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Failed to load created transfer.");
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpTransferApproveHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTransferApproveCmd, EmpTransferListDto>
{
    public async Task<EmpTransferListDto> Handle(EmpTransferApproveCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await LoadPending(uow, request.Dto, ct);
            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Approved);
            entity.ApprovedById = request.Dto.ApprovedById;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.Comments = request.Dto.Comments?.Trim();
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpTransferByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Transfer not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }

    internal static async Task<EmpTransfer> LoadPending(IUnitOfWork uow, EmpTransferDecisionDto dto, CancellationToken ct)
    {
        var entity = await uow.Set<EmpTransfer>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new DomainException("Transfer not found.", 404);
        if (!string.IsNullOrEmpty(dto.RowVersion) && entity.xmin.ToString() != dto.RowVersion)
            throw new DomainException("The record has been modified by another user. Please refresh and try again.");
        if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) != HrChangeStatus.Pending)
            throw new DomainException("Only Pending transfers can be approved/rejected.");
        return entity;
    }
}

public class EmpTransferRejectHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTransferRejectCmd, EmpTransferListDto>
{
    public async Task<EmpTransferListDto> Handle(EmpTransferRejectCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await EmpTransferApproveHandler.LoadPending(uow, request.Dto, ct);
            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Rejected);
            entity.ApprovedById = request.Dto.ApprovedById;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.Comments = request.Dto.Comments?.Trim();
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpTransferByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Transfer not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpTransferApplyHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTransferApplyCmd, EmpTransferListDto>
{
    public async Task<EmpTransferListDto> Handle(EmpTransferApplyCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpTransfer>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Transfer not found.", 404);

            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) != HrChangeStatus.Approved)
                throw new DomainException("Only Approved transfers can be applied.");

            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == entity.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            emp.DepartmentId = entity.ToDepartmentId;
            emp.PositionId = entity.ToPositionId;
            if (entity.ToJobGradeId.HasValue)
                emp.JobGradeId = entity.ToJobGradeId.Value;
            emp.DateMod = DateTime.UtcNow;
            await uow.Update(emp);

            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Applied);
            entity.AppliedDate = DateTime.UtcNow;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);

            return await med.Send(new EmpTransferByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Transfer not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpTransferDelHandler(IUnitOfWork uow) : IRequestHandler<EmpTransferDelCmd>
{
    public async Task Handle(EmpTransferDelCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpTransfer>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Transfer not found.", 404);
            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) == HrChangeStatus.Applied)
                throw new DomainException("Applied transfers cannot be deleted.");
            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}
