using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Commands;

public class AddBudgetCmd : IRequest<BudgetDto>
{
    public AddBudgetDto AddDto { get; set; } = default!;
}

public class EditBudgetCmd : IRequest<BudgetDto>
{
    public EditBudgetDto EditDto { get; set; } = default!;
}

public class DeleteBudgetCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ToggleBudgetStatusCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ==================== TOGGLE BUDGET STATUS HANDLER ====================

public class ToggleBudgetStatusHandler : IRequestHandler<ToggleBudgetStatusCmd, bool>
{
    private readonly FinanceDbContext _context;

    public ToggleBudgetStatusHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleBudgetStatusCmd request, CancellationToken ct)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (budget == null)
            return false;

        // ? VALIDATE PERIOD IS OPEN BEFORE TOGGLING STATUS
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == budget.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {budget.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot toggle budget status in a closed period: {period.Name}");

        budget.Status = budget.Status == "Active" ? "Inactive" : "Active";
        budget.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

// ==================== ADD BUDGET HANDLER ====================

// E:\untitled46\RST_ERP\src\Svc.Finance\Cor.Finance\Commands\BudgetCmd.cs

public class AddBudgetHandler : IRequestHandler<AddBudgetCmd, BudgetDto>
{
    private readonly FinanceDbContext _context;

    public AddBudgetHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetDto> Handle(AddBudgetCmd request, CancellationToken ct)
    {
        // ✅ STEP 1: VALIDATE BUDGET CODE EXISTS
        var budgetCode = await _context.BudgetCodes
            .FirstOrDefaultAsync(bc => bc.Id == request.AddDto.BudgetCodeId && !bc.IsDeleted, ct);

        if (budgetCode == null)
            throw new InvalidOperationException($"Budget Code with ID {request.AddDto.BudgetCodeId} not found");

        if (!budgetCode.IsActive)
            throw new InvalidOperationException($"Budget Code {budgetCode.Code} is inactive");

        // ✅ STEP 2: VALIDATE PERIOD EXISTS AND IS OPEN
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AddDto.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {request.AddDto.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot create budget in a closed period: {period.Name}");

        // ✅ STEP 3: VALIDATE BUDGET DATES ARE WITHIN PERIOD RANGE
        if (request.AddDto.StartDate < period.StartDate || request.AddDto.EndDate > period.EndDate)
            throw new InvalidOperationException(
                $"Budget dates must be within period range: {period.StartDate:yyyy-MM-dd} to {period.EndDate:yyyy-MM-dd}");

        // ✅ STEP 4: Validate budget lines exist
        if (request.AddDto.Lines == null || !request.AddDto.Lines.Any())
            throw new InvalidOperationException("Budget must have at least one line");

        // ✅ STEP 5: Validate all accounts exist
        var accountIds = request.AddDto.Lines.Select(x => x.AccountId).Distinct().ToList();
        var existingAccounts = await _context.ChartOfAccounts
            .Where(x => accountIds.Contains(x.Id) && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingAccounts.Count != accountIds.Count)
            throw new InvalidOperationException("One or more accounts do not exist");

        // ✅ STEP 6: Create Budget with BudgetCodeId
        var budget = new Budget
        {
            Id = Guid.CreateVersion7(),
            Name = request.AddDto.Name,
            BudgetCodeId = budgetCode.Id,  // ✅ Store the Id, not the Code
            StartDate = request.AddDto.StartDate,
            EndDate = request.AddDto.EndDate,
            Description = request.AddDto.Description,
            Status = "Draft",
            PeriodId = period.Id,
            BranchId = request.AddDto.BranchId,
            DepartmentId = request.AddDto.DepartmentId,
            TotalAmount = request.AddDto.Lines.Sum(x => x.AllocatedAmount),
            SpentAmount = 0,
            DateAdd = DateTime.UtcNow,
            DateMod = null,
            IsDeleted = false
        };

        foreach (var lineDto in request.AddDto.Lines)
        {
            budget.Lines.Add(new BudgetLine
            {
                Id = Guid.CreateVersion7(),
                BudgetId = budget.Id,
                AccountId = lineDto.AccountId,
                AllocatedAmount = lineDto.AllocatedAmount,
                SpentAmount = 0,
                Description = lineDto.Description,
                PeriodId = period.Id,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            });
        }

        await _context.Budgets.AddAsync(budget, ct);
        await _context.SaveChangesAsync(ct);

        return await MapToDto(budget, period, budgetCode, ct);
    }

    private async Task<BudgetDto> MapToDto(Budget budget, FinancialPeriod period, BudgetCode budgetCode, CancellationToken ct)
    {
        var lines = await _context.BudgetLines
            .Where(x => x.BudgetId == budget.Id && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        return new BudgetDto
        {
            Id = budget.Id,
            Name = budget.Name,
            BudgetCodeId = budget.BudgetCodeId,
            BudgetCode = budgetCode?.Code?.Trim() ?? string.Empty,
            StartDate = budget.StartDate,
            EndDate = budget.EndDate,
            TotalAmount = budget.TotalAmount,
            Status = budget.Status,
            Description = budget.Description,
            PeriodId = budget.PeriodId,
            PeriodName = period?.Name,
            BranchId = budget.BranchId,
            DepartmentId = budget.DepartmentId,
            Lines = lines.Select(line => new BudgetLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountName = line.Account?.Name,
                AccountCode = line.Account?.Code,
                AllocatedAmount = line.AllocatedAmount,
                SpentAmount = line.SpentAmount,
                RemainingAmount = line.AllocatedAmount - line.SpentAmount,
                Description = line.Description,
                PeriodId = line.PeriodId,
                PeriodName = period?.Name
            }).ToList(),
            DateAdd = budget.DateAdd,
            DateMod = budget.DateMod,
            RowVersion = budget.RowVersion ?? ""
        };
    }
}

// ==================== EDIT BUDGET HANDLER ====================

public class EditBudgetHandler : IRequestHandler<EditBudgetCmd, BudgetDto>
{
    private readonly FinanceDbContext _context;

    public EditBudgetHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetDto> Handle(EditBudgetCmd request, CancellationToken ct)
    {
        var budget = await _context.Budgets
               .AsTracking()
               .Include(x => x.Lines)
               .FirstOrDefaultAsync(
                   x => x.Id == request.EditDto.Id && !x.IsDeleted,
                   ct);

           if (budget == null)
               throw new InvalidOperationException(
                   $"Budget with ID '{request.EditDto.Id}' not found");

        // Validate period
        if (budget.PeriodId != request.EditDto.PeriodId)
        {
            var newPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    p => p.Id == request.EditDto.PeriodId &&
                         !p.IsDeleted,
                    ct);

            if (newPeriod == null)
                throw new InvalidOperationException(
                    $"Period with ID '{request.EditDto.PeriodId}' not found.");

            if (newPeriod.IsClosed)
                throw new InvalidOperationException(
                    $"Cannot move budget to a closed period: {newPeriod.Name}");

            if (request.EditDto.StartDate < newPeriod.StartDate ||
                request.EditDto.EndDate > newPeriod.EndDate)
            {
                throw new InvalidOperationException(
                    $"Budget dates must be within period range: {newPeriod.StartDate:yyyy-MM-dd} to {newPeriod.EndDate:yyyy-MM-dd}");
            }

            budget.PeriodId = newPeriod.Id;
        }
        else
        {
            var currentPeriod = await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    p => p.Id == budget.PeriodId &&
                         !p.IsDeleted,
                    ct);

            if (currentPeriod != null && currentPeriod.IsClosed)
                throw new InvalidOperationException(
                    $"Cannot update budget in a closed period: {currentPeriod.Name}");
        }

         budget.Name = request.EditDto.Name;
           budget.StartDate = request.EditDto.StartDate;
           budget.EndDate = request.EditDto.EndDate;
           budget.Description = request.EditDto.Description;
           budget.BranchId = request.EditDto.BranchId;
           budget.DepartmentId = request.EditDto.DepartmentId;
           budget.DateMod = DateTime.UtcNow;

           if (request.EditDto.Lines != null && request.EditDto.Lines.Any())
           {
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

               budget.TotalAmount = request.EditDto.Lines.Sum(x => x.AllocatedAmount);

               // Soft-delete existing lines (must be tracked)
               var existingLines = await _context.BudgetLines
                   .AsTracking()
                   .Where(x => x.BudgetId == budget.Id)
                   .ToListAsync(ct);

               foreach (var line in existingLines)
               {
                   line.IsDeleted = true;
                   line.DateMod = DateTime.UtcNow;
               }

               // Add new lines via DbSet so they are definitely tracked
               foreach (var lineDto in request.EditDto.Lines)
               {
                   _context.BudgetLines.Add(new BudgetLine
                   {
                       Id = Guid.CreateVersion7(),
                       BudgetId = budget.Id,
                       AccountId = lineDto.AccountId,
                       AllocatedAmount = lineDto.AllocatedAmount,
                       SpentAmount = 0,
                       Description = lineDto.Description,
                       PeriodId = budget.PeriodId,
                       DateAdd = DateTime.UtcNow,
                       DateMod = null,
                       IsDeleted = false
                   });
               }
           }

           try
           {
               await _context.SaveChangesAsync(ct);
           }
           catch (DbUpdateConcurrencyException)
           {
               throw new InvalidOperationException(
                   "Budget was modified or deleted by another process. Reload and try again.");
           }

           var budgetPeriod = await _context.FinancialPeriods
               .FirstOrDefaultAsync(p => p.Id == budget.PeriodId && !p.IsDeleted, ct);

           return await MapToDto(budget, budgetPeriod, ct);
       }

    private async Task<BudgetDto> MapToDto(
        Budget budget,
        FinancialPeriod? period,
        CancellationToken ct)
    {
        var lines = await _context.BudgetLines
            .Where(x => x.BudgetId == budget.Id && !x.IsDeleted)
            .Include(x => x.Account)
            .ToListAsync(ct);

        return new BudgetDto
        {
            Id = budget.Id,
            Name = budget.Name,
            StartDate = budget.StartDate,
            EndDate = budget.EndDate,
            TotalAmount = budget.TotalAmount,
            Status = budget.Status,
            Description = budget.Description,

            PeriodId = budget.PeriodId,
            PeriodName = period?.Name,

            BranchId = budget.BranchId,
            DepartmentId = budget.DepartmentId,

            Lines = lines.Select(line => new BudgetLineDto
            {
                Id = line.Id,
                AccountId = line.AccountId,
                AccountName = line.Account?.Name,
                AccountCode = line.Account?.Code,
                AllocatedAmount = line.AllocatedAmount,
                SpentAmount = line.SpentAmount,
                RemainingAmount = line.AllocatedAmount - line.SpentAmount,
                Description = line.Description,
                PeriodId = line.PeriodId,
                PeriodName = period?.Name
            }).ToList(),

            DateAdd = budget.DateAdd,
            DateMod = budget.DateMod
        };
    }
}

