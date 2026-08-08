using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cor.Finance.Commands;

public class AddJournalEntryCmd : IRequest<JournalEntryDto>
{
    public AddJournalEntryDto AddDto { get; set; } = default!;
}

public class EditJournalEntryCmd : IRequest<JournalEntryDto>
{
    public EditJournalEntryDto EditDto { get; set; } = default!;
}

public class DeleteJournalEntryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class PostJournalEntryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class UnpostJournalEntryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ApproveJournalEntryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class RejectJournalEntryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ReverseJournalEntryCmd : IRequest<JournalEntryDto>
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
    public DateTime ReverseDate { get; set; }
}

// ============================================================
// HANDLERS
// ============================================================

// ==================== ADD JOURNAL ENTRY HANDLER ====================
public class AddJournalEntryHandler : IRequestHandler<AddJournalEntryCmd, JournalEntryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<AddJournalEntryHandler> _logger;

    public AddJournalEntryHandler(FinanceDbContext context, ILogger<AddJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(AddJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            // 1. Validate lines exist
            if (request.AddDto.Lines == null || request.AddDto.Lines.Count == 0)
                throw new InvalidOperationException("Journal entry must have at least one line");

            // 2. Validate total debit equals total credit
            var totalDebit = request.AddDto.Lines.Where(x => x.Direction == "Debit").Sum(x => x.Amount);
            var totalCredit = request.AddDto.Lines.Where(x => x.Direction == "Credit").Sum(x => x.Amount);

            if (totalDebit != totalCredit)
                throw new InvalidOperationException($"Total Debit ({totalDebit}) must equal Total Credit ({totalCredit})");

            // 3. Validate all accounts exist
            var accountIds = request.AddDto.Lines.Select(x => x.AccountId).Distinct().ToList();
            var existingAccounts = await _context.ChartOfAccounts
                .Where(x => accountIds.Contains(x.Id) && !x.IsDeleted)
                .Select(x => x.Id)
                .ToListAsync(ct);

            if (existingAccounts.Count != accountIds.Count)
                throw new InvalidOperationException("One or more accounts do not exist");

            // 4. Validate period exists and is open
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {request.AddDto.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot create journal entry in a closed period: {period.Name}");

            // 5. Ensure EntryDate is UTC
            var entryDate = request.AddDto.EntryDate;
            if (entryDate.Kind == DateTimeKind.Unspecified)
                entryDate = DateTime.SpecifyKind(entryDate, DateTimeKind.Utc);
            else if (entryDate.Kind == DateTimeKind.Local)
                entryDate = entryDate.ToUniversalTime();

            // 6. Validate entry date is within period range
            var periodStartUtc = period.StartDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(period.StartDate, DateTimeKind.Utc)
                : period.StartDate.ToUniversalTime();

            var periodEndUtc = period.EndDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(period.EndDate, DateTimeKind.Utc)
                : period.EndDate.ToUniversalTime();

            if (entryDate < periodStartUtc || entryDate > periodEndUtc)
                throw new InvalidOperationException(
                    $"Entry date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

            // 7. Create the journal entry
            var entry = new JournalEntry
            {
                Id = Guid.CreateVersion7(),
                Reference = request.AddDto.Reference,
                PeriodId = period.Id,
                EntryDate = entryDate,
                Description = request.AddDto.Description,
                EntryType = request.AddDto.EntryType ?? "General",
                BranchId = request.AddDto.BranchId,
                DepartmentId = request.AddDto.DepartmentId,
                EmployeeId = request.AddDto.EmployeeId,
                 CreatedByUserId = request.AddDto.CreatedByUserId,
                 CreatedByUserName = request.AddDto.CreatedByUserName,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                IsPosted = false,
                IsApproved = false,
                IsReversed = false,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };

            // 8. Add journal lines
            foreach (var lineDto in request.AddDto.Lines)
            {
                entry.Lines.Add(new JournalLine
                {
                    Id = Guid.CreateVersion7(),
                    JournalEntryId = entry.Id,
                    AccountId = lineDto.AccountId,
                    Direction = lineDto.Direction,
                    Amount = lineDto.Amount,
                    Description = lineDto.Description,
                    PeriodId = period.Id,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                });
            }

            // 9. Save to database
            await _context.JournalEntries.AddAsync(entry, ct);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} created successfully", entry.Reference);

            return await MapToDto(entry, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating journal entry");
            throw;
        }
    }

    private async Task<JournalEntryDto> MapToDto(JournalEntry entry, CancellationToken ct)
    {
        var lines = await _context.JournalLines
            .Where(x => x.JournalEntryId == entry.Id && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion ,
            Lines = lines.Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}

// ==================== EDIT JOURNAL ENTRY HANDLER ====================
public class EditJournalEntryHandler : IRequestHandler<EditJournalEntryCmd, JournalEntryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<EditJournalEntryHandler> _logger;

    public EditJournalEntryHandler(FinanceDbContext context, ILogger<EditJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(EditJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

            if (entry == null)
                throw new InvalidOperationException($"Journal entry with ID '{request.EditDto.Id}' not found");

            // ? FIX: RowVersion comparison - convert byte[] to string
            if (!string.IsNullOrEmpty(request.EditDto.RowVersion))
            {
                var currentRowVersion = entry.RowVersion ;

                if (currentRowVersion != request.EditDto.RowVersion)
                {
                    throw new InvalidOperationException(
                        "The journal entry has been modified by another user. Please refresh and try again.");
                }
            }

            if (entry.IsPosted)
                throw new InvalidOperationException("Cannot update a posted journal entry.");

            // Validate period
            if (entry.PeriodId != request.EditDto.PeriodId)
            {
                var period = await _context.FinancialPeriods
                    .FirstOrDefaultAsync(p => p.Id == request.EditDto.PeriodId && !p.IsDeleted, ct);

                if (period == null)
                    throw new InvalidOperationException($"Period with ID {request.EditDto.PeriodId} not found");

                if (period.IsClosed)
                    throw new InvalidOperationException($"Cannot move journal entry to a closed period: {period.Name}");

                if (request.EditDto.EntryDate < period.StartDate || request.EditDto.EntryDate > period.EndDate)
                    throw new InvalidOperationException(
                        $"Entry date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

                entry.PeriodId = period.Id;
            }
            else
            {
                var period = await _context.FinancialPeriods
                    .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

                if (period != null && period.IsClosed)
                    throw new InvalidOperationException($"Cannot update journal entry in a closed period: {period.Name}");
            }

            // Update basic fields
            entry.Reference = request.EditDto.Reference;
            entry.EntryDate = request.EditDto.EntryDate;
            entry.Description = request.EditDto.Description;
            entry.EntryType = request.EditDto.EntryType ?? "General";
            entry.TotalDebit = request.EditDto.TotalDebit;
            entry.TotalCredit = request.EditDto.TotalCredit;
            entry.DateMod = DateTime.UtcNow;
          entry.UpdatedByUserId = request.EditDto.UpdatedByUserId;
            entry.UpdatedByUserName = request.EditDto.UpdatedByUserName;
            // Soft delete existing lines
            var existingLines = await _context.JournalLines
                .Where(x => x.JournalEntryId == entry.Id && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var line in existingLines)
            {
                line.IsDeleted = true;
                line.DateMod = DateTime.UtcNow;
            }

            // Add new lines
            foreach (var lineDto in request.EditDto.Lines)
            {
                var line = new JournalLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = entry.Id,
                    AccountId = lineDto.AccountId,
                    Direction = lineDto.Direction,
                    Amount = lineDto.Amount,
                    Description = lineDto.Description,
                    PeriodId = entry.PeriodId,
                    IsDeleted = false,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                };
                _context.JournalLines.Add(line);
            }

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} updated successfully", entry.Reference);

            return await MapToDto(entry, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating journal entry");
            throw;
        }
    }

    private async Task<JournalEntryDto> MapToDto(JournalEntry entry, CancellationToken ct)
    {
        var lines = await _context.JournalLines
            .Where(x => x.JournalEntryId == entry.Id && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion,
            Lines = lines.Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}

// ==================== DELETE JOURNAL ENTRY HANDLER ====================
public class DeleteJournalEntryHandler : IRequestHandler<DeleteJournalEntryCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<DeleteJournalEntryHandler> _logger;

    public DeleteJournalEntryHandler(FinanceDbContext context, ILogger<DeleteJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                return false;

            if (entry.IsPosted)
                throw new InvalidOperationException("Cannot delete a posted journal entry.");

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {entry.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot delete journal entry in a closed period: {period.Name}");

            entry.IsDeleted = true;
            entry.DateMod = DateTime.UtcNow;

            var lines = await _context.JournalLines
                .Where(x => x.JournalEntryId == entry.Id && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var line in lines)
            {
                line.IsDeleted = true;
                line.DateMod = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} deleted successfully", entry.Reference);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting journal entry");
            throw;
        }
    }
}

// ==================== POST JOURNAL ENTRY HANDLER ====================
public class PostJournalEntryHandler : IRequestHandler<PostJournalEntryCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<PostJournalEntryHandler> _logger;

    public PostJournalEntryHandler(FinanceDbContext context, ILogger<PostJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(PostJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                return false;

            if (entry.IsPosted)
                throw new InvalidOperationException("Journal entry is already posted.");

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {entry.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot post journal entry in a closed period: {period.Name}");

            if (entry.EntryDate < period.StartDate || entry.EntryDate > period.EndDate)
                throw new InvalidOperationException(
                    $"Entry date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

            entry.IsPosted = true;
            entry.PostedDate = DateTime.UtcNow;
            entry.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} posted successfully", entry.Reference);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting journal entry");
            throw;
        }
    }
}

// ==================== UNPOST JOURNAL ENTRY HANDLER ====================
public class UnpostJournalEntryHandler : IRequestHandler<UnpostJournalEntryCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<UnpostJournalEntryHandler> _logger;

    public UnpostJournalEntryHandler(FinanceDbContext context, ILogger<UnpostJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UnpostJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                return false;

            if (!entry.IsPosted)
                throw new InvalidOperationException("Journal entry is not posted.");

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {entry.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot unpost journal entry in a closed period: {period.Name}");

            entry.IsPosted = false;
            entry.PostedDate = null;
            entry.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} unposted successfully", entry.Reference);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unposting journal entry");
            throw;
        }
    }
}

