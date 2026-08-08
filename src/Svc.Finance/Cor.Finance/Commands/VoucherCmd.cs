// Commands/VoucherCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddVoucherCmd : IRequest<VoucherDto>
{
    public AddVoucherDto AddDto { get; set; } = new();
}

public class EditVoucherCmd : IRequest<VoucherDto>
{
    public EditVoucherDto EditDto { get; set; } = new();
}

public class DeleteVoucherCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ApproveVoucherCmd : IRequest<VoucherDto>
{
    public Guid Id { get; set; }
}

public class RejectVoucherCmd : IRequest<VoucherDto>
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class PostVoucherCmd : IRequest<VoucherDto>
{
    public Guid Id { get; set; }
}

// Commands/VoucherCmd.cs - AddVoucherHandler

public class AddVoucherHandler : IRequestHandler<AddVoucherCmd, VoucherDto>
{
    private readonly FinanceDbContext _context;

    public AddVoucherHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherDto> Handle(AddVoucherCmd request, CancellationToken ct)
    {
        // Validate total debit = total credit
        var totalDebit = request.AddDto.Lines.Sum(l => l.DebitAmount);
        var totalCredit = request.AddDto.Lines.Sum(l => l.CreditAmount);
        if (totalDebit != totalCredit)
            throw new InvalidOperationException("Total Debits must equal Total Credits");

        // ✅ Validate period is open AND active
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID '{request.AddDto.PeriodId}' not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Period '{period.Name}' is closed. Cannot create voucher.");

        // ✅ Check if period is active (current date within period range)
        var now = DateTime.UtcNow.Date;
        var periodStart = period.StartDate.Date;
        var periodEnd = period.EndDate.Date;

        if (now < periodStart || now > periodEnd)
        {
            throw new InvalidOperationException(
                $"Period '{period.Name}' is not active. Active period range: {periodStart:yyyy-MM-dd} to {periodEnd:yyyy-MM-dd}");
        }

        // ✅ Validate voucher date is within period range
        var voucherDate = request.AddDto.VoucherDate.Date;
        if (voucherDate < periodStart || voucherDate > periodEnd)
        {
            throw new InvalidOperationException(
                $"Voucher date must be within period range: {periodStart:yyyy-MM-dd} to {periodEnd:yyyy-MM-dd}");
        }

        // ✅ Validate accounts exist
        var accountIds = request.AddDto.Lines
            .Select(x => x.AccountId)
            .Distinct()
            .ToList();

        var existingAccounts = await _context.ChartOfAccounts
            .Where(x => accountIds.Contains(x.Id) && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingAccounts.Count != accountIds.Count)
            throw new InvalidOperationException("One or more accounts do not exist.");

        // Generate voucher number
        var year = DateTime.UtcNow.Year;
        var lastVoucher = await _context.Vouchers
            .Where(v => v.VoucherNumber.StartsWith($"VCH-{year}-"))
            .OrderByDescending(v => v.VoucherNumber)
            .FirstOrDefaultAsync(ct);

        var sequence = 1;
        if (lastVoucher != null)
        {
            var parts = lastVoucher.VoucherNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out var lastSeq))
                sequence = lastSeq + 1;
        }

        var voucherNumber = $"VCH-{year}-{sequence:D4}";

        var voucher = new Voucher
        {
            Id = Guid.NewGuid(),
            VoucherNumber = voucherNumber,
            VoucherType = request.AddDto.VoucherType,
            VendorId = request.AddDto.VendorId,
            VoucherDate = request.AddDto.VoucherDate,
            Description = request.AddDto.Description,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            Status = "Draft",
            PeriodId = request.AddDto.PeriodId,
            DateAdd = DateTime.UtcNow,
            RowVersion = Guid.NewGuid().ToString("N"),
            IsDeleted = false,
            Lines = request.AddDto.Lines.Select(l => new VoucherLine
            {
                Id = Guid.NewGuid(),
                AccountId = l.AccountId,
                Description = l.Description,
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount,
                PeriodId = request.AddDto.PeriodId,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            }).ToList()
        };

        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync(ct);

        // Return mapped DTO
        return await MapToDto(voucher, ct);
    }

    public async Task<VoucherDto> MapToDto(Voucher voucher, CancellationToken ct)
    {
        var result = await _context.Vouchers
            .Include(v => v.Lines)
                .ThenInclude(l => l.Account)
            .Include(v => v.Period)
            .Include(v => v.Vendor)
            .FirstAsync(v => v.Id == voucher.Id, ct);

        return new VoucherDto
        {
            Id = result.Id,
            VoucherNumber = result.VoucherNumber,
            VoucherType = result.VoucherType,
            VendorId = result.VendorId,
            VendorName = result.Vendor?.Name,
            VoucherDate = result.VoucherDate,
            Description = result.Description,
            TotalDebit = result.TotalDebit,
            TotalCredit = result.TotalCredit,
            Status = result.Status,
            PeriodId = result.PeriodId,
            PeriodName = result.Period?.Name,
            Lines = result.Lines.Where(l => !l.IsDeleted).Select(l => new VoucherLineDto
            {
                Id = l.Id,
                AccountId = l.AccountId,
                AccountName = l.Account?.Name,
                AccountCode = l.Account?.Code,
                Description = l.Description,
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount,
                PeriodId = l.PeriodId
            }).ToList(),
            ApprovedBy = result.ApprovedBy,
            ApprovedAt = result.ApprovedAt,
            PostedBy = result.PostedBy,
            PostedAt = result.PostedAt,
            DateAdd = result.DateAdd,
            DateMod = result.DateMod,
            RowVersion = result.RowVersion
        };
    }
}
// Commands/VoucherCmd.cs - EditVoucherHandler

