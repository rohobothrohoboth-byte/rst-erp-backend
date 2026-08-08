// Queries/ConsolidationQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;




public class GetAllConsolidationGroupsQry : IRequest<List<ConsolidationGroupDto>>
{
    public string? Status { get; set; }
    public Guid? PeriodId { get; set; }
}

public class GetConsolidationGroupByIdQry : IRequest<ConsolidationGroupDto>
{
    public Guid Id { get; set; }
}


public class GetConsolidationResultsQry : IRequest<ConsolidationResultDto>
{
    public Guid Id { get; set; }
}
public class GetConsolidationResultsHandler : IRequestHandler<GetConsolidationResultsQry, ConsolidationResultDto>
{
    private readonly FinanceDbContext _context;

    public GetConsolidationResultsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationResultDto> Handle(GetConsolidationResultsQry request, CancellationToken ct)
    {
        var group = await _context.ConsolidationGroups
            .Include(x => x.Entities)
                .ThenInclude(x => x.Entity)
            .Include(x => x.EliminationEntries)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (group == null)
            throw new InvalidOperationException($"Consolidation group with ID '{request.Id}' not found");

        return new ConsolidationResultDto
        {
            GroupId = group.Id,
            GroupName = group.Name,
            ConsolidationDate = group.ConsolidationDate ?? DateTime.UtcNow,
            TotalRevenue = group.TotalRevenue,
            TotalExpenses = group.TotalExpenses,
            TotalProfit = group.TotalProfit,
            TotalAssets = group.TotalAssets,
            TotalLiabilities = group.TotalLiabilities,
            TotalEquity = group.TotalEquity,
            EntitiesCount = group.Entities?.Count ?? 0,
            EliminationEntriesCount = group.EliminationEntries?.Count ?? 0,
            Status = group.Status,
            EntityResults = group.Entities?.Select(e => new EntityConsolidationResultDto
            {
                EntityId = e.EntityId,
                EntityName = e.Entity?.Name ?? "Unknown",
                Revenue = 0, // Would come from entity financial data
                Expenses = 0,
                Profit = 0,
                Assets = 0,
                Liabilities = 0,
                Equity = 0,
                OwnershipPercentage = e.Entity?.OwnershipPercentage ?? 0
            }).ToList() ?? new List<EntityConsolidationResultDto>()
        };
    }
}


public class GetAllConsolidationGroupsHandler : IRequestHandler<GetAllConsolidationGroupsQry, List<ConsolidationGroupDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllConsolidationGroupsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsolidationGroupDto>> Handle(GetAllConsolidationGroupsQry request, CancellationToken ct)
    {
        var query = _context.ConsolidationGroups
            .Include(x => x.ParentEntity)
            .Include(x => x.Period)
            .Include(x => x.Entities)
                .ThenInclude(ge => ge.Entity)
            .Include(x => x.EliminationEntries)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.PeriodId.HasValue)
            query = query.Where(x => x.PeriodId == request.PeriodId.Value);

        return await query
            .Select(x => new ConsolidationGroupDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ParentEntityId = x.ParentEntityId,
                ParentEntityName = x.ParentEntity != null ? x.ParentEntity.Name : null,
                Status = x.Status,
                ConsolidationDate = x.ConsolidationDate,
                PeriodId = x.PeriodId,
                PeriodName = x.Period != null ? x.Period.Name : null,
                TotalRevenue = x.TotalRevenue,
                TotalExpenses = x.TotalExpenses,
                TotalProfit = x.TotalProfit,
                TotalAssets = x.TotalAssets,
                TotalLiabilities = x.TotalLiabilities,
                TotalEquity = x.TotalEquity,
                Entities = x.Entities.Select(ge => new EntityDto
                {
                    Id = ge.Entity!.Id,
                    Code = ge.Entity.Code,
                    Name = ge.Entity.Name,
                    LegalName = ge.Entity.LegalName,
                    Type = ge.Entity.Type,
                    Country = ge.Entity.Country,
                    Currency = ge.Entity.Currency,
                    IsActive = ge.Entity.IsActive
                }).ToList(),
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetConsolidationGroupByIdHandler : IRequestHandler<GetConsolidationGroupByIdQry, ConsolidationGroupDto>
{
    private readonly FinanceDbContext _context;

    public GetConsolidationGroupByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ConsolidationGroupDto> Handle(GetConsolidationGroupByIdQry request, CancellationToken ct)
    {
        var group = await _context.ConsolidationGroups
            .Include(x => x.ParentEntity)
            .Include(x => x.Period)
            .Include(x => x.Entities)
                .ThenInclude(ge => ge.Entity)
            .Include(x => x.EliminationEntries)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (group == null)
            throw new InvalidOperationException($"Consolidation group with ID '{request.Id}' not found");

        return new ConsolidationGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            ParentEntityId = group.ParentEntityId,
            ParentEntityName = group.ParentEntity?.Name,
            Status = group.Status,
            ConsolidationDate = group.ConsolidationDate,
            PeriodId = group.PeriodId,
            PeriodName = group.Period?.Name,
            TotalRevenue = group.TotalRevenue,
            TotalExpenses = group.TotalExpenses,
            TotalProfit = group.TotalProfit,
            TotalAssets = group.TotalAssets,
            TotalLiabilities = group.TotalLiabilities,
            TotalEquity = group.TotalEquity,
            Entities = group.Entities.Select(ge => new EntityDto
            {
                Id = ge.Entity!.Id,
                Code = ge.Entity.Code,
                Name = ge.Entity.Name,
                LegalName = ge.Entity.LegalName,
                Type = ge.Entity.Type,
                Country = ge.Entity.Country,
                Currency = ge.Entity.Currency,
                IsActive = ge.Entity.IsActive
            }).ToList(),
            EliminationEntries = group.EliminationEntries
                .Where(e => !e.IsDeleted)
                .Select(e => new EliminationEntryDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Description = e.Description,
                    Type = e.Type,
                    FromEntityId = e.FromEntityId,
                    FromEntityName = e.FromEntity != null ? e.FromEntity.Name : null,
                    ToEntityId = e.ToEntityId,
                    ToEntityName = e.ToEntity != null ? e.ToEntity.Name : null,
                    Amount = e.Amount,
                    Currency = e.Currency,
                    ExchangeRate = e.ExchangeRate,
                    AmountInReportingCurrency = e.AmountInReportingCurrency,
                    AccountCode = e.AccountCode,
                    AccountName = e.AccountName,
                    Status = e.Status,
                    Period = e.Period,
                    PostedAt = e.PostedAt,
                    PostedBy = e.PostedBy,
                    DateAdd = e.DateAdd,
                    DateMod = e.DateMod
                }).ToList(),
            DateAdd = group.DateAdd,
            DateMod = group.DateMod
        };
    }
}