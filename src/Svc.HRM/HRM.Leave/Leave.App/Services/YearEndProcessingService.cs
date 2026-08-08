// Leave.App/Services/YearEndProcessingService.cs
// Leave.App/Services/YearEndProcessingService.cs

using Leave.Domain.Entities;
using Leave.App.Interfaces;
using Leave.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Helpers;
using System.Linq;
using System.Data;

namespace Leave.App.Services;

public interface IYearEndProcessingService
{
    Task<YearEndProcessResultDto> ProcessYearEndAsync(YearEndProcessRequestDto request, CancellationToken ct);
    Task<List<CarryoverCalculationDto>> PreviewCarryoverCalculationsAsync(Guid fiscalYearId, CancellationToken ct);
    Task<Dictionary<Guid, double>> GetMaxCarryoverDaysAsync(CancellationToken ct);
    Task<bool> CanProcessYearEnd(Guid fiscalYearId, CancellationToken ct);
    Task<List<CarryoverHistoryDto>> GetCarryoverHistoryAsync(int year, CancellationToken ct);
    Task<YearEndProcessResultDto> RevertYearEndProcessingAsync(int fiscalYear, CancellationToken ct);
    Task<List<FiscalYearDto>> GetAvailableFiscalYearsAsync(CancellationToken ct);
    Task<object> DebugCheckCarryoverPolicies(CancellationToken ct);
    Task<Dictionary<Guid, double>> GetMaxAccrualAsync(CancellationToken ct);
    Task<EncashmentResultDto> ProcessEncashmentAsync(EncashmentRequestDto request, CancellationToken ct);
    Task<List<EncashmentHistoryDto>> GetEncashmentHistoryAsync(Guid employeeId, CancellationToken ct);
    Task<Dictionary<Guid, EncashmentConfigDto>> GetEncashmentConfigsAsync(CancellationToken ct);
    Task<decimal> GetTotalEncashedDaysAsync(Guid employeeId, int fiscalYear, CancellationToken ct);
    Task DeleteEncashmentsByYearAsync(int fiscalYear, CancellationToken ct);
    Task<List<EncashmentHistoryDto>> GetAllEncashmentHistoryAsync(Guid? fiscalYearId, CancellationToken ct);
    Task<FiscalYearDto?> GetFiscalYearByIdAsync(Guid fiscalYearId, CancellationToken ct);
    Task<bool> IsYearEndProcessedAsync(string? fiscalYearName, CancellationToken ct);

    Task<List<PendingEncashmentApprovalDto>> GetPendingEncashmentApprovalsAsync(string approverId, string approvalLevel, CancellationToken ct);
    Task<ProcessResultDto> ProcessEncashmentApprovalAsync(Guid requestId, EncashmentApprovalDto approval, CancellationToken ct);
    Task<EncashmentApprovalDetailsDto> GetEncashmentApprovalDetailsAsync(Guid requestId, CancellationToken ct);


}
public class YearEndProcessingService : IYearEndProcessingService
{
    private readonly IUnitOfWork _uow;
    private readonly ILeaveLedgerService _leaveLedgerService;
    private readonly ILogger<YearEndProcessingService> _logger;
    private readonly IDapperHelper _leaveDapper;
    private readonly ICoreDapperHelper _coreDapper;  // Use interface
     private readonly IHistoryRepository _historyRepository;
     private readonly IHrmProDapperHelper _hrmProDapper;
    public YearEndProcessingService(
        IUnitOfWork uow,
        ILeaveLedgerService leaveLedgerService,
         IHrmProDapperHelper hrmProDapper,
        ILogger<YearEndProcessingService> logger,
        IDapperHelper leaveDapper,
        ICoreDapperHelper coreDapper,IHistoryRepository historyRepository)  // Use interface
    {
        _uow = uow;
        _leaveLedgerService = leaveLedgerService;
        _logger = logger;
        _leaveDapper = leaveDapper;
          _hrmProDapper = hrmProDapper;
        _coreDapper = coreDapper;
        _historyRepository=historyRepository;
    }
public async Task<object> DebugCheckCarryoverPolicies(CancellationToken ct)
{
    var policies = await _uow.Set<EmpLeavePolicy>()
        .Where(p => p.AssignmentReason != null && p.AssignmentReason.Contains("Carryover"))
        .ToListAsync(ct);

    return new { count = policies.Count, policies = policies.Select(p => new { p.Id, p.AssignmentReason, p.AssignedEntitlement, p.EffectiveFrom }) };
}
    public async Task<List<FiscalYearDto>> GetAvailableFiscalYearsAsync(CancellationToken ct)
    {
        const string sql = @"
            SELECT
                ""Id"",
                ""Name"",
                ""DateStart"",
                ""DateEnd"",
                ""IsActive"",
                ""DateAdd"",
                ""DateMod"",
                ""IsDeleted""
            FROM ""FiscalYear""
            WHERE ""IsDeleted"" = false
            ORDER BY ""DateStart"" DESC";

        var fiscalYears = await _coreDapper.QueryAsync<FiscalYearDto>(sql, null, ct);
        return fiscalYears.ToList();
    }