public class EditVoucherHandler : IRequestHandler<EditVoucherCmd, VoucherDto>
{
    private readonly FinanceDbContext _context;

    public EditVoucherHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherDto> Handle(EditVoucherCmd request, CancellationToken ct)
    {
        // ✅ Include tracking for concurrency
        var voucher = await _context.Vouchers
            .AsTracking()
            .Include(v => v.Lines)
            .Include(v => v.Period)
            .Include(v => v.Vendor)
            .FirstOrDefaultAsync(v => v.Id == request.EditDto.Id && !v.IsDeleted, ct);

        if (voucher == null)
            throw new InvalidOperationException($"Voucher with ID '{request.EditDto.Id}' not found");

        // ✅ Check RowVersion for concurrency
        if (!string.IsNullOrEmpty(voucher.RowVersion) &&
            voucher.RowVersion != request.EditDto.RowVersion)
        {
            throw new InvalidOperationException(
                "Voucher was modified by another user. Please refresh and try again.");
        }

        // Can only edit Draft or Rejected vouchers
        if (voucher.Status != "Draft" && voucher.Status != "Rejected")
            throw new InvalidOperationException($"Cannot edit voucher with status '{voucher.Status}'. Only Draft or Rejected can be edited.");

        // ✅ Validate the selected period is open AND active
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.EditDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID '{request.EditDto.PeriodId}' not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Period '{period.Name}' is closed. Cannot edit voucher.");

        // ✅ Check if period is active (current date within period range)
        var now = DateTime.UtcNow.Date;
        var periodStart = period.StartDate.Date;
        var periodEnd = period.EndDate.Date;

        if (now < periodStart || now > periodEnd)
        {
            throw new InvalidOperationException(
                $"Period '{period.Name}' is not active. Active period range: {periodStart:yyyy-MM-dd} to {periodEnd:yyyy-MM-dd}");
        }

        // ✅ Validate voucher date is within period range
        var voucherDate = request.EditDto.VoucherDate.Date;
        if (voucherDate < periodStart || voucherDate > periodEnd)
        {
            throw new InvalidOperationException(
                $"Voucher date must be within period range: {periodStart:yyyy-MM-dd} to {periodEnd:yyyy-MM-dd}");
        }

        // ✅ If period is changing, validate the new period for the date
        if (voucher.PeriodId != request.EditDto.PeriodId)
        {
            var newPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == request.EditDto.PeriodId && !p.IsDeleted, ct);

            if (newPeriod == null)
                throw new InvalidOperationException($"New period with ID '{request.EditDto.PeriodId}' not found");

            if (newPeriod.IsClosed)
                throw new InvalidOperationException($"Cannot move voucher to a closed period: {newPeriod.Name}");

            var newPeriodStart = newPeriod.StartDate.Date;
            var newPeriodEnd = newPeriod.EndDate.Date;

            if (voucherDate < newPeriodStart || voucherDate > newPeriodEnd)
            {
                throw new InvalidOperationException(
                    $"Voucher date must be within the new period range: {newPeriodStart:yyyy-MM-dd} to {newPeriodEnd:yyyy-MM-dd}");
            }
        }

