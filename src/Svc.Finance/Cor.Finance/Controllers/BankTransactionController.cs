// Cor.Finance/Controllers/BankTransactionController.cs
using Common;
using Cor.Finance.Commands;
using Cor.Finance.Models.DTOs;
using Cor.Finance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace Cor.Finance.Controllers;

[ApiController]
[Route("api/finance/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class BankTransactionController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<BankTransactionController> _logger;

    public BankTransactionController(IMediator mediator, ILogger<BankTransactionController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all bank transactions with filters
    /// </summary>
    [HttpGet]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(PaginatedResponse<BankTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? bankAccountId = null,
        [FromQuery] bool? isReconciled = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? transactionType = null,
        [FromQuery] string? paymentMethod = null,
        [FromQuery] string? status = null,
        [FromQuery] Guid? periodId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            _logger.LogInformation("📊 Getting bank transactions - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var result = await _mediator.Send(new GetAllBankTransactionsQry
            {
                BankAccountId = bankAccountId,
                IsReconciled = isReconciled,
                FromDate = fromDate,
                ToDate = toDate,
                TransactionType = transactionType,
                PaymentMethod = paymentMethod,
                Status = status,
                PeriodId = periodId,
                Page = page,
                PageSize = pageSize
            });

            return Ok(new
            {
                success = true,
                data = result,
                count = result.Count,
                page = page,
                pageSize = pageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting bank transactions");
            return HandleException(ex, "GetAllBankTransactions");
        }
    }

    /// <summary>
    /// Get bank transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(BankTransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("📄 Getting bank transaction by ID: {Id}", id);

            var result = await _mediator.Send(new GetBankTransactionByIdQry { Id = id });

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting bank transaction by ID: {Id}", id);
            return HandleException(ex, "GetBankTransactionById", id);
        }
    }

    /// <summary>
    /// Get recent transactions (for dashboard)
    /// </summary>
    [HttpGet("Recent")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(List<BankTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent(
        [FromQuery] Guid? bankAccountId = null,
        [FromQuery] int count = 10)
    {
        try
        {
            _logger.LogInformation("📊 Getting {Count} recent transactions", count);

            var result = await _mediator.Send(new GetRecentTransactionsQry
            {
                BankAccountId = bankAccountId,
                Count = count
            });

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting recent transactions");
            return HandleException(ex, "GetRecentTransactions");
        }
    }

    /// <summary>
    /// Get transactions by period
    /// </summary>
    [HttpGet("ByPeriod/{periodId:guid}")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(List<BankTransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPeriod(Guid periodId)
    {
        try
        {
            _logger.LogInformation("📊 Getting transactions for period: {PeriodId}", periodId);

            var result = await _mediator.Send(new GetBankTransactionsByPeriodQry { PeriodId = periodId });

            return Ok(new
            {
                success = true,
                data = result,
                count = result.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting transactions by period: {PeriodId}", periodId);
            return HandleException(ex, "GetBankTransactionsByPeriod", periodId);
        }
    }

    /// <summary>
    /// Get transaction statistics
    /// </summary>
    [HttpGet("Stats")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(TransactionStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(
        [FromQuery] Guid? bankAccountId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            _logger.LogInformation("📊 Getting transaction statistics");

            var result = await _mediator.Send(new GetTransactionStatsQry
            {
                BankAccountId = bankAccountId,
                FromDate = fromDate,
                ToDate = toDate
            });

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting transaction statistics");
            return HandleException(ex, "GetTransactionStats");
        }
    }

    /// <summary>
    /// Get reconciliation summary
    /// </summary>
    [HttpGet("ReconciliationSummary")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(ReconciliationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReconciliationSummary(
        [FromQuery] Guid? bankAccountId = null,
        [FromQuery] Guid? periodId = null)
    {
        try
        {
            _logger.LogInformation("📊 Getting reconciliation summary");

            var result = await _mediator.Send(new GetReconciliationSummaryQry
            {
                BankAccountId = bankAccountId,
                PeriodId = periodId
            });

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting reconciliation summary");
            return HandleException(ex, "GetReconciliationSummary");
        }
    }

    /// <summary>
    /// Get daily transaction summary
    /// </summary>
    [HttpGet("DailySummary")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(List<DailyTransactionSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDailySummary(
        [FromQuery] Guid? bankAccountId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            _logger.LogInformation("📊 Getting daily transaction summary");

            var result = await _mediator.Send(new GetDailyTransactionSummaryQry
            {
                BankAccountId = bankAccountId,
                FromDate = fromDate,
                ToDate = toDate
            });

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting daily transaction summary");
            return HandleException(ex, "GetDailyTransactionSummary");
        }
    }

    /// <summary>
    /// Get transaction types
    /// </summary>
    [HttpGet("Types")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactionTypes()
    {
        try
        {
            _logger.LogInformation("📋 Getting transaction types");

            var result = await _mediator.Send(new GetTransactionTypesQry());

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting transaction types");
            return HandleException(ex, "GetTransactionTypes");
        }
    }

    /// <summary>
    /// Export transactions
    /// </summary>
    [HttpGet("Export")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] Guid? bankAccountId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string format = "csv")
    {
        try
        {
            _logger.LogInformation("📤 Exporting transactions - Format: {Format}", format);

            var data = await _mediator.Send(new ExportTransactionsQry
            {
                BankAccountId = bankAccountId,
                FromDate = fromDate,
                ToDate = toDate
            });

            if (format.ToLower() == "csv")
            {
                var csv = ConvertToCsv(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"transactions-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            else if (format.ToLower() == "json")
            {
                var json = System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = true,
                    data = data,
                    exportedAt = DateTime.UtcNow,
                    totalCount = data.Count
                }, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                return File(bytes, "application/json", $"transactions-{DateTime.UtcNow:yyyyMMdd}.json");
            }

            return BadRequest(new { success = false, message = "Unsupported format. Use 'csv' or 'json'." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error exporting transactions");
            return HandleException(ex, "ExportTransactions");
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new bank transaction
    /// </summary>
    [HttpPost]
    [PerAuth("fnm.cash.transaction.add")]
    [ProducesResponseType(typeof(BankTransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AddBankTransactionDto dto)
    {
        try
        {
            _logger.LogInformation("📝 Creating bank transaction for account: {BankAccountId}", dto.BankAccountId);

            // ✅ Validate required fields
            if (dto.BankAccountId == Guid.Empty)
                return BadRequest(new { success = false, message = "Bank account ID is required" });

            if (dto.PeriodId == Guid.Empty)
                return BadRequest(new { success = false, message = "PeriodId is required" });

            if (dto.Amount <= 0)
                return BadRequest(new { success = false, message = "Amount must be greater than 0" });

            if (string.IsNullOrWhiteSpace(dto.TransactionType))
                return BadRequest(new { success = false, message = "Transaction type is required" });

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest(new { success = false, message = "Description is required" });

            var result = await _mediator.Send(new AddBankTransactionCmd { AddDto = dto });

            _logger.LogInformation("✅ Bank transaction created: {Id} - {Amount}", result.Id, result.Amount);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Bank transaction created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "⚠️ Validation error creating bank transaction");
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating bank transaction");
            return HandleException(ex, "CreateBankTransaction");
        }
    }

    /// <summary>
    /// Bulk reconcile transactions
    /// </summary>
    [HttpPost("BulkReconcile")]
    [PerAuth("fnm.cash.transaction.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkReconcile([FromBody] BulkReconcileDto dto)
    {
        try
        {
            _logger.LogInformation("🔄 Bulk reconciling {Count} transactions", dto.TransactionIds?.Count ?? 0);

            if (dto.TransactionIds == null || !dto.TransactionIds.Any())
                return BadRequest(new { success = false, message = "No transaction IDs provided" });

            var result = await _mediator.Send(new BulkReconcileCmd { Dto = dto });

            _logger.LogInformation("✅ Bulk reconciled {Count} transactions", result.ReconciledCount);

            return Ok(new
            {
                success = true,
                message = $"{result.ReconciledCount} transactions reconciled successfully",
                reconciledCount = result.ReconciledCount,
                failedCount = result.FailedCount,
                errors = result.Errors
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk reconciling transactions");
            return HandleException(ex, "BulkReconcile");
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a bank transaction
    /// </summary>
    [HttpPut]
    [PerAuth("fnm.cash.transaction.mod")]
    [ProducesResponseType(typeof(BankTransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] EditBankTransactionDto dto)
    {
        try
        {
            _logger.LogInformation("📝 Updating bank transaction: {Id}", dto.Id);

            // ✅ Validate required fields
            if (dto.Id == Guid.Empty)
                return BadRequest(new { success = false, message = "Transaction ID is required" });

            if (dto.PeriodId == Guid.Empty)
                return BadRequest(new { success = false, message = "PeriodId is required" });

            if (dto.Amount <= 0)
                return BadRequest(new { success = false, message = "Amount must be greater than 0" });

            if (string.IsNullOrWhiteSpace(dto.TransactionType))
                return BadRequest(new { success = false, message = "Transaction type is required" });

            var result = await _mediator.Send(new EditBankTransactionCmd { EditDto = dto });

            _logger.LogInformation("✅ Bank transaction updated: {Id}", result.Id);

            return Ok(new
            {
                success = true,
                message = "Bank transaction updated successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("reconciled"))
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating bank transaction: {Id}", dto.Id);
            return HandleException(ex, "UpdateBankTransaction", dto.Id);
        }
    }

    // ============================================================
    // PATCH ENDPOINTS
    // ============================================================

    /// <summary>
    /// Reconcile a transaction
    /// </summary>
    [HttpPatch("{id}/reconcile")]
    [PerAuth("fnm.cash.transaction.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reconcile(Guid id, [FromBody] ReconcileBankTransactionDto dto)
    {
        try
        {
            _logger.LogInformation("🔄 Reconciling transaction: {Id}", id);

            if (id != dto.Id)
                return BadRequest(new { success = false, message = "ID mismatch" });

            var result = await _mediator.Send(new ReconcileBankTransactionCmd { ReconcileDto = dto });

            if (!result)
                return HandleNotFound("Bank Transaction", id);

            _logger.LogInformation("✅ Transaction reconciled: {Id}", id);

            return Ok(new
            {
                success = true,
                message = dto.IsReconciled ? "Transaction reconciled successfully" : "Transaction unreconciled successfully",
                isReconciled = dto.IsReconciled
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error reconciling transaction: {Id}", id);
            return HandleException(ex, "ReconcileBankTransaction", id);
        }
    }

    /// <summary>
    /// Void a transaction
    /// </summary>
    [HttpPatch("{id}/void")]
    [PerAuth("fnm.cash.transaction.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Void(Guid id, [FromBody] VoidTransactionDto? dto = null)
    {
        try
        {
            _logger.LogInformation("🚫 Voiding transaction: {Id}", id);

            var result = await _mediator.Send(new VoidTransactionCmd
            {
                Id = id,
                Dto = dto ?? new VoidTransactionDto { Reason = "Voided by user" }
            });

            _logger.LogInformation("✅ Transaction voided: {Id}", id);

            return Ok(new
            {
                success = true,
                message = "Transaction voided successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("reconciled"))
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error voiding transaction: {Id}", id);
            return HandleException(ex, "VoidBankTransaction", id);
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a bank transaction
    /// </summary>
    [HttpDelete("{id}")]
    [PerAuth("fnm.cash.transaction.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("🗑️ Deleting bank transaction: {Id}", id);

            var result = await _mediator.Send(new DeleteBankTransactionCmd { Id = id });

            if (!result)
                return HandleNotFound("Bank Transaction", id);

            _logger.LogInformation("✅ Bank transaction deleted: {Id}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("reconciled"))
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting bank transaction: {Id}", id);
            return HandleException(ex, "DeleteBankTransaction", id);
        }
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private string ConvertToCsv(List<BankTransactionDto> transactions)
    {
        var sb = new System.Text.StringBuilder();

        // Header with all fields
        sb.AppendLine("Date,Type,Description,Amount,PaymentMethod,CheckNumber,BankReference,Status,Reference,Reconciled,ReconciliationDate,Period");

        // Data
        foreach (var t in transactions)
        {
            sb.AppendLine($"{t.TransactionDate:yyyy-MM-dd},{t.TransactionType},{t.Description},{t.Amount},{t.PaymentMethod},{t.CheckNumber},{t.BankReference},{t.Status},{t.Reference},{t.IsReconciled},{t.ReconciliationDate?.ToString("yyyy-MM-dd")},{t.PeriodName}");
        }

        return sb.ToString();
    }

    private IActionResult HandleNotFound(string entityName, Guid id)
    {
        return NotFound(new
        {
            success = false,
            message = $"{entityName} with ID '{id}' not found"
        });
    }
}