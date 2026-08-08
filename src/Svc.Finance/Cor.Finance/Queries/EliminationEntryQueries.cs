using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============ ELIMINATION ENTRY QUERIES ============
public class GetAllEliminationEntriesQry : IRequest<List<EliminationEntryDto>>
{
    public Guid? ConsolidationGroupId { get; set; }
    public string? Status { get; set; }
    public string? Type { get; set; }
    public Guid? FromEntityId { get; set; }
    public Guid? ToEntityId { get; set; }
}

public class GetEliminationEntryByIdQry : IRequest<EliminationEntryDto>
{
    public Guid Id { get; set; }
}

// ============ ELIMINATION QUERY HANDLERS ============
public class GetAllEliminationEntriesHandler : IRequestHandler<GetAllEliminationEntriesQry, List<EliminationEntryDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllEliminationEntriesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<EliminationEntryDto>> Handle(GetAllEliminationEntriesQry request, CancellationToken ct)
    {
        var query = _context.EliminationEntries
            .Include(x => x.FromEntity)
            .Include(x => x.ToEntity)
            .Include(x => x.ConsolidationGroup)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.ConsolidationGroupId.HasValue)
            query = query.Where(x => x.ConsolidationGroupId == request.ConsolidationGroupId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.Type))
            query = query.Where(x => x.Type == request.Type);

        if (request.FromEntityId.HasValue)
            query = query.Where(x => x.FromEntityId == request.FromEntityId.Value);

        if (request.ToEntityId.HasValue)
            query = query.Where(x => x.ToEntityId == request.ToEntityId.Value);

        return await query
            .Select(x => new EliminationEntryDto
            {
                Id = x.Id,
                Code = x.Code,
                Description = x.Description,
                Type = x.Type,
                FromEntityId = x.FromEntityId,
                FromEntityName = x.FromEntity != null ? x.FromEntity.Name : null,
                FromEntityCode = x.FromEntity != null ? x.FromEntity.Code : null,
                ToEntityId = x.ToEntityId,
                ToEntityName = x.ToEntity != null ? x.ToEntity.Name : null,
                ToEntityCode = x.ToEntity != null ? x.ToEntity.Code : null,
                Amount = x.Amount,
                Currency = x.Currency,
                ExchangeRate = x.ExchangeRate,
                AmountInReportingCurrency = x.AmountInReportingCurrency,
                AccountCode = x.AccountCode,
                AccountName = x.AccountName,
                Status = x.Status,
                ConsolidationGroupId = x.ConsolidationGroupId,
                ConsolidationGroupName = x.ConsolidationGroup != null ? x.ConsolidationGroup.Name : null,
                Period = x.Period,
                PostedAt = x.PostedAt,
                PostedBy = x.PostedBy,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetEliminationEntryByIdHandler : IRequestHandler<GetEliminationEntryByIdQry, EliminationEntryDto>
{
    private readonly FinanceDbContext _context;

    public GetEliminationEntryByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EliminationEntryDto> Handle(GetEliminationEntryByIdQry request, CancellationToken ct)
    {
        var entry = await _context.EliminationEntries
            .Include(x => x.FromEntity)
            .Include(x => x.ToEntity)
            .Include(x => x.ConsolidationGroup)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (entry == null)
            throw new InvalidOperationException($"Elimination entry with ID '{request.Id}' not found");

        return new EliminationEntryDto
        {
            Id = entry.Id,
            Code = entry.Code,
            Description = entry.Description,
            Type = entry.Type,
            FromEntityId = entry.FromEntityId,
            FromEntityName = entry.FromEntity?.Name,
            FromEntityCode = entry.FromEntity?.Code,
            ToEntityId = entry.ToEntityId,
            ToEntityName = entry.ToEntity?.Name,
            ToEntityCode = entry.ToEntity?.Code,
            Amount = entry.Amount,
            Currency = entry.Currency,
            ExchangeRate = entry.ExchangeRate,
            AmountInReportingCurrency = entry.AmountInReportingCurrency,
            AccountCode = entry.AccountCode,
            AccountName = entry.AccountName,
            Status = entry.Status,
            ConsolidationGroupId = entry.ConsolidationGroupId,
            ConsolidationGroupName = entry.ConsolidationGroup?.Name,
            Period = entry.Period,
            PostedAt = entry.PostedAt,
            PostedBy = entry.PostedBy,
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}