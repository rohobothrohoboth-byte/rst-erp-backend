// Persistence/FinanceWarehouseContext.cs
using Cor.Finance.Models.Analytics;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Persistence;

public class FinanceWarehouseContext : DbContext
{
    // ✅ Parameterless constructor for design-time
    public FinanceWarehouseContext()
    {
    }

    // ✅ Options constructor for runtime
    public FinanceWarehouseContext(DbContextOptions<FinanceWarehouseContext> options)
        : base(options)
    {
    }

    // Fact Tables
    public DbSet<FactInvoice> FactInvoices { get; set; }
    public DbSet<FactPayment> FactPayments { get; set; }
    public DbSet<FactExpense> FactExpenses { get; set; }
    public DbSet<FactBudget> FactBudgets { get; set; }

    // Dimension Tables
    public DbSet<DimDate> DimDates { get; set; }
    public DbSet<DimAccount> DimAccounts { get; set; }
    public DbSet<DimVendor> DimVendors { get; set; }
    public DbSet<DimCustomer> DimCustomers { get; set; }
    public DbSet<DimEmployee> DimEmployees { get; set; }
    public DbSet<DimDepartment> DimDepartments { get; set; }
    public DbSet<DimBranch> DimBranches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fact Invoices
        modelBuilder.Entity<FactInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceKey);
            entity.HasIndex(e => e.DateKey);
            entity.HasIndex(e => e.CustomerKey);
            entity.HasIndex(e => e.VendorKey);
            entity.HasIndex(e => e.InvoiceDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.InvoiceType);

            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.InvoiceType).HasMaxLength(20);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.VendorName).HasMaxLength(200);

            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAmount).HasPrecision(18, 2);
        });

        // Fact Payments
        modelBuilder.Entity<FactPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentKey);
            entity.HasIndex(e => e.DateKey);
            entity.HasIndex(e => e.InvoiceKey);
            entity.HasIndex(e => e.CustomerKey);
            entity.HasIndex(e => e.VendorKey);
            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PaymentType);

            entity.Property(e => e.PaymentNumber).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.PaymentType).HasMaxLength(20);

            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // Fact Expenses
        modelBuilder.Entity<FactExpense>(entity =>
        {
            entity.HasKey(e => e.ExpenseKey);
            entity.HasIndex(e => e.DateKey);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.ExpenseDate);

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // Fact Budgets
        modelBuilder.Entity<FactBudget>(entity =>
        {
            entity.HasKey(e => e.BudgetKey);
            entity.HasIndex(e => e.DateKey);
            entity.HasIndex(e => e.DepartmentKey);
            entity.HasIndex(e => e.Status);

            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2);
        });

        // Dimension Tables
        modelBuilder.Entity<DimDate>(entity =>
        {
            entity.HasKey(e => e.DateKey);
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Year);
            entity.HasIndex(e => e.Month);
            entity.HasIndex(e => e.Quarter);

            entity.Property(e => e.DayOfWeekName).HasMaxLength(20);
            entity.Property(e => e.MonthName).HasMaxLength(20);
            entity.Property(e => e.QuarterName).HasMaxLength(10);
        });

        modelBuilder.Entity<DimAccount>(entity =>
        {
            entity.HasKey(e => e.AccountKey);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.AccountType).HasMaxLength(50);
            entity.Property(e => e.AccountSubType).HasMaxLength(50);
        });

        modelBuilder.Entity<DimVendor>(entity =>
        {
            entity.HasKey(e => e.VendorKey);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => e.Name);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<DimCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerKey);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => e.Name);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<DimEmployee>(entity =>
        {
            entity.HasKey(e => e.EmployeeKey);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => e.FirstName);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
        });

        modelBuilder.Entity<DimDepartment>(entity =>
        {
            entity.HasKey(e => e.DepartmentKey);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => e.Name);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<DimBranch>(entity =>
        {
            entity.HasKey(e => e.BranchKey);
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => e.Name);

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(500);
        });
    }

    // ✅ Configure design-time connection
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("WAREHOUSE_CONNECTION_STRING")
                ?? "Host=localhost;Port=5432;Database=core.FinanceWarehouse;Username=postgres;Password=root";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}