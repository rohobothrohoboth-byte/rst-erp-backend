using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ============ CONSOLIDATION GROUP COMMANDS ============
public class AddConsolidationGroupCmd : IRequest<ConsolidationGroupDto>
{
    public AddConsolidationGroupDto AddDto { get; set; } = new();
}

public class EditConsolidationGroupCmd : IRequest<ConsolidationGroupDto>
{
    public EditConsolidationGroupDto EditDto { get; set; } = new();
}

public class DeleteConsolidationGroupCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class RunConsolidationCmd : IRequest<ConsolidationGroupDto>
{
    public Guid Id { get; set; }
}

// ============ CONSOLIDATION HANDLERS ============
public class AddConsolidationGroupHandler : IRequestHandler<AddConsolidationGroupCmd, ConsolidationGroupDto>
{
    private readonly FinanceDbContext _context;

    public AddConsolidationGroupHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationGroupDto> Handle(AddConsolidationGroupCmd request, CancellationToken ct)
    {
        if (request.AddDto.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.PeriodId && !x.IsClosed, ct);
            if (period == null)
                throw new InvalidOperationException("Invalid period or period is closed");
        }

        var entityIds = request.AddDto.EntityIds ?? new List<Guid>();
        var entities = await _context.Entities
            .Where(x => entityIds.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(ct);

        if (entities.Count != entityIds.Count)
            throw new InvalidOperationException("One or more entities not found");

        var group = new ConsolidationGroup
        {
            Id = Guid.NewGuid(),
            Name = request.AddDto.Name,
            Description = request.AddDto.Description,
            ParentEntityId = request.AddDto.ParentEntityId,
            PeriodId = request.AddDto.PeriodId,
            ConsolidationDate = request.AddDto.ConsolidationDate ?? DateTime.UtcNow,
            Status = "Draft",
            DateAdd = DateTime.UtcNow
        };

        foreach (var entity in entities)
        {
            group.Entities.Add(new ConsolidationGroupEntity
            {
                Id = Guid.NewGuid(),
                EntityId = entity.Id
            });
        }

        _context.ConsolidationGroups.Add(group);
        await _context.SaveChangesAsync(ct);

        return await new GetConsolidationGroupByIdHandler(_context).Handle(
            new GetConsolidationGroupByIdQry { Id = group.Id }, ct);
    }
}

public class EditConsolidationGroupHandler : IRequestHandler<EditConsolidationGroupCmd, ConsolidationGroupDto>
{
    private readonly FinanceDbContext _context;

    public EditConsolidationGroupHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationGroupDto> Handle(EditConsolidationGroupCmd request, CancellationToken ct)
    {
        var group = await _context.ConsolidationGroups
            .Include(x => x.Entities)
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (group == null)
            throw new InvalidOperationException($"Consolidation group with ID '{request.EditDto.Id}' not found");

        if (group.Status == "Approved" || group.Status == "Completed")
            throw new InvalidOperationException($"Cannot edit a {group.Status} consolidation group");

        if (request.EditDto.PeriodId.HasValue)
        {
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == request.EditDto.PeriodId && !x.IsClosed, ct);
            if (period == null)
                throw new InvalidOperationException("Invalid period or period is closed");
        }

        group.Name = request.EditDto.Name;
        group.Description = request.EditDto.Description;
        group.ParentEntityId = request.EditDto.ParentEntityId;
        group.PeriodId = request.EditDto.PeriodId;
        group.ConsolidationDate = request.EditDto.ConsolidationDate ?? group.ConsolidationDate;
        group.Status = request.EditDto.Status ?? group.Status;
        group.DateMod = DateTime.UtcNow;

        var newEntityIds = request.EditDto.EntityIds ?? new List<Guid>();
        var existingEntityIds = group.Entities.Select(e => e.EntityId).ToList();

        var toRemove = group.Entities
            .Where(e => !newEntityIds.Contains(e.EntityId))
            .ToList();
        foreach (var item in toRemove)
        {
            _context.ConsolidationGroupEntities.Remove(item);
        }

        var toAdd = newEntityIds
            .Where(id => !existingEntityIds.Contains(id))
            .ToList();

        if (toAdd.Any())
        {
            var entities = await _context.Entities
                .Where(x => toAdd.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var entity in entities)
            {
                group.Entities.Add(new ConsolidationGroupEntity
                {
                    Id = Guid.NewGuid(),
                    EntityId = entity.Id
                });
            }
        }

        await _context.SaveChangesAsync(ct);

        return await new GetConsolidationGroupByIdHandler(_context).Handle(
            new GetConsolidationGroupByIdQry { Id = group.Id }, ct);
    }
}

public class DeleteConsolidationGroupHandler : IRequestHandler<DeleteConsolidationGroupCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteConsolidationGroupHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteConsolidationGroupCmd request, CancellationToken ct)
    {
        var group = await _context.ConsolidationGroups
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (group == null)
            return false;

        if (group.Status == "Approved" || group.Status == "Completed")
            throw new InvalidOperationException($"Cannot delete a {group.Status} consolidation group");

        group.IsDeleted = true;
        group.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class RunConsolidationHandler : IRequestHandler<RunConsolidationCmd, ConsolidationGroupDto>
{
    private readonly FinanceDbContext _context;

    public RunConsolidationHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationGroupDto> Handle(RunConsolidationCmd request, CancellationToken ct)
    {
        var group = await _context.ConsolidationGroups
            .Include(x => x.Entities)
                .ThenInclude(ge => ge.Entity)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (group == null)
            throw new InvalidOperationException($"Consolidation group with ID '{request.Id}' not found");

        if (group.Status == "Completed" || group.Status == "Approved")
            throw new InvalidOperationException($"Consolidation is already {group.Status}");

        decimal totalRevenue = 0;
        decimal totalExpenses = 0;
        decimal totalAssets = 0;
        decimal totalLiabilities = 0;
        decimal totalEquity = 0;

        // In real scenario, you'd fetch financial data for each entity
        foreach (var entityGroup in group.Entities)
        {
            totalRevenue += 1000000;
            totalExpenses += 800000;
            totalAssets += 2000000;
            totalLiabilities += 1200000;
            totalEquity += 800000;
        }

        group.TotalRevenue = totalRevenue;
        group.TotalExpenses = totalExpenses;
        group.TotalProfit = totalRevenue - totalExpenses;
        group.TotalAssets = totalAssets;
        group.TotalLiabilities = totalLiabilities;
        group.TotalEquity = totalEquity;
        group.Status = "Completed";
        group.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return await new GetConsolidationGroupByIdHandler(_context).Handle(
            new GetConsolidationGroupByIdQry { Id = group.Id }, ct);
    }
}