    public async Task<bool> CanProcessYearEnd(Guid fiscalYearId, CancellationToken ct)
    {
        // Use Core database connection
        const string sql = @"
            SELECT
                ""Id"",
                ""Name"",
                ""DateStart"",
                ""DateEnd"",
                ""IsActive""
            FROM ""FiscalYear""
            WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

        var fiscalYear = await _coreDapper.QueryFirstOrDefaultAsync<FiscalYearDto>(sql, new { Id = fiscalYearId }, ct);

        if (fiscalYear == null)
        {
            _logger.LogWarning($"Fiscal year {fiscalYearId} not found");
            return false;
        }

        var today = DateTime.UtcNow;
        var yearEnd = fiscalYear.DateEnd;

        if (today < yearEnd)
        {
            _logger.LogWarning($"Cannot process year-end for fiscal year {fiscalYear.Name}. Current date {today} is before year-end {yearEnd}");
            return false;
        }

        // Check if already processed (Leave database)
        var existingPolicies = await _uow.Set<EmpLeavePolicy>()
            .AnyAsync(ep => ep.AssignmentReason == $"Carryover from {fiscalYear.Name}" && !ep.IsDeleted, ct);

        if (existingPolicies)
        {
            _logger.LogWarning($"Year-end for fiscal year {fiscalYear.Name} has already been processed");
            return false;
        }

        return true;
    }
  public async Task<Dictionary<Guid, double>> GetMaxCarryoverDaysAsync(CancellationToken ct)
  {
      var result = new Dictionary<Guid, double>();

      // PRIORITY 1: LeavePolicyConfig (most specific)
      var activeConfigs = await _uow.Set<LeavePolicyConfig>()
          .Where(c => c.IsActive && !c.IsDeleted && c.MaxCarryOverDays > 0)
          .Include(c => c.LeavePolicy)
          .ToListAsync(ct);

      foreach (var config in activeConfigs)
      {
          if (config.LeavePolicy != null)
          {
              result[config.LeavePolicy.LeaveTypeId] = (double)config.MaxCarryOverDays;
              _logger.LogDebug($"From PolicyConfig: LeaveType {config.LeavePolicy.LeaveTypeId} = {config.MaxCarryOverDays}");
          }
      }

      // PRIORITY 2: LeaveType (fallback for leave types without specific policy config)
      // Fix: Load all leave types first, then filter in memory
      var allLeaveTypes = await _uow.Set<LeaveType>()
          .Where(lt => !lt.IsDeleted && lt.AllowCarryover && lt.MaxCarryoverDays > 0)
          .ToListAsync(ct);

      foreach (var leaveType in allLeaveTypes)
      {
          if (!result.ContainsKey(leaveType.Id))
          {
              result[leaveType.Id] = (double)leaveType.MaxCarryoverDays;
              _logger.LogDebug($"From LeaveType: {leaveType.Name} = {leaveType.MaxCarryoverDays}");
          }
      }

      _logger.LogInformation($"Found {result.Count} leave types with carryover configured");
      return result;
  }


public async Task<List<CarryoverCalculationDto>> PreviewCarryoverCalculationsAsync(Guid fiscalYearId, CancellationToken ct)
{
    var results = new List<CarryoverCalculationDto>();
    var maxCarryoverMap = await GetMaxCarryoverDaysAsync(ct);

    // Get fiscal year
    const string fiscalYearSql = @"
        SELECT
            ""Id"",
            ""Name"",
            ""DateStart"",
            ""DateEnd"",
            CASE WHEN ""IsActive"" = '1' THEN true ELSE false END as ""IsActive""
        FROM ""FiscalYear""
        WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

    var fiscalYear = await _coreDapper.QueryFirstOrDefaultAsync<FiscalYearDto>(fiscalYearSql, new { Id = fiscalYearId }, ct);

    if (fiscalYear == null)
    {
        throw new DomainException($"Fiscal year with ID {fiscalYearId} not found");
    }

    var yearEnd = fiscalYear.DateEnd;
    var fiscalYearName = fiscalYear.Name;

    // Get employee policies
    var empPolicies = await _uow.Set<EmpLeavePolicy>()
        .Where(ep => ep.EffectiveFrom <= yearEnd
            && (ep.EffectiveTo == null || ep.EffectiveTo >= yearEnd)
            && !ep.IsDeleted)
        .Include(ep => ep.LeaveType)
        .ToListAsync(ct);

    // Filter: Only include policies where LeaveType allows carryover
    empPolicies = empPolicies
        .Where(ep => ep.LeaveType != null && ep.LeaveType.AllowCarryover)
        .ToList();

    _logger.LogInformation($"Found {empPolicies.Count} employee policies for leave types that allow carryover");

    var leaveTypes = await _uow.Set<LeaveType>()
        .Where(lt => !lt.IsDeleted)
        .ToDictionaryAsync(lt => lt.Id, lt => lt.Name, ct);

    foreach (var policy in empPolicies)
    {
        double remainingBalance = (double)(policy.AssignedEntitlement - policy.UsedEntitlement);
        double maxCarryover = maxCarryoverMap.GetValueOrDefault(policy.LeaveTypeId, 0);
        double carryoverAmount = Math.Max(0, Math.Min(remainingBalance, maxCarryover));
        double lostAmount = remainingBalance - carryoverAmount > 0 ? remainingBalance - carryoverAmount : 0;

        var leaveTypeName = leaveTypes.GetValueOrDefault(policy.LeaveTypeId, "Unknown");

        results.Add(new CarryoverCalculationDto
        {
            EmployeeId = policy.EmployeeId,
            EmployeeName = policy.EmployeeId.ToString(), // Will be replaced by frontend
            LeaveTypeId = policy.LeaveTypeId,
            LeaveTypeName = leaveTypeName,
            RemainingBalance = (decimal)remainingBalance,
            MaxCarryoverDays = (decimal)maxCarryover,
            CarryoverAmount = (decimal)carryoverAmount,
            EncashmentAmount = 0,
            LostAmount = (decimal)lostAmount,
            FiscalYearName = fiscalYearName
        });
    }

    return results;
}
// In YearEndProcessingService.cs - Update the GetCarryoverHistoryAsync method

public async Task<List<CarryoverHistoryDto>> GetCarryoverHistoryAsync(int year, CancellationToken ct)
{
    var results = new List<CarryoverHistoryDto>();

    try
    {
        // Look for carryover policies by AssignmentReason, not by year
        var carryoverPolicies = await _uow.Set<EmpLeavePolicy>()
            .Where(ep => ep.AssignmentReason == $"Carryover from {year}"
                && !ep.IsDeleted)
            .Include(ep => ep.LeaveType)
            .ToListAsync(ct);

        var leaveTypes = await _uow.Set<LeaveType>()
            .Where(lt => !lt.IsDeleted)
            .ToDictionaryAsync(lt => lt.Id, lt => lt.Name, ct);

        foreach (var policy in carryoverPolicies)
        {
            var leaveTypeName = leaveTypes.GetValueOrDefault(policy.LeaveTypeId, "Unknown");

            results.Add(new CarryoverHistoryDto
            {
                Id = policy.Id,
                EmployeeId = policy.EmployeeId,
                EmployeeName = policy.EmployeeId.ToString(),
                LeaveTypeId = policy.LeaveTypeId,
                LeaveTypeName = leaveTypeName,
                CarryoverAmount = policy.CarryForward,
                EffectiveFrom = policy.EffectiveFrom,
                ProcessedAt = policy.DateAdd,
                ProcessedBy = policy.AssignmentReason
            });
        }

        _logger.LogInformation($"Retrieved {results.Count} carryover history records for year {year}");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting carryover history for year {Year}", year);
        throw;
    }

    return results;
}



public async Task<YearEndProcessResultDto> RevertYearEndProcessingAsync(int fiscalYear, CancellationToken ct)
{
    var result = new YearEndProcessResultDto
    {
        Success = true,
        ProcessedAt = DateTime.UtcNow,
        Message = "Year-end processing reverted successfully."
    };

    await _uow.Begin(ct);

    try
    {
        // Get history records for this fiscal year
        var historyRecords = await _historyRepository.GetHistoryByYearAsync(fiscalYear, ct);
        _logger.LogInformation($"Found {historyRecords.Count} history records for year {fiscalYear}");

        var nextYear = fiscalYear + 1;

        // Find and soft delete carryover policies created during processing
        var carryoverPolicies = await _uow.Set<EmpLeavePolicy>()
            .Where(ep => ep.EffectiveFrom.Year == nextYear
                && ep.AssignmentReason == $"Carryover from {fiscalYear}"
                && !ep.IsDeleted)
            .ToListAsync(ct);

        var policyIds = carryoverPolicies.Select(p => p.Id).ToList();

        var ledgerEntries = await _uow.Set<LeaveLedger>()
            .Where(ll => ll.SourceType == BoolToStr.EnumToString(LedgerSource.CarryOver)
                && ll.ReferenceId.HasValue
                && policyIds.Contains(ll.ReferenceId.Value)
                && !ll.IsDeleted)
            .ToListAsync(ct);

        _logger.LogInformation($"Found {carryoverPolicies.Count} carryover policies and {ledgerEntries.Count} ledger entries to delete");

        foreach (var ledger in ledgerEntries)
        {
            await _uow.Delete(ledger);
        }

        foreach (var policy in carryoverPolicies)
        {
            await _uow.Delete(policy);
        }

        // ========== DELETE ENCASHMENT RECORDS CREATED DURING YEAR-END ==========
        // Get encashment records created during the year-end process
        // These would have been created around the time of processing
        var encashmentRecords = await _uow.Set<LeaveEncashment>()
            .Where(e => !e.IsDeleted
                && e.DateAdd.Year == fiscalYear)
            .ToListAsync(ct);

        // Alternative: Get encashments created after a certain date
        // You can also add a reference to link encashment to year-end process

        _logger.LogInformation($"Found {encashmentRecords.Count} encashment records to revert");

        foreach (var encashment in encashmentRecords)
        {
            // Also delete associated ledger entries for encashment
            var encashmentLedgers = await _uow.Set<LeaveLedger>()
                .Where(ll => ll.SourceType == BoolToStr.EnumToString(LedgerSource.Encashment)
                    && ll.ReferenceId == encashment.Id)
                .ToListAsync(ct);

            foreach (var ledger in encashmentLedgers)
            {
                await _uow.Delete(ledger);
            }

            await _uow.Delete(encashment);
        }
        // ========== END DELETE ENCASHMENT RECORDS ==========

        // Restore each policy from history
        foreach (var history in historyRecords)
        {
            await _historyRepository.RestoreFromHistoryAsync(history.Id, ct);
            _logger.LogInformation($"Restored policy from history {history.Id}");
            result.CarryoverRecordsCreated++;
        }

        result.EmployeesProcessed = historyRecords.Count;

        // DELETE HISTORY AFTER SUCCESSFUL RESTORE
        foreach (var history in historyRecords)
        {
            await _historyRepository.DeleteHistoryAsync(history.Id, ct);
        }

        await _uow.Commit(ct);
        _logger.LogInformation($"Year-end processing reverted successfully for fiscal year {fiscalYear}. Restored {historyRecords.Count} policies from history, deleted {encashmentRecords.Count} encashment records.");
    }
    catch (Exception ex)
    {
        await _uow.Rollback(ct);
        result.Success = false;
        result.Message = $"Failed to revert year-end processing: {ex.Message}";
        result.Errors.Add(new YearEndProcessError { ErrorMessage = ex.Message });
        _logger.LogError(ex, "Failed to revert year-end processing for fiscal year {FiscalYear}", fiscalYear);
    }

    return result;
}

public async Task<Dictionary<Guid, double>> GetMaxAccrualAsync(CancellationToken ct)
{
    var result = new Dictionary<Guid, double>();

    var leaveTypes = await _uow.Set<LeaveType>()
        .Where(lt => !lt.IsDeleted && lt.MaxAccrual > 0)
        .ToListAsync(ct);

    foreach (var leaveType in leaveTypes)
    {
        result[leaveType.Id] = (double)leaveType.MaxAccrual;
        _logger.LogDebug($"LeaveType {leaveType.Name}: MaxAccrual = {leaveType.MaxAccrual}");
    }

    return result;
}

public async Task<YearEndProcessResultDto> ProcessYearEndAsync(YearEndProcessRequestDto request, CancellationToken ct)
{
    var result = new YearEndProcessResultDto
    {
        Success = true,
        ProcessedAt = DateTime.UtcNow,
        Message = "Year-end processing completed successfully."
    };

    _uow.DetachAllEntities();

    const string fiscalYearSql = @"
        SELECT
            ""Id"",
            ""Name"",
            ""DateStart"",
            ""DateEnd"",
            CASE WHEN ""IsActive"" = '1' THEN true ELSE false END as ""IsActive""
        FROM ""FiscalYear""
        WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

    var fiscalYear = await _coreDapper.QueryFirstOrDefaultAsync<FiscalYearDto>(fiscalYearSql, new { Id = request.FiscalYearId }, ct);

    if (fiscalYear == null)
    {
        result.Success = false;
        result.Message = $"Fiscal year with ID {request.FiscalYearId} not found";
        return result;
    }

    var yearEnd = fiscalYear.DateEnd;
    var nextYearStart = yearEnd.AddDays(1);
    var fiscalYearName = fiscalYear.Name;

    await _uow.Begin(ct);
    _logger.LogInformation("Transaction began");

    try
    {
        // ========== GET ENCASHMENT APPROVAL CHAIN DYNAMICALLY ==========
        // Get the approval chain for encashment (find chain associated with leave types that allow encashment)
        var encashmentChain = await _uow.Set<LeaveAppChain>()
            .FirstOrDefaultAsync(x => !x.IsDeleted, ct);

        // Or get chain by LeaveType or LeavePolicy if needed
        Guid? encashmentChainId = encashmentChain?.Id;
        List<LeaveAppStep> allSteps = new List<LeaveAppStep>();

        if (encashmentChainId.HasValue)
        {
            allSteps = await _uow.Set<LeaveAppStep>()
                .Where(x => x.LeaveAppChainId == encashmentChainId && !x.IsDeleted)
                .OrderBy(x => x.StepOrder)
                .ToListAsync(ct);

            _logger.LogInformation($"Found approval chain {encashmentChainId} with {allSteps.Count} steps");
        }
        else
        {
            _logger.LogWarning("No approval chain found for encashment. Encashments will be created with pending status but no workflow.");
        }

        var finalStepOrder = allSteps.Any() ? allSteps.Last().StepOrder : 1;
        // ========== END GET ENCASHMENT APPROVAL CHAIN ==========

        var empPolicies = await _uow.Set<EmpLeavePolicy>()
            .Where(ep => ep.EffectiveFrom <= yearEnd
                && (ep.EffectiveTo == null || ep.EffectiveTo >= yearEnd)
                && !ep.IsDeleted)
            .ToListAsync(ct);

        _logger.LogInformation($"Found {empPolicies.Count} employee policies");

        var maxCarryoverMap = await GetMaxCarryoverDaysAsync(ct);
        var maxAccrualMap = await GetMaxAccrualAsync(ct);

        // Get encashment configurations from LeavePolicy
        var encashmentConfigs = await GetEncashmentConfigsAsync(ct);

        // Get employee daily rates from Core database
        var employeeDailyRates = await GetEmployeeDailyRatesAsync(empPolicies.Select(p => p.EmployeeId).Distinct().ToList(), ct);

        // Load all policy configs once for efficiency
        var policyConfigs = await _uow.Set<LeavePolicyConfig>()
            .AsNoTracking()
            .Where(c => c.IsActive && !c.IsDeleted)
            .ToDictionaryAsync(c => c.LeavePolicyId, c => c, ct);

        // Load all leave types for fallback
        var leaveTypes = await _uow.Set<LeaveType>()
            .Where(lt => !lt.IsDeleted)
            .ToDictionaryAsync(lt => lt.Id, lt => lt, ct);

        // ========== ARCHIVE BEFORE PROCESSING ==========
        var policiesToArchive = empPolicies.Where(p =>
            maxCarryoverMap.GetValueOrDefault(p.LeaveTypeId, 0) > 0).ToList();

        if (policiesToArchive.Any())
        {
            await _historyRepository.ArchivePoliciesAsync(
                policiesToArchive,
                $"Year-end processing for {fiscalYearName} - Archived before carryover",
                request.ProcessedBy,
                int.Parse(fiscalYearName),
                ct);
            _logger.LogInformation($"Archived {policiesToArchive.Count} policies before processing");
        }

        var existingPoliciesForNextYear = await _uow.Set<EmpLeavePolicy>()
            .Where(ep => ep.EffectiveFrom.Year == nextYearStart.Year)
            .ToListAsync(ct);

        var existingPoliciesDict = existingPoliciesForNextYear
            .ToDictionary(ep => $"{ep.EmployeeId}|{ep.LeaveTypeId}|{ep.LeavePolicyId}", ep => ep);

        _logger.LogInformation($"Found {existingPoliciesDict.Count} existing policies for next year");

        var policiesToUpdate = new List<EmpLeavePolicy>();
        var policiesToAdd = new List<EmpLeavePolicy>();

        foreach (var currentPolicy in empPolicies)
        {
            double remainingBalance = (double)(currentPolicy.AssignedEntitlement - currentPolicy.UsedEntitlement);

            // ========== ENCASHMENT PROCESSING ==========
            double encashedDays = 0;

            // Check if encashment is allowed for this leave type via its policy
            if (encashmentConfigs.TryGetValue(currentPolicy.LeaveTypeId, out var encConfig) && encConfig.AllowEncashment)
            {
                // Max days that can be encashed (capped by MaxEncashableDays and remaining balance)
                double maxEncashableDays = Math.Min(remainingBalance, (double)encConfig.MaxEncashableDays);

                if (maxEncashableDays > 0)
                {
                    // Get employee daily rate
                    decimal dailyRate = employeeDailyRates.GetValueOrDefault(currentPolicy.EmployeeId, 0);

                    if (dailyRate > 0)
                    {
                        encashedDays = maxEncashableDays;
                        decimal encashableAmount = (decimal)maxEncashableDays * dailyRate * (encConfig.EncashmentRate / 100);
                        double taxRate = await GetTaxRateAsync((decimal)maxEncashableDays, ct);
                        decimal taxAmount = encashableAmount * (decimal)taxRate;
                        decimal netAmount = encashableAmount - taxAmount;

                        // Create encashment record
                        var encashment = new LeaveEncashment
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = currentPolicy.EmployeeId,
                            LeaveTypeId = currentPolicy.LeaveTypeId,
                            LeavePolicyId = currentPolicy.LeavePolicyId,
                            DaysEncashed = maxEncashableDays,
                            RatePerDay = (double)dailyRate,
                            TotalAmount = (double)encashableAmount,
                            Status = "Pending",  // Will go through approval workflow
                            CurrentAppStep = finalStepOrder,  // Set to final step (auto-approved for year-end)
                            LeaveAppChainId = encashmentChainId,
                            DateAdd = DateTime.UtcNow
                        };

                        await _uow.Add(encashment, ct);

                        // Create ledger entry for encashment (debit)
                        var ledgerDto = new LedgerEntryDto
                        {
                            EmployeeId = currentPolicy.EmployeeId,
                            LeaveTypeId = currentPolicy.LeaveTypeId,
                            LeavePolicyId = currentPolicy.LeavePolicyId,
                            Amount = maxEncashableDays,
                            EntryType = BoolToStr.EnumToString(LedgerEntryType.Debit),
                            SourceType = BoolToStr.EnumToString(LedgerSource.Encashment),
                            ReferenceId = encashment.Id,
                            Date = DateTime.UtcNow,
                            DateAdd = DateTime.UtcNow,
                            DateMod = null
                        };
                        await _leaveLedgerService.Credit(ledgerDto, ct);

                        // Update policy used entitlement (encashment counts as used)
                        currentPolicy.UsedEntitlement += (decimal)maxEncashableDays;
                        await _uow.Update(currentPolicy);

                        // Reduce remaining balance by encashed days
                        remainingBalance -= maxEncashableDays;

                        _logger.LogInformation($"Employee {currentPolicy.EmployeeId}: Encashed {maxEncashableDays} days for {encashableAmount:C}");
                        result.EncashmentRecordsCreated++;
                    }
                }
            }
            // ========== END ENCASHMENT PROCESSING ==========

            // ========== CARRYOVER PROCESSING ==========
            double maxCarryover = maxCarryoverMap.GetValueOrDefault(currentPolicy.LeaveTypeId, 0);
            double carryoverAmount = Math.Max(0, Math.Min(remainingBalance, maxCarryover));

            if (carryoverAmount > 0)
            {
                var key = $"{currentPolicy.EmployeeId}|{currentPolicy.LeaveTypeId}|{currentPolicy.LeavePolicyId}";

                // Update current policy's end date
                currentPolicy.EffectiveTo = yearEnd;
                currentPolicy.DateMod = DateTime.UtcNow;
                await _uow.Update(currentPolicy);

                // Get next year's annual entitlement
                double nextYearAnnualEntitlement = 0;
                bool hasEntitlementConfig = false;

                if (currentPolicy.LeavePolicyId.HasValue)
                {
                    if (policyConfigs.TryGetValue(currentPolicy.LeavePolicyId.Value, out var policyConfig))
                    {
                        nextYearAnnualEntitlement = (double)policyConfig.AnnualEntitlement;
                        hasEntitlementConfig = true;
                    }
                }

                if (!hasEntitlementConfig)
                {
                    if (leaveTypes.TryGetValue(currentPolicy.LeaveTypeId, out var leaveType) && leaveType.MaxDaysPerYear > 0)
                    {
                        nextYearAnnualEntitlement = (double)leaveType.MaxDaysPerYear;
                        hasEntitlementConfig = true;
                    }
                }

                if (!hasEntitlementConfig)
                {
                    throw new DomainException($"No annual entitlement configuration found for employee {currentPolicy.EmployeeId}");
                }

                // Calculate total for next year
                double totalNextYear = nextYearAnnualEntitlement + carryoverAmount;

                // Apply MaxAccrual cap
                double maxAccrual = maxAccrualMap.GetValueOrDefault(currentPolicy.LeaveTypeId, 999);
                if (totalNextYear > maxAccrual)
                {
                    totalNextYear = maxAccrual;
                }

                if (existingPoliciesDict.TryGetValue(key, out var existingPolicy))
                {
                    existingPolicy.AssignedEntitlement = (decimal)totalNextYear;
                    existingPolicy.CarryForward = (decimal)carryoverAmount;
                    existingPolicy.UsedEntitlement = 0;
                    existingPolicy.IsDeleted = false;
                    existingPolicy.DateMod = DateTime.UtcNow;
                    existingPolicy.AssignmentReason = $"Carryover from {fiscalYearName}";
                    existingPolicy.EffectiveFrom = nextYearStart;
                    existingPolicy.EffectiveTo = null;
                    existingPolicy.IsActive = true;

                    policiesToUpdate.Add(existingPolicy);

                    var ledgerDto = new LedgerEntryDto
                    {
                        EmployeeId = currentPolicy.EmployeeId,
                        LeaveTypeId = currentPolicy.LeaveTypeId,
                        LeavePolicyId = currentPolicy.LeavePolicyId,
                        Amount = carryoverAmount,
                        EntryType = BoolToStr.EnumToString(LedgerEntryType.Credit),
                        SourceType = BoolToStr.EnumToString(LedgerSource.CarryOver),
                        ReferenceId = existingPolicy.Id,
                        Date = DateTime.UtcNow,
                        DateAdd = DateTime.UtcNow
                    };
                    await _leaveLedgerService.Credit(ledgerDto, ct);
                }
                else
                {
                    var newPolicy = new EmpLeavePolicy
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = currentPolicy.EmployeeId,
                        LeaveTypeId = currentPolicy.LeaveTypeId,
                        LeavePolicyId = currentPolicy.LeavePolicyId,
                        AssignedEntitlement = (decimal)totalNextYear,
                        UsedEntitlement = 0,
                        CarryForward = (decimal)carryoverAmount,
                        EffectiveFrom = nextYearStart,
                        EffectiveTo = null,
                        IsActive = true,
                        DateAdd = DateTime.UtcNow,
                        IsDeleted = false,
                        AssignmentReason = $"Carryover from {fiscalYearName}",
                        Reason = "Carryover"
                    };

                    policiesToAdd.Add(newPolicy);

                    var ledgerDto = new LedgerEntryDto
                    {
                        EmployeeId = currentPolicy.EmployeeId,
                        LeaveTypeId = currentPolicy.LeaveTypeId,
                        LeavePolicyId = currentPolicy.LeavePolicyId,
                        Amount = carryoverAmount,
                        EntryType = BoolToStr.EnumToString(LedgerEntryType.Credit),
                        SourceType = BoolToStr.EnumToString(LedgerSource.CarryOver),
                        ReferenceId = newPolicy.Id,
                        Date = DateTime.UtcNow,
                        DateAdd = DateTime.UtcNow
                    };
                    await _leaveLedgerService.Credit(ledgerDto, ct);
                }

                result.CarryoverRecordsCreated++;
            }
            else if (remainingBalance > 0)
            {
                // No carryover, just close the policy
                currentPolicy.EffectiveTo = yearEnd;
                currentPolicy.DateMod = DateTime.UtcNow;
                await _uow.Update(currentPolicy);
            }

            result.EmployeesProcessed++;
        }

