// Recruit.App/Commands/OnboardingTaskCommands.cs
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

public class OnboardingTaskAddCmd : IRequest<OnboardingTaskListDto>
{
    public OnboardingTaskAddDto AddDto { get; set; } = default!;
}

public class OnboardingTaskModCmd : IRequest<OnboardingTaskListDto>
{
    public OnboardingTaskModDto ModDto { get; set; } = default!;
}

public class OnboardingTaskDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ----- Handlers -----

public class OnboardingTaskAddHandler : IRequestHandler<OnboardingTaskAddCmd, OnboardingTaskListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public OnboardingTaskAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<OnboardingTaskListDto> Handle(OnboardingTaskAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = new OnboardingTask
            {
                Id = Guid.CreateVersion7(),
                TaskName = request.AddDto.TaskName.Trim(),
                Description = request.AddDto.Description.Trim(),
                SequenceOrder = request.AddDto.SequenceOrder,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };

            await _uow.Add(entity, ct);
            await _uow.Commit(ct);

            // Fetch the created record with xmin
            var response = await _med.Send(new OnboardingTaskByIdQry { Id = entity.Id }, ct);
            if (response == null)
                throw new DomainException("Failed to retrieve created ONBOARDING TASK.");

            return response;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingTaskModHandler : IRequestHandler<OnboardingTaskModCmd, OnboardingTaskListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public OnboardingTaskModHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<OnboardingTaskListDto> Handle(OnboardingTaskModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingTask>()
                .FirstOrDefaultAsync(x => x.Id == request.ModDto.Id && !x.IsDeleted, ct);

            if (entity == null)
                throw new DomainException($"ONBOARDING TASK with id [{request.ModDto.Id}] NOT FOUND.");

            // Check concurrency using xmin
            if (!string.IsNullOrEmpty(request.ModDto.RowVersion))
            {
                var currentVersion = entity.xmin.ToString();
                if (currentVersion != request.ModDto.RowVersion)
                    throw new DomainException("The record has been modified by another user. Please refresh and try again.");
            }

            entity.TaskName = request.ModDto.TaskName.Trim();
            entity.Description = request.ModDto.Description.Trim();
            entity.SequenceOrder = request.ModDto.SequenceOrder;
            entity.DateMod = DateTime.UtcNow;

            await _uow.Update(entity);
            await _uow.Commit(ct);

            // Fetch updated record with new xmin
            var response = await _med.Send(new OnboardingTaskByIdQry { Id = request.ModDto.Id }, ct);
            if (response == null)
                throw new DomainException($"ONBOARDING TASK with id [{request.ModDto.Id}] NOT FOUND.");

            return response;
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OnboardingTaskDelHandler : IRequestHandler<OnboardingTaskDelCmd>
{
    private readonly IUnitOfWork _uow;

    public OnboardingTaskDelHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(OnboardingTaskDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var entity = await _uow.Set<OnboardingTask>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entity == null)
                throw new DomainException($"ONBOARDING TASK with id [{request.Id}] NOT FOUND.");

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