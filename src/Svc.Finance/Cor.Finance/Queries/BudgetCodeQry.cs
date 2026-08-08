// Queries/BudgetCodeQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

public class GetAllBudgetCodesQry : IRequest<List<BudgetCodeDto>>
{
    public bool? IsActive { get; set; }
    public string? BudgetType { get; set; }
    public string? FiscalYear { get; set; }
}

public class GetBudgetCodeByIdQry : IRequest<BudgetCodeDto>
{
    public Guid Id { get; set; }
}

public class GetAllBudgetCodesHandler : IRequestHandler<GetAllBudgetCodesQry, List<BudgetCodeDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllBudgetCodesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<BudgetCodeDto>> Handle(GetAllBudgetCodesQry request, CancellationToken ct)
    {
        var query = _context.BudgetCodes
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.BudgetType))
            query = query.Where(x => x.BudgetType == request.BudgetType);

        if (!string.IsNullOrEmpty(request.FiscalYear))
            query = query.Where(x => x.FiscalYear == request.FiscalYear);

        return await query
            .Select(x => new BudgetCodeDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                NameAm = x.NameAm,
                Description = x.Description,
                BudgetType = x.BudgetType,
                FiscalYear = x.FiscalYear,
                TotalAmount = x.TotalAmount,
                IsActive = x.IsActive,
                DateAdd = x.DateAdd,
                DateMod = x.DateMod
            })
            .ToListAsync(ct);
    }
}

public class GetBudgetCodeByIdHandler : IRequestHandler<GetBudgetCodeByIdQry, BudgetCodeDto>
{
    private readonly FinanceDbContext _context;

    public GetBudgetCodeByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetCodeDto> Handle(GetBudgetCodeByIdQry request, CancellationToken ct)
    {
        var code = await _context.BudgetCodes
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (code == null)
            throw new InvalidOperationException($"Budget code with ID '{request.Id}' not found");

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