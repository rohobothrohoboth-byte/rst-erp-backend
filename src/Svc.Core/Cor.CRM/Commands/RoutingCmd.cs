// Cor.CRM/Commands/RoutingCmd.cs

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

public class RoutingRuleAddCmd : IRequest<RoutingRuleDto>
{
    public CreateRoutingRuleDto Dto { get; set; } = default!;
}

public class RoutingRuleModCmd : IRequest<RoutingRuleDto>
{
    public Guid Id { get; set; }
    public UpdateRoutingRuleDto Dto { get; set; } = default!;
}

public class RoutingRuleDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// ADD ROUTING RULE
// ============================================================

public class RoutingRuleAddHandler : IRequestHandler<RoutingRuleAddCmd, RoutingRuleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public RoutingRuleAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RoutingRuleDto> Handle(RoutingRuleAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rule = new LeadRoutingRule
            {
                Id = Guid.CreateVersion7(),
                Name = request.Dto.Name,
                Description = request.Dto.Description,
                Type = Enum.Parse<RoutingType>(request.Dto.Type),
                Conditions = request.Dto.Conditions,
                IsActive = request.Dto.IsActive,
                Priority = request.Dto.Priority,
                AssignedToUserId = request.Dto.AssignedToUserId,
                AssignedToTeamId = request.Dto.AssignedToTeamId,
                FallbackRule = request.Dto.FallbackRule,
                MaxLeadsPerDay = request.Dto.MaxLeadsPerDay,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(rule, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Routing rule created: {RuleId} - {RuleName}", rule.Id, rule.Name);

            return new RoutingRuleDto
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Type = rule.Type.ToString(),
                Conditions = rule.Conditions,
                IsActive = rule.IsActive,
                Priority = rule.Priority,
                AssignedToUserId = rule.AssignedToUserId,
                AssignedToTeamId = rule.AssignedToTeamId,
                FallbackRule = rule.FallbackRule,
                MaxLeadsPerDay = rule.MaxLeadsPerDay,
                CreatedAt = rule.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// UPDATE ROUTING RULE
// ============================================================

public class RoutingRuleModHandler : IRequestHandler<RoutingRuleModCmd, RoutingRuleDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public RoutingRuleModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<RoutingRuleDto> Handle(RoutingRuleModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rule = await _uow.Set<LeadRoutingRule>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (rule == null)
            {
                throw new DomainException($"Routing rule with id [{request.Id}] NOT FOUND.");
            }

            if (request.Dto.Name != null)
                rule.Name = request.Dto.Name;
            if (request.Dto.Description != null)
                rule.Description = request.Dto.Description;
            if (request.Dto.Type != null)
                rule.Type = Enum.Parse<RoutingType>(request.Dto.Type);
            if (request.Dto.Conditions != null)
                rule.Conditions = request.Dto.Conditions;
            if (request.Dto.IsActive.HasValue)
                rule.IsActive = request.Dto.IsActive.Value;
            if (request.Dto.Priority.HasValue)
                rule.Priority = request.Dto.Priority.Value;
            if (request.Dto.AssignedToUserId.HasValue)
                rule.AssignedToUserId = request.Dto.AssignedToUserId;
            if (request.Dto.AssignedToTeamId.HasValue)
                rule.AssignedToTeamId = request.Dto.AssignedToTeamId;
            if (request.Dto.FallbackRule != null)
                rule.FallbackRule = request.Dto.FallbackRule;
            if (request.Dto.MaxLeadsPerDay.HasValue)
                rule.MaxLeadsPerDay = request.Dto.MaxLeadsPerDay;

            rule.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(rule);
            await _uow.Commit(ct);

            _logger.LogInformation("Routing rule updated: {RuleId} - {RuleName}", rule.Id, rule.Name);

            return new RoutingRuleDto
            {
                Id = rule.Id,
                Name = rule.Name,
                Description = rule.Description,
                Type = rule.Type.ToString(),
                Conditions = rule.Conditions,
                IsActive = rule.IsActive,
                Priority = rule.Priority,
                AssignedToUserId = rule.AssignedToUserId,
                AssignedToTeamId = rule.AssignedToTeamId,
                FallbackRule = rule.FallbackRule,
                MaxLeadsPerDay = rule.MaxLeadsPerDay,
                UpdatedAt = rule.UpdatedAt,
                CreatedAt = rule.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// DELETE ROUTING RULE
// ============================================================

public class RoutingRuleDelHandler : IRequestHandler<RoutingRuleDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public RoutingRuleDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(RoutingRuleDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var rule = await _uow.Set<LeadRoutingRule>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (rule == null)
            {
                throw new DomainException($"Routing rule with id [{request.Id}] NOT FOUND.");
            }

            await _uow.Delete(rule);
            await _uow.Commit(ct);

            _logger.LogInformation("Routing rule deleted: {RuleId} - {RuleName}", rule.Id, rule.Name);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}