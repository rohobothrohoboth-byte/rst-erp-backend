// Models/Analytics/WarehouseModels.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cor.Finance.Models.Analytics;

// ============================================================
// FACT TABLES
// ============================================================

[Table("FactInvoices")]
public class FactInvoice
{
    [Key]
    public long InvoiceKey { get; set; }

    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; }
    public int DateKey { get; set; }

    public int CustomerKey { get; set; }
    [MaxLength(200)]
    public string? CustomerName { get; set; }

    public int VendorKey { get; set; }
    [MaxLength(200)]
    public string? VendorName { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAmount { get; set; }

    public int DaysOverdue { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(20)]
    public string InvoiceType { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }
    public DateTime ETLDate { get; set; }
}

[Table("FactPayments")]
public class FactPayment
{
    [Key]
    public long PaymentKey { get; set; }

    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;

    public DateTime PaymentDate { get; set; }
    public int DateKey { get; set; }

    public long InvoiceKey { get; set; }
    public int CustomerKey { get; set; }
    public int VendorKey { get; set; }

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(20)]
    public string PaymentType { get; set; } = string.Empty;

    public DateTime ETLDate { get; set; }
}

[Table("FactExpenses")]
public class FactExpense
{
    [Key]
    public long ExpenseKey { get; set; }

    public DateTime ExpenseDate { get; set; }
    public int DateKey { get; set; }

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public int EmployeeKey { get; set; }
    public int DepartmentKey { get; set; }

    public DateTime ETLDate { get; set; }
}

[Table("FactBudgets")]
public class FactBudget
{
    [Key]
    public long BudgetKey { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DateKey { get; set; }

    public int DepartmentKey { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SpentAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Utilization { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    public DateTime ETLDate { get; set; }
}

// ============================================================
// DIMENSION TABLES
// ============================================================

[Table("DimDates")]
public class DimDate
{
    [Key]
    public int DateKey { get; set; }
    public DateTime Date { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Quarter { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public string MonthName { get; set; } = string.Empty;
    public string QuarterName { get; set; } = string.Empty;
    public bool IsWeekend { get; set; }
    public bool IsHoliday { get; set; }
}

[Table("DimAccounts")]
public class DimAccount
{
    [Key]
    public int AccountKey { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string AccountType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string AccountSubType { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public int? ParentKey { get; set; }
}

[Table("DimVendors")]
public class DimVendor
{
    [Key]
    public int VendorKey { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; }
}

[Table("DimCustomers")]
public class DimCustomer
{
    [Key]
    public int CustomerKey { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; }
}

[Table("DimEmployees")]
public class DimEmployee
{
    [Key]
    public int EmployeeKey { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Email { get; set; }

    public int? DepartmentKey { get; set; }

    public bool IsActive { get; set; }
}

[Table("DimDepartments")]
public class DimDepartment
{
    [Key]
    public int DepartmentKey { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

[Table("DimBranches")]
public class DimBranch
{
    [Key]
    public int BranchKey { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Location { get; set; }

    public bool IsActive { get; set; }
}