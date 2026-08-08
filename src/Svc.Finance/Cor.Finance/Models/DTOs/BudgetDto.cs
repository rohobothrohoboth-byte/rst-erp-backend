namespace Cor.Finance.Models.DTOs;




// ==================== BUDGET LINE DTOs ====================

public class BudgetLineDto
{
    public Guid? Id { get; set; }
    public Guid BudgetId { get; set; }
    public Guid AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountCode { get; set; }
    public decimal AllocatedAmount { get; set; }
      public decimal RemainingAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public string? Description { get; set; }

    // ? PeriodId - REQUIRED (denormalized)
    public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }
     public DateTime DateAdd { get; set; }
        public DateTime? DateMod { get; set; }
}

public class AddBudgetLineDto
{
    public Guid BudgetId { get; set; }
    public Guid AccountId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public string? Description { get; set; }

    // ? PeriodId - REQUIRED
    public Guid? PeriodId { get; set; }
}

public class EditBudgetLineDto
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public Guid AccountId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public string? Description { get; set; }

    // ? PeriodId - REQUIRED
    public Guid? PeriodId { get; set; }

    public string RowVersion { get; set; } = default!;
}



// ==================== BUDGET DTOs ====================

public class BudgetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = default!;
    public string? Description { get; set; }
  public Guid BudgetCodeId { get; set; }
   public string BudgetCode { get; set; } = default!;
    // ? PeriodId - REQUIRED
     public Guid? PeriodId { get; set; }
    public string? PeriodName { get; set; }

    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public List<BudgetLineDto> Lines { get; set; } = new();
    public DateTime DateAdd { get; set; }
    public DateTime? DateMod { get; set; }
    public string RowVersion { get; set; } = default!;
}

public class AddBudgetDto
{
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Description { get; set; }
  public Guid BudgetCodeId { get; set; }
    // ? PeriodId - REQUIRED
    public Guid? PeriodId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<BudgetLineDto> Lines { get; set; } = new();
}

public class EditBudgetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Description { get; set; }
   public Guid BudgetCodeId { get; set; }
    // ? PeriodId - REQUIRED
    public Guid? PeriodId { get; set; }

    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<BudgetLineDto> Lines { get; set; } = new();
    public string RowVersion { get; set; } = default!;
}


