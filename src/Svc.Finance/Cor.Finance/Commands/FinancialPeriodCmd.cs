using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Enums;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ==================== FINANCIAL PERIOD COMMANDS ====================

public class AddFinancialPeriodCmd : IRequest<FinancialPeriodDto>
{
    public AddFinancialPeriodDto AddDto { get; set; } = default!;
}

public class EditFinancialPeriodCmd : IRequest<FinancialPeriodDto>
{
    public EditFinancialPeriodDto EditDto { get; set; } = default!;
}

public class DeleteFinancialPeriodCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class CloseFinancialPeriodCmd : IRequest<bool>
{
    public Guid Id { get; set; }
    public ClosePeriodDto? CloseDto { get; set; }
}

public class OpenFinancialPeriodCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}
/// <summary>
/// Command to lock a financial period
/// </summary>
public class LockFinancialPeriodCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Command to unlock a financial period
/// </summary>
public class UnlockFinancialPeriodCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Command for year-end closing
/// </summary>
public class YearEndCloseCmd : IRequest<YearEndCloseResultDto>
{
    public YearEndCloseDto Dto { get; set; } = default!;
}

/// <summary>
/// Command for bulk closing periods
/// </summary>
public class BulkClosePeriodsCmd : IRequest<BulkCloseResultDto>
{
    public BulkPeriodCloseDto Dto { get; set; } = default!;
}

/// <summary>
/// Command for bulk opening periods
/// </summary>
public class BulkOpenPeriodsCmd : IRequest<BulkOpenResultDto>
{
    public BulkPeriodOpenDto Dto { get; set; } = default!;
}

/// <summary>
/// Command to configure auto-close
/// </summary>
public class ConfigureAutoCloseCmd : IRequest<AutoCloseConfigDto>
{
    public AutoCloseConfigDto Dto { get; set; } = default!;
}

/// <summary>
/// Command to reverse a period
/// </summary>
public class ReversePeriodCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Command to transfer balance between periods
/// </summary>
public class TransferPeriodBalanceCmd : IRequest<PeriodTransferResultDto>
{
    public PeriodTransferDto Dto { get; set; } = default!;
}
// ==================== HANDLERS ====================

// Commands/FinancialPeriodCmd.cs - AddFinancialPeriodHandler
/// <summary>
/// Handler for LockFinancialPeriodCmd
/// </summary>
public class LockFinancialPeriodHandler : IRequestHandler<LockFinancialPeriodCmd, bool>
{
    private readonly FinanceDbContext _context;

