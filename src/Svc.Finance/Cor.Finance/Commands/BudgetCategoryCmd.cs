// Commands/BudgetCategoryCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddBudgetCategoryCmd : IRequest<BudgetCategoryDto>
{
    public AddBudgetCategoryDto AddDto { get; set; } = new();
}

public class EditBudgetCategoryCmd : IRequest<BudgetCategoryDto>
{
    public EditBudgetCategoryDto EditDto { get; set; } = new();
}

public class DeleteBudgetCategoryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class AddBudgetCategoryHandler : IRequestHandler<AddBudgetCategoryCmd, BudgetCategoryDto>
{
    private readonly FinanceDbContext _context;

    public AddBudgetCategoryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCategoryDto> Handle(AddBudgetCategoryCmd request, CancellationToken ct)
    {
        var exists = await _context.BudgetCategories
            .AnyAsync(x => x.Code == request.AddDto.Code && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Budget category with code '{request.AddDto.Code}' already exists");

        var category = new BudgetCategory
        {
            Id = Guid.NewGuid(),
            Code = request.AddDto.Code,
            Name = request.AddDto.Name,
            NameAm = request.AddDto.NameAm,
            Description = request.AddDto.Description,
            Color = request.AddDto.Color,
            Icon = request.AddDto.Icon,
            IsActive = request.AddDto.IsActive,
            DateAdd = DateTime.UtcNow
        };

        _context.BudgetCategories.Add(category);
        await _context.SaveChangesAsync(ct);

        return new BudgetCategoryDto
        {
            Id = category.Id,
            Code = category.Code,
            Name = category.Name,
            NameAm = category.NameAm,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon,
            IsActive = category.IsActive,
            DateAdd = category.DateAdd,
            DateMod = category.DateMod
        };
    }
}

public class EditBudgetCategoryHandler : IRequestHandler<EditBudgetCategoryCmd, BudgetCategoryDto>
{
    private readonly FinanceDbContext _context;

    public EditBudgetCategoryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCategoryDto> Handle(EditBudgetCategoryCmd request, CancellationToken ct)
    {
        var category = await _context.BudgetCategories
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, ct);

        if (category == null)
            throw new InvalidOperationException($"Budget category with ID '{request.EditDto.Id}' not found");

        var exists = await _context.BudgetCategories
            .AnyAsync(x => x.Code == request.EditDto.Code && x.Id != request.EditDto.Id && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Budget category with code '{request.EditDto.Code}' already exists");

        category.Code = request.EditDto.Code;
        category.Name = request.EditDto.Name;
        category.NameAm = request.EditDto.NameAm;
        category.Description = request.EditDto.Description;
        category.Color = request.EditDto.Color;
        category.Icon = request.EditDto.Icon;
        category.IsActive = request.EditDto.IsActive;
        category.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return new BudgetCategoryDto
        {
            Id = category.Id,
            Code = category.Code,
            Name = category.Name,
            NameAm = category.NameAm,
            Description = category.Description,
            Color = category.Color,
            Icon = category.Icon,
            IsActive = category.IsActive,
            DateAdd = category.DateAdd,
            DateMod = category.DateMod
        };
    }
}

public class DeleteBudgetCategoryHandler : IRequestHandler<DeleteBudgetCategoryCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteBudgetCategoryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBudgetCategoryCmd request, CancellationToken ct)
    {
        var category = await _context.BudgetCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (category == null)
            return false;

        category.IsDeleted = true;
        category.IsActive = false;
        category.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}