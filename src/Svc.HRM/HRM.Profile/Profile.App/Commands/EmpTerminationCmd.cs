using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Profile.App.Interfaces;
using Profile.App.Queries;
using Profile.Domain.DTOs;
using Profile.Domain.Entities;

namespace Profile.App.Commands;

public class EmpTerminationAddCmd : IRequest<EmpTerminationListDto> { public EmpTerminationAddDto Dto { get; set; } = default!; }
public class EmpTerminationModCmd : IRequest<EmpTerminationListDto> { public EmpTerminationModDto Dto { get; set; } = default!; }
public class EmpTerminationApproveCmd : IRequest<EmpTerminationListDto> { public EmpTerminationDecisionDto Dto { get; set; } = default!; }
public class EmpTerminationRejectCmd : IRequest<EmpTerminationListDto> { public EmpTerminationDecisionDto Dto { get; set; } = default!; }
public class EmpTerminationApplyCmd : IRequest<EmpTerminationListDto> { public EmpTerminationDecisionDto Dto { get; set; } = default!; }
public class EmpTerminationDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpOffboardingTaskAddCmd : IRequest<EmpOffboardingTaskDto> { public EmpOffboardingTaskAddDto Dto { get; set; } = default!; }
public class EmpOffboardingTaskUpdateCmd : IRequest<EmpOffboardingTaskDto> { public EmpOffboardingTaskUpdateDto Dto { get; set; } = default!; }
public class EmpOffboardingTaskDelCmd : IRequest { public Guid Id { get; set; } }

public class EmpTerminationAddHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTerminationAddCmd, EmpTerminationListDto>
{
    public async Task<EmpTerminationListDto> Handle(EmpTerminationAddCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == request.Dto.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            var open = await uow.Set<EmpTermination>().AnyAsync(x =>
                x.EmployeeId == emp.Id &&
                !x.IsDeleted &&
                (x.Status == BoolToStr.EnumToString(HrChangeStatus.Pending) ||
                 x.Status == BoolToStr.EnumToString(HrChangeStatus.Approved)), ct);
            if (open)
                throw new DomainException("Employee already has an open termination request.");

            var entity = new EmpTermination
            {
                Id = Guid.CreateVersion7(),
                Status = BoolToStr.EnumToString(HrChangeStatus.Pending),
                TerminationType = string.IsNullOrWhiteSpace(request.Dto.TerminationType) ? "Voluntary" : request.Dto.TerminationType.Trim(),
                LastWorkingDate = DateTime.SpecifyKind(request.Dto.LastWorkingDate, DateTimeKind.Utc),
                NoticeDate = request.Dto.NoticeDate.HasValue
                    ? DateTime.SpecifyKind(request.Dto.NoticeDate.Value, DateTimeKind.Utc)
                    : null,
                Reason = request.Dto.Reason.Trim(),
                Comments = request.Dto.Comments?.Trim(),
                ExitInterviewNotes = request.Dto.ExitInterviewNotes?.Trim(),
                RequestFinalPay = request.Dto.RequestFinalPay,
                RequestLeaveSettlement = request.Dto.RequestLeaveSettlement,
                SettlementStatus = "Pending",
                EmployeeId = emp.Id,
                DateAdd = DateTime.UtcNow
            };

            await uow.Add(entity, ct);

            if (request.Dto.SeedDefaultChecklist)
            {
                foreach (var task in DefaultChecklist(entity.Id, entity.LastWorkingDate))
                    await uow.Add(task, ct);
            }

            await uow.Commit(ct);
            return await med.Send(new EmpTerminationByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Failed to load created termination.");
        }
        catch { await uow.Rollback(ct); throw; }
    }

    internal static List<EmpOffboardingTask> DefaultChecklist(Guid terminationId, DateTime lastWorkingDate) =>
    [
        Task(terminationId, "Asset", "Return laptop / workstation", 1, lastWorkingDate),
        Task(terminationId, "Asset", "Return access badge / keys", 2, lastWorkingDate),
        Task(terminationId, "Access", "Disable domain / email account", 3, lastWorkingDate),
        Task(terminationId, "Access", "Revoke ERP / application access", 4, lastWorkingDate),
        Task(terminationId, "Document", "Collect signed clearance form", 5, lastWorkingDate),
        Task(terminationId, "Document", "Archive personnel file updates", 6, lastWorkingDate),
        Task(terminationId, "Other", "Conduct exit interview notes", 7, lastWorkingDate)
    ];

    private static EmpOffboardingTask Task(Guid terminationId, string category, string title, int order, DateTime due) => new()
    {
        Id = Guid.CreateVersion7(),
        TerminationId = terminationId,
        Category = category,
        Title = title,
        Status = "Pending",
        SortOrder = order,
        DueDate = DateTime.SpecifyKind(due, DateTimeKind.Utc),
        DateAdd = DateTime.UtcNow
    };
}