        // Update all reactivated policies
        foreach (var policy in policiesToUpdate)
        {
            await _uow.Update(policy);
        }

        // Add all new policies
        foreach (var policy in policiesToAdd)
        {
            await _uow.Add(policy, ct);
        }

        await _uow.Commit(ct);
        _logger.LogInformation("Transaction committed successfully");

        _logger.LogInformation(
            "Year-end processing completed. Employees: {Employees}, Carryover: {Carryover}, Encashment: {Encashment}",
            result.EmployeesProcessed, result.CarryoverRecordsCreated, result.EncashmentRecordsCreated);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error during year-end processing, rolling back");
        await _uow.Rollback(ct);
        result.Success = false;
        result.Message = $"Year-end processing failed: {ex.Message}";
        result.Errors.Add(new YearEndProcessError { ErrorMessage = ex.Message });
    }

    return result;
}

 // ========== ENCASHMENT METHODS ==========

// Update GetEncashmentConfigsAsync to use AsNoTracking
public async Task<Dictionary<Guid, EncashmentConfigDto>> GetEncashmentConfigsAsync(CancellationToken ct)
{
    var result = new Dictionary<Guid, EncashmentConfigDto>();

    var activePolicies = await _uow.Set<LeavePolicy>()
        .AsNoTracking()  // Add this
        .Where(p => p.Status == "Active" && !p.IsDeleted)
        .ToListAsync(ct);

    foreach (var policy in activePolicies)
    {
        result[policy.LeaveTypeId] = new EncashmentConfigDto
        {
            AllowEncashment = policy.AllowEncashment,
            MaxEncashableDays = policy.MaxEncashableDays,
            EncashmentRate = policy.EncashmentRate,
            RequiresApproval = policy.RequiresAttachment
        };
    }

    return result;
}
public async Task<EncashmentResultDto> ProcessEncashmentAsync(EncashmentRequestDto request, CancellationToken ct)
{
    var result = new EncashmentResultDto
    {
        Success = false,
        ProcessedAt = DateTime.UtcNow
    };

    // Get the approval chain for encashment (you can store this ID in configuration or get from leave policy)
    var encashmentChainId = Guid.Parse("0caabc79-3720-4f71-b7bb-32f2cacdd32f");

    // Get all steps to determine first step
    var allSteps = await _uow.Set<LeaveAppStep>()
        .Where(x => x.LeaveAppChainId == encashmentChainId && !x.IsDeleted)
        .OrderBy(x => x.StepOrder)
        .ToListAsync(ct);

    if (!allSteps.Any())
    {
        result.Message = "No approval chain configured for encashment";
        return result;
    }

    var firstStep = allSteps.First();

    // Get employee's current active policy
    var policy = await _uow.Set<EmpLeavePolicy>()
        .FirstOrDefaultAsync(p => p.EmployeeId == request.EmployeeId
            && p.LeaveTypeId == request.LeaveTypeId
            && !p.IsDeleted
            && p.EffectiveTo == null, ct);

    if (policy == null)
    {
        result.Message = "No active leave policy found for this employee";
        return result;
    }

    var leavePolicy = await _uow.Set<LeavePolicy>()
        .FirstOrDefaultAsync(lp => lp.Id == policy.LeavePolicyId && lp.Status == "Active" && !lp.IsDeleted, ct);

    if (leavePolicy == null || !leavePolicy.AllowEncashment)
    {
        result.Message = "Encashment is not allowed for this leave policy";
        return result;
    }

    var currentYear = DateTime.UtcNow.Year;
    var totalEncashedThisYear = await _uow.Set<LeaveEncashment>()
        .Where(e => e.EmployeeId == request.EmployeeId
            && e.LeaveTypeId == request.LeaveTypeId
            && !e.IsDeleted
            && e.DateAdd.Year == currentYear
            && e.Status != "Rejected")
        .SumAsync(e => e.DaysEncashed, ct);

    var remainingEncashable = leavePolicy.MaxEncashableDays - (decimal)totalEncashedThisYear;

    if (request.EncashmentDays > remainingEncashable)
    {
        result.Message = $"You have already encashed {totalEncashedThisYear} days this year. " +
                         $"Maximum encashable is {leavePolicy.MaxEncashableDays} days. " +
                         $"You can only encash {remainingEncashable} more days.";
        return result;
    }

    decimal remainingBalance = policy.AssignedEntitlement - policy.UsedEntitlement;

    if (request.EncashmentDays <= 0 || request.EncashmentDays > remainingBalance)
    {
        result.Message = request.EncashmentDays <= 0
            ? "Encashment days must be greater than zero"
            : $"Cannot encash more than remaining balance ({remainingBalance} days)";
        return result;
    }

    var dailyRate = await GetEmployeeDailyRateAsync(request.EmployeeId, ct);

    if (dailyRate <= 0)
    {
        result.Message = "Unable to calculate daily rate.";
        return result;
    }

    double encashableAmount = (double)(request.EncashmentDays * dailyRate * (leavePolicy.EncashmentRate / 100));
    double taxRate = 0.05;
    double taxAmount = encashableAmount * taxRate;
    double netAmount = encashableAmount - taxAmount;

    // Create encashment record with PENDING status
    var encashment = new LeaveEncashment
    {
        Id = Guid.NewGuid(),
        EmployeeId = request.EmployeeId,
        LeaveTypeId = request.LeaveTypeId,
        LeavePolicyId = policy.LeavePolicyId,
        LeaveAppChainId = encashmentChainId,  // Link to approval chain
        DaysEncashed = (double)request.EncashmentDays,
        RatePerDay = (double)dailyRate,
        TotalAmount = encashableAmount,
        Status = "Pending",  // PENDING, not approved
        CurrentAppStep = firstStep.StepOrder,  // Start at first step
        Reason = request.Reason,
        DateAdd = DateTime.UtcNow
    };

    await _uow.Add(encashment, ct);

    result.Success = true;
    result.Message = $"Encashment request submitted for approval. Waiting for {firstStep.StepName} approval.";
    result.EncashedDays = (double)request.EncashmentDays;
    result.TotalAmount = encashableAmount;
    result.TaxAmount = taxAmount;
    result.NetAmount = netAmount;
    result.EncashmentId = encashment.Id;

    return result;
}

  public async Task<List<EncashmentHistoryDto>> GetEncashmentHistoryAsync(Guid employeeId, CancellationToken ct)
  {
      var results = new List<EncashmentHistoryDto>();

      var encashments = await _uow.Set<LeaveEncashment>()
          .Where(e => e.EmployeeId == employeeId && !e.IsDeleted)
          .Include(e => e.LeaveType)
          .Include(e => e.LeavePolicy)
          .OrderByDescending(e => e.DateAdd)
          .ToListAsync(ct);

      // Get employee name from HRM.Pro
      var employeeName = await GetEmployeeNameAsync(employeeId, ct);

      foreach (var encashment in encashments)
      {
          double taxAmount = encashment.TotalAmount * 0.05;
          double netAmount = encashment.TotalAmount - taxAmount;

          results.Add(new EncashmentHistoryDto
          {
              Id = encashment.Id,
              EmployeeId = encashment.EmployeeId,
              EmployeeName = employeeName,
              LeaveTypeId = encashment.LeaveTypeId,
              LeaveTypeName = encashment.LeaveType?.Name ?? "Unknown",
              LeavePolicyName = encashment.LeavePolicy?.Name ?? "Unknown",
              DaysEncashed = encashment.DaysEncashed,
              RatePerDay = encashment.RatePerDay,
              TotalAmount = encashment.TotalAmount,
              TaxAmount = taxAmount,
              NetAmount = netAmount,
              Status = encashment.Status,
              DateAdd = encashment.DateAdd
          });
      }

      return results;
  }




 // Helper method to get tax rate based on encashment days
 private async Task<double> GetTaxRateAsync(decimal encashmentDays, CancellationToken ct)
 {
     // This could be configured in a Tax table in Core database
     if (encashmentDays <= 5) return 0.05;   // 5% tax
     if (encashmentDays <= 10) return 0.10;  // 10% tax
     if (encashmentDays <= 15) return 0.15;  // 15% tax
     return 0.20; // 20% tax
 }

 // Add these three methods to your YearEndProcessingService.cs
 // Add these three methods to your YearEndProcessingService.cs
 // In YearEndProcessingService.cs - Fix the GetPendingEncashmentApprovalsAsync method
 public async Task<List<PendingEncashmentApprovalDto>> GetPendingEncashmentApprovalsAsync(
     string? approverId,
     string? approvalLevel,
     CancellationToken ct)
 {
     try
     {
         var chainId = Guid.Parse("0caabc79-3720-4f71-b7bb-32f2cacdd32f");

         var allSteps = await _uow.Set<LeaveAppStep>()
             .Where(x => x.LeaveAppChainId == chainId && !x.IsDeleted)
             .OrderBy(x => x.StepOrder)
             .ToListAsync(ct);

         if (!allSteps.Any())
         {
             _logger.LogWarning("No approval steps found for chain {ChainId}", chainId);
             return new List<PendingEncashmentApprovalDto>();
         }

         var roleMap = new Dictionary<string, string>
         {
             { "0", "CEO" },
             { "1", "MGR" },
             { "2", "HR" }
         };

         var pendingEncashments = await _uow.Set<LeaveEncashment>()
             .AsNoTracking()
             .Include(x => x.LeaveType)
             .Where(x => !x.IsDeleted && x.Status == "Pending")
             .ToListAsync(ct);

         _logger.LogInformation($"Found {pendingEncashments.Count} pending encashments");

         var result = new List<PendingEncashmentApprovalDto>();

         foreach (var encashment in pendingEncashments)
         {
             // If CurrentAppStep is 0 or not set, assume first step
             var currentStepOrder = encashment.CurrentAppStep;
             if (currentStepOrder == 0)
             {
                 currentStepOrder = allSteps.First().StepOrder;
                 _logger.LogInformation($"Encashment {encashment.Id} has CurrentAppStep 0, defaulting to step {currentStepOrder}");
             }

             var currentStep = allSteps.FirstOrDefault(x => x.StepOrder == currentStepOrder);
             if (currentStep == null)
             {
                 _logger.LogWarning($"Step {currentStepOrder} not found for encashment {encashment.Id}, skipping");
                 continue;
             }

             var requiredRole = roleMap.GetValueOrDefault(currentStep.Role, currentStep.Role);

             // Map approval level to role for comparison
             var userRole = approvalLevel?.ToUpper() ?? "";
             if (roleMap.ContainsKey(userRole))
             {
                 userRole = roleMap[userRole];
             }
             else if (userRole == "MANAGER")
             {
                 userRole = "MGR";
             }
             else if (userRole == "HR")
             {
                 userRole = "HR";
             }
             else if (userRole == "CEO")
             {
                 userRole = "CEO";
             }

             _logger.LogInformation($"Encashment {encashment.Id}: Step {currentStepOrder} requires role {requiredRole}, user role {userRole}");

             // Check if user has the required role for this step
             if (!string.IsNullOrEmpty(approvalLevel))
             {
                 // Admin can approve anything
                 if (userRole != "ADMIN" && requiredRole != userRole)
                 {
                     _logger.LogInformation($"Skipping encashment {encashment.Id} - role mismatch");
                     continue;
                 }
             }

             var employeeName = await GetEmployeeNameAsync(encashment.EmployeeId, ct);
             var department = await GetEmployeeDepartmentAsync(encashment.EmployeeId, ct);

             var approvalChain = allSteps.Select(step => new ApprovalStepDto
             {
                 StepOrder = step.StepOrder,
                 StepName = step.StepName,
                 Role = roleMap.GetValueOrDefault(step.Role, step.Role),
                 IsFinal = step.IsFinal,
                 Status = step.StepOrder < currentStepOrder ? "Approved" :
                         (step.StepOrder == currentStepOrder ? "Pending" : "Pending")
             }).ToList();

             result.Add(new PendingEncashmentApprovalDto
             {
                 Id = encashment.Id,
                 EmployeeId = encashment.EmployeeId,
                 EmployeeName = employeeName,
                 Department = department,
                 LeaveTypeName = encashment.LeaveType?.Name ?? "Unknown",
                 EncashmentDays = (decimal)encashment.DaysEncashed,
                 RatePerDay = (decimal)encashment.RatePerDay,
                 TotalAmount = (decimal)encashment.TotalAmount,
                 Reason = encashment.Reason ?? "",
                 RequestDate = encashment.DateAdd,
                 Status = encashment.Status,
                 CurrentStep = currentStepOrder,
                 MaxSteps = allSteps.Count,
                 ApprovalChain = approvalChain
             });
         }

         _logger.LogInformation($"Returning {result.Count} pending encashment approvals for role {approvalLevel}");
         return result;
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, "Error getting pending encashment approvals");
         return new List<PendingEncashmentApprovalDto>();
     }
 }
