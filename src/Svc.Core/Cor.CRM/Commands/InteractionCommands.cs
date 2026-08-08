// Cor.CRM/Commands/InteractionCommands.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class InteractionAddCmd : IRequest<InteractionDto>
{
    public CreateInteractionDto Dto { get; set; } = default!;
}

public class InteractionModCmd : IRequest<InteractionDto>
{
    public Guid Id { get; set; }
    public UpdateInteractionDto Dto { get; set; } = default!;
}

public class InteractionDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class InteractionAddHandler : IRequestHandler<InteractionAddCmd, InteractionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InteractionAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InteractionDto> Handle(InteractionAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // ✅ Parse enum values to integers
            var type = string.IsNullOrEmpty(request.Dto.Type) ? InteractionType.Call : Enum.Parse<InteractionType>(request.Dto.Type);
            var status = string.IsNullOrEmpty(request.Dto.Status) ? (InteractionStatus?)null : Enum.Parse<InteractionStatus>(request.Dto.Status);
            var priority = string.IsNullOrEmpty(request.Dto.Priority) ? (InteractionPriority?)null : Enum.Parse<InteractionPriority>(request.Dto.Priority);

            var interaction = new Interaction
            {
                Id = Guid.CreateVersion7(),
                Subject = request.Dto.Subject,
                Description = request.Dto.Description,
                Type = type,
                Status = status,
                Priority = priority,
                LeadId = request.Dto.LeadId,
                CustomerId = request.Dto.CustomerId,
                ContactId = request.Dto.ContactId,
                OpportunityId = request.Dto.OpportunityId,
                AssignedToUserId = request.Dto.AssignedToUserId,
                ScheduledDate = request.Dto.ScheduledDate,
                Duration = request.Dto.Duration,
                Outcome = request.Dto.Outcome,
                Location = request.Dto.Location,
                IsAllDay = request.Dto.IsAllDay,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // If status is Completed, set CompletedDate
            if (interaction.Status == InteractionStatus.Completed)
            {
                interaction.CompletedDate = DateTime.UtcNow;
            }

            await _uow.Add(interaction, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Interaction created: {InteractionId} - {Subject}", interaction.Id, interaction.Subject);

            return MapToDto(interaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private InteractionDto MapToDto(Interaction interaction)
    {
        return new InteractionDto
        {
            Id = interaction.Id,
            Subject = interaction.Subject,
            Description = interaction.Description,
            Type = interaction.Type.ToString(),
            Status = interaction.Status?.ToString(),
            Priority = interaction.Priority?.ToString(),
            LeadId = interaction.LeadId,
            CustomerId = interaction.CustomerId,
            ContactId = interaction.ContactId,
            OpportunityId = interaction.OpportunityId,
            AssignedToUserId = interaction.AssignedToUserId,
            ScheduledDate = interaction.ScheduledDate,
            CompletedDate = interaction.CompletedDate,
            Duration = interaction.Duration,
            Outcome = interaction.Outcome,
            Location = interaction.Location,
            IsAllDay = interaction.IsAllDay,
            CreatedAt = interaction.CreatedAt,
            UpdatedAt = interaction.UpdatedAt
        };
    }
}

public class InteractionModHandler : IRequestHandler<InteractionModCmd, InteractionDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InteractionModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<InteractionDto> Handle(InteractionModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var interaction = await _uow.Set<Interaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (interaction == null)
                throw new DomainException($"Interaction with id [{request.Id}] NOT FOUND.");

            if (request.Dto.Subject != null)
                interaction.Subject = request.Dto.Subject;
            if (request.Dto.Description != null)
                interaction.Description = request.Dto.Description;
            if (request.Dto.Type != null)
                interaction.Type = Enum.Parse<InteractionType>(request.Dto.Type);
            if (request.Dto.Status != null)
                interaction.Status = Enum.Parse<InteractionStatus>(request.Dto.Status);
            if (request.Dto.Priority != null)
                interaction.Priority = Enum.Parse<InteractionPriority>(request.Dto.Priority);
            if (request.Dto.LeadId.HasValue)
                interaction.LeadId = request.Dto.LeadId;
            if (request.Dto.CustomerId.HasValue)
                interaction.CustomerId = request.Dto.CustomerId;
            if (request.Dto.ContactId.HasValue)
                interaction.ContactId = request.Dto.ContactId;
            if (request.Dto.OpportunityId.HasValue)
                interaction.OpportunityId = request.Dto.OpportunityId;
            if (request.Dto.AssignedToUserId.HasValue)
                interaction.AssignedToUserId = request.Dto.AssignedToUserId;
            if (request.Dto.ScheduledDate.HasValue)
                interaction.ScheduledDate = request.Dto.ScheduledDate;
            if (request.Dto.Duration.HasValue)
                interaction.Duration = request.Dto.Duration;
            if (request.Dto.Outcome != null)
                interaction.Outcome = request.Dto.Outcome;
            if (request.Dto.Location != null)
                interaction.Location = request.Dto.Location;
            if (request.Dto.IsAllDay.HasValue)
                interaction.IsAllDay = request.Dto.IsAllDay.Value;

            // Auto-complete if status is Completed
            if (interaction.Status == InteractionStatus.Completed && !interaction.CompletedDate.HasValue)
            {
                interaction.CompletedDate = DateTime.UtcNow;
            }

            interaction.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(interaction);
            await _uow.Commit(ct);

            _logger.LogInformation("Interaction updated: {InteractionId} - {Subject}", interaction.Id, interaction.Subject);

            return MapToDto(interaction);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private InteractionDto MapToDto(Interaction interaction)
    {
        return new InteractionDto
        {
            Id = interaction.Id,
            Subject = interaction.Subject,
            Description = interaction.Description,
            Type = interaction.Type.ToString(),
            Status = interaction.Status?.ToString(),
            Priority = interaction.Priority?.ToString(),
            LeadId = interaction.LeadId,
            CustomerId = interaction.CustomerId,
            ContactId = interaction.ContactId,
            OpportunityId = interaction.OpportunityId,
            AssignedToUserId = interaction.AssignedToUserId,
            ScheduledDate = interaction.ScheduledDate,
            CompletedDate = interaction.CompletedDate,
            Duration = interaction.Duration,
            Outcome = interaction.Outcome,
            Location = interaction.Location,
            IsAllDay = interaction.IsAllDay,
            CreatedAt = interaction.CreatedAt,
            UpdatedAt = interaction.UpdatedAt
        };
    }
}

public class InteractionDelHandler : IRequestHandler<InteractionDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public InteractionDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(InteractionDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var interaction = await _uow.Set<Interaction>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (interaction == null)
                throw new DomainException($"Interaction with id [{request.Id}] NOT FOUND.");

            interaction.IsDeleted = true;
            interaction.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(interaction);
            await _uow.Commit(ct);

            _logger.LogInformation("Interaction deleted: {InteractionId} - {Subject}", interaction.Id, interaction.Subject);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}