// ==================== APPROVE JOURNAL ENTRY HANDLER ====================
public class ApproveJournalEntryHandler : IRequestHandler<ApproveJournalEntryCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ApproveJournalEntryHandler> _logger;

    public ApproveJournalEntryHandler(FinanceDbContext context, ILogger<ApproveJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(ApproveJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                return false;

            if (entry.IsPosted)
                throw new InvalidOperationException("Cannot approve a posted journal entry.");

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {entry.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot approve journal entry in a closed period: {period.Name}");

            entry.IsApproved = true;
            entry.ApprovedDate = DateTime.UtcNow;
            entry.ApprovedBy = "System";
            entry.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} approved successfully", entry.Reference);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving journal entry");
            throw;
        }
    }
}

// ==================== REJECT JOURNAL ENTRY HANDLER ====================
public class RejectJournalEntryHandler : IRequestHandler<RejectJournalEntryCmd, bool>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<RejectJournalEntryHandler> _logger;

    public RejectJournalEntryHandler(FinanceDbContext context, ILogger<RejectJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(RejectJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                return false;

            if (entry.IsPosted)
                throw new InvalidOperationException("Cannot reject a posted journal entry.");

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {entry.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot reject journal entry in a closed period: {period.Name}");

            entry.IsApproved = false;
            entry.RejectionReason = request.Reason;
            entry.DateMod = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} rejected successfully", entry.Reference);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting journal entry");
            throw;
        }
    }
}