// In YearEndProcessingService.cs - Fix the ProcessEncashmentApprovalAsync method
public async Task<ProcessResultDto> ProcessEncashmentApprovalAsync(
    Guid requestId,
    EncashmentApprovalDto approval,
    CancellationToken ct)
{
    try
    {
        var encashment = await _uow.Set<LeaveEncashment>()
            .FirstOrDefaultAsync(x => x.Id == requestId && !x.IsDeleted, ct);

        if (encashment == null)
        {
            return new ProcessResultDto
            {
                Success = false,
                Message = "Encashment request not found"
            };
        }

        // Get the approval chain for encashment (you need to store LeaveAppChainId in encashment)
        // For now, use the existing chain ID from your data
        var chainId = Guid.Parse("0caabc79-3720-4f71-b7bb-32f2cacdd32f");

        // Get all steps for this chain, ordered by StepOrder
        var allSteps = await _uow.Set<LeaveAppStep>()
            .Where(x => x.LeaveAppChainId == chainId && !x.IsDeleted)
            .OrderBy(x => x.StepOrder)
            .ToListAsync(ct);

        if (!allSteps.Any())
        {
            return new ProcessResultDto
            {
                Success = false,
                Message = "No approval steps configured for encashment"
            };
        }

        // Get current step
        var currentStep = allSteps.FirstOrDefault(x => x.StepOrder == encashment.CurrentAppStep);
        if (currentStep == null && encashment.Status == "Pending")
        {
            // If no current step but status is pending, start from first step
            currentStep = allSteps.First();
            encashment.CurrentAppStep = currentStep.StepOrder;
            await _uow.Update(encashment);
            await _uow.SaveChangesAsync(ct);
        }

        if (approval.Status == "Rejected")
        {
            encashment.Status = "Rejected";
            encashment.DateMod = DateTime.UtcNow;
            await _uow.Update(encashment);
            await _uow.SaveChangesAsync(ct);

            return new ProcessResultDto
            {
                Success = true,
                Message = $"Encashment request rejected by {approval.ApproverName} at step {currentStep?.StepName}",
                EmployeesProcessed = 1
            };
        }

        // Find next step
        var nextStep = allSteps.FirstOrDefault(x => x.StepOrder > (currentStep?.StepOrder ?? 0));

        if (nextStep == null)
        {
            // Final approval
            encashment.Status = "Approved";
            encashment.DateMod = DateTime.UtcNow;
            await _uow.Update(encashment);
            await _uow.SaveChangesAsync(ct);

            return new ProcessResultDto
            {
                Success = true,
                Message = $"Encashment request fully approved by {approval.ApproverName}",
                EmployeesProcessed = 1
            };
        }

        // Move to next step
        encashment.CurrentAppStep = nextStep.StepOrder;
        encashment.DateMod = DateTime.UtcNow;
        encashment.Status = "Pending";
        await _uow.Update(encashment);
        await _uow.SaveChangesAsync(ct);

        var roleMap = new Dictionary<string, string>
        {
            { "0", "CEO" },
            { "1", "MGR" },
            { "2", "HR" }
        };

        var nextRole = roleMap.GetValueOrDefault(nextStep.Role, nextStep.Role);

        return new ProcessResultDto
        {
            Success = true,
            Message = $"Encashment request approved. Next approver: {nextRole}",
            EmployeesProcessed = 1
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing encashment approval");
        return new ProcessResultDto
        {
            Success = false,
            Message = $"Error: {ex.Message}"
        };
    }
}

 public async Task<EncashmentApprovalDetailsDto> GetEncashmentApprovalDetailsAsync(
     Guid requestId,
     CancellationToken ct)
 {
     try
     {
         var encashment = await _uow.Set<LeaveEncashment>()
             .AsNoTracking()
             .Include(x => x.LeaveType)
             .FirstOrDefaultAsync(x => x.Id == requestId && !x.IsDeleted, ct);

         if (encashment == null)
         {
             return new EncashmentApprovalDetailsDto();
         }

         // Get employee name and department from HRM.Pro
         var employeeName = await GetEmployeeNameAsync(encashment.EmployeeId, ct);
         var department = await GetEmployeeDepartmentAsync(encashment.EmployeeId, ct);

         // Build approval chain based on CurrentAppStep
         var approvalChain = new List<ApprovalStepDetailDto>();

         // Step 1: Manager Approval
         approvalChain.Add(new ApprovalStepDetailDto
         {
             StepOrder = 1,
             StepName = "Manager Approval",
             Role = "Manager",
             IsFinal = false,
             Status = encashment.CurrentAppStep >= 1 ? "Approved" : "Pending"
         });

         // Step 2: HR Approval
         approvalChain.Add(new ApprovalStepDetailDto
         {
             StepOrder = 2,
             StepName = "HR Approval",
             Role = "HR",
             IsFinal = true,
             Status = encashment.CurrentAppStep >= 2 ? "Approved" :
                     (encashment.CurrentAppStep == 1 ? "Pending" : "Pending")
         });

         return new EncashmentApprovalDetailsDto
         {
             Id = encashment.Id,
             EmployeeId = encashment.EmployeeId,
             EmployeeName = employeeName,
             Department = department,
             LeaveTypeName = encashment.LeaveType?.Name ?? "Unknown",
             EncashmentDays = (decimal)encashment.DaysEncashed,
             RatePerDay = (decimal)encashment.RatePerDay,
             TotalAmount = (decimal)encashment.TotalAmount,
             Reason = "", // No Reason field in entity
             RequestDate = encashment.DateAdd,
             Status = encashment.Status,
             CurrentStep = encashment.CurrentAppStep,
             ApprovalChain = approvalChain
         };
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, "Error getting encashment approval details for {RequestId}", requestId);
         return new EncashmentApprovalDetailsDto();
     }
 }

 // Helper method to get employee department
