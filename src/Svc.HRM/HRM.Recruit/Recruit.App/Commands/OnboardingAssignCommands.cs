using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class OnboardingAssignAddCmd : IRequest<OnboardingAssignmentListDto> { public OnboardingAssignmentAddDto AddDto { get; set; } = default!; }
public class OnboardingAssignModCmd : IRequest<OnboardingAssignmentListDto> { public OnboardingAssignmentModDto ModDto { get; set; } = default!; }
public class OnboardingAssignStatusCmd : IRequest<OnboardingAssignmentListDto> { public Guid Id { get; set; } public string Status { get; set; } = default!; }
public class OnboardingAssignDelCmd : IRequest { public Guid Id { get; set; } }

public class OnboardingAssignAddHandler : IRequestHandler<OnboardingAssignAddCmd, OnboardingAssignmentListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public OnboardingAssignAddHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<OnboardingAssignmentListDto> Handle(OnboardingAssignAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var task = await _uow.Set<OnboardingTask>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.TaskId && !x.IsDeleted, ct);
            if (task == null) { throw new DomainException($"ONBOARDING TASK with id [{request.AddDto.TaskId}] NOT FOUND."); }

            var entity = new OnboardingAssign
            {
                Id = Guid.CreateVersion7(),
                EmployeeId = request.AddDto.EmployeeId,
                OnboardingTaskId = request.AddDto.TaskId,
                ScheduledDate = request.AddDto.ScheduledDate,
                IsMandatory = request.AddDto.IsMandatory,
                Status = BoolToStr.EnumToString(OnboardingStatus.Pending),
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false,
            };

            await _uow.Add(entity, ct);
            await _uow.Commit(ct);

            var res = await _med.Send(new OnboardingAssignByIdQry { Id = entity.Id }, ct);
            if (res == null) { throw new DomainException("Failed to retrieve created ONBOARDING ASSIGNMENT."); }
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingAssignModHandler : IRequestHandler<OnboardingAssignModCmd, OnboardingAssignmentListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public OnboardingAssignModHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<OnboardingAssignmentListDto> Handle(OnboardingAssignModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingAssign>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct);
            if (entity == null) { throw new DomainException($"ONBOARDING ASSIGNMENT with id [{request.ModDto.Id}] NOT FOUND."); }

            if (!string.IsNullOrEmpty(request.ModDto.RowVersion) && entity.xmin.ToString() != request.ModDto.RowVersion)
            {
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");
            }

            if (request.ModDto.ScheduledDate.HasValue) { entity.ScheduledDate = request.ModDto.ScheduledDate.Value; }
            if (request.ModDto.IsMandatory.HasValue) { entity.IsMandatory = request.ModDto.IsMandatory.Value; }
            if (!string.IsNullOrWhiteSpace(request.ModDto.Status))
            {
                entity.Status = request.ModDto.Status;
                entity.CompletedDate = IsCompleted(request.ModDto.Status) ? DateTime.UtcNow : null;
            }
            entity.DateMod = DateTime.UtcNow;

            await _uow.Update(entity);
            await _uow.Commit(ct);

            var res = await _med.Send(new OnboardingAssignByIdQry { Id = entity.Id }, ct);
            if (res == null) { throw new DomainException($"ONBOARDING ASSIGNMENT with id [{entity.Id}] NOT FOUND."); }
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    internal static bool IsCompleted(string status) =>
        status.Equals("Completed", StringComparison.OrdinalIgnoreCase);
}

public class OnboardingAssignStatusHandler : IRequestHandler<OnboardingAssignStatusCmd, OnboardingAssignmentListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;
    public OnboardingAssignStatusHandler(IUnitOfWork uow, IMediator med) { _uow = uow; _med = med; }

    public async Task<OnboardingAssignmentListDto> Handle(OnboardingAssignStatusCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingAssign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
            if (entity == null) { throw new DomainException($"ONBOARDING ASSIGNMENT with id [{request.Id}] NOT FOUND."); }

            entity.Status = request.Status;
            entity.CompletedDate = OnboardingAssignModHandler.IsCompleted(request.Status) ? DateTime.UtcNow : null;
            entity.DateMod = DateTime.UtcNow;

            await _uow.Update(entity);
            await _uow.Commit(ct);

            var res = await _med.Send(new OnboardingAssignByIdQry { Id = entity.Id }, ct);
            if (res == null) { throw new DomainException($"ONBOARDING ASSIGNMENT with id [{entity.Id}] NOT FOUND."); }
            return res;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingAssignDelHandler : IRequestHandler<OnboardingAssignDelCmd>
{
    private readonly IUnitOfWork _uow;
    public OnboardingAssignDelHandler(IUnitOfWork uow) { _uow = uow; }

    public async Task Handle(OnboardingAssignDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingAssign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
            if (entity == null) { throw new DomainException($"ONBOARDING ASSIGNMENT with id [{request.Id}] NOT FOUND."); }

            entity.IsDeleted = true;
            entity.DateMod = DateTime.UtcNow;
            await _uow.Update(entity);
            await _uow.Commit(ct);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}