public class EmpTerminationModHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTerminationModCmd, EmpTerminationListDto>
{
    public async Task<EmpTerminationListDto> Handle(EmpTerminationModCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpTermination>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Termination not found.", 404);
            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");
            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) is not (HrChangeStatus.Pending or HrChangeStatus.Rejected))
                throw new DomainException("Only Pending/Rejected terminations can be edited.");

            entity.LastWorkingDate = DateTime.SpecifyKind(request.Dto.LastWorkingDate, DateTimeKind.Utc);
            entity.NoticeDate = request.Dto.NoticeDate.HasValue
                ? DateTime.SpecifyKind(request.Dto.NoticeDate.Value, DateTimeKind.Utc)
                : null;
            entity.Reason = request.Dto.Reason.Trim();
            entity.TerminationType = request.Dto.TerminationType.Trim();
            entity.Comments = request.Dto.Comments?.Trim();
            entity.ExitInterviewNotes = request.Dto.ExitInterviewNotes?.Trim();
            entity.RequestFinalPay = request.Dto.RequestFinalPay;
            entity.RequestLeaveSettlement = request.Dto.RequestLeaveSettlement;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpTerminationByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Termination not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpTerminationApproveHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTerminationApproveCmd, EmpTerminationListDto>
{
    public async Task<EmpTerminationListDto> Handle(EmpTerminationApproveCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await LoadPending(uow, request.Dto, ct);
            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Approved);
            entity.ApprovedById = request.Dto.ApprovedById;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.Comments = request.Dto.Comments?.Trim() ?? entity.Comments;
            if (request.Dto.ExitInterviewNotes != null)
                entity.ExitInterviewNotes = request.Dto.ExitInterviewNotes.Trim();
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpTerminationByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Termination not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }

    internal static async Task<EmpTermination> LoadPending(IUnitOfWork uow, EmpTerminationDecisionDto dto, CancellationToken ct)
    {
        var entity = await uow.Set<EmpTermination>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new DomainException("Termination not found.", 404);
        if (!string.IsNullOrEmpty(dto.RowVersion) && entity.xmin.ToString() != dto.RowVersion)
            throw new DomainException("The record has been modified by another user. Please refresh and try again.");
        if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) != HrChangeStatus.Pending)
            throw new DomainException("Only Pending terminations can be approved/rejected.");
        return entity;
    }
}

