// Commands/TaxReturnCommands.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddTaxReturnCmd : IRequest<TaxReturnDto>
{
    public AddTaxReturnDto AddDto { get; set; } = new();
}

public class EditTaxReturnCmd : IRequest<TaxReturnDto>
{
    public EditTaxReturnDto EditDto { get; set; } = new();
}

public class DeleteTaxReturnCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class FileTaxReturnCmd : IRequest<TaxReturnDto>
{
    public Guid Id { get; set; }
}
public static class TaxReturnMapper
{
    public static TaxReturnDto MapToDto(TaxReturn taxReturn)
    {
        if (taxReturn == null) return null!;

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
public class AddTaxReturnHandler : IRequestHandler<AddTaxReturnCmd, TaxReturnDto>
{
    private readonly FinanceDbContext _context;

    public AddTaxReturnHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<TaxReturnDto> Handle(AddTaxReturnCmd request, CancellationToken ct)
    {
        var exists = await _context.TaxReturns
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Tax return with code '{request.AddDto.Code}' already exists");

        var taxReturn = new TaxReturn
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            TaxType = request.AddDto.TaxType,
            Period = request.AddDto.Period,
            FiscalYear = request.AddDto.FiscalYear,
            FilingDate = request.AddDto.FilingDate,
            DueDate = request.AddDto.DueDate,
            TaxableAmount = request.AddDto.TaxableAmount,
            TaxAmount = request.AddDto.TaxAmount,
            AmountPaid = request.AddDto.AmountPaid,
            BalanceDue = request.AddDto.BalanceDue,
            Status = request.AddDto.Status ?? "Pending",
            FiledBy = request.AddDto.FiledBy,
            Notes = request.AddDto.Notes,
            DateAdd = DateTime.UtcNow
        };

        _context.TaxReturns.Add(taxReturn);
        await _context.SaveChangesAsync(ct);

        return TaxReturnMapper.MapToDto(taxReturn);
    }


}

public class EditTaxReturnHandler : IRequestHandler<EditTaxReturnCmd, TaxReturnDto>
{
    private readonly FinanceDbContext _context;

    public EditTaxReturnHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<TaxReturnDto> Handle(EditTaxReturnCmd request, CancellationToken ct)
    {
        var taxReturn = await _context.TaxReturns
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (taxReturn == null)
            throw new InvalidOperationException($"Tax return with ID '{request.EditDto.Id}' not found");

        taxReturn.Code = request.EditDto.Code;
        taxReturn.TaxType = request.EditDto.TaxType;
        taxReturn.Period = request.EditDto.Period;
        taxReturn.FiscalYear = request.EditDto.FiscalYear;
        taxReturn.FilingDate = request.EditDto.FilingDate;
        taxReturn.DueDate = request.EditDto.DueDate;
        taxReturn.TaxableAmount = request.EditDto.TaxableAmount;
        taxReturn.TaxAmount = request.EditDto.TaxAmount;
        taxReturn.AmountPaid = request.EditDto.AmountPaid;
        taxReturn.BalanceDue = request.EditDto.BalanceDue;
        taxReturn.Status = request.EditDto.Status;
        taxReturn.FiledBy = request.EditDto.FiledBy;
        taxReturn.Notes = request.EditDto.Notes;
        taxReturn.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return TaxReturnMapper.MapToDto(taxReturn);
    }


}

public class DeleteTaxReturnHandler : IRequestHandler<DeleteTaxReturnCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteTaxReturnHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteTaxReturnCmd request, CancellationToken ct)
    {
        var taxReturn = await _context.TaxReturns
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (taxReturn == null)
            return false;

        taxReturn.IsDeleted = true;
        taxReturn.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class FileTaxReturnHandler : IRequestHandler<FileTaxReturnCmd, TaxReturnDto>
{
    private readonly FinanceDbContext _context;

    public FileTaxReturnHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<TaxReturnDto> Handle(FileTaxReturnCmd request, CancellationToken ct)
    {
        var taxReturn = await _context.TaxReturns
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (taxReturn == null)
            throw new InvalidOperationException($"Tax return with ID '{request.Id}' not found");

        taxReturn.Status = "Filed";
        taxReturn.FilingDate = DateTime.UtcNow;
        taxReturn.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);


         return TaxReturnMapper.MapToDto(taxReturn);
    }
}