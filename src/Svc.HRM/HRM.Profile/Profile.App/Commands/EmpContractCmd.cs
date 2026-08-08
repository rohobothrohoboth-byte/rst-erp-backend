using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpContractAddCmd : IRequest<EmpContractListDto> { public EmpContractAddDto Dto { get; set; } = default!; }
public class EmpContractModCmd : IRequest<EmpContractListDto> { public EmpContractModDto Dto { get; set; } = default!; }
public class EmpContractActivateCmd : IRequest<EmpContractListDto> { public Guid Id { get; set; } }
public class EmpContractTerminateCmd : IRequest<EmpContractListDto> { public EmpContractTerminateDto Dto { get; set; } = default!; }
public class EmpContractRenewCmd : IRequest<EmpContractListDto> { public EmpContractRenewDto Dto { get; set; } = default!; }
public class EmpContractDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpContractAddHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpContractAddCmd, EmpContractListDto>
{
    public async Task<EmpContractListDto> Handle(EmpContractAddCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Dto.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            if (request.Dto.EndDate.HasValue && request.Dto.EndDate.Value.Date < request.Dto.StartDate.Date)
                throw new ValException("End date must be on or after start date.");

            var status = request.Dto.ActivateImmediately
                ? BoolToStr.EnumToString(ContractStatus.Active)
                : BoolToStr.EnumToString(ContractStatus.Draft);

            if (request.Dto.ActivateImmediately)
            {
                var actives = await uow.Set<EmpContract>()
                    .Where(x => x.EmployeeId == emp.Id && !x.IsDeleted && x.Status == BoolToStr.EnumToString(ContractStatus.Active))
                    .ToListAsync(ct);
                foreach (var a in actives)
                {
                    a.Status = BoolToStr.EnumToString(ContractStatus.Expired);
                    a.DateMod = DateTime.UtcNow;
                    await uow.Update(a);
                }
            }

            var entity = new EmpContract
            {
                Id = Guid.CreateVersion7(),
                ContractNumber = await GenerateNumber(ct),
                Status = status,
                ContractType = request.Dto.ContractType.Trim(),
                StartDate = DateTime.SpecifyKind(request.Dto.StartDate, DateTimeKind.Utc),
                EndDate = request.Dto.EndDate.HasValue ? DateTime.SpecifyKind(request.Dto.EndDate.Value, DateTimeKind.Utc) : null,
                SignedDate = request.Dto.SignedDate.HasValue ? DateTime.SpecifyKind(request.Dto.SignedDate.Value, DateTimeKind.Utc) : null,
                DocumentRef = request.Dto.DocumentRef?.Trim(),
                Notes = request.Dto.Notes?.Trim(),
                EmployeeId = emp.Id,
                DateAdd = DateTime.UtcNow
            };

            await uow.Add(entity, ct);
            await uow.Commit(ct);

            return await med.Send(new EmpContractByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Failed to load created contract.");
        }
        catch { await uow.Rollback(ct); throw; }
    }

    private async Task<string> GenerateNumber(CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"CTR-{year}-";
        var count = await uow.Set<EmpContract>().CountAsync(x => x.ContractNumber.StartsWith(prefix), ct);
        return $"{prefix}{(count + 1):D4}";
    }
}

public class EmpContractModHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpContractModCmd, EmpContractListDto>
{
    public async Task<EmpContractListDto> Handle(EmpContractModCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpContract>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Contract not found.", 404);

            EnsureRowVersion(entity, request.Dto.RowVersion);
            var status = MyEnumHelper.TryParseEnum<ContractStatus>(entity.Status);
            if (status is ContractStatus.Terminated or ContractStatus.Renewed)
                throw new DomainException("Terminated/Renewed contracts cannot be modified.");

            entity.ContractType = request.Dto.ContractType.Trim();
            entity.StartDate = DateTime.SpecifyKind(request.Dto.StartDate, DateTimeKind.Utc);
            entity.EndDate = request.Dto.EndDate.HasValue ? DateTime.SpecifyKind(request.Dto.EndDate.Value, DateTimeKind.Utc) : null;
            entity.SignedDate = request.Dto.SignedDate.HasValue ? DateTime.SpecifyKind(request.Dto.SignedDate.Value, DateTimeKind.Utc) : null;
            entity.DocumentRef = request.Dto.DocumentRef?.Trim();
            entity.Notes = request.Dto.Notes?.Trim();
            entity.DateMod = DateTime.UtcNow;

            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpContractByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Contract not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }

    private static void EnsureRowVersion(EmpContract entity, string rowVersion)
    {
        if (!string.IsNullOrEmpty(rowVersion) && entity.xmin.ToString() != rowVersion)
            throw new DomainException("The record has been modified by another user. Please refresh and try again.");
    }
}

public class EmpContractActivateHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpContractActivateCmd, EmpContractListDto>
{
    public async Task<EmpContractListDto> Handle(EmpContractActivateCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpContract>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Contract not found.", 404);

            if (MyEnumHelper.TryParseEnum<ContractStatus>(entity.Status) != ContractStatus.Draft)
                throw new DomainException("Only Draft contracts can be activated.");

            var actives = await uow.Set<EmpContract>()
                .Where(x => x.EmployeeId == entity.EmployeeId && x.Id != entity.Id && !x.IsDeleted
                            && x.Status == BoolToStr.EnumToString(ContractStatus.Active))
                .ToListAsync(ct);
            foreach (var a in actives)
            {
                a.Status = BoolToStr.EnumToString(ContractStatus.Expired);
                a.DateMod = DateTime.UtcNow;
                await uow.Update(a);
            }

            entity.Status = BoolToStr.EnumToString(ContractStatus.Active);
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);

            return await med.Send(new EmpContractByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Contract not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpContractTerminateHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpContractTerminateCmd, EmpContractListDto>
{
    public async Task<EmpContractListDto> Handle(EmpContractTerminateCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpContract>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Contract not found.", 404);

            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            entity.Status = BoolToStr.EnumToString(ContractStatus.Terminated);
            entity.TerminatedDate = DateTime.SpecifyKind(request.Dto.TerminatedDate ?? DateTime.UtcNow, DateTimeKind.Utc);
            entity.TerminationReason = request.Dto.Reason.Trim();
            entity.DateMod = DateTime.UtcNow;

            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpContractByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Contract not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpContractRenewHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpContractRenewCmd, EmpContractListDto>
{
    public async Task<EmpContractListDto> Handle(EmpContractRenewCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var old = await uow.Set<EmpContract>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Contract not found.", 404);

            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && old.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            old.Status = BoolToStr.EnumToString(ContractStatus.Renewed);
            old.DateMod = DateTime.UtcNow;
            await uow.Update(old);

            var year = DateTime.UtcNow.Year;
            var prefix = $"CTR-{year}-";
            var count = await uow.Set<EmpContract>().CountAsync(x => x.ContractNumber.StartsWith(prefix), ct);

            var renewed = new EmpContract
            {
                Id = Guid.CreateVersion7(),
                ContractNumber = $"{prefix}{(count + 1):D4}",
                Status = BoolToStr.EnumToString(ContractStatus.Active),
                ContractType = old.ContractType,
                StartDate = DateTime.SpecifyKind(request.Dto.StartDate, DateTimeKind.Utc),
                EndDate = request.Dto.EndDate.HasValue ? DateTime.SpecifyKind(request.Dto.EndDate.Value, DateTimeKind.Utc) : null,
                SignedDate = DateTime.UtcNow,
                DocumentRef = request.Dto.DocumentRef?.Trim() ?? old.DocumentRef,
                Notes = request.Dto.Notes?.Trim(),
                EmployeeId = old.EmployeeId,
                RenewedFromId = old.Id,
                DateAdd = DateTime.UtcNow
            };
            await uow.Add(renewed, ct);
            await uow.Commit(ct);

            return await med.Send(new EmpContractByIdQry { Id = renewed.Id }, ct)
                ?? throw new DomainException("Failed to load renewed contract.");
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpContractDelHandler(IUnitOfWork uow) : IRequestHandler<EmpContractDelCmd>
{
    public async Task Handle(EmpContractDelCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpContract>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Contract not found.", 404);

            if (MyEnumHelper.TryParseEnum<ContractStatus>(entity.Status) == ContractStatus.Active)
                throw new DomainException("Active contracts cannot be deleted. Terminate or renew instead.");

            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}
