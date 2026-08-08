using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpPromotionAddCmd : IRequest<EmpPromotionListDto> { public EmpPromotionAddDto Dto { get; set; } = default!; }
public class EmpPromotionApproveCmd : IRequest<EmpPromotionListDto> { public EmpPromotionDecisionDto Dto { get; set; } = default!; }
public class EmpPromotionRejectCmd : IRequest<EmpPromotionListDto> { public EmpPromotionDecisionDto Dto { get; set; } = default!; }
public class EmpPromotionApplyCmd : IRequest<EmpPromotionListDto> { public EmpPromotionDecisionDto Dto { get; set; } = default!; }
public class EmpPromotionDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpPromotionAddHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpPromotionAddCmd, EmpPromotionListDto>
{
    public async Task<EmpPromotionListDto> Handle(EmpPromotionAddCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Dto.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            var currentSalary = await uow.Set<EmpSalary>()
                .Where(x => x.EmployeeId == emp.Id && !x.IsDeleted && x.EffectiveTo == null)
                .OrderByDescending(x => x.EffectiveFrom)
                .FirstOrDefaultAsync(ct);

            var entity = new EmpPromotion
            {
                Id = Guid.CreateVersion7(),
                Status = BoolToStr.EnumToString(HrChangeStatus.Pending),
                EffectiveDate = DateTime.SpecifyKind(request.Dto.EffectiveDate, DateTimeKind.Utc),
                Reason = request.Dto.Reason?.Trim(),
                EmployeeId = emp.Id,
                FromJobGradeId = emp.JobGradeId,
                FromPositionId = emp.PositionId,
                FromDepartmentId = emp.DepartmentId,
                FromJgStepId = currentSalary?.JgStepId,
                ToJobGradeId = request.Dto.ToJobGradeId,
                ToPositionId = request.Dto.ToPositionId,
                ToDepartmentId = request.Dto.ToDepartmentId,
                ToJgStepId = request.Dto.ToJgStepId,
                DateAdd = DateTime.UtcNow
            };

            await uow.Add(entity, ct);
            await uow.Commit(ct);
            return await med.Send(new EmpPromotionByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Failed to load created promotion.");
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpPromotionApproveHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpPromotionApproveCmd, EmpPromotionListDto>
{
    public async Task<EmpPromotionListDto> Handle(EmpPromotionApproveCmd request, CancellationToken ct)
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
            return await med.Send(new EmpPromotionByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Promotion not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }

    internal static async Task<EmpPromotion> LoadPending(IUnitOfWork uow, EmpPromotionDecisionDto dto, CancellationToken ct)
    {
        var entity = await uow.Set<EmpPromotion>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new DomainException("Promotion not found.", 404);
        if (!string.IsNullOrEmpty(dto.RowVersion) && entity.xmin.ToString() != dto.RowVersion)
            throw new DomainException("The record has been modified by another user. Please refresh and try again.");
        if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) != HrChangeStatus.Pending)
            throw new DomainException("Only Pending promotions can be approved/rejected.");
        return entity;
    }
}

public class EmpPromotionRejectHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpPromotionRejectCmd, EmpPromotionListDto>
{
    public async Task<EmpPromotionListDto> Handle(EmpPromotionRejectCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await EmpPromotionApproveHandler.LoadPending(uow, request.Dto, ct);
            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Rejected);
            entity.ApprovedById = request.Dto.ApprovedById;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.Comments = request.Dto.Comments?.Trim();
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpPromotionByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Promotion not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpPromotionApplyHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpPromotionApplyCmd, EmpPromotionListDto>
{
    public async Task<EmpPromotionListDto> Handle(EmpPromotionApplyCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpPromotion>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Promotion not found.", 404);

            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) != HrChangeStatus.Approved)
                throw new DomainException("Only Approved promotions can be applied.");

            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == entity.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            emp.JobGradeId = entity.ToJobGradeId;
            emp.PositionId = entity.ToPositionId;
            emp.DepartmentId = entity.ToDepartmentId;
            emp.DateMod = DateTime.UtcNow;
            await uow.Update(emp);

            if (entity.ToJgStepId.HasValue)
            {
                var current = await uow.Set<EmpSalary>()
                    .Where(x => x.EmployeeId == emp.Id && !x.IsDeleted && x.EffectiveTo == null)
                    .OrderByDescending(x => x.EffectiveFrom)
                    .FirstOrDefaultAsync(ct);

                if (current != null)
                {
                    current.EffectiveTo = entity.EffectiveDate;
                    current.DateMod = DateTime.UtcNow;
                    await uow.Update(current);

                    await uow.Add(new EmpSalary
                    {
                        Id = Guid.CreateVersion7(),
                        EmployeeId = emp.Id,
                        JgStepId = entity.ToJgStepId.Value,
                        BaseSalary = current.BaseSalary,
                        Currency = current.Currency,
                        SalaryPayFreq = current.SalaryPayFreq,
                        EffectiveFrom = entity.EffectiveDate,
                        DateAdd = DateTime.UtcNow
                    }, ct);
                }
            }

            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Applied);
            entity.AppliedDate = DateTime.UtcNow;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);

            return await med.Send(new EmpPromotionByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Promotion not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpPromotionDelHandler(IUnitOfWork uow) : IRequestHandler<EmpPromotionDelCmd>
{
    public async Task Handle(EmpPromotionDelCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpPromotion>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Promotion not found.", 404);
            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) == HrChangeStatus.Applied)
                throw new DomainException("Applied promotions cannot be deleted.");
            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}
