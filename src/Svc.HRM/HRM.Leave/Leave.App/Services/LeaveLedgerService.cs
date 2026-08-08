using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Leave.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dapper;

namespace Leave.App.Services;

/// <summary>
/// Service for managing leave ledger entries including credit/debit operations and queries
/// </summary>
public sealed class LeaveLedgerService : ILeaveLedgerService
{
    private readonly IUnitOfWork _uow;
    private readonly IDapperHelper _dapper;
    private readonly ILogger<LeaveLedgerService> _logger;

    public LeaveLedgerService(
        IUnitOfWork uow,
        IDapperHelper dapper,
        ILogger<LeaveLedgerService> logger)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _dapper = dapper ?? throw new ArgumentNullException(nameof(dapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ============================================================
    // CREDIT / DEBIT OPERATIONS
    // ============================================================

    /// <summary>
    /// Credits (adds) leave days to an employee's ledger
    /// </summary>
    public async Task Credit(LedgerEntryDto dto, CancellationToken ct)
        => await WriteLedgerEntry(dto, Math.Abs(dto.Amount), ct);

    /// <summary>
    /// Debits (subtracts) leave days from an employee's ledger
    /// </summary>
    public async Task Debit(LedgerEntryDto dto, CancellationToken ct)
        => await WriteLedgerEntry(dto, -Math.Abs(dto.Amount), ct);

    /// <summary>
    /// Writes a ledger entry with the specified amount
    /// </summary>
    private async Task WriteLedgerEntry(LedgerEntryDto dto, double amount, CancellationToken ct)
    {
        var entry = new LeaveLedger
        {
            Id = Guid.NewGuid(),
            Date = dto.Date,
            Amount = amount,
            EntryType = dto.EntryType,
            SourceType = dto.SourceType,
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            LeavePolicyId = dto.LeavePolicyId,
            ReferenceId = dto.ReferenceId,
            DateAdd = DateTime.UtcNow,
            DateMod = DateTime.UtcNow,
            IsDeleted = false
        };

        await _uow.Add(entry, ct);
        _logger.LogDebug($"Ledger entry added: {entry.Id} for Employee {entry.EmployeeId}, Amount: {amount}");
    }

    // ============================================================
    // QUERY METHODS
    // ============================================================

    /// <summary>
    /// Gets all ledger entries for a specific employee
    /// </summary>
    public async Task<IEnumerable<LeaveLedger>> GetEmployeeLeaveLedgerAsync(string employeeId, CancellationToken ct = default)
    {
        var sql = @"
            SELECT * FROM ""LeaveLedger""
            WHERE ""EmployeeId"" = @EmployeeId
            AND ""IsDeleted"" = false
            ORDER BY ""Date"" DESC";

        return await _dapper.QueryAsync<LeaveLedger>(sql, new { EmployeeId = employeeId }, ct);
    }

    /// <summary>
    /// Gets the current (most recent) ledger entry for an employee
    /// </summary>
    public async Task<LeaveLedger?> GetCurrentLeaveLedgerAsync(string employeeId, CancellationToken ct = default)
    {
        var sql = @"
            SELECT * FROM ""LeaveLedger""
            WHERE ""EmployeeId"" = @EmployeeId
            AND ""IsDeleted"" = false
            AND ""Date"" >= @Today
            ORDER BY ""Date"" DESC
            LIMIT 1";

        return await _dapper.QueryFirstOrDefaultAsync<LeaveLedger>(sql,
            new { EmployeeId = employeeId, Today = DateTime.Today }, ct);
    }

    /// <summary>
    /// Gets a specific ledger entry by ID
    /// </summary>
    public async Task<LeaveLedger?> GetLeaveLedgerByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _uow.Set<LeaveLedger>()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    /// <summary>
    /// Gets all ledger entries for an employee filtered by leave type
    /// </summary>
    public async Task<IEnumerable<LeaveLedger>> GetEmployeeLeaveLedgerByTypeAsync(
        string employeeId,
        string leaveTypeId,
        CancellationToken ct = default)
    {
        var sql = @"
            SELECT * FROM ""LeaveLedger""
            WHERE ""EmployeeId"" = @EmployeeId
            AND ""LeaveTypeId"" = @LeaveTypeId
            AND ""IsDeleted"" = false
            ORDER BY ""Date"" DESC";

        return await _dapper.QueryAsync<LeaveLedger>(sql,
            new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId }, ct);
    }

    /// <summary>
    /// Gets a ledger entry for an employee by leave type and year
    /// </summary>
    public async Task<LeaveLedger?> GetLeaveLedgerByEmployeeAndTypeAsync(
        string employeeId,
        string leaveTypeId,
        DateTime yearStart,
        CancellationToken ct = default)
    {
        var sql = @"
            SELECT * FROM ""LeaveLedger""
            WHERE ""EmployeeId"" = @EmployeeId
            AND ""LeaveTypeId"" = @LeaveTypeId
            AND ""Date"" >= @YearStart
            AND ""IsDeleted"" = false
            ORDER BY ""Date"" DESC
            LIMIT 1";

        return await _dapper.QueryFirstOrDefaultAsync<LeaveLedger>(sql,
            new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId, YearStart = yearStart }, ct);
    }

    /// <summary>
    /// Gets the available leave days balance for an employee by leave type
    /// </summary>
    public async Task<double> GetAvailableLeaveDaysAsync(
        string employeeId,
        string leaveTypeId,
        CancellationToken ct = default)
    {
        var sql = @"
            SELECT COALESCE(SUM(""Amount""), 0)
            FROM ""LeaveLedger""
            WHERE ""EmployeeId"" = @EmployeeId
            AND ""LeaveTypeId"" = @LeaveTypeId
            AND ""IsDeleted"" = false";

        return await _dapper.ExecuteScalarAsync<double>(sql,
            new { EmployeeId = employeeId, LeaveTypeId = leaveTypeId }, ct);
    }
}