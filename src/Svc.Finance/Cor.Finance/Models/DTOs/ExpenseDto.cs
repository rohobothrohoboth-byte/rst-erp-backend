namespace Cor.Finance.Models.DTOs;

// ==================== EXPENSE CATEGORY DTOs ====================
// ? No PeriodId needed - Master data

public class ExpenseCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string CategoryType { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
}

public class AddExpenseCategoryDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string CategoryType { get; set; } = default!;
}

public class EditExpenseCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string CategoryType { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}

// ==================== EXPENSE DTOs ====================
// ? PeriodId added - Expense is a financial transaction

public class ExpenseDto
{
    public Guid Id { get; set; }
    public DateTime ExpenseDate { get; set; }
    public Guid ExpenseCategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = default!;
    public string Status { get; set; } = default!;
 public Guid? VendorId { get; set; }  // ✅ ADD THIS
   public string? VendorName { get; set; }
    // ? ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }  // For display

    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
      public string CategoryType { get; set; } = default!;
       public string RowVersion { get; set; } = default!;
}

public class AddExpenseDto
{
    public DateTime ExpenseDate { get; set; }
    public Guid ExpenseCategoryId { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = default!;

    // ? ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
}

public class EditExpenseDto
{
    public Guid Id { get; set; }
    public DateTime ExpenseDate { get; set; }
    public Guid ExpenseCategoryId { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = default!;

    // ? ADDED - PeriodId is REQUIRED
    public Guid? PeriodId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? EmployeeId { get; set; }
    public string RowVersion { get; set; } = default!;
}