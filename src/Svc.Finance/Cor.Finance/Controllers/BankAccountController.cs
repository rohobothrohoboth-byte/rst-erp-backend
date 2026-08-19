// Cor.Finance/Controllers/BankAccountController.cs
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
public class BankAccountController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<BankAccountController> _logger;

    public BankAccountController(IMediator mediator, ILogger<BankAccountController> logger)
        : base(mediator, logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // ============================================================
    // GET ENDPOINTS
    // ============================================================

    /// <summary>
    /// Get all bank accounts with pagination and filtering
    /// </summary>
    [HttpGet]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(PaginatedResponse<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isActive = null,
        [FromQuery] Guid? branchId = null,
        [FromQuery] string? accountType = null,
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "AccountName",
        [FromQuery] string? sortOrder = "ASC")
    {
        try
        {
            _logger.LogInformation("📊 Getting bank accounts - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var result = await _mediator.Send(new GetAllBankAccountsQry
            {
                IsActive = isActive,
                BranchId = branchId,
                AccountType = accountType,
                Search = search,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            });

            return Ok(new
            {
                success = true,
                data = result.Data,
                totalCount = result.TotalCount,
                page = result.Page,
                pageSize = result.PageSize,
                totalPages = result.TotalPages,
                hasNextPage = result.HasNextPage,
                hasPreviousPage = result.HasPreviousPage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting bank accounts");
            return HandleException(ex, "GetAllBankAccounts");
        }
    }

    /// <summary>
    /// Get bank account by ID
    /// </summary>
    [HttpGet("{id}")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("📄 Getting bank account by ID: {Id}", id);

            var result = await _mediator.Send(new GetBankAccountByIdQry { Id = id });

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
            _logger.LogError(ex, "❌ Error getting bank account by ID: {Id}", id);
            return HandleException(ex, "GetBankAccountById", id);
        }
    }

    /// <summary>
    /// Get bank account by account number
    /// </summary>
    [HttpGet("ByNumber/{accountNumber}")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByAccountNumber(string accountNumber)
    {
        try
        {
            _logger.LogInformation("📄 Getting bank account by number: {AccountNumber}", accountNumber);

            var result = await _mediator.Send(new GetBankAccountByNumberQry { AccountNumber = accountNumber });

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
            _logger.LogError(ex, "❌ Error getting bank account by number: {AccountNumber}", accountNumber);
            return HandleException(ex, "GetBankAccountByNumber", accountNumber);
        }
    }

    /// <summary>
    /// Get bank account balance with period filtering
    /// </summary>
    [HttpGet("{id}/balance")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(BankAccountBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalance(
        Guid id,
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? asOfDate = null)
    {
        try
        {
            _logger.LogInformation("💰 Getting balance for bank account: {Id}", id);

            var result = await _mediator.Send(new GetBankAccountBalanceQry
            {
                Id = id,
                PeriodId = periodId,
                AsOfDate = asOfDate
            });

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
            _logger.LogError(ex, "❌ Error getting bank account balance: {Id}", id);
            return HandleException(ex, "GetBankAccountBalance", id);
        }
    }

    /// <summary>
    /// Get bank account transaction summary
    /// </summary>
    [HttpGet("{id}/summary")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(BankAccountSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummary(
        Guid id,
        [FromQuery] Guid? periodId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            _logger.LogInformation("📊 Getting summary for bank account: {Id}", id);

            var result = await _mediator.Send(new GetBankAccountSummaryQry
            {
                Id = id,
                PeriodId = periodId,
                FromDate = fromDate,
                ToDate = toDate
            });

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
            _logger.LogError(ex, "❌ Error getting bank account summary: {Id}", id);
            return HandleException(ex, "GetBankAccountSummary", id);
        }
    }

    /// <summary>
    /// Get bank accounts by branch
    /// </summary>
    [HttpGet("ByBranch/{branchId}")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(List<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByBranch(
        Guid branchId,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            _logger.LogInformation("📊 Getting bank accounts for branch: {BranchId}", branchId);

            var result = await _mediator.Send(new GetBankAccountsByBranchQry
            {
                BranchId = branchId,
                IsActive = isActive
            });

            return Ok(new
            {
                success = true,
                data = result,
                count = result.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting bank accounts by branch: {BranchId}", branchId);
            return HandleException(ex, "GetBankAccountsByBranch", branchId);
        }
    }

    /// <summary>
    /// Get bank account types
    /// </summary>
    [HttpGet("Types")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccountTypes()
    {
        try
        {
            _logger.LogInformation("📋 Getting bank account types");

            var result = await _mediator.Send(new GetBankAccountTypesQry());

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting bank account types");
            return HandleException(ex, "GetBankAccountTypes");
        }
    }

    /// <summary>
    /// Get default bank accounts
    /// </summary>
    [HttpGet("Default")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(List<BankAccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDefaultAccounts()
    {
        try
        {
            _logger.LogInformation("🏦 Getting default bank accounts");

            var result = await _mediator.Send(new GetDefaultBankAccountsQry());

            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting default bank accounts");
            return HandleException(ex, "GetDefaultBankAccounts");
        }
    }

    /// <summary>
    /// Export bank accounts
    /// </summary>
    [HttpGet("Export")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Export(
        [FromQuery] bool? isActive = null,
        [FromQuery] Guid? branchId = null,
        [FromQuery] string format = "csv")
    {
        try
        {
            _logger.LogInformation("📤 Exporting bank accounts - Format: {Format}", format);

            var data = await _mediator.Send(new ExportBankAccountsQry
            {
                IsActive = isActive,
                BranchId = branchId
            });

            if (format.ToLower() == "csv")
            {
                var csv = ConvertToCsv(data);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"bank-accounts-{DateTime.UtcNow:yyyyMMdd}.csv");
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
                return File(bytes, "application/json", $"bank-accounts-{DateTime.UtcNow:yyyyMMdd}.json");
            }
            else if (format.ToLower() == "excel")
            {
                // You can add Excel export here using EPPlus or ClosedXML
                return BadRequest(new { success = false, message = "Excel export coming soon" });
            }

            return BadRequest(new { success = false, message = "Unsupported format. Use 'csv' or 'json'." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error exporting bank accounts");
            return HandleException(ex, "ExportBankAccounts");
        }
    }

    // ============================================================
    // POST ENDPOINTS
    // ============================================================

    /// <summary>
    /// Create a new bank account with period validation
    /// </summary>
    [HttpPost]
    [PerAuth("fnm.cash.bank.add")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] AddBankAccountDto dto)
    {
        try
        {
            _logger.LogInformation("📝 Creating bank account: {AccountName}", dto.AccountName);

            // ✅ Validate required fields
            if (string.IsNullOrWhiteSpace(dto.AccountName))
                return BadRequest(new { success = false, message = "Account name is required" });

            if (string.IsNullOrWhiteSpace(dto.AccountNumber))
                return BadRequest(new { success = false, message = "Account number is required" });

            if (string.IsNullOrWhiteSpace(dto.BankName))
                return BadRequest(new { success = false, message = "Bank name is required" });

            if (string.IsNullOrWhiteSpace(dto.AccountType))
                return BadRequest(new { success = false, message = "Account type is required" });

            // ✅ Validate period if provided
            if (dto.PeriodId.HasValue)
            {
                var periodValidation = await ValidatePeriod(dto.PeriodId.Value);
                if (periodValidation != null)
                    return periodValidation;
            }

            var result = await _mediator.Send(new AddBankAccountCmd { AddDto = dto });

            _logger.LogInformation("✅ Bank account created: {AccountName} (Id: {Id})", result.AccountName, result.Id);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
            {
                success = true,
                message = "Bank account created successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating bank account");
            return HandleException(ex, "CreateBankAccount");
        }
    }

    /// <summary>
    /// Bulk create bank accounts
    /// </summary>
    [HttpPost("Bulk")]
    [PerAuth("fnm.cash.bank.add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] List<AddBankAccountDto> dtos)
    {
        try
        {
            _logger.LogInformation("📝 Bulk creating {Count} bank accounts", dtos.Count);

            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No accounts provided" });

            // ✅ Validate all required fields
            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.AccountName))
                    return BadRequest(new { success = false, message = "Account name is required for all accounts" });

                if (string.IsNullOrWhiteSpace(dto.AccountNumber))
                    return BadRequest(new { success = false, message = "Account number is required for all accounts" });
            }

            var result = await _mediator.Send(new BulkCreateBankAccountsCmd { Dtos = dtos });

            _logger.LogInformation("✅ Bulk created {Created} bank accounts, {Failed} failed", result.CreatedCount, result.FailedCount);

            return Ok(new
            {
                success = true,
                created = result.CreatedCount,
                failed = result.FailedCount,
                errors = result.Errors
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk creating bank accounts");
            return HandleException(ex, "BulkCreateBankAccounts");
        }
    }

    // ============================================================
    // PUT ENDPOINTS
    // ============================================================

    /// <summary>
    /// Update a bank account with period validation
    /// </summary>
    [HttpPut]
    [PerAuth("fnm.cash.bank.mod")]
    [ProducesResponseType(typeof(BankAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromBody] EditBankAccountDto dto)
    {
        try
        {
            _logger.LogInformation("📝 Updating bank account: {Id} - {AccountName}", dto.Id, dto.AccountName);

            // ✅ Validate required fields
            if (dto.Id == Guid.Empty)
                return BadRequest(new { success = false, message = "Account ID is required" });

            if (string.IsNullOrWhiteSpace(dto.AccountName))
                return BadRequest(new { success = false, message = "Account name is required" });

            if (string.IsNullOrWhiteSpace(dto.AccountNumber))
                return BadRequest(new { success = false, message = "Account number is required" });

            // ✅ Validate period if changed
            if (dto.PeriodId.HasValue)
            {
                var periodValidation = await ValidatePeriod(dto.PeriodId.Value);
                if (periodValidation != null)
                    return periodValidation;
            }

            var result = await _mediator.Send(new EditBankAccountCmd { EditDto = dto });

            _logger.LogInformation("✅ Bank account updated: {AccountName} (Id: {Id})", result.AccountName, result.Id);

            return Ok(new
            {
                success = true,
                message = "Bank account updated successfully",
                data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating bank account: {Id}", dto.Id);
            return HandleException(ex, "UpdateBankAccount", dto.Id);
        }
    }

    // ============================================================
    // PATCH ENDPOINTS
    // ============================================================

    /// <summary>
    /// Toggle bank account active status
    /// </summary>
    [HttpPatch("{id}/toggle-active")]
    [PerAuth("fnm.cash.bank.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        try
        {
            _logger.LogInformation("🔄 Toggling bank account active status: {Id}", id);

            var result = await _mediator.Send(new ToggleBankAccountActiveCmd { Id = id });

            _logger.LogInformation("✅ Bank account status toggled: {Id} - IsActive: {IsActive}", id, result.IsActive);

            return Ok(new
            {
                success = true,
                message = result.IsActive ? "Bank account activated" : "Bank account deactivated",
                isActive = result.IsActive
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error toggling bank account active status: {Id}", id);
            return HandleException(ex, "ToggleBankAccountActive", id);
        }
    }

    /// <summary>
    /// Update bank account balance (manual adjustment)
    /// </summary>
    [HttpPatch("{id}/balance")]
    [PerAuth("fnm.cash.bank.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBalance(Guid id, [FromBody] UpdateBankAccountBalanceDto dto)
    {
        try
        {
            _logger.LogInformation("💰 Updating bank account balance: {Id} - New Balance: {NewBalance}", id, dto.NewBalance);

            if (id != dto.Id)
                return BadRequest(new { success = false, message = "ID mismatch" });

            if (dto.NewBalance < 0)
                return BadRequest(new { success = false, message = "Balance cannot be negative" });

            // ✅ Validate period
            if (dto.PeriodId.HasValue)
            {
                var periodValidation = await ValidatePeriod(dto.PeriodId.Value);
                if (periodValidation != null)
                    return periodValidation;
            }

            var result = await _mediator.Send(new UpdateBankAccountBalanceCmd { Dto = dto });

            _logger.LogInformation("✅ Bank account balance updated: {Id} - New Balance: {NewBalance}", id, dto.NewBalance);

            return Ok(new
            {
                success = true,
                message = "Bank account balance updated successfully",
                data = result
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
            _logger.LogError(ex, "❌ Error updating bank account balance: {Id}", id);
            return HandleException(ex, "UpdateBankAccountBalance", id);
        }
    }

    /// <summary>
    /// Set bank account as default
    /// </summary>
    [HttpPatch("{id}/set-default")]
    [PerAuth("fnm.cash.bank.mod")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        try
        {
            _logger.LogInformation("⭐ Setting bank account as default: {Id}", id);

            var result = await _mediator.Send(new SetDefaultBankAccountCmd { Id = id });

            _logger.LogInformation("✅ Bank account set as default: {Id}", id);

            return Ok(new
            {
                success = true,
                message = "Bank account set as default",
                data = result
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error setting bank account as default: {Id}", id);
            return HandleException(ex, "SetDefaultBankAccount", id);
        }
    }

    // ============================================================
    // DELETE ENDPOINTS
    // ============================================================

    /// <summary>
    /// Delete a bank account (soft delete) with validation
    /// </summary>
    [HttpDelete("{id}")]
    [PerAuth("fnm.cash.bank.del")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("🗑️ Deleting bank account: {Id}", id);

            // ✅ Check if account is default
            var account = await _mediator.Send(new GetBankAccountByIdQry { Id = id });
            if (account != null && account.IsDefault)
                return Conflict(new
                {
                    success = false,
                    message = "Cannot delete default bank account. Set another account as default first."
                });

            // ✅ Check if account has transactions
            var hasTransactions = await _mediator.Send(new BankAccountHasTransactionsQry { Id = id });
            if (hasTransactions)
                return Conflict(new
                {
                    success = false,
                    message = "Cannot delete bank account with existing transactions",
                    hint = "Archive the account instead"
                });

            var result = await _mediator.Send(new DeleteBankAccountCmd { Id = id });
            if (!result)
                return NotFound(new { success = false, message = $"Bank account with ID '{id}' not found" });

            _logger.LogInformation("✅ Bank account deleted: {Id}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting bank account: {Id}", id);
            return HandleException(ex, "DeleteBankAccount", id);
        }
    }

    /// <summary>
    /// Bulk delete bank accounts
    /// </summary>
    [HttpPost("BulkDelete")]
    [PerAuth("fnm.cash.bank.view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteDto dto)
    {
        try
        {
            _logger.LogInformation("🗑️ Bulk deleting {Count} bank accounts", dto.Ids.Count);

            if (dto.Ids == null || !dto.Ids.Any())
                return BadRequest(new { success = false, message = "No account IDs provided" });

            var result = await _mediator.Send(new BulkDeleteBankAccountsCmd { Ids = dto.Ids });

            _logger.LogInformation("✅ Bulk deleted {Deleted} bank accounts, {Failed} failed", result.DeletedCount, result.FailedCount);

            return Ok(new
            {
                success = true,
                deleted = result.DeletedCount,
                failed = result.FailedCount,
                errors = result.Errors
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error bulk deleting bank accounts");
            return HandleException(ex, "BulkDeleteBankAccounts");
        }
    }

    // ============================================================
    // PERIOD VALIDATION HELPER
    // ============================================================

    /// <summary>
    /// Validate if a period exists and is open
    /// </summary>
    private async Task<IActionResult?> ValidatePeriod(Guid periodId)
    {
        var period = await _mediator.Send(new GetPeriodByIdQry { Id = periodId });
        if (period == null)
            return BadRequest(new
            {
                success = false,
                message = $"Financial period with ID '{periodId}' not found"
            });

        if (period.IsClosed)
            return Conflict(new
            {
                success = false,
                message = $"Cannot perform operation in closed period: {period.Name}",
                periodId = period.Id,
                periodName = period.Name,
                hint = "Please reopen the period or select a different one"
            });

        if (period.Status == "LOCKED")
            return Conflict(new
            {
                success = false,
                message = $"Cannot perform operation in locked period: {period.Name}",
                periodId = period.Id,
                periodName = period.Name,
                hint = "Please unlock the period first"
            });

        return null;
    }

    // ============================================================
    // HELPER METHODS
    // ============================================================

    private string ConvertToCsv(List<BankAccountDto> accounts)
    {
        var sb = new System.Text.StringBuilder();

        // Header with all fields
        sb.AppendLine("Id,AccountName,AccountNumber,BankName,AccountType,GLCode,Currency,OpeningBalance,CurrentBalance,AvailableBalance,IsActive,IsDefault,BranchName,IBAN,SwiftCode,OverdraftLimit,IsReconciled,CreatedDate,SyncedAt");

        // Data
        foreach (var acc in accounts)
        {
            sb.AppendLine($"{acc.Id},{acc.AccountName},{acc.AccountNumber},{acc.BankName},{acc.AccountType},{acc.GLCode},{acc.Currency},{acc.OpeningBalance},{acc.CurrentBalance},{acc.AvailableBalance},{acc.IsActive},{acc.IsDefault},{acc.BranchName},{acc.IBAN},{acc.SwiftCode},{acc.OverdraftLimit},{acc.IsReconciled},{acc.DateAdd:yyyy-MM-dd HH:mm:ss},{acc.SyncedAt:yyyy-MM-dd HH:mm:ss}");
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