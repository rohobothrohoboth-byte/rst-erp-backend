using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Models.Entities;

namespace Cor.Finance.Queries;

// ==================== QUERIES ====================

public class GetAllExpensesQry : IRequest<List<ExpenseDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Status { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? PeriodId { get; set; }  // ? ADDED
     public Guid? BranchId { get; set; }
        public Guid? DepartmentId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class GetExpenseByIdQry : IRequest<ExpenseDto>
{
    public Guid Id { get; set; }
}

public class GetExpensesByPeriodQry : IRequest<List<ExpenseDto>>  // ? NEW
{
    public Guid PeriodId { get; set; }
}

public class GetAllExpenseCategoriesQry : IRequest<List<ExpenseCategoryDto>>
{
    public bool? IsActive { get; set; }
}

public class GetExpenseCategoryByIdQry : IRequest<ExpenseCategoryDto>
{
    public Guid Id { get; set; }
}

// ==================== GET ALL EXPENSES HANDLER ====================

public class GetAllExpensesHandler : IRequestHandler<GetAllExpensesQry, List<ExpenseDto>>
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<GetAllExpensesHandler> _logger;

    public GetAllExpensesHandler(FinanceDbContext context, ILogger<GetAllExpensesHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ExpenseDto>> Handle(GetAllExpensesQry request, CancellationToken ct)
    {
        try
        {
            // ? SINGLE QUERY with ALL includes - NO N+1!
            var query = _context.Expenses
                .Include(x => x.Period)
                .Include(x => x.ExpenseCategory)  // ? Eager load category
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            // Apply filters
            if (request.PeriodId.HasValue)
                query = query.Where(x => x.PeriodId == request.PeriodId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.ExpenseDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.ExpenseDate <= request.ToDate.Value);

            if (!string.IsNullOrEmpty(request.Status))
                query = query.Where(x => x.Status == request.Status);

            if (request.BranchId.HasValue)
                query = query.Where(x => x.BranchId == request.BranchId.Value);

            if (request.DepartmentId.HasValue)
                query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

            if (request.MinAmount.HasValue)
                query = query.Where(x => x.Amount >= request.MinAmount.Value);

            if (request.MaxAmount.HasValue)
                query = query.Where(x => x.Amount <= request.MaxAmount.Value);

            // ? Execute ONE query
            var expenses = await query
                .OrderByDescending(x => x.ExpenseDate)
                .ToListAsync(ct);

            _logger.LogInformation("? Retrieved {Count} expenses with all related data", expenses.Count);

            // ? Map to DTOs (no additional queries)
            return expenses.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error retrieving expenses");
            throw;
        }
    }

    private static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            Amount = expense.Amount,
            Description = expense.Description,
            Status = expense.Status,
            PaymentMethod = expense.PaymentMethod,
            PeriodId = expense.PeriodId,
            PeriodName = expense.Period?.Name ?? "Unknown Period",
            BranchId = expense.BranchId,
            DepartmentId = expense.DepartmentId,
            EmployeeId = expense.EmployeeId,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            CategoryName = expense.ExpenseCategory?.Name ?? "Unknown Category",
            CategoryType = expense.ExpenseCategory?.CategoryType ?? "Unknown",
             VendorId = expense.VendorId,
                        VendorName = expense.Vendor?.Name ?? null,
            DateAdd = expense.DateAdd,
            DateMod = expense.DateMod,
            RowVersion = expense.RowVersion
        };
    }
}

// ==================== GET EXPENSE BY ID HANDLER ====================

public class GetExpenseByIdHandler : IRequestHandler<GetExpenseByIdQry, ExpenseDto>
{
    private readonly FinanceDbContext _context;