        // Validate total debit = total credit
        var totalDebit = request.EditDto.Lines.Sum(l => l.DebitAmount);
        var totalCredit = request.EditDto.Lines.Sum(l => l.CreditAmount);
        if (totalDebit != totalCredit)
            throw new InvalidOperationException("Total Debits must equal Total Credits");

        // ✅ Validate accounts exist
        var accountIds = request.EditDto.Lines
            .Select(x => x.AccountId)
            .Distinct()
            .ToList();

        var existingAccounts = await _context.ChartOfAccounts
            .Where(x => accountIds.Contains(x.Id) && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingAccounts.Count != accountIds.Count)
            throw new InvalidOperationException("One or more accounts do not exist.");

        // ✅ Update voucher properties
        voucher.VoucherType = request.EditDto.VoucherType;
        voucher.VendorId = request.EditDto.VendorId;
        voucher.VoucherDate = request.EditDto.VoucherDate;
        voucher.Description = request.EditDto.Description;
        voucher.PeriodId = request.EditDto.PeriodId;
        voucher.TotalDebit = totalDebit;
        voucher.TotalCredit = totalCredit;
        voucher.DateMod = DateTime.UtcNow;

        // ✅ Generate new RowVersion for this update
        voucher.RowVersion = Guid.NewGuid().ToString("N");

        // ✅ Update lines - soft delete existing, add new
        var existingLines = await _context.VoucherLines
            .AsTracking()
            .Where(x => x.VoucherId == voucher.Id)
            .ToListAsync(ct);

        // Soft delete existing lines
        foreach (var line in existingLines)
        {
            line.IsDeleted = true;
            line.DateMod = DateTime.UtcNow;
        }

        // Add new lines
        foreach (var lineDto in request.EditDto.Lines)
        {
            _context.VoucherLines.Add(new VoucherLine
            {
                Id = Guid.CreateVersion7(),
                VoucherId = voucher.Id,
                AccountId = lineDto.AccountId,
                Description = lineDto.Description,
                DebitAmount = lineDto.DebitAmount,
                CreditAmount = lineDto.CreditAmount,
                PeriodId = request.EditDto.PeriodId,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            });
        }

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "Voucher was modified or deleted by another process. Reload and try again.");
        }

        // ✅ Return mapped DTO with new RowVersion
        return await MapToDto(voucher, ct);
    }

    private async Task<VoucherDto> MapToDto(Voucher voucher, CancellationToken ct)
    {
        // Load related data if needed
        if (voucher.Vendor == null)
        {
            await _context.Entry(voucher)
                .Reference(v => v.Vendor)
                .LoadAsync(ct);
        }

        if (voucher.Period == null)
        {
            await _context.Entry(voucher)
                .Reference(v => v.Period)
                .LoadAsync(ct);
        }

        return new VoucherDto
        {
            Id = voucher.Id,
            VoucherNumber = voucher.VoucherNumber,
            VoucherType = voucher.VoucherType,
            VendorId = voucher.VendorId,
            VendorName = voucher.Vendor?.Name,
            VoucherDate = voucher.VoucherDate,
            Description = voucher.Description,
            TotalDebit = voucher.TotalDebit,
            TotalCredit = voucher.TotalCredit,
            Status = voucher.Status,
            PeriodId = voucher.PeriodId,
            PeriodName = voucher.Period?.Name,
            Lines = voucher.Lines
                .Where(l => !l.IsDeleted)
                .Select(l => new VoucherLineDto
                {
                    Id = l.Id,
                    AccountId = l.AccountId,
                    AccountName = l.Account?.Name,
                    AccountCode = l.Account?.Code,
                    Description = l.Description,
                    DebitAmount = l.DebitAmount,
                    CreditAmount = l.CreditAmount,
                    PeriodId = l.PeriodId
                }).ToList(),
            ApprovedBy = voucher.ApprovedBy,
            ApprovedAt = voucher.ApprovedAt,
            PostedBy = voucher.PostedBy,
            PostedAt = voucher.PostedAt,
            DateAdd = voucher.DateAdd,
            DateMod = voucher.DateMod,
            RowVersion = voucher.RowVersion
        };
    }
}

