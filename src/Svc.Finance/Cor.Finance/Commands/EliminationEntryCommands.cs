// Commands/EliminationEntryCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddEliminationEntryCmd : IRequest<EliminationEntryDto>
{
    public AddEliminationEntryDto AddDto { get; set; } = new();
}

public class EditEliminationEntryCmd : IRequest<EliminationEntryDto>
{
    public EditEliminationEntryDto EditDto { get; set; } = new();
}

public class DeleteEliminationEntryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class PostEliminationEntryCmd : IRequest<EliminationEntryDto>
{
    public PostEliminationEntryDto PostDto { get; set; } = new();
}

public class RejectEliminationEntryCmd : IRequest<EliminationEntryDto>
{
    public RejectEliminationEntryDto RejectDto { get; set; } = new();
}
public static class EliminationEntryMapper
{
    public static EliminationEntryDto MapToDto(EliminationEntry entry)
    {
        if (entry == null) return null!;

        return new EliminationEntryDto
        {
            Id = entry.Id,
            Code = entry.Code,
            Description = entry.Description,
            Type = entry.Type,
            FromEntityId = entry.FromEntityId,
            ToEntityId = entry.ToEntityId,
            Amount = entry.Amount,
            Currency = entry.Currency,
            ExchangeRate = entry.ExchangeRate,
            AmountInReportingCurrency = entry.AmountInReportingCurrency,
            AccountCode = entry.AccountCode,
            AccountName = entry.AccountName,
            Status = entry.Status,
            ConsolidationGroupId = entry.ConsolidationGroupId,
            Period = entry.Period,
            PostedAt = entry.PostedAt,
            PostedBy = entry.PostedBy,
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}
public class AddEliminationEntryHandler : IRequestHandler<AddEliminationEntryCmd, EliminationEntryDto>
{
    private readonly FinanceDbContext _context;

    public AddEliminationEntryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EliminationEntryDto> Handle(AddEliminationEntryCmd request, CancellationToken ct)
    {
        var exists = await _context.EliminationEntries
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Elimination entry with code '{request.AddDto.Code}' already exists");

        var entry = new EliminationEntry
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Description = request.AddDto.Description,
            Type = request.AddDto.Type,
            FromEntityId = request.AddDto.FromEntityId,
            ToEntityId = request.AddDto.ToEntityId,
            Amount = request.AddDto.Amount,
            Currency = request.AddDto.Currency ?? "USD",
            ExchangeRate = request.AddDto.ExchangeRate,
            AmountInReportingCurrency = request.AddDto.AmountInReportingCurrency,
            AccountCode = request.AddDto.AccountCode,
            AccountName = request.AddDto.AccountName,
            Status = request.AddDto.Status ?? "Draft",
            ConsolidationGroupId = request.AddDto.ConsolidationGroupId,
            Period = request.AddDto.Period,
            DateAdd = DateTime.UtcNow
        };

        _context.EliminationEntries.Add(entry);
        await _context.SaveChangesAsync(ct);

        return EliminationEntryMapper.MapToDto(entry);
    }


}

public class EditEliminationEntryHandler : IRequestHandler<EditEliminationEntryCmd, EliminationEntryDto>
{
    private readonly FinanceDbContext _context;

    public EditEliminationEntryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EliminationEntryDto> Handle(EditEliminationEntryCmd request, CancellationToken ct)
    {
        var entry = await _context.EliminationEntries
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (entry == null)
            throw new InvalidOperationException($"Elimination entry with ID '{request.EditDto.Id}' not found");

        entry.Code = request.EditDto.Code;
        entry.Description = request.EditDto.Description;
        entry.Type = request.EditDto.Type;
        entry.FromEntityId = request.EditDto.FromEntityId;
        entry.ToEntityId = request.EditDto.ToEntityId;
        entry.Amount = request.EditDto.Amount;
        entry.Currency = request.EditDto.Currency;
        entry.ExchangeRate = request.EditDto.ExchangeRate;
        entry.AmountInReportingCurrency = request.EditDto.AmountInReportingCurrency;
        entry.AccountCode = request.EditDto.AccountCode;
        entry.AccountName = request.EditDto.AccountName;
        entry.Status = request.EditDto.Status;
        entry.ConsolidationGroupId = request.EditDto.ConsolidationGroupId;
        entry.Period = request.EditDto.Period;
        entry.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

         return EliminationEntryMapper.MapToDto(entry);
    }


}

public class DeleteEliminationEntryHandler : IRequestHandler<DeleteEliminationEntryCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteEliminationEntryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteEliminationEntryCmd request, CancellationToken ct)
    {
        var entry = await _context.EliminationEntries
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (entry == null)
            return false;

        entry.IsDeleted = true;
        entry.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class PostEliminationEntryHandler : IRequestHandler<PostEliminationEntryCmd, EliminationEntryDto>
{
    private readonly FinanceDbContext _context;

    public PostEliminationEntryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EliminationEntryDto> Handle(PostEliminationEntryCmd request, CancellationToken ct)
    {
        var entry = await _context.EliminationEntries
            .FirstOrDefaultAsync(x => x.Id == request.PostDto.Id && !x.IsDeleted, ct);

        if (entry == null)
            throw new InvalidOperationException($"Elimination entry with ID '{request.PostDto.Id}' not found");

        if (entry.Status == "Posted")
            throw new InvalidOperationException("Elimination entry is already posted");

        entry.Status = "Posted";
        entry.PostedAt = DateTime.UtcNow;
        entry.PostedBy = request.PostDto.PostedBy ?? "System";
        entry.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

         return EliminationEntryMapper.MapToDto(entry);
    }
}

public class RejectEliminationEntryHandler : IRequestHandler<RejectEliminationEntryCmd, EliminationEntryDto>
{
    private readonly FinanceDbContext _context;

    public RejectEliminationEntryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<EliminationEntryDto> Handle(RejectEliminationEntryCmd request, CancellationToken ct)
    {
        var entry = await _context.EliminationEntries
            .FirstOrDefaultAsync(x => x.Id == request.RejectDto.Id && !x.IsDeleted, ct);

        if (entry == null)
            throw new InvalidOperationException($"Elimination entry with ID '{request.RejectDto.Id}' not found");

        if (entry.Status == "Posted")
            throw new InvalidOperationException("Cannot reject a posted elimination entry");

        entry.Status = "Rejected";
        entry.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return EliminationEntryMapper.MapToDto(entry);
    }
}