// ==================== REVERSE JOURNAL ENTRY HANDLER ====================
public class ReverseJournalEntryHandler : IRequestHandler<ReverseJournalEntryCmd, JournalEntryDto>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<ReverseJournalEntryHandler> _logger;

    public ReverseJournalEntryHandler(FinanceDbContext context, ILogger<ReverseJournalEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JournalEntryDto> Handle(ReverseJournalEntryCmd request, CancellationToken ct)
    {
        try
        {
            var entry = await _context.JournalEntries
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (entry == null)
                throw new InvalidOperationException($"Journal entry with ID '{request.Id}' not found");

            if (!entry.IsPosted)
                throw new InvalidOperationException("Cannot reverse an unposted journal entry.");

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {entry.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot reverse journal entry in a closed period: {period.Name}");

            if (request.ReverseDate < period.StartDate || request.ReverseDate > period.EndDate)
                throw new InvalidOperationException(
                    $"Reverse date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

            // Create reversal entry
            var reversalEntry = new JournalEntry
            {
                Id = Guid.CreateVersion7(),
                Reference = $"REV-{entry.Reference}",
                PeriodId = entry.PeriodId,
                EntryDate = request.ReverseDate,
                Description = $"Reversal of {entry.Reference}: {request.Reason ?? "No reason provided"}",
                EntryType = "Reversal",
                BranchId = entry.BranchId,
                DepartmentId = entry.DepartmentId,
                EmployeeId = entry.EmployeeId,
                TotalDebit = entry.TotalCredit,
                TotalCredit = entry.TotalDebit,
                IsPosted = true,
                IsApproved = true,
                IsReversed = false,
                PostedDate = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };

            // Add reversed lines
            foreach (var line in entry.Lines.Where(x => !x.IsDeleted))
            {
                reversalEntry.Lines.Add(new JournalLine
                {
                    Id = Guid.CreateVersion7(),
                    JournalEntryId = reversalEntry.Id,
                    AccountId = line.AccountId,
                    Direction = line.Direction == "Debit" ? "Credit" : "Debit",
                    Amount = line.Amount,
                    Description = $"Reversal: {line.Description}",
                    PeriodId = entry.PeriodId,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                });
            }

            // Mark original as reversed
            entry.IsReversed = true;
            entry.ReversedDate = DateTime.UtcNow;
            entry.ReversedBy = "System";
            entry.DateMod = DateTime.UtcNow;

            await _context.JournalEntries.AddAsync(reversalEntry, ct);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Journal entry {Reference} reversed successfully", entry.Reference);

            return await MapToDto(reversalEntry, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing journal entry");
            throw;
        }
    }

    private async Task<JournalEntryDto> MapToDto(JournalEntry entry, CancellationToken ct)
    {
        var lines = await _context.JournalLines
            .Where(x => x.JournalEntryId == entry.Id && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == entry.PeriodId && !p.IsDeleted, ct);

        return new JournalEntryDto
        {
            Id = entry.Id,
            Reference = entry.Reference,
            EntryDate = entry.EntryDate,
            Description = entry.Description,
            EntryType = entry.EntryType,
            TotalDebit = entry.TotalDebit,
            TotalCredit = entry.TotalCredit,
            IsPosted = entry.IsPosted,
            IsApproved = entry.IsApproved,
            IsReversed = entry.IsReversed,
            PostedDate = entry.PostedDate,
            ApprovedDate = entry.ApprovedDate,
            ApprovedBy = entry.ApprovedBy,
            ReversedDate = entry.ReversedDate,
            ReversedBy = entry.ReversedBy,
            RejectionReason = entry.RejectionReason,
            PeriodId = entry.PeriodId,
            PeriodName = period?.Name,
            BranchId = entry.BranchId,
            DepartmentId = entry.DepartmentId,
            EmployeeId = entry.EmployeeId,
            RowVersion = entry.RowVersion,
             CreatedByUserId = entry.CreatedByUserId,
                    CreatedByUserName = entry.CreatedByUserName,
                    UpdatedByUserId = entry.UpdatedByUserId,
                    UpdatedByUserName = entry.UpdatedByUserName,
            Lines = lines.Select(line => new JournalLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountCode = line.Account?.Code,
                AccountName = line.Account?.Name,
                Direction = line.Direction,
                Amount = line.Amount,
                Description = line.Description
            }).ToList(),
            DateAdd = entry.DateAdd,
            DateMod = entry.DateMod
        };
    }
}