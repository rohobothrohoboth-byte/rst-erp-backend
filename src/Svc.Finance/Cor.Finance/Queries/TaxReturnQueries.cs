// Queries/TaxReturnQueries.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllTaxReturnsQry : IRequest<List<TaxReturnDto>>
{
    public string? Period { get; set; }
    public string? FiscalYear { get; set; }
    public string? Status { get; set; }
    public string? TaxType { get; set; }
}

public class GetTaxReturnByIdQry : IRequest<TaxReturnDto>
{
    public Guid Id { get; set; }
}

public class GetAllTaxReturnsHandler : IRequestHandler<GetAllTaxReturnsQry, List<TaxReturnDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllTaxReturnsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaxReturnDto>> Handle(GetAllTaxReturnsQry request, CancellationToken ct)
    {
        var query = _context.TaxReturns
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Period))
            query = query.Where(x => x.Period == request.Period);

        if (!string.IsNullOrEmpty(request.FiscalYear))
            query = query.Where(x => x.FiscalYear == request.FiscalYear);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (!string.IsNullOrEmpty(request.TaxType))
            query = query.Where(x => x.TaxType == request.TaxType);

        return await query
            .Select(x => new TaxReturnDto
            {
                Id = x.Id,
                Code = x.Code,
                TaxType = x.TaxType,
                Period = x.Period,
                FiscalYear = x.FiscalYear,
                FilingDate = x.FilingDate,
                DueDate = x.DueDate,
                TaxableAmount = x.TaxableAmount,
                TaxAmount = x.TaxAmount,
                AmountPaid = x.AmountPaid,
                BalanceDue = x.BalanceDue,
                Status = x.Status,
                FiledBy = x.FiledBy,
                Notes = x.Notes,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetTaxReturnByIdHandler : IRequestHandler<GetTaxReturnByIdQry, TaxReturnDto>
{
    private readonly FinanceDbContext _context;

    public GetTaxReturnByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<TaxReturnDto> Handle(GetTaxReturnByIdQry request, CancellationToken ct)
    {
        var taxReturn = await _context.TaxReturns
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (taxReturn == null)
            throw new InvalidOperationException($"Tax return with ID '{request.Id}' not found");

        return new TaxReturnDto
        {
            Id = taxReturn.Id,
            Code = taxReturn.Code,
            TaxType = taxReturn.TaxType,
            Period = taxReturn.Period,
            FiscalYear = taxReturn.FiscalYear,
            FilingDate = taxReturn.FilingDate,
            DueDate = taxReturn.DueDate,
            TaxableAmount = taxReturn.TaxableAmount,
            TaxAmount = taxReturn.TaxAmount,
            AmountPaid = taxReturn.AmountPaid,
            BalanceDue = taxReturn.BalanceDue,
            Status = taxReturn.Status,
            FiledBy = taxReturn.FiledBy,
            Notes = taxReturn.Notes,
            DateAdd = taxReturn.DateAdd,
            DateMod = taxReturn.DateMod
        };
    }
}