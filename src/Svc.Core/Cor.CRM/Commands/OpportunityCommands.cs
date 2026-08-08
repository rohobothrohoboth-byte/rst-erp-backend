// Cor.CRM/Commands/OpportunityCommands.cs

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

public class OpportunityAddCmd : IRequest<OpportunityDto>
{
    public CreateOpportunityDto Dto { get; set; } = default!;
}

public class OpportunityModCmd : IRequest<OpportunityDto>
{
    public Guid Id { get; set; }
    public UpdateOpportunityDto Dto { get; set; } = default!;
}

public class OpportunityDelCmd : IRequest
{
    public Guid Id { get; set; }
}

public class OpportunityUpdateStageCmd : IRequest<OpportunityDto>
{
    public Guid Id { get; set; }
    public string Stage { get; set; } = string.Empty;
}

public class OpportunityUpdateProbabilityCmd : IRequest<OpportunityDto>
{
    public Guid Id { get; set; }
    public int WinProbability { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

public class OpportunityAddHandler : IRequestHandler<OpportunityAddCmd, OpportunityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OpportunityAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(OpportunityAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var opportunity = new Opportunity
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                CustomerId = request.Dto.CustomerId,
                LeadId = request.Dto.LeadId,
                Amount = request.Dto.Amount,
                Stage = string.IsNullOrEmpty(request.Dto.Stage)
                    ? OpportunityStage.Discovery
                    : Enum.Parse<OpportunityStage>(request.Dto.Stage),
                WinProbability = request.Dto.WinProbability.HasValue
                    ? (WinProbability)request.Dto.WinProbability.Value
                    : WinProbability.Medium,
                ExpectedCloseDate = request.Dto.ExpectedCloseDate,
                AssignedToUserId = request.Dto.AssignedToUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(opportunity, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Opportunity created: {OpportunityId} - {Name}", opportunity.Id, opportunity.Name);

            return MapToDto(opportunity);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private OpportunityDto MapToDto(Opportunity opportunity)
    {
        return new OpportunityDto
        {
            Id = opportunity.Id,
            Name = opportunity.Name,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            LeadId = opportunity.LeadId,
            Amount = opportunity.Amount,
            Stage = opportunity.Stage.ToString(),
            WinProbability = (int)opportunity.WinProbability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedToUserId = opportunity.AssignedToUserId,
            IsActive = opportunity.IsActive,
            ActivityCount = opportunity.ActivityCount,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        };
    }
}

public class OpportunityModHandler : IRequestHandler<OpportunityModCmd, OpportunityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OpportunityModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(OpportunityModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var opportunity = await _uow.Set<Opportunity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (opportunity == null)
                throw new DomainException($"Opportunity with id [{request.Id}] NOT FOUND.");

            if (request.Dto.Name != null)
                opportunity.Name = request.Dto.Name;
            if (request.Dto.Description != null)
                opportunity.Description = request.Dto.Description;
            if (request.Dto.Amount.HasValue)
                opportunity.Amount = request.Dto.Amount.Value;
            if (request.Dto.Stage != null)
                opportunity.Stage = Enum.Parse<OpportunityStage>(request.Dto.Stage);
            if (request.Dto.WinProbability.HasValue)
                opportunity.WinProbability = (WinProbability)request.Dto.WinProbability.Value;
            if (request.Dto.ExpectedCloseDate.HasValue)
                opportunity.ExpectedCloseDate = request.Dto.ExpectedCloseDate;
            if (request.Dto.AssignedToUserId.HasValue)
                opportunity.AssignedToUserId = request.Dto.AssignedToUserId;

            // Auto-set ActualCloseDate when stage is ClosedWon or ClosedLost
            if (opportunity.Stage == OpportunityStage.ClosedWon || opportunity.Stage == OpportunityStage.ClosedLost)
            {
                opportunity.ActualCloseDate = DateTime.UtcNow;
            }

            opportunity.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(opportunity);
            await _uow.Commit(ct);

            _logger.LogInformation("Opportunity updated: {OpportunityId} - {Name}", opportunity.Id, opportunity.Name);

            return MapToDto(opportunity);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private OpportunityDto MapToDto(Opportunity opportunity)
    {
        return new OpportunityDto
        {
            Id = opportunity.Id,
            Name = opportunity.Name,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            LeadId = opportunity.LeadId,
            Amount = opportunity.Amount,
            Stage = opportunity.Stage.ToString(),
            WinProbability = (int)opportunity.WinProbability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedToUserId = opportunity.AssignedToUserId,
            IsActive = opportunity.IsActive,
            ActivityCount = opportunity.ActivityCount,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        };
    }
}

public class OpportunityDelHandler : IRequestHandler<OpportunityDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OpportunityDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(OpportunityDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var opportunity = await _uow.Set<Opportunity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (opportunity == null)
                throw new DomainException($"Opportunity with id [{request.Id}] NOT FOUND.");

            opportunity.IsDeleted = true;
            opportunity.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(opportunity);
            await _uow.Commit(ct);

            _logger.LogInformation("Opportunity deleted: {OpportunityId} - {Name}", opportunity.Id, opportunity.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

public class OpportunityUpdateStageHandler : IRequestHandler<OpportunityUpdateStageCmd, OpportunityDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public OpportunityUpdateStageHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<OpportunityDto> Handle(OpportunityUpdateStageCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var opportunity = await _uow.Set<Opportunity>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (opportunity == null)
                throw new DomainException($"Opportunity with id [{request.Id}] NOT FOUND.");

            opportunity.Stage = Enum.Parse<OpportunityStage>(request.Stage);

            if (opportunity.Stage == OpportunityStage.ClosedWon || opportunity.Stage == OpportunityStage.ClosedLost)
            {
                opportunity.ActualCloseDate = DateTime.UtcNow;
            }

            opportunity.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(opportunity);
            await _uow.Commit(ct);

            _logger.LogInformation("Opportunity stage updated: {OpportunityId} -> {Stage}",
                opportunity.Id, opportunity.Stage);

            return MapToDto(opportunity);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }

    private OpportunityDto MapToDto(Opportunity opportunity)
    {
        return new OpportunityDto
        {
            Id = opportunity.Id,
            Name = opportunity.Name,
            Description = opportunity.Description,
            CustomerId = opportunity.CustomerId,
            LeadId = opportunity.LeadId,
            Amount = opportunity.Amount,
            Stage = opportunity.Stage.ToString(),
            WinProbability = (int)opportunity.WinProbability,
            ExpectedCloseDate = opportunity.ExpectedCloseDate,
            ActualCloseDate = opportunity.ActualCloseDate,
            AssignedToUserId = opportunity.AssignedToUserId,
            IsActive = opportunity.IsActive,
            ActivityCount = opportunity.ActivityCount,
            CreatedAt = opportunity.CreatedAt,
            UpdatedAt = opportunity.UpdatedAt
        };
    }
}