// Queries/VoucherQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;
namespace Cor.Finance.Queries;

public class GetAllVouchersQry : IRequest<List<VoucherDto>>
{
    public string? Status { get; set; }
    public string? VoucherType { get; set; }
    public Guid? PeriodId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetVoucherByIdQry : IRequest<VoucherDto>
{
    public Guid Id { get; set; }
}

// Queries/VoucherQry.cs

public class GetAllVouchersHandler : IRequestHandler<GetAllVouchersQry, List<VoucherDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllVouchersHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<VoucherDto>> Handle(GetAllVouchersQry request, CancellationToken ct)
    {
        var query = _context.Vouchers
            .Include(x => x.Lines)
                .ThenInclude(l => l.Account)
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.VoucherType))
            query = query.Where(x => x.VoucherType == request.VoucherType);

        if (request.PeriodId.HasValue)
            query = query.Where(x => x.PeriodId == request.PeriodId);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.VoucherDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.VoucherDate <= request.ToDate.Value);

        return await query
            .Select(x => new VoucherDto
            {
                Id = x.Id,
                VoucherNumber = x.VoucherNumber,
                VoucherType = x.VoucherType,
                VendorId = x.VendorId,
                VendorName = x.Vendor != null ? x.Vendor.Name : null,
                VoucherDate = x.VoucherDate,
                Description = x.Description,
                TotalDebit = x.TotalDebit,
                TotalCredit = x.TotalCredit,
                Status = x.Status,
                PeriodId = x.PeriodId,
                PeriodName = x.Period != null ? x.Period.Name : null,
                Lines = x.Lines.Select(l => new VoucherLineDto
                {
                    Id = l.Id,
                    AccountId = l.AccountId,
                    AccountName = l.Account != null ? l.Account.Name : null,
                    AccountCode = l.Account != null ? l.Account.Code : null,
                    Description = l.Description,
                    DebitAmount = l.DebitAmount,
                    CreditAmount = l.CreditAmount,
                    PeriodId = l.PeriodId
                }).ToList(),
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
                PostedBy = x.PostedBy,
                PostedAt = x.PostedAt,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod,

                RowVersion = x.RowVersion
            })
            .ToListAsync(ct);
    }
}
// Queries/VoucherQry.cs

public class GetVoucherByIdHandler : IRequestHandler<GetVoucherByIdQry, VoucherDto>
{
    private readonly FinanceDbContext _context;

    public GetVoucherByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherDto> Handle(GetVoucherByIdQry request, CancellationToken ct)
    {
        var voucher = await _context.Vouchers
            .Include(x => x.Lines)
                .ThenInclude(l => l.Account)
            .Include(x => x.Period)
            .Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (voucher == null)
            throw new InvalidOperationException($"Voucher with ID '{request.Id}' not found");

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
            Lines = voucher.Lines.Select(l => new VoucherLineDto
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
