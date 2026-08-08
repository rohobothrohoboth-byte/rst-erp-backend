// Queries/BudgetCategoryQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllBudgetCategoriesQry : IRequest<List<BudgetCategoryDto>>
{
    public bool? IsActive { get; set; }
}

public class GetBudgetCategoryByIdQry : IRequest<BudgetCategoryDto>
{
    public Guid Id { get; set; }
}

public class GetAllBudgetCategoriesHandler : IRequestHandler<GetAllBudgetCategoriesQry, List<BudgetCategoryDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllBudgetCategoriesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BudgetCategoryDto>> Handle(GetAllBudgetCategoriesQry request, CancellationToken ct)
    {
        var query = _context.BudgetCategories
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        return await query
            .Select(x => new BudgetCategoryDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                NameAm = x.NameAm,
                Description = x.Description,
                Color = x.Color,
                Icon = x.Icon,
                IsActive = x.IsActive,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetBudgetCategoryByIdHandler : IRequestHandler<GetBudgetCategoryByIdQry, BudgetCategoryDto>
{
    private readonly FinanceDbContext _context;

    public GetBudgetCategoryByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCategoryDto> Handle(GetBudgetCategoryByIdQry request, CancellationToken ct)
    {
        var category = await _context.BudgetCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (category == null)
            throw new InvalidOperationException($"Budget category with ID '{request.Id}' not found");

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