// In YearEndProcessingService.cs - Fix the GetEmployeeDepartmentAsync method

private async Task<string> GetEmployeeDepartmentAsync(Guid employeeId, CancellationToken ct)
{
    try
    {
        // First try to get department name directly from Employee table if it has department name
        const string sql = @"
            SELECT
                e.""DepartmentId"" as DepartmentId
            FROM ""Employee"" e
            WHERE e.""Id"" = @EmployeeId";

        var result = await _hrmProDapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { EmployeeId = employeeId }, ct);

        if (result?.DepartmentId != null)
        {
            // Try to get department name from Department table if it exists
            try
            {
                const string deptSql = @"
                    SELECT d.""Name"" as DepartmentName
                    FROM ""Department"" d
                    WHERE d.""Id"" = @DepartmentId";

                var deptResult = await _hrmProDapper.QueryFirstOrDefaultAsync<dynamic>(deptSql, new { DepartmentId = result.DepartmentId }, ct);
                return deptResult?.DepartmentName ?? "";
            }
            catch
            {
                // Department table doesn't exist or can't be accessed
                _logger.LogDebug("Department table not accessible, returning DepartmentId");
                return result.DepartmentId.ToString();
            }
        }

        return "";
    }
    catch (Exception ex)
    {
        _logger.LogDebug(ex, "Could not fetch employee department - returning empty string");
        return "";  // Return empty string instead of failing
    }
}




