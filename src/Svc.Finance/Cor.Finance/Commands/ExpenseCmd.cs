using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

// ==================== EXPENSE CATEGORY COMMANDS ====================

public class AddExpenseCategoryCmd : IRequest<ExpenseCategoryDto>
{
    public AddExpenseCategoryDto AddDto { get; set; } = default!;
}

public class EditExpenseCategoryCmd : IRequest<ExpenseCategoryDto>
{
    public EditExpenseCategoryDto EditDto { get; set; } = default!;
}

public class DeleteExpenseCategoryCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleExpenseCategoryStatusCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ==================== EXPENSE CATEGORY HANDLERS ====================

public class AddExpenseCategoryHandler : IRequestHandler<AddExpenseCategoryCmd, ExpenseCategoryDto>
{
    private readonly FinanceDbContext _context;

    public AddExpenseCategoryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseCategoryDto> Handle(AddExpenseCategoryCmd request, CancellationToken cancellationToken)
    {
        // Check if category already exists
        var exists = await _context.ExpenseCategories
            .AnyAsync(x => x.Name == request.AddDto.Name && !x.IsDeleted, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Category '{request.AddDto.Name}' already exists.");

        var category = new ExpenseCategory
        {
            Id = Guid.NewGuid(),
            Name = request.AddDto.Name,
            NameAm = request.AddDto.NameAm,
            CategoryType = request.AddDto.CategoryType,
            IsActive = true,
            DateAdd = DateTime.UtcNow,
            DateMod = null,
            IsDeleted = false
        };

        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

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

public class EditExpenseCategoryHandler : IRequestHandler<EditExpenseCategoryCmd, ExpenseCategoryDto>
{
    private readonly FinanceDbContext _context;

    public EditExpenseCategoryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseCategoryDto> Handle(EditExpenseCategoryCmd request, CancellationToken cancellationToken)
    {
        var category = await _context.ExpenseCategories
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, cancellationToken);

        if (category == null)
            throw new InvalidOperationException($"Category with ID '{request.EditDto.Id}' not found");

        // Check if name already exists (excluding current)
        var exists = await _context.ExpenseCategories
            .AnyAsync(x => x.Name == request.EditDto.Name && x.Id != request.EditDto.Id && !x.IsDeleted, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Category '{request.EditDto.Name}' already exists.");

        category.Name = request.EditDto.Name;
        category.NameAm = request.EditDto.NameAm;
        category.CategoryType = request.EditDto.CategoryType;
        category.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

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

public class DeleteExpenseCategoryHandler : IRequestHandler<DeleteExpenseCategoryCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteExpenseCategoryHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteExpenseCategoryCmd request, CancellationToken cancellationToken)
    {
        var category = await _context.ExpenseCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (category == null)
            return false;

        // Check if category has expenses
        var hasExpenses = await _context.Expenses
            .AnyAsync(x => x.ExpenseCategoryId == request.Id && !x.IsDeleted, cancellationToken);

        if (hasExpenses)
            throw new InvalidOperationException("Cannot delete category with existing expenses.");

        category.IsDeleted = true;
        category.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class ToggleExpenseCategoryStatusHandler : IRequestHandler<ToggleExpenseCategoryStatusCmd, bool>
{
    private readonly FinanceDbContext _context;

    public ToggleExpenseCategoryStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleExpenseCategoryStatusCmd request, CancellationToken cancellationToken)
    {
        var category = await _context.ExpenseCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (category == null)
            return false;

        category.IsActive = !category.IsActive;
        category.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// ==================== EXPENSE COMMANDS ====================

public class AddExpenseCmd : IRequest<ExpenseDto>
{
    public AddExpenseDto AddDto { get; set; } = default!;
}

public class EditExpenseCmd : IRequest<ExpenseDto>
{
    public EditExpenseDto EditDto { get; set; } = default!;
}

public class DeleteExpenseCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class BulkAddExpenseCmd : IRequest<List<ExpenseDto>>
{
    public List<AddExpenseDto> AddDtos { get; set; } = new();
}

public class BulkAddExpenseCategoryCmd : IRequest<List<ExpenseCategoryDto>>
{
    public List<AddExpenseCategoryDto> AddDtos { get; set; } = new();
}

// ==================== BULK ADD EXPENSE CATEGORY HANDLER ====================


// ==================== BULK ADD EXPENSE HANDLER ====================

public class BulkAddExpenseHandler : IRequestHandler<BulkAddExpenseCmd, List<ExpenseDto>>
{
    private readonly FinanceDbContext _context;

    public BulkAddExpenseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseDto>> Handle(BulkAddExpenseCmd request, CancellationToken ct)
    {
        var results = new List<ExpenseDto>();

        foreach (var dto in request.AddDtos)
        {
            // ? VALIDATE PERIOD FOR EACH EXPENSE
            var period = await _context.FinancialPeriods
                .FirstOrDefaultAsync(p => p.Id == dto.PeriodId && !p.IsDeleted, ct);

            if (period == null)
                throw new InvalidOperationException($"Period with ID {dto.PeriodId} not found");

            if (period.IsClosed)
                throw new InvalidOperationException($"Cannot create expense in a closed period: {period.Name}");

            // Convert date to UTC
            var expenseDate = dto.ExpenseDate;
            if (expenseDate.Kind != DateTimeKind.Utc)
                expenseDate = DateTime.SpecifyKind(expenseDate, DateTimeKind.Utc);

            // ? VALIDATE EXPENSE DATE IS WITHIN PERIOD RANGE
            if (expenseDate < period.StartDate || expenseDate > period.EndDate)
                throw new InvalidOperationException(
                    $"Expense date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                ExpenseDate = expenseDate,
                ExpenseCategoryId = dto.ExpenseCategoryId,
                Description = dto.Description,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = "Pending",
                // ? PeriodId - REQUIRED
                PeriodId = period.Id,
                BranchId = dto.BranchId,
                DepartmentId = dto.DepartmentId,
                EmployeeId = dto.EmployeeId,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(ct);

            results.Add(new ExpenseDto
            {
                Id = expense.Id,
                ExpenseDate = expense.ExpenseDate,
                ExpenseCategoryId = expense.ExpenseCategoryId,
                Description = expense.Description,
                Amount = expense.Amount,
                PaymentMethod = expense.PaymentMethod,
                Status = expense.Status,
                PeriodId = expense.PeriodId,
                PeriodName = period.Name,
                BranchId = expense.BranchId,
                DepartmentId = expense.DepartmentId,
                EmployeeId = expense.EmployeeId,
                DateAdd = expense.DateAdd,
                DateMod = expense.DateMod
            });
        }

        return results;
    }
}

// ==================== EXPENSE HANDLERS ====================

public class AddExpenseHandler : IRequestHandler<AddExpenseCmd, ExpenseDto>
{
    private readonly FinanceDbContext _context;

    public AddExpenseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseDto> Handle(AddExpenseCmd request, CancellationToken cancellationToken)
    {
        // ? STEP 1: VALIDATE PERIOD EXISTS AND IS OPEN
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, cancellationToken);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {request.AddDto.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot create expense in a closed period: {period.Name}");

        // ? STEP 2: VALIDATE EXPENSE DATE IS WITHIN PERIOD RANGE
        var expenseDate = request.AddDto.ExpenseDate;
        if (expenseDate.Kind != DateTimeKind.Utc)
            expenseDate = DateTime.SpecifyKind(expenseDate, DateTimeKind.Utc);

        if (expenseDate < period.StartDate || expenseDate > period.EndDate)
            throw new InvalidOperationException(
                $"Expense date must be between {period.StartDate:yyyy-MM-dd} and {period.EndDate:yyyy-MM-dd}");

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            ExpenseDate = expenseDate,
            ExpenseCategoryId = request.AddDto.ExpenseCategoryId,
            Description = request.AddDto.Description,
            Amount = request.AddDto.Amount,
            PaymentMethod = request.AddDto.PaymentMethod,
            Status = "Pending",
            // ? PeriodId - REQUIRED
            PeriodId = period.Id,
            BranchId = request.AddDto.BranchId,
            DepartmentId = request.AddDto.DepartmentId,
            EmployeeId = request.AddDto.EmployeeId,
            DateAdd = DateTime.UtcNow,
            DateMod = null,
            IsDeleted = false
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);

        return new ExpenseDto
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            Description = expense.Description,
            Amount = expense.Amount,
            PaymentMethod = expense.PaymentMethod,
            Status = expense.Status,
            PeriodId = expense.PeriodId,
            PeriodName = period.Name,
            BranchId = expense.BranchId,
            DepartmentId = expense.DepartmentId,
            EmployeeId = expense.EmployeeId,
            DateAdd = expense.DateAdd,
            DateMod = expense.DateMod
        };
    }
}
public class EditExpenseHandler : IRequestHandler<EditExpenseCmd, ExpenseDto>
{
    private readonly FinanceDbContext _context;

    public EditExpenseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseDto> Handle(EditExpenseCmd request, CancellationToken cancellationToken)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(x => x.Id == request.EditDto.Id && !x.IsDeleted, cancellationToken);

        if (expense == null)
            throw new InvalidOperationException($"Expense with ID '{request.EditDto.Id}' not found");

        // Validate Period
        if (expense.PeriodId != request.EditDto.PeriodId)
        {
            var newPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    p => p.Id == request.EditDto.PeriodId && !p.IsDeleted,
                    cancellationToken);

            if (newPeriod == null)
                throw new InvalidOperationException(
                    $"Period with ID '{request.EditDto.PeriodId}' not found.");

            if (newPeriod.IsClosed)
                throw new InvalidOperationException(
                    $"Cannot move expense to closed period '{newPeriod.Name}'.");

            var expenseDate = request.EditDto.ExpenseDate;

            if (expenseDate.Kind != DateTimeKind.Utc)
                expenseDate = DateTime.SpecifyKind(expenseDate, DateTimeKind.Utc);

            if (expenseDate < newPeriod.StartDate ||
                expenseDate > newPeriod.EndDate)
            {
                throw new InvalidOperationException(
                    $"Expense date must be between {newPeriod.StartDate:yyyy-MM-dd} and {newPeriod.EndDate:yyyy-MM-dd}.");
            }

            expense.PeriodId = newPeriod.Id;
        }
        else
        {
            var currentPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    p => p.Id == expense.PeriodId && !p.IsDeleted,
                    cancellationToken);

            if (currentPeriod != null && currentPeriod.IsClosed)
                throw new InvalidOperationException(
                    $"Cannot update expense in closed period '{currentPeriod.Name}'.");
        }

        expense.ExpenseDate = request.EditDto.ExpenseDate;
        expense.ExpenseCategoryId = request.EditDto.ExpenseCategoryId;
        expense.Description = request.EditDto.Description;
        expense.Amount = request.EditDto.Amount;
        expense.PaymentMethod = request.EditDto.PaymentMethod;
        expense.BranchId = request.EditDto.BranchId;
        expense.DepartmentId = request.EditDto.DepartmentId;
        expense.EmployeeId = request.EditDto.EmployeeId;
        expense.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var expensePeriod = await _context.FinancialPeriods
            .FirstOrDefaultAsync(
                p => p.Id == expense.PeriodId && !p.IsDeleted,
                cancellationToken);

        return new ExpenseDto
        {
            Id = expense.Id,
            ExpenseDate = expense.ExpenseDate,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            Description = expense.Description,
            Amount = expense.Amount,
            PaymentMethod = expense.PaymentMethod,
            Status = expense.Status,

            PeriodId = expense.PeriodId,
            PeriodName = expensePeriod?.Name,

            BranchId = expense.BranchId,
            DepartmentId = expense.DepartmentId,
            EmployeeId = expense.EmployeeId,

            DateAdd = expense.DateAdd,
            DateMod = expense.DateMod
        };
    }
}
public class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteExpenseHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteExpenseCmd request, CancellationToken cancellationToken)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (expense == null)
            return false;

        // ? VALIDATE PERIOD IS OPEN BEFORE DELETING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == expense.PeriodId && !p.IsDeleted, cancellationToken);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {expense.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot delete expense in a closed period: {period.Name}");

        expense.IsDeleted = true;
        expense.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}