public class DeleteVoucherHandler : IRequestHandler<DeleteVoucherCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteVoucherHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteVoucherCmd request, CancellationToken ct)
    {
        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, ct);

        if (voucher == null)
            return false;

        if (voucher.Status != "Draft" && voucher.Status != "Rejected")
            throw new InvalidOperationException($"Cannot delete voucher with status '{voucher.Status}'. Only Draft or Rejected can be deleted.");

        voucher.IsDeleted = true;
        voucher.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class ApproveVoucherHandler : IRequestHandler<ApproveVoucherCmd, VoucherDto>
{
    private readonly FinanceDbContext _context;

    public ApproveVoucherHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherDto> Handle(ApproveVoucherCmd request, CancellationToken ct)
    {
        var voucher = await _context.Vouchers
            .Include(v => v.Lines)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, ct);

        if (voucher == null)
            throw new InvalidOperationException($"Voucher with ID '{request.Id}' not found");

        if (voucher.Status != "Pending")
            throw new InvalidOperationException($"Cannot approve voucher with status '{voucher.Status}'. Only Pending can be approved.");

        voucher.Status = "Approved";
        voucher.ApprovedBy = "System"; // In real app, get from current user
        voucher.ApprovedAt = DateTime.UtcNow;
        voucher.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        var handler = new AddVoucherHandler(_context);
        return await handler.MapToDto(voucher, ct);
    }
}

public class RejectVoucherHandler : IRequestHandler<RejectVoucherCmd, VoucherDto>
{
    private readonly FinanceDbContext _context;

    public RejectVoucherHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherDto> Handle(RejectVoucherCmd request, CancellationToken ct)
    {
        var voucher = await _context.Vouchers
            .Include(v => v.Lines)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, ct);

        if (voucher == null)
            throw new InvalidOperationException($"Voucher with ID '{request.Id}' not found");

        if (voucher.Status != "Pending")
            throw new InvalidOperationException($"Cannot reject voucher with status '{voucher.Status}'. Only Pending can be rejected.");

        voucher.Status = "Rejected";
        voucher.Description = (voucher.Description ?? "") + $" | Rejected: {request.Reason}";
        voucher.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        var handler = new AddVoucherHandler(_context);
        return await handler.MapToDto(voucher, ct);
    }
}

public class PostVoucherHandler : IRequestHandler<PostVoucherCmd, VoucherDto>
{
    private readonly FinanceDbContext _context;

    public PostVoucherHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherDto> Handle(PostVoucherCmd request, CancellationToken ct)
    {
        var voucher = await _context.Vouchers
            .Include(v => v.Lines)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsDeleted, ct);

        if (voucher == null)
            throw new InvalidOperationException($"Voucher with ID '{request.Id}' not found");

        if (voucher.Status != "Approved")
            throw new InvalidOperationException($"Cannot post voucher with status '{voucher.Status}'. Only Approved can be posted.");

        // Create journal entries from voucher lines
        // This would typically create actual accounting entries
        // For now, just mark as posted

        voucher.Status = "Posted";
        voucher.PostedBy = "System";
        voucher.PostedAt = DateTime.UtcNow;
        voucher.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        var handler = new AddVoucherHandler(_context);
        return await handler.MapToDto(voucher, ct);
    }
}