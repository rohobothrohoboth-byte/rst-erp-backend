// Commands/BudgetCodeCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddBudgetCodeCmd : IRequest<BudgetCodeDto>
{
    public AddBudgetCodeDto AddDto { get; set; } = new();
}

public class EditBudgetCodeCmd : IRequest<BudgetCodeDto>
{
    public EditBudgetCodeDto EditDto { get; set; } = new();
}

public class DeleteBudgetCodeCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class AddBudgetCodeHandler : IRequestHandler<AddBudgetCodeCmd, BudgetCodeDto>
{
    private readonly FinanceDbContext _context;

    public AddBudgetCodeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCodeDto> Handle(AddBudgetCodeCmd request, CancellationToken ct)
    {
        var exists = await _context.BudgetCodes
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Budget code with code '{request.AddDto.Code}' already exists");

        var code = new BudgetCode
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            NameAm = request.AddDto.NameAm,
            Description = request.AddDto.Description,
            BudgetType = request.AddDto.BudgetType,
            FiscalYear = request.AddDto.FiscalYear,
            TotalAmount = request.AddDto.TotalAmount,
            IsActive = request.AddDto.IsActive,
            DateAdd = DateTime.UtcNow
        };

        _context.BudgetCodes.Add(code);
        await _context.SaveChangesAsync(ct);

        return new BudgetCodeDto
        {
            Id = code.Id,
            Code = code.Code,
            Name = code.Name,
            NameAm = code.NameAm,
            Description = code.Description,
            BudgetType = code.BudgetType,
            FiscalYear = code.FiscalYear,
            TotalAmount = code.TotalAmount,
            IsActive = code.IsActive,
            DateAdd = code.DateAdd,
            DateMod = code.DateMod
        };
    }
}

public class EditBudgetCodeHandler : IRequestHandler<EditBudgetCodeCmd, BudgetCodeDto>
{
    private readonly FinanceDbContext _context;

    public EditBudgetCodeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCodeDto> Handle(EditBudgetCodeCmd request, CancellationToken ct)
    {
        var code = await _context.BudgetCodes
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (code == null)
            throw new InvalidOperationException($"Budget code with ID '{request.EditDto.Id}' not found");

        var exists = await _context.BudgetCodes
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Budget code with code '{request.EditDto.Code}' already exists");

        code.Code = request.EditDto.Code;
        code.Name = request.EditDto.Name;
        code.NameAm = request.EditDto.NameAm;
        code.Description = request.EditDto.Description;
        code.BudgetType = request.EditDto.BudgetType;
        code.FiscalYear = request.EditDto.FiscalYear;
        code.TotalAmount = request.EditDto.TotalAmount;
        code.IsActive = request.EditDto.IsActive;
        code.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new BudgetCodeDto
        {
            Id = code.Id,
            Code = code.Code,
            Name = code.Name,
            NameAm = code.NameAm,
            Description = code.Description,
            BudgetType = code.BudgetType,
            FiscalYear = code.FiscalYear,
            TotalAmount = code.TotalAmount,
            IsActive = code.IsActive,
            DateAdd = code.DateAdd,
            DateMod = code.DateMod
        };
    }
}

public class DeleteBudgetCodeHandler : IRequestHandler<DeleteBudgetCodeCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteBudgetCodeHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBudgetCodeCmd request, CancellationToken ct)
    {
        var code = await _context.BudgetCodes
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (code == null)
            return false;

        code.IsDeleted = true;
        code.IsActive = false;
        code.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}