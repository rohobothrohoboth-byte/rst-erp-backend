using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Controllers;

/// <summary>
/// Safe draft-edit endpoint for journal entries.
/// Existing JournalLine IDs are updated in place; only genuinely new lines are inserted.
/// Lines removed from the request are soft-deleted.
/// </summary>
[ApiController]
[Route("api/finance/v{version:apiVersion}/JournalEntry")]
[ApiVersion("1.0")]
[Authorize]
public sealed class JournalEntryDraftController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<JournalEntryDraftController> _logger;

    public JournalEntryDraftController(
        FinanceDbContext context,
        ILogger<JournalEntryDraftController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPut("{id:guid}/draft")]
    [PerAuth("fnm.gl.journal.mod")]
    public async Task<IActionResult> UpdateDraft(
        Guid id,
        [FromBody] DraftJournalEntryUpdateDto request,
        CancellationToken ct)
    {
        try
        {
            if (request.Lines == null || request.Lines.Count == 0)
                return BadRequest(new { success = false, message = "Journal entry must have at least one line" });

            var entry = await _context.JournalEntries
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

            if (entry == null)
                return NotFound(new { success = false, message = "Journal entry not found" });

            if (entry.IsPosted)
                return BadRequest(new { success = false, message = "Cannot update a posted journal entry." });

            if (request.PeriodId == Guid.Empty)
                return BadRequest(new { success = false, message = "PeriodId is required" });

            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(x => x.Id == request.PeriodId && !x.IsDeleted, ct);

            if (period == null)
                return BadRequest(new { success = false, message = "Financial period not found" });

            if (period.IsClosed)
                return BadRequest(new { success = false, message = $"Cannot update journal entry in a closed period: {period.Name}" });

            var entryDate = request.EntryDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(request.EntryDate, DateTimeKind.Utc)
                : request.EntryDate.ToUniversalTime();

            var periodStart = period.StartDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(period.StartDate, DateTimeKind.Utc)
                : period.StartDate.ToUniversalTime();
            var periodEnd = period.EndDate.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(period.EndDate, DateTimeKind.Utc)
                : period.EndDate.ToUniversalTime();

            if (entryDate < periodStart || entryDate > periodEnd)
                return BadRequest(new
                {
                    success = false,
                    message = $"Entry date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}"
                });

            if (!string.IsNullOrWhiteSpace(request.RowVersion) &&
                !string.Equals(entry.RowVersion, request.RowVersion, StringComparison.Ordinal))
            {
                return Conflict(new
                {
                    success = false,
                    message = "The journal entry has been modified by another user. Please refresh and try again."
                });
            }

            var accountIds = request.Lines.Select(x => x.AccountId).Distinct().ToList();
            var validAccountIds = await _context.ChartOfAccounts
                .Where(x => accountIds.Contains(x.Id) && !x.IsDeleted)
                .Select(x => x.Id)
                .ToListAsync(ct);

            if (validAccountIds.Count != accountIds.Count)
                return BadRequest(new { success = false, message = "One or more accounts do not exist" });

            var totalDebit = request.Lines
                .Where(x => string.Equals(x.Direction, "Debit", StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.Amount);
            var totalCredit = request.Lines
                .Where(x => string.Equals(x.Direction, "Credit", StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.Amount);

            if (totalDebit != totalCredit)
                return BadRequest(new
                {
                    success = false,
                    message = $"Total Debit ({totalDebit}) must equal Total Credit ({totalCredit})"
                });

            entry.Reference = request.Reference;
            entry.EntryDate = entryDate;
            entry.Description = request.Description;
            entry.EntryType = string.IsNullOrWhiteSpace(request.EntryType) ? "General" : request.EntryType;
            entry.PeriodId = period.Id;
            entry.TotalDebit = totalDebit;
            entry.TotalCredit = totalCredit;
            entry.DateMod = DateTime.UtcNow;
            entry.UpdatedByUserId = request.UpdatedByUserId;
            entry.UpdatedByUserName = request.UpdatedByUserName;

            var existingLines = entry.Lines
                .Where(x => !x.IsDeleted)
                .ToDictionary(x => x.Id);

            var retainedIds = new HashSet<Guid>();

            foreach (var dtoLine in request.Lines)
            {
                if (dtoLine.Id.HasValue && dtoLine.Id.Value != Guid.Empty)
                {
                    if (!existingLines.TryGetValue(dtoLine.Id.Value, out var existingLine))
                        return BadRequest(new
                        {
                            success = false,
                            message = $"Journal line {dtoLine.Id} does not belong to journal entry {id}"
                        });

                    existingLine.AccountId = dtoLine.AccountId;
                    existingLine.Direction = dtoLine.Direction;
                    existingLine.Amount = dtoLine.Amount;
                    existingLine.Description = dtoLine.Description;
                    existingLine.PeriodId = period.Id;
                    existingLine.DateMod = DateTime.UtcNow;
                    existingLine.IsDeleted = false;
                    retainedIds.Add(existingLine.Id);
                }
                else
                {
                    var newLine = new JournalLine
                    {
                        Id = Guid.CreateVersion7(),
                        JournalEntryId = entry.Id,
                        AccountId = dtoLine.AccountId,
                        Direction = dtoLine.Direction,
                        Amount = dtoLine.Amount,
                        Description = dtoLine.Description,
                        PeriodId = period.Id,
                        IsDeleted = false,
                        DateAdd = DateTime.UtcNow,
                        DateMod = null
                    };

                    entry.Lines.Add(newLine);
                    retainedIds.Add(newLine.Id);
                }
            }

            foreach (var existingLine in existingLines.Values)
            {
                if (!retainedIds.Contains(existingLine.Id))
                {
                    existingLine.IsDeleted = true;
                    existingLine.DateMod = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(ct);

            var savedLines = await _context.JournalLines
                .Where(x => x.JournalEntryId == entry.Id && !x.IsDeleted)
                .Include(x => x.Account)
                .ToListAsync(ct);

            _logger.LogInformation(
                "Draft journal entry {Reference} updated in place. Existing lines preserved; active lines: {Count}",
                entry.Reference,
                savedLines.Count);

            return Ok(new
            {
                success = true,
                message = "Draft journal entry updated successfully",
                data = new
                {
                    id = entry.Id,
                    reference = entry.Reference,
                    entryDate = entry.EntryDate,
                    description = entry.Description,
                    entryType = entry.EntryType,
                    totalDebit = entry.TotalDebit,
                    totalCredit = entry.TotalCredit,
                    isPosted = entry.IsPosted,
                    periodId = entry.PeriodId,
                    periodName = period.Name,
                    rowVersion = entry.RowVersion,
                    lines = savedLines.Select(x => new
                    {
                        id = x.Id,
                        accountId = x.AccountId,
                        accountCode = x.Account?.Code,
                        accountName = x.Account?.Name,
                        direction = x.Direction,
                        amount = x.Amount,
                        description = x.Description
                    })
                }
            });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating draft journal entry {Id}", id);
            return Conflict(new { success = false, message = "The journal entry was changed by another user. Please refresh and try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating draft journal entry {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
        }
    }
}

public sealed class DraftJournalEntryUpdateDto
{
    public string Reference { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public string? EntryType { get; set; }
    public Guid PeriodId { get; set; }
    public string? RowVersion { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public List<DraftJournalLineUpdateDto> Lines { get; set; } = new();
}

public sealed class DraftJournalLineUpdateDto
{
    public Guid? Id { get; set; }
    public Guid AccountId { get; set; }
    public string Direction { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