    public GetExpenseByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseDto> Handle(GetExpenseByIdQry request, CancellationToken ct)
    {
        var expense = await _context.Expenses
            .Include(x => x.Period)  // ? Include Period for PeriodName
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (expense == null)
            throw new InvalidOperationException($"Expense with ID '{request.Id}' not found");

        var category = await _context.ExpenseCategories
            .FirstOrDefaultAsync(x => x.Id == expense.ExpenseCategoryId && !x.IsDeleted, ct);

        return new ExpenseDto
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            CategoryName = category?.Name,
            Description = expense.Description,
            Amount = expense.Amount,
            PaymentMethod = expense.PaymentMethod,
            Status = expense.Status,
            PeriodId = expense.PeriodId,
            PeriodName = expense.Period?.Name,  // ? Get PeriodName
            BranchId = expense.BranchId,
            DepartmentId = expense.DepartmentId,
            EmployeeId = expense.EmployeeId,
             VendorId = expense.VendorId,
                    VendorName = expense.Vendor?.Name,
            DateAdd = expense.DateAdd,
            DateMod = expense.DateMod
        };
    }
}

// ==================== GET EXPENSES BY PERIOD HANDLER ====================

public class GetExpensesByPeriodHandler : IRequestHandler<GetExpensesByPeriodQry, List<ExpenseDto>>
{
    private readonly FinanceDbContext _context;

    public GetExpensesByPeriodHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseDto>> Handle(GetExpensesByPeriodQry request, CancellationToken ct)
    {
        var expenses = await _context.Expenses
            .Include(x => x.Period)
            .Where(x => x.PeriodId == request.PeriodId && !x.IsDeleted)
            .OrderByDescending(x => x.ExpenseDate)
            .ToListAsync(ct);

        var result = new List<ExpenseDto>();
        foreach (var expense in expenses)
        {
            var category = await _context.ExpenseCategories
                .FirstOrDefaultAsync(x => x.Id == expense.ExpenseCategoryId && !x.IsDeleted, ct);

            result.Add(new ExpenseDto
            {
                Id = expense.Id,
                ExpenseDate = expense.ExpenseDate,
                ExpenseCategoryId = expense.ExpenseCategoryId,
                CategoryName = category?.Name,
                Description = expense.Description,
                Amount = expense.Amount,
                PaymentMethod = expense.PaymentMethod,
                Status = expense.Status,
                PeriodId = expense.PeriodId,
                PeriodName = expense.Period?.Name,
                BranchId = expense.BranchId,
                DepartmentId = expense.DepartmentId,
                EmployeeId = expense.EmployeeId,
                DateAdd = expense.DateAdd,
                DateMod = expense.DateMod
            });
        }

        return result;
    }
}

// ==================== GET ALL EXPENSE CATEGORIES HANDLER ====================

public class GetAllExpenseCategoriesHandler : IRequestHandler<GetAllExpenseCategoriesQry, List<ExpenseCategoryDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllExpenseCategoriesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseCategoryDto>> Handle(GetAllExpenseCategoriesQry request, CancellationToken ct)
    {
        var query = _context.ExpenseCategories
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        var categories = await query
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return categories.Select(category => new ExpenseCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAm = category.NameAm,
            CategoryType = category.CategoryType,
            IsActive = category.IsActive,
            DateAdd = category.DateAdd,
            DateMod = category.DateMod
        }).ToList();
    }
}

// ==================== GET EXPENSE CATEGORY BY ID HANDLER ====================

public class GetExpenseCategoryByIdHandler : IRequestHandler<GetExpenseCategoryByIdQry, ExpenseCategoryDto>
{
    private readonly FinanceDbContext _context;

    public GetExpenseCategoryByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseCategoryDto> Handle(GetExpenseCategoryByIdQry request, CancellationToken ct)
    {
        var category = await _context.ExpenseCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (category == null)
            throw new InvalidOperationException($"Expense category with ID '{request.Id}' not found");

        return new ExpenseCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAm = category.NameAm,
            CategoryType = category.CategoryType,
            IsActive = category.IsActive,
            DateAdd = category.DateAdd,
            DateMod = category.DateMod
        };
    }
}