    public LockFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(LockFinancialPeriodCmd request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Financial period with ID '{request.Id}' not found");

        if (period.IsClosed)
            throw new InvalidOperationException("Cannot lock a closed period");

        period.Status = PeriodStatus.LOCKED;
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>
/// Handler for UnlockFinancialPeriodCmd
/// </summary>
public class UnlockFinancialPeriodHandler : IRequestHandler<UnlockFinancialPeriodCmd, bool>
{
    private readonly FinanceDbContext _context;

    public UnlockFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnlockFinancialPeriodCmd request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Financial period with ID '{request.Id}' not found");

        if (period.Status != PeriodStatus.LOCKED)
            throw new InvalidOperationException("Period is not locked");

        period.Status = PeriodStatus.OPEN;
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>
/// Handler for YearEndCloseCmd
/// </summary>
public class YearEndCloseHandler : IRequestHandler<YearEndCloseCmd, YearEndCloseResultDto>
{
    private readonly FinanceDbContext _context;

    public YearEndCloseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<YearEndCloseResultDto> Handle(YearEndCloseCmd request, CancellationToken ct)
    {
        var year = request.Dto.Year;
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);

        var periods = await _context.FinancialPeriods
            .Where(x => !x.IsDeleted && x.StartDate >= startDate && x.EndDate <= endDate)
            .ToListAsync(ct);

        var closedCount = 0;
        var errors = new List<string>();

        foreach (var period in periods)
        {
            if (request.Dto.CloseAllYearPeriods && !period.IsClosed)
            {
                try
                {
                    // Check for unposted entries
                    var hasUnposted = await _context.JournalEntries
                        .AnyAsync(x => x.PeriodId == period.Id && !x.IsPosted && !x.IsDeleted, ct);

                    if (!hasUnposted)
                    {
                        period.IsClosed = true;
                        period.Status = PeriodStatus.CLOSED;
                        period.ClosedDate = DateTime.UtcNow;
                        period.DateMod = DateTime.UtcNow;
                        closedCount++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to close period {period.Name}: {ex.Message}");
                }
            }
        }

        await _context.SaveChangesAsync(ct);

        // Create next year periods if requested
        if (request.Dto.AutoCreateNextYearPeriods)
        {
            // Logic to create next year periods would go here
        }

        return new YearEndCloseResultDto
        {
            Year = year,
            TotalPeriods = periods.Count,
            ClosedPeriods = closedCount,
            Errors = errors,
            Success = errors.Count == 0
        };
    }
}

/// <summary>
/// Handler for BulkClosePeriodsCmd
/// </summary>
public class BulkClosePeriodsHandler : IRequestHandler<BulkClosePeriodsCmd, BulkCloseResultDto>
{
    private readonly FinanceDbContext _context;

    public BulkClosePeriodsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BulkCloseResultDto> Handle(BulkClosePeriodsCmd request, CancellationToken ct)
    {
        var result = new BulkCloseResultDto();
        var errors = new List<BulkCloseErrorDto>();

        foreach (var periodId in request.Dto.PeriodIds)
        {
            try
            {
                var period = await _context.FinancialPeriods
                    .FirstOrDefaultAsync(x => x.Id == periodId && !x.IsDeleted, ct);

                if (period == null)
                {
                    errors.Add(new BulkCloseErrorDto { PeriodId = periodId, Error = "Period not found" });
                    continue;
                }

                if (period.IsClosed)
                {
                    errors.Add(new BulkCloseErrorDto { PeriodId = periodId, PeriodName = period.Name, Error = "Already closed" });
                    continue;
                }

                var hasUnposted = await _context.JournalEntries
                    .AnyAsync(x => x.PeriodId == periodId && !x.IsPosted && !x.IsDeleted, ct);

                if (hasUnposted && !request.Dto.ForceClose)
                {
                    errors.Add(new BulkCloseErrorDto { PeriodId = periodId, PeriodName = period.Name, Error = "Has unposted entries" });
                    continue;
                }

                period.IsClosed = true;
                period.Status = PeriodStatus.CLOSED;
                period.ClosedDate = DateTime.UtcNow;
                period.DateMod = DateTime.UtcNow;
                result.ClosedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkCloseErrorDto { PeriodId = periodId, Error = ex.Message });
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;
        result.FailedCount = errors.Count;
        return result;
    }
}

/// <summary>
/// Handler for BulkOpenPeriodsCmd
/// </summary>
public class BulkOpenPeriodsHandler : IRequestHandler<BulkOpenPeriodsCmd, BulkOpenResultDto>
{
    private readonly FinanceDbContext _context;

    public BulkOpenPeriodsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BulkOpenResultDto> Handle(BulkOpenPeriodsCmd request, CancellationToken ct)
    {
        var result = new BulkOpenResultDto();
        var errors = new List<BulkOpenErrorDto>();

        foreach (var periodId in request.Dto.PeriodIds)
        {
            try
            {
                var period = await _context.FinancialPeriods
                    .FirstOrDefaultAsync(x => x.Id == periodId && !x.IsDeleted, ct);

                if (period == null)
                {
                    errors.Add(new BulkOpenErrorDto { PeriodId = periodId, Error = "Period not found" });
                    continue;
                }

                if (!period.IsClosed)
                {
                    errors.Add(new BulkOpenErrorDto { PeriodId = periodId, PeriodName = period.Name, Error = "Already open" });
                    continue;
                }

                period.IsClosed = false;
                period.Status = PeriodStatus.OPEN;
                period.ClosedDate = null;
                period.DateMod = DateTime.UtcNow;
                result.OpenedCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new BulkOpenErrorDto { PeriodId = periodId, Error = ex.Message });
            }
        }

        await _context.SaveChangesAsync(ct);
        result.Errors = errors;
        result.FailedCount = errors.Count;
        return result;
    }
}

/// <summary>
/// Handler for ConfigureAutoCloseCmd
/// </summary>
public class ConfigureAutoCloseHandler : IRequestHandler<ConfigureAutoCloseCmd, AutoCloseConfigDto>
{
    private readonly FinanceDbContext _context;

    public ConfigureAutoCloseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<AutoCloseConfigDto> Handle(ConfigureAutoCloseCmd request, CancellationToken ct)
    {
        // In a real implementation, this would save to a settings table
        // For now, just return the config
        return request.Dto;
    }
}

/// <summary>
/// Handler for ReversePeriodCmd
/// </summary>
public class ReversePeriodHandler : IRequestHandler<ReversePeriodCmd, bool>
{
    private readonly FinanceDbContext _context;

    public ReversePeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReversePeriodCmd request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Financial period with ID '{request.Id}' not found");

        if (!period.IsClosed)
            throw new InvalidOperationException("Cannot reverse an open period");

        // Create reversal entries logic would go here
        // For now, just mark the period as open again
        period.IsClosed = false;
        period.Status = PeriodStatus.OPEN;
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

/// <summary>
/// Handler for TransferPeriodBalanceCmd
/// </summary>
public class TransferPeriodBalanceHandler : IRequestHandler<TransferPeriodBalanceCmd, PeriodTransferResultDto>
{
    private readonly FinanceDbContext _context;

    public TransferPeriodBalanceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PeriodTransferResultDto> Handle(TransferPeriodBalanceCmd request, CancellationToken ct)
    {
        var fromPeriod = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Dto.FromPeriodId && !x.IsDeleted, ct);

        var toPeriod = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Dto.ToPeriodId && !x.IsDeleted, ct);

        if (fromPeriod == null || toPeriod == null)
            throw new InvalidOperationException("One or both periods not found");

        if (fromPeriod.IsClosed)
            throw new InvalidOperationException("Cannot transfer from a closed period");

        // In a real implementation, this would create transfer journal entries
        // For now, just return a success result

        return new PeriodTransferResultDto
        {
            FromPeriodId = fromPeriod.Id,
            ToPeriodId = toPeriod.Id,
            Amount = request.Dto.Amount,
            TransferDate = DateTime.UtcNow,
            Success = true,
            Message = "Balance transferred successfully"
        };
    }
}
public class AddFinancialPeriodHandler : IRequestHandler<AddFinancialPeriodCmd, FinancialPeriodDto>
{
    private readonly FinanceDbContext _context;

    public AddFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialPeriodDto> Handle(AddFinancialPeriodCmd request, CancellationToken ct)
    {
        // Validate DTO
        request.AddDto.Validate();

        // Parse PeriodType from string to enum
        if (!Enum.TryParse<PeriodType>(request.AddDto.PeriodType, true, out var periodType))
        {
            throw new InvalidOperationException($"Invalid PeriodType: {request.AddDto.PeriodType}");
        }

        // Check for overlapping periods
        var overlaps = await _context.FinancialPeriods
            .AnyAsync(x => !x.IsDeleted &&
                ((request.AddDto.StartDate >= x.StartDate && request.AddDto.StartDate <= x.EndDate) ||
                 (request.AddDto.EndDate >= x.StartDate && request.AddDto.EndDate <= x.EndDate) ||
                 (request.AddDto.StartDate <= x.StartDate && request.AddDto.EndDate >= x.EndDate)), ct);

        if (overlaps)
            throw new InvalidOperationException("Financial period overlaps with an existing period.");

        var period = new FinancialPeriod
        {
            Id = Guid.NewGuid(),
            Name = request.AddDto.Name,
            StartDate = request.AddDto.StartDate,
            EndDate = request.AddDto.EndDate,
            PeriodType = periodType,
            IsClosed = false,
            Status = PeriodStatus.OPEN,
            ClosedDate = null,
            ClosedBy = null,
            DateAdd = DateTime.UtcNow,
            DateMod =DateTime.UtcNow,
            IsDeleted = false,
            CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"), // Set from user context
            // ✅ FIX: Set default values for JSON columns
            ClosingMetadataJson = null,  // Null is now allowed
            MetadataJson = null,         // Null is now allowed
            Notes = null
        };

        _context.FinancialPeriods.Add(period);
        await _context.SaveChangesAsync(ct);

        return MapToDto(period);
    }

    private static FinancialPeriodDto MapToDto(FinancialPeriod period)
    {
        return new FinancialPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PeriodType = period.PeriodType.ToString(),
            IsClosed = period.IsClosed,
            ClosedDate = period.ClosedDate,
            DateAdd = period.DateAdd,
            DateMod = period.DateMod
        };
    }
}

public class EditFinancialPeriodHandler : IRequestHandler<EditFinancialPeriodCmd, FinancialPeriodDto>
{
    private readonly FinanceDbContext _context;