public class EmpTerminationRejectHandler(IUnitOfWork uow, IMediator med) : IRequestHandler<EmpTerminationRejectCmd, EmpTerminationListDto>
{
    public async Task<EmpTerminationListDto> Handle(EmpTerminationRejectCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await EmpTerminationApproveHandler.LoadPending(uow, request.Dto, ct);
            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Rejected);
            entity.ApprovedById = request.Dto.ApprovedById;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.Comments = request.Dto.Comments?.Trim() ?? entity.Comments;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return await med.Send(new EmpTerminationByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Termination not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpTerminationApplyHandler(
    IUnitOfWork uow,
    IMediator med,
    IPayrollSettlementClient payrollClient,
    ILeaveSettlementClient leaveClient) : IRequestHandler<EmpTerminationApplyCmd, EmpTerminationListDto>
{
    public async Task<EmpTerminationListDto> Handle(EmpTerminationApplyCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpTermination>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Termination not found.", 404);

            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) != HrChangeStatus.Approved)
                throw new DomainException("Only Approved terminations can be applied.");

            var emp = await uow.Set<Employee>().FirstOrDefaultAsync(x => x.Id == entity.EmployeeId && !x.IsDeleted, ct)
                ?? throw new DomainException("Employee not found.", 404);

            emp.EmpState = BoolToStr.EnumToString(EmpState.Term);
            emp.DateMod = DateTime.UtcNow;
            await uow.Update(emp);

            var activeContracts = await uow.Set<EmpContract>()
                .Where(x => x.EmployeeId == emp.Id && !x.IsDeleted &&
                            x.Status == BoolToStr.EnumToString(ContractStatus.Active))
                .ToListAsync(ct);
            foreach (var contract in activeContracts)
            {
                contract.Status = BoolToStr.EnumToString(ContractStatus.Terminated);
                contract.TerminatedDate = entity.LastWorkingDate;
                contract.TerminationReason = entity.Reason;
                contract.DateMod = DateTime.UtcNow;
                await uow.Update(contract);
            }

            if (request.Dto.ExitInterviewNotes != null)
                entity.ExitInterviewNotes = request.Dto.ExitInterviewNotes.Trim();
            if (request.Dto.Comments != null)
                entity.Comments = request.Dto.Comments.Trim();

            var settlementNotes = new List<string>();
            entity.SettlementStatus = "Skipped";

            if (entity.RequestLeaveSettlement)
            {
                var from = new DateTime(entity.LastWorkingDate.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var leave = await leaveClient.GetUnpaidDaysAsync(entity.EmployeeId, from, entity.LastWorkingDate, ct);
                if (leave.Success)
                {
                    entity.LeaveUnpaidDaysSnapshot = leave.UnpaidDays;
                    settlementNotes.Add($"Leave unpaid days snapshot: {leave.UnpaidDays}");
                    entity.SettlementStatus = "Requested";
                }
                else
                {
                    settlementNotes.Add($"Leave settlement: {leave.Message}");
                    entity.SettlementStatus = "Failed";
                }
            }

            if (entity.RequestFinalPay)
            {
                var pay = await payrollClient.CreateFinalPayRunAsync(entity.EmployeeId, entity.LastWorkingDate, entity.Reason, ct);
                if (pay.Success)
                {
                    entity.SettlementPayrollRunId = pay.PayrollRunId;
                    settlementNotes.Add(pay.Message + (pay.PayrollRunId.HasValue ? $" RunId={pay.PayrollRunId}" : ""));
                    if (entity.SettlementStatus != "Failed")
                        entity.SettlementStatus = "Requested";
                }
                else
                {
                    settlementNotes.Add($"Final pay: {pay.Message}");
                    entity.SettlementStatus = "Failed";
                }
            }

            entity.SettlementNotes = string.Join(" | ", settlementNotes);
            entity.Status = BoolToStr.EnumToString(HrChangeStatus.Applied);
            entity.AppliedDate = DateTime.UtcNow;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);

            return await med.Send(new EmpTerminationByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Termination not found.", 404);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpTerminationDelHandler(IUnitOfWork uow) : IRequestHandler<EmpTerminationDelCmd>
{
    public async Task Handle(EmpTerminationDelCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpTermination>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Termination not found.", 404);
            if (MyEnumHelper.TryParseEnum<HrChangeStatus>(entity.Status) == HrChangeStatus.Applied)
                throw new DomainException("Applied terminations cannot be deleted.");
            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);

            var tasks = await uow.Set<EmpOffboardingTask>().Where(x => x.TerminationId == entity.Id && !x.IsDeleted).ToListAsync(ct);
            foreach (var task in tasks)
            {
                task.IsDeleted = true;
                task.DateMod = DateTime.UtcNow;
                await uow.Update(task);
            }

            await uow.Commit(ct);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpOffboardingTaskAddHandler(IUnitOfWork uow) : IRequestHandler<EmpOffboardingTaskAddCmd, EmpOffboardingTaskDto>
{
    public async Task<EmpOffboardingTaskDto> Handle(EmpOffboardingTaskAddCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            _ = await uow.Set<EmpTermination>().FirstOrDefaultAsync(x => x.Id == request.Dto.TerminationId && !x.IsDeleted, ct)
                ?? throw new DomainException("Termination not found.", 404);

            var entity = new EmpOffboardingTask
            {
                Id = Guid.CreateVersion7(),
                TerminationId = request.Dto.TerminationId,
                Category = request.Dto.Category.Trim(),
                Title = request.Dto.Title.Trim(),
                Status = "Pending",
                AssignedToId = request.Dto.AssignedToId,
                DueDate = request.Dto.DueDate.HasValue
                    ? DateTime.SpecifyKind(request.Dto.DueDate.Value, DateTimeKind.Utc)
                    : null,
                Notes = request.Dto.Notes?.Trim(),
                SortOrder = request.Dto.SortOrder,
                DateAdd = DateTime.UtcNow
            };
            await uow.Add(entity, ct);
            await uow.Commit(ct);
            return Map(entity);
        }
        catch { await uow.Rollback(ct); throw; }
    }

    internal static EmpOffboardingTaskDto Map(EmpOffboardingTask x) => new()
    {
        Id = x.Id,
        TerminationId = x.TerminationId,
        Category = x.Category,
        Title = x.Title,
        Status = x.Status,
        AssignedToId = x.AssignedToId,
        DueDate = x.DueDate,
        CompletedAt = x.CompletedAt,
        Notes = x.Notes,
        SortOrder = x.SortOrder,
        RowVersion = x.xmin.ToString()
    };
}

public class EmpOffboardingTaskUpdateHandler(IUnitOfWork uow) : IRequestHandler<EmpOffboardingTaskUpdateCmd, EmpOffboardingTaskDto>
{
    public async Task<EmpOffboardingTaskDto> Handle(EmpOffboardingTaskUpdateCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpOffboardingTask>().FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Offboarding task not found.", 404);
            if (!string.IsNullOrEmpty(request.Dto.RowVersion) && entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            entity.Status = request.Dto.Status.Trim();
            entity.AssignedToId = request.Dto.AssignedToId ?? entity.AssignedToId;
            entity.DueDate = request.Dto.DueDate.HasValue
                ? DateTime.SpecifyKind(request.Dto.DueDate.Value, DateTimeKind.Utc)
                : entity.DueDate;
            entity.Notes = request.Dto.Notes?.Trim() ?? entity.Notes;
            entity.CompletedAt = entity.Status is "Completed" or "Skipped" ? DateTime.UtcNow : null;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
            return EmpOffboardingTaskAddHandler.Map(entity);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}

public class EmpOffboardingTaskDelHandler(IUnitOfWork uow) : IRequestHandler<EmpOffboardingTaskDelCmd>
{
    public async Task Handle(EmpOffboardingTaskDelCmd request, CancellationToken ct)
    {
        await uow.Begin(ct);
        try
        {
            var entity = await uow.Set<EmpOffboardingTask>().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException("Offboarding task not found.", 404);
            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await uow.Update(entity);
            await uow.Commit(ct);
        }
        catch { await uow.Rollback(ct); throw; }
    }
}
