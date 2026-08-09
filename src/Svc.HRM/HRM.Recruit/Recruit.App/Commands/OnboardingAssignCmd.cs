using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class OnboardingAssignAddCmd : IRequest<OnboardingAssignListDto>
{
    public OnboardingAssignAddDto AddDto { get; set; } = default!;
}

public class OnboardingAssignBulkAddCmd : IRequest<List<OnboardingAssignListDto>>
{
    public OnboardingAssignBulkAddDto AddDto { get; set; } = default!;
}

public class OnboardingAssignModCmd : IRequest<OnboardingAssignListDto>
{
    public OnboardingAssignModDto ModDto { get; set; } = default!;
}

public class OnboardingAssignCompleteCmd : IRequest<OnboardingAssignListDto>
{
    public OnboardingAssignCompleteDto Dto { get; set; } = default!;
}

public class OnboardingAssignDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class OnboardingAssignAddHandler : IRequestHandler<OnboardingAssignAddCmd, OnboardingAssignListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public OnboardingAssignAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<OnboardingAssignListDto> Handle(OnboardingAssignAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var task = await _uow.Set<OnboardingTask>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.OnboardingTaskId && !x.IsDeleted, ct)
                ?? throw new DomainException($"ONBOARDING TASK [{request.AddDto.OnboardingTaskId}] NOT FOUND.");

            var exists = await _uow.Set<OnboardingAssign>()
                .AnyAsync(x => x.EmployeeId == request.AddDto.EmployeeId
                               && x.OnboardingTaskId == task.Id
                               && !x.IsDeleted, ct);
            if (exists)
                throw new DomainException("This onboarding task is already assigned to the employee.");

            var entity = new OnboardingAssign
            {
                Id = Guid.CreateVersion7(),
                EmployeeId = request.AddDto.EmployeeId,
                OnboardingTaskId = task.Id,
                IsMandatory = request.AddDto.IsMandatory,
                Status = BoolToStr.EnumToString(OnboardingStatus.Pending),
                ScheduledDate = DateTime.SpecifyKind(request.AddDto.ScheduledDate, DateTimeKind.Utc),
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(entity, ct);
            await _uow.Commit(ct);

            return await _med.Send(new OnboardingAssignByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException("Failed to retrieve created ONBOARDING ASSIGN.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingAssignBulkAddHandler : IRequestHandler<OnboardingAssignBulkAddCmd, List<OnboardingAssignListDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public OnboardingAssignBulkAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<List<OnboardingAssignListDto>> Handle(OnboardingAssignBulkAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var tasks = await _uow.Set<OnboardingTask>()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SequenceOrder)
                .ToListAsync(ct);

            if (tasks.Count == 0)
                throw new DomainException("No onboarding tasks defined. Create tasks first.");

            var schedule = request.AddDto.ScheduledDate
                ?? DateTime.UtcNow.Date.AddDays(Math.Max(1, request.AddDto.DaysOffset));

            var createdIds = new List<Guid>();
            foreach (var task in tasks)
            {
                var exists = await _uow.Set<OnboardingAssign>()
                    .AnyAsync(x => x.EmployeeId == request.AddDto.EmployeeId
                                   && x.OnboardingTaskId == task.Id
                                   && !x.IsDeleted, ct);
                if (exists) continue;

                var entity = new OnboardingAssign
                {
                    Id = Guid.CreateVersion7(),
                    EmployeeId = request.AddDto.EmployeeId,
                    OnboardingTaskId = task.Id,
                    IsMandatory = request.AddDto.IsMandatory,
                    Status = BoolToStr.EnumToString(OnboardingStatus.Pending),
                    ScheduledDate = DateTime.SpecifyKind(schedule, DateTimeKind.Utc),
                    DateAdd = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _uow.Add(entity, ct);
                createdIds.Add(entity.Id);
            }

            await _uow.Commit(ct);

            var results = new List<OnboardingAssignListDto>();
            foreach (var id in createdIds)
            {
                var row = await _med.Send(new OnboardingAssignByIdQry { Id = id }, ct);
                if (row != null) results.Add(row);
            }
            return results;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingAssignModHandler : IRequestHandler<OnboardingAssignModCmd, OnboardingAssignListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public OnboardingAssignModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<OnboardingAssignListDto> Handle(OnboardingAssignModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingAssign>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"ONBOARDING ASSIGN [{request.ModDto.Id}] NOT FOUND.");

            if (!string.IsNullOrEmpty(request.ModDto.RowVersion) &&
                entity.xmin.ToString() != request.ModDto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            if (MyEnumHelper.TryParseEnum<OnboardingStatus>(request.ModDto.Status) is null)
                throw new ValException("Invalid onboarding status.");

            entity.IsMandatory = request.ModDto.IsMandatory;
            entity.Status = request.ModDto.Status;
            entity.ScheduledDate = DateTime.SpecifyKind(request.ModDto.ScheduledDate, DateTimeKind.Utc);
            entity.CompletedDate = request.ModDto.CompletedDate.HasValue
                ? DateTime.SpecifyKind(request.ModDto.CompletedDate.Value, DateTimeKind.Utc)
                : null;
            entity.VerifyById = request.ModDto.VerifyById;
            entity.DateMod = DateTime.UtcNow;

            await _uow.Update(entity);
            await _uow.Commit(ct);

            return await _med.Send(new OnboardingAssignByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException($"ONBOARDING ASSIGN [{entity.Id}] NOT FOUND.");
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingAssignCompleteHandler : IRequestHandler<OnboardingAssignCompleteCmd, OnboardingAssignListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public OnboardingAssignCompleteHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<OnboardingAssignListDto> Handle(OnboardingAssignCompleteCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingAssign>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"ONBOARDING ASSIGN [{request.Dto.Id}] NOT FOUND.");

            if (!string.IsNullOrEmpty(request.Dto.RowVersion) &&
                entity.xmin.ToString() != request.Dto.RowVersion)
                throw new DomainException("The record has been modified by another user. Please refresh and try again.");

            entity.Status = BoolToStr.EnumToString(OnboardingStatus.Completed);
            entity.CompletedDate = DateTime.UtcNow;
            entity.VerifyById = request.Dto.VerifyById;
            entity.DateMod = DateTime.UtcNow;

            await _uow.Update(entity);
            await _uow.Commit(ct);

            return await _med.Send(new OnboardingAssignByIdQry { Id = entity.Id }, ct)
                ?? throw new DomainException($"ONBOARDING ASSIGN [{entity.Id}] NOT FOUND.");
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

    public OnboardingAssignDelHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(OnboardingAssignDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingAssign>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct)
                ?? throw new DomainException($"ONBOARDING ASSIGN [{request.Id}] NOT FOUND.");

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
