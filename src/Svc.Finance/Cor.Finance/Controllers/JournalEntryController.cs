using Common;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Shared.Helpers.Services;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class JournalEntryController : BaseApiController
{
    private readonly CachedReferenceDataService _cachedService;

    public JournalEntryController(
        IMediator mediator,
        ILogger<JournalEntryController> logger,
        CachedReferenceDataService cachedService)
        : base(mediator, logger)
    {
        _cachedService = cachedService;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all journal entries with pagination (CACHED)
    /// </summary>
    [HttpGet("All")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(PagedResult<JournalEntryDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "page", "pageSize", "sortBy", "sortOrder", "fromDate", "toDate", "isPosted", "entryType", "periodId", "minAmount", "maxAmount" })]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "EntryDate",
        [FromQuery] string? sortOrder = "DESC",
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] bool? isPosted = null,
        [FromQuery] string? entryType = null,
        [FromQuery] Guid? periodId = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null)
    {
        try
        {
            // ? Use paginated query with caching
            var result = await Mediator.Send(new GetAllJournalEntriesQry
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                FromDate = fromDate,
                ToDate = toDate,
                IsPosted = isPosted,
                EntryType = entryType,
                PeriodId = periodId,
                MinAmount = minAmount,
                MaxAmount = maxAmount
            });

           // _logger.LogInformation("? Retrieved {Count} journal entries (Page {Page}/{TotalPages})",
             //   result.Data.Count, result.Page, result.TotalPages);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetAllJournalEntries");
        }
    }

    /// <summary>
    /// Get unposted journal entries
    /// </summary>
    [HttpGet("Unposted")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(List<JournalEntryDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetUnposted()
    {
        try
        {
            var result = await Mediator.Send(new GetUnpostedJournalEntriesQry());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetUnpostedJournalEntries");
        }
    }

    /// <summary>
    /// Get journal entries by period
    /// </summary>
    [HttpGet("ByPeriod/{periodId}")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(List<JournalEntryDto>), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetByPeriod(Guid periodId)
    {
        try
        {
            var result = await Mediator.Send(new GetJournalEntriesByPeriodQry { PeriodId = periodId });
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetJournalEntriesByPeriod", periodId);
        }
    }

    /// <summary>
    /// Get journal entry by reference
    /// </summary>
    [HttpGet("ByReference/{reference}")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetByReference(string reference)
    {
        try
        {
            var result = await Mediator.Send(new GetJournalEntryByReferenceQry { Reference = reference });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetJournalEntryByReference", reference);
        }
    }

    /// <summary>
    /// Get journal entry by ID
    /// </summary>
    [HttpGet("{id}")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new GetJournalEntryByIdQry { Id = id });
            if (result == null)
                return HandleNotFound("Journal Entry", id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetJournalEntryById", id);
        }
    }

    /// <summary>
    /// Get journal entry summary (CACHED)
    /// </summary>
    [HttpGet("Summary")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(JournalEntrySummaryDto), StatusCodes.Status200OK)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "periodId", "fromDate", "toDate" })]
    public async Task<IActionResult> GetSummary(
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var result = await Mediator.Send(new GetJournalEntrySummaryQry
            {
                PeriodId = periodId,
                FromDate = fromDate,
                ToDate = toDate
            });

           // _logger.LogInformation("? Retrieved journal summary: {TotalEntries} entries, {PostedEntries} posted",
              //  result.TotalEntries, result.PostedEntries);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "GetJournalEntrySummary");
        }
    }

    /// <summary>
    /// Export journal entries
    /// </summary>
    [HttpGet("Export")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string format = "csv")
    {
        try
        {
            var data = await Mediator.Send(new ExportJournalEntriesQry
            {
                PeriodId = periodId,
                FromDate = fromDate,
                ToDate = toDate
            });

            if (format.ToLower() == "csv")
            {
                var csv = ConvertJournalEntriesToCsv(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"journal-entries-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            else if (format.ToLower() == "json")
            {
                var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", $"journal-entries-{DateTime.UtcNow:yyyyMMdd}.json");
            }

            return BadRequest(new { message = "Unsupported format. Use 'csv' or 'json'." });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ExportJournalEntries");
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new journal entry
    /// </summary>
    [HttpPost]
    [PerAuth("fnm.gl.journal.add")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddJournalEntryDto dto)
    {
        try
        {
            // ? FIX: Check nullable PeriodId
            if (!dto.PeriodId.HasValue || dto.PeriodId.Value == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            // Validate debits = credits
            var totalDebit = dto.Lines.Where(l => l.Direction == "Debit").Sum(l => l.Amount);
            var totalCredit = dto.Lines.Where(l => l.Direction == "Credit").Sum(l => l.Amount);

            if (totalDebit != totalCredit)
                return HandleBadRequest($"Debits ({totalDebit}) must equal Credits ({totalCredit})");

            var result = await Mediator.Send(new AddJournalEntryCmd { AddDto = dto });

            // ? Invalidate journal caches after creating
            await InvalidateJournalCachesAsync();

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Journal entry created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "CreateJournalEntry");
        }
    }

    /// <summary>
    /// Post a journal entry
    /// </summary>
    [HttpPost("{id}/post")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new PostJournalEntryCmd { Id = id });
            if (!result)
                return HandleNotFound("Journal Entry", id);

            // ? Invalidate journal caches after posting
            await InvalidateJournalCachesAsync();

            return SuccessResponse("Journal entry posted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "PostJournalEntry", id);
        }
    }

    /// <summary>
    /// Unpost a journal entry
    /// </summary>
    [HttpPost("{id}/unpost")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Unpost(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new UnpostJournalEntryCmd { Id = id });
            if (!result)
                return HandleNotFound("Journal Entry", id);

            // ? Invalidate journal caches after unposting
            await InvalidateJournalCachesAsync();

            return SuccessResponse("Journal entry unposted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UnpostJournalEntry", id);
        }
    }

    /// <summary>
    /// Approve a journal entry
    /// </summary>
    [HttpPost("{id}/approve")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new ApproveJournalEntryCmd { Id = id });
            if (!result)
                return HandleNotFound("Journal Entry", id);

            // ? Invalidate journal caches after approving
            await InvalidateJournalCachesAsync();

            return SuccessResponse("Journal entry approved successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ApproveJournalEntry", id);
        }
    }

    /// <summary>
    /// Reject a journal entry
    /// </summary>
    [HttpPost("{id}/reject")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectJournalEntryDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.Reason))
                return HandleBadRequest("Rejection reason is required");

            var result = await Mediator.Send(new RejectJournalEntryCmd {
                Id = id,
                Reason = dto.Reason
            });
            if (!result)
                return HandleNotFound("Journal Entry", id);

            // ? Invalidate journal caches after rejecting
            await InvalidateJournalCachesAsync();

            return SuccessResponse("Journal entry rejected successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "RejectJournalEntry", id);
        }
    }

    /// <summary>
    /// Reverse a journal entry
    /// </summary>
    [HttpPost("{id}/reverse")]
    [PerAuth("fnm.gl.journal.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reverse(Guid id, [FromBody] ReverseJournalEntryDto dto)
    {
        try
        {
            var result = await Mediator.Send(new ReverseJournalEntryCmd {
                Id = id,
                Reason = dto.Reason,
                ReverseDate = dto.ReverseDate ?? DateTime.UtcNow
            });
            if (result == null)
                return HandleNotFound("Journal Entry", id);

            // ? Invalidate journal caches after reversing
            await InvalidateJournalCachesAsync();

            return Ok(new {
                success = true,
                message = "Journal entry reversed successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "ReverseJournalEntry", id);
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a journal entry
    /// </summary>
    [HttpPut]
    [PerAuth("fnm.gl.journal.mod")]
    [ProducesResponseType(typeof(JournalEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditJournalEntryDto dto)
    {
        try
        {
            // ? FIX: Check nullable PeriodId
            if (!dto.PeriodId.HasValue || dto.PeriodId.Value == Guid.Empty)
                return HandleBadRequest("PeriodId is required");

            if (dto.Id == Guid.Empty)
                return HandleBadRequest("Journal entry ID is required");

            if (dto.Lines == null || !dto.Lines.Any())
                return HandleBadRequest("Journal entry must have at least one line");

            var totalDebit = dto.Lines.Where(l => l.Direction == "Debit").Sum(l => l.Amount);
            var totalCredit = dto.Lines.Where(l => l.Direction == "Credit").Sum(l => l.Amount);

            if (totalDebit != totalCredit)
                return HandleBadRequest($"Debits ({totalDebit}) must equal Credits ({totalCredit})");

            var result = await Mediator.Send(new EditJournalEntryCmd { EditDto = dto });
            if (result == null)
                return HandleNotFound("Journal Entry", dto.Id);

            // ? Invalidate journal caches after updating
            await InvalidateJournalCachesAsync();

            return Ok(new { success = true, message = "Journal entry updated successfully", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "UpdateJournalEntry", dto.Id);
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a journal entry
    /// </summary>
    [HttpDelete("{id}")]
    [PerAuth("fnm.gl.journal.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await Mediator.Send(new DeleteJournalEntryCmd { Id = id });
            if (!result)
                return HandleNotFound("Journal Entry", id);

            // ? Invalidate journal caches after deleting
            await InvalidateJournalCachesAsync();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "DeleteJournalEntry", id);
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private async Task InvalidateJournalCachesAsync()
    {
        // ? Invalidate journal entry caches
        await _cachedService.InvalidateReferenceDataAsync();

        // You can add more specific cache invalidation here
        // For example, if you have specific cache keys for journal entries
        // _cache.Remove("journal:list:*");
        // _cache.Remove("journal:summary:*");

        //_logger.LogInformation("??? Journal caches invalidated");
    }

    private string ConvertJournalEntriesToCsv(List<JournalEntryDto> entries)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Reference,EntryDate,EntryType,Description,TotalDebit,TotalCredit,IsPosted,PeriodName,CreatedDate");

        foreach (var entry in entries)
        {
            sb.AppendLine($"{entry.Id},{entry.Reference},{entry.EntryDate:yyyy-MM-dd},{entry.EntryType},{entry.Description},{entry.TotalDebit},{entry.TotalCredit},{entry.IsPosted},{entry.PeriodName},{entry.DateAdd:yyyy-MM-dd HH:mm:ss}");
        }

        return sb.ToString();
    }
}