// Helper method to get employee name from HRM.Pro database (Person table)
private async Task<string> GetEmployeeNameAsync(Guid employeeId, CancellationToken ct)
{
    try
    {
        // First, get the PersonId associated with this Employee
        const string getPersonIdSql = @"
            SELECT ""PersonId""
            FROM ""Employee""
            WHERE ""Id"" = @EmployeeId";

        var personId = await _hrmProDapper.QueryFirstOrDefaultAsync<Guid?>(getPersonIdSql, new { EmployeeId = employeeId }, ct);

        if (personId == null)
        {
            return employeeId.ToString();
        }

        // Then get the full name from Person table
        const string sql = @"
            SELECT
                ""FirstName"" || ' ' || COALESCE(""MiddleName"" || ' ', '') || ""LastName"" as FullName
            FROM ""Person""
            WHERE ""Id"" = @PersonId";

        var result = await _hrmProDapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { PersonId = personId }, ct);
        return result?.FullName ?? employeeId.ToString();
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Could not fetch employee name from HRM.Pro database");
        return employeeId.ToString();
    }
}

// Helper method to get employee daily rate from HRM.Pro database
private async Task<decimal> GetEmployeeDailyRateAsync(Guid employeeId, CancellationToken ct)
{
    try
    {
        // Get the latest salary for the employee
        const string sql = @"
            SELECT
                COALESCE(""BaseSalary"", 0) / 30.0 as DailyRate
            FROM ""EmpSalary""
            WHERE ""EmployeeId"" = @EmployeeId
                AND (""EffectiveTo"" IS NULL OR ""EffectiveTo"" >= CURRENT_DATE)
            ORDER BY ""EffectiveFrom"" DESC
            LIMIT 1";

        var result = await _hrmProDapper.QueryFirstOrDefaultAsync<dynamic>(sql, new { EmployeeId = employeeId }, ct);
        return result?.DailyRate ?? 100; // Default to 100 if not found
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Could not fetch employee daily rate from HRM.Pro database, using default rate");
        return 100; // Default daily rate
    }
}