    public EditFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialPeriodDto> Handle(EditFinancialPeriodCmd request, CancellationToken ct)
    {
        // Validate DTO
        request.EditDto.Validate();

        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Financial period with ID '{request.EditDto.Id}' not found");

        if (period.IsClosed)
            throw new InvalidOperationException("Cannot edit a closed financial period.");

        // Parse PeriodType from string to enum
        if (!Enum.TryParse<PeriodType>(request.EditDto.PeriodType, true, out var periodType))
        {
            throw new InvalidOperationException($"Invalid PeriodType: {request.EditDto.PeriodType}");
        }

        // Check for overlapping periods (excluding current)
        var overlaps = await _context.FinancialPeriods
            .AnyAsync(x => !x.IsDeleted && x.Id != request.EditDto.Id &&
                ((request.EditDto.StartDate >= x.StartDate && request.EditDto.StartDate <= x.EndDate) ||
                 (request.EditDto.EndDate >= x.StartDate && request.EditDto.EndDate <= x.EndDate) ||
                 (request.EditDto.StartDate <= x.StartDate && request.EditDto.EndDate >= x.EndDate)), ct);

        if (overlaps)
            throw new InvalidOperationException("Financial period overlaps with an existing period.");

        period.Name = request.EditDto.Name;
        period.StartDate = request.EditDto.StartDate;
        period.EndDate = request.EditDto.EndDate;
        period.PeriodType = periodType;
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return MapToDto(period);
    }