// ==================== DELETE BUDGET HANDLER ====================

public class DeleteBudgetHandler : IRequestHandler<DeleteBudgetCmd, bool>
{
    private readonly FinanceDbContext _context;

    public DeleteBudgetHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBudgetCmd request, CancellationToken ct)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (budget == null)
            return false;

        // ? VALIDATE PERIOD IS OPEN BEFORE DELETING
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(p => p.Id == budget.PeriodId && !p.IsDeleted, ct);

        if (period == null)
            throw new InvalidOperationException($"Period with ID {budget.PeriodId} not found");

        if (period.IsClosed)
            throw new InvalidOperationException($"Cannot delete budget in a closed period: {period.Name}");

        // Check if budget has actual spending
        var hasSpending = await _context.BudgetLines
            .AnyAsync(x => x.BudgetId == request.Id && x.SpentAmount > 0 && !x.IsDeleted, ct);

        if (hasSpending)
            throw new InvalidOperationException("Cannot delete budget with actual spending.");

        budget.IsDeleted = true;
        budget.DateMod = DateTime.UtcNow;

        // Soft delete all budget lines
        var lines = await _context.BudgetLines
            .Where(x => x.BudgetId == budget.Id && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var line in lines)
        {
            line.IsDeleted = true;
            line.DateMod = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}