// Helper method to get employee daily rates (batch version for year-end processing)
private async Task<Dictionary<Guid, decimal>> GetEmployeeDailyRatesAsync(List<Guid> employeeIds, CancellationToken ct)
{
    var result = new Dictionary<Guid, decimal>();

    if (!employeeIds.Any()) return result;

    try
    {
        // Get latest salary for all employees in one query
        const string sql = @"
            SELECT DISTINCT ON (s.""EmployeeId"")
                s.""EmployeeId"",
                s.""BaseSalary"" / 30.0 as DailyRate
            FROM ""EmpSalary"" s
            WHERE s.""EmployeeId"" = ANY(@EmployeeIds)
                AND (s.""EffectiveTo"" IS NULL OR s.""EffectiveTo"" >= CURRENT_DATE)
            ORDER BY s.""EmployeeId"", s.""EffectiveFrom"" DESC";

        var salaries = await _hrmProDapper.QueryAsync<dynamic>(sql, new { EmployeeIds = employeeIds.ToArray() }, ct);

        foreach (var salary in salaries)
        {
            result[(Guid)salary.EmployeeId] = (decimal)salary.DailyRate;
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Could not fetch employee daily rates from HRM.Pro database");
    }

    // Set default for any missing employees
    foreach (var employeeId in employeeIds)
    {
        if (!result.ContainsKey(employeeId))
        {
            result[employeeId] = 100; // Default daily rate
        }
    }

    return result;
}

// Helper method to get employee name for multiple employees (for admin views)
private async Task<Dictionary<Guid, string>> GetEmployeeNamesAsync(List<Guid> employeeIds, CancellationToken ct)
{
    var result = new Dictionary<Guid, string>();

    if (!employeeIds.Any()) return result;

    try
    {
        // Get PersonIds for employees
        const string getPersonIdsSql = @"
            SELECT ""Id"", ""PersonId""
            FROM ""Employee""
            WHERE ""Id"" = ANY(@EmployeeIds)";

        var employees = await _hrmProDapper.QueryAsync<dynamic>(getPersonIdsSql, new { EmployeeIds = employeeIds.ToArray() }, ct);
        var personIdMap = new Dictionary<Guid, Guid>();

        foreach (var emp in employees)
        {
            personIdMap[(Guid)emp.Id] = (Guid)emp.PersonId;
        }

        var personIds = personIdMap.Values.Distinct().ToList();

        if (personIds.Any())
        {
            // Get names from Person table
            const string namesSql = @"
                SELECT
                    p.""Id"",
                    p.""FirstName"" || ' ' || COALESCE(p.""MiddleName"" || ' ', '') || p.""LastName"" as FullName
                FROM ""Person"" p
                WHERE p.""Id"" = ANY(@PersonIds)";

            var persons = await _hrmProDapper.QueryAsync<dynamic>(namesSql, new { PersonIds = personIds.ToArray() }, ct);
            var personNameMap = persons.ToDictionary(p => (Guid)p.Id, p => (string)p.FullName);

            foreach (var emp in employees)
            {
                var employeeId = (Guid)emp.Id;
                var personId = (Guid)emp.PersonId;
                result[employeeId] = personNameMap.GetValueOrDefault(personId, employeeId.ToString());
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Could not fetch employee names from HRM.Pro database");
    }

    // Set default for any missing
    foreach (var employeeId in employeeIds)
    {
        if (!result.ContainsKey(employeeId))
        {
            result[employeeId] = employeeId.ToString();
        }
    }

    return result;
}

// In YearEndService.cs
// YearEndProcessingService.cs - Corrected GetAllEncashmentHistoryAsync
// In YearEndProcessingService.cs - Update the mapping

public async Task<List<EncashmentHistoryDto>> GetAllEncashmentHistoryAsync(Guid? fiscalYearId, CancellationToken ct)
{
    try
    {
        var query = _uow.Set<LeaveEncashment>()
            .Where(x => !x.IsDeleted)
            .Include(x => x.LeaveType)
            .OrderByDescending(x => x.DateAdd);

        var records = await query.ToListAsync(ct);

        // Get unique employee IDs
        var employeeIds = records.Select(x => x.EmployeeId).Distinct().ToList();

        // Get employee names
        var employeeNames = await GetEmployeeNamesAsync(employeeIds, ct);

        return records.Select(x => new EncashmentHistoryDto
        {
            Id = x.Id,
            EmployeeId = x.EmployeeId,
            EmployeeName = employeeNames.GetValueOrDefault(x.EmployeeId, x.EmployeeId.ToString()),
            LeaveTypeId = x.LeaveTypeId,
            LeaveTypeName = x.LeaveType?.Name ?? "Annual Leave",
            LeavePolicyName = x.LeavePolicy?.Name ?? string.Empty,
            DaysEncashed = x.DaysEncashed,  // Use the correct field name
            RatePerDay = x.RatePerDay,
            TotalAmount = x.TotalAmount,
            TaxAmount = x.TotalAmount * 0.05,
            NetAmount = x.TotalAmount * 0.95,
            Status = x.Status ?? "Pending",
            DateAdd = x.DateAdd,
            ProcessedBy = null,
            ProcessedByName = null,
            Notes = null
        }).ToList();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting all encashment history");
        throw;
    }
}
// In YearEndProcessingService.cs
public async Task<decimal> GetTotalEncashedDaysAsync(Guid employeeId, int fiscalYear, CancellationToken ct)
{
    // Get ALL encashment records (Approved, Completed, Pending)
    // Only exclude Rejected and Deleted
    var allRecords = await _uow.Set<LeaveEncashment>()
        .Where(e => e.EmployeeId == employeeId
            && !e.IsDeleted
            && e.Status != "Rejected"  // Exclude rejected
            && e.DateAdd.Year == fiscalYear)
        .ToListAsync(ct);

    // Log for debugging
    foreach (var record in allRecords)
    {
        _logger.LogInformation($"Encashment record: Days={record.DaysEncashed}, Status={record.Status}, Date={record.DateAdd}");
    }

    var totalEncashed = allRecords.Sum(e => e.DaysEncashed);

    _logger.LogInformation($"Total encashed for employee {employeeId}: {totalEncashed} days");

    return (decimal)totalEncashed;
}

// Add to IYearEndProcessingService interface


// Implementation
public async Task DeleteEncashmentsByYearAsync(int fiscalYear, CancellationToken ct)
{
    var encashments = await _uow.Set<LeaveEncashment>()
        .Where(e => !e.IsDeleted && e.DateAdd.Year == fiscalYear)
        .ToListAsync(ct);

    foreach (var encashment in encashments)
    {
        // Delete associated ledgers
        var ledgers = await _uow.Set<LeaveLedger>()
            .Where(l => l.SourceType == BoolToStr.EnumToString(LedgerSource.Encashment)
                && l.ReferenceId == encashment.Id)
            .ToListAsync(ct);

        foreach (var ledger in ledgers)
        {
            await _uow.Delete(ledger);
        }

        await _uow.Delete(encashment);
    }

    await _uow.SaveChangesAsync(ct);
}


public async Task<FiscalYearDto?> GetFiscalYearByIdAsync(Guid fiscalYearId, CancellationToken ct)
{
    const string sql = @"
        SELECT
            ""Id"",
            ""Name"",
            ""DateStart"",
            ""DateEnd"",
            ""IsActive""
        FROM ""FiscalYear""
        WHERE ""Id"" = @Id AND ""IsDeleted"" = false";

    return await _coreDapper.QueryFirstOrDefaultAsync<FiscalYearDto>(sql, new { Id = fiscalYearId }, ct);
}

public async Task<bool> IsYearEndProcessedAsync(string? fiscalYearName, CancellationToken ct)
{
    if (string.IsNullOrEmpty(fiscalYearName)) return false;

    // Check if carryover policies exist for this fiscal year
    var hasCarryoverPolicies = await _uow.Set<EmpLeavePolicy>()
        .AnyAsync(ep => ep.AssignmentReason == $"Carryover from {fiscalYearName}"
            && !ep.IsDeleted, ct);

    // Check if encashment records exist for this fiscal year
    var hasEncashments = await _uow.Set<LeaveEncashment>()
        .AnyAsync(e => !e.IsDeleted && e.DateAdd.Year.ToString() == fiscalYearName, ct);

    return hasCarryoverPolicies || hasEncashments;
}
}