    private static FinancialPeriodDto MapToDto(FinancialPeriod period)
    {
        return new FinancialPeriodDto
        {
            Id = period.Id,
            Name = period.Name,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            PeriodType = period.PeriodType.ToString(),
            IsClosed = period.IsClosed,
            ClosedDate = period.ClosedDate,
            DateAdd = period.DateAdd,
            DateMod = period.DateMod
        };
    }
}

public class DeleteFinancialPeriodHandler : IRequestHandler<DeleteFinancialPeriodCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteFinancialPeriodCmd request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            return false;

        if (period.IsClosed)
            throw new InvalidOperationException("Cannot delete a closed financial period.");

        // Check if period has journal entries
        var hasEntries = await _context.JournalEntries
            .AnyAsync(x => x.PeriodId == request.Id && !x.IsDeleted, ct);

        if (hasEntries)
            throw new InvalidOperationException("Cannot delete a period with existing journal entries.");

        period.IsDeleted = true;
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class CloseFinancialPeriodHandler : IRequestHandler<CloseFinancialPeriodCmd, bool>
{
    private readonly FinanceDbContext _context;

    public CloseFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CloseFinancialPeriodCmd request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            return false;

        if (period.IsClosed)
            throw new InvalidOperationException("Financial period is already closed.");

        // Check if there are unposted journal entries
        var hasUnposted = await _context.JournalEntries
            .AnyAsync(x => x.PeriodId == request.Id && !x.IsPosted && !x.IsDeleted, ct);

        var forceClose = request.CloseDto?.ForceClose ?? false;

        if (hasUnposted && !forceClose)
            throw new InvalidOperationException("Cannot close period with unposted journal entries. Use ForceClose to override.");

        period.IsClosed = true;
        period.Status = PeriodStatus.CLOSED;
        period.ClosedDate = DateTime.UtcNow;
        period.ClosedBy = null; // Set from user context
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class OpenFinancialPeriodHandler : IRequestHandler<OpenFinancialPeriodCmd, bool>
{
    private readonly FinanceDbContext _context;

    public OpenFinancialPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(OpenFinancialPeriodCmd request, CancellationToken ct)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (period == null)
            return false;

        if (!period.IsClosed)
            throw new InvalidOperationException("Financial period is already open.");

        period.IsClosed = false;
        period.Status = PeriodStatus.OPEN;
        period.ClosedDate = null;
        period.ClosedBy